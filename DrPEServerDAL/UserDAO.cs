using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrPEServer.DrPEServerEntities;
using System.Data.SqlClient;
using System.Configuration;

namespace DrPEServer.DrPEServerDAL {

    public class UserDAO {

        #region 查询排队状况 MyQueueStatus(Guid gAppointment)

        /*查询排队状况 MyQueueStatus(Guid gAppointment)*/
        public QueueStatusEntity MyQueueStatus(Guid gAppointment) {

            /*数据库访问实例*/
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            QueueStatusEntity queueStatusEntity = new QueueStatusEntity();

            /*查询该Appointment*/
            Appointment appointment = (from app in DEntities.Appointments
                                       where app.AppointmentID == gAppointment
                                       select app).FirstOrDefault();
            if (appointment == null) {
                queueStatusEntity.ErrorMessage = "471 No Such Appointment!";
                return queueStatusEntity;
            }

            /*根据Appointment获取医生信息*/
            string doctorID = appointment.Doctor.DoctorID;

            /*验证当前时间是否为预约时段*/
            DateTime nowTime = DateTime.Now;
            DateTime appointDate = (DateTime)nowTime;
            appointDate = appointDate.Date;
            if (nowTime >= appointDate.AddHours(12)) {
                appointDate = appointDate.AddHours(12);
            }

            if (appointDate != appointment.Date) {
                queueStatusEntity.ErrorMessage = "472 Not Now!";
                return queueStatusEntity;
            }

            /*获取Capacity*/
            AvailableDateEntity availableDateEntity = GetAvailableDate(doctorID);
            if (availableDateEntity == null) {
                queueStatusEntity.ErrorMessage = "473 All Appointment Cancelled!";
                return queueStatusEntity;
            }
            else {
                /*查询当前时段已预约人数*/
                queueStatusEntity.Capacity = availableDateEntity.Capacity;
                for (int i = 0; i < availableDateEntity.Count; i++) {
                    if (availableDateEntity.date[i] == appointDate) {
                        queueStatusEntity.Capacity = availableDateEntity.appointed[i];
                    }
                }
            }

            /*获取Mine*/
            queueStatusEntity.Mine = appointment.Rank;
            if (queueStatusEntity.Mine == null) {
                queueStatusEntity.ErrorMessage = "474 Anytime Will Do, Just Visit!";
                return queueStatusEntity;
            }

            /*获取Process*/
            Appointment processApp = (from app in DEntities.Appointments
                                      where (app.DoctorID == doctorID) && (app.Date == appointDate) && (app.Status != null)
                                      orderby app.Rank descending
                                      select app).FirstOrDefault();
            if (processApp == null) {
                //queueStatusEntity.ErrorMessage = "475 Nobody is There!";
                //return queueStatusEntity;
                queueStatusEntity.Process = 0;
            }
            else {
                if (processApp.Rank == null) {
                    queueStatusEntity.ErrorMessage = "476 Unknown Queue, You May Go!";
                    return queueStatusEntity;
                }
                else {
                    queueStatusEntity.Process = processApp.Rank;
                }
            }

            return queueStatusEntity;
        }

        #endregion


        #region 用户登录 Login(string userID, string password)

        /*用户登录: 向数据库提交select查询，若成功则并更新LastLoginDate域，将结果转写为Entity*/
        public UserInfoEntity Login(string userID, string password) {

            /*数据库访问实例*/
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询(UserID,password)对是否匹配*/
            User user = (from u in DEntities.Users
                         where ((u.UserID == userID) && (u.Password == password))
                         select u).FirstOrDefault();

            /*将结果转写为Entity，仅转写必要登录信息*/
            UserInfoEntity userInfoEntity = null;
            if (user != null) {
                userInfoEntity = new UserInfoEntity() {
                    UserID          = user.UserID,
                    LastName        = user.LastName,
                    FirstName       = user.FirstName,
                    LastLoginDate   = user.LastLoginDate
                };

                /*更新该User的LastLoginDate域*/
                user.LastLoginDate = DateTime.Now;
                DEntities.SaveChanges();
            }

            return userInfoEntity;
        }

        #endregion

        
        #region 获取用户资料 GetUserInfo(string userID)

        /*获取用户资料: 向数据库提交select查询，将结果转写为Entity*/
        public UserInfoEntity GetUserInfo(string userID) {

            /*数据库访问实例*/
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询UserID是否有匹配*/
            User user = (from u in DEntities.Users
                         where u.UserID == userID
                         select u).FirstOrDefault();

            /*将结果转写为Entity，未转写密码等敏感信息*/
            UserInfoEntity userInfoEntity = null;
            if (user != null) {
                userInfoEntity = new UserInfoEntity() {
                    UserID                  = user.UserID,
                    LastName                = user.LastName,
                    FirstName               = user.FirstName,
                    Nationality             = user.Nationality,
                    Gender                  = user.Gender,
                    ABO                     = user.ABO,
                    Rh                      = user.Rh,
                    Birthplace              = user.Birthplace,
                    Birthday                = user.Birthday,
                    Deathplace              = user.Deathplace,
                    Deathday                = user.Deathday,
                    Balance                 = user.Balance,
                    LastLoginDate           = user.LastLoginDate,
                    City                    = user.City,
                    Address                 = user.Address,
                    Phone                   = user.Phone,
                    Email                   = user.Email,
                    EmergencyContactPerson  = user.EmergencyContactPerson
                };
            }

            return userInfoEntity;
        }

        #endregion

        
        #region 查询医生最近可以预约的时间段 GetAvailableDate(string doctorID)

        private const int maxAvailableDataCount     = 21;
        private const int maxNoScheduleCapacity     = 64;//128;
        private const int maxDefaultCapacity        = 20;//64;
        private const int maxNoScheduleRangeDays    = 8;
        private const int maxNoWeekdayRangeDays     = 8;
        private const int maxNiceDoctorRangeDays    = 32;

        /*查询医生最近可以预约的时间段*/
        public AvailableDateEntity GetAvailableDate(string doctorID) {
            AvailableDateEntity availableDate = new AvailableDateEntity();

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*确认医生ID有效*/
            Doctor doctor = (from dr in DEntities.Doctors
                             where dr.DoctorID == doctorID
                             select dr).FirstOrDefault();
            if (doctor == null) {
                availableDate.ErrorMessage = "417 Invalid DoctorID! @Data";
                return availableDate;
            }

            /*查找预约规则*/
            Schedule schedule = (from sc in DEntities.Schedules
                                 where sc.DoctorID == doctorID
                                 select sc).FirstOrDefault();

            int cnt = 0;
            availableDate.date = new DateTime[maxAvailableDataCount];
            availableDate.appointed = new int[maxAvailableDataCount];
            DateTime startDate  = DateTime.Now.Date;
            DateTime endDate;

            /*未定义预约规则*/
            if (schedule == null) {
                availableDate.Capacity = maxNoScheduleCapacity;
                endDate = startDate.AddDays(maxNoScheduleRangeDays);
                for (DateTime bedTime = startDate; bedTime <= endDate; bedTime = bedTime.AddHours(12)) {
                    if ((bedTime.DayOfWeek != DayOfWeek.Saturday) && (bedTime.DayOfWeek != DayOfWeek.Sunday)) {
                        availableDate.date[cnt] = bedTime;
                        cnt++;
                    }
                }
            }
            else {
                if (schedule.Capacity == null) {
                    availableDate.Capacity = maxDefaultCapacity;
                }
                else {
                    availableDate.Capacity = (int)schedule.Capacity;
                }

                /*未定义预约规则的固定工作时间单位*/
                if (schedule.Weekday == null) {
                    endDate = startDate.AddDays(maxNoWeekdayRangeDays);
                    for (DateTime bedTime = startDate; bedTime <= endDate; bedTime = bedTime.AddHours(12)) {
                        if (((bedTime.DayOfWeek != DayOfWeek.Saturday) && (bedTime.DayOfWeek != DayOfWeek.Sunday) &&
                              (bedTime != schedule.Exception)) ||
                             (bedTime == schedule.Addition)) {
                            availableDate.date[cnt] = bedTime;
                            cnt++;
                        }
                    }
                }
                else {
                    DayOfWeek dayWeek = DayOfWeek.Monday;
                    switch (schedule.Weekday.Substring(0, 3).ToLower()) {
                        case "sun":
                            dayWeek = DayOfWeek.Sunday;
                            break;
                        case "mon":
                            dayWeek = DayOfWeek.Monday;
                            break;
                        case "tue":
                            dayWeek = DayOfWeek.Tuesday;
                            break;
                        case "wed":
                            dayWeek = DayOfWeek.Wednesday;
                            break;
                        case "thu":
                            dayWeek = DayOfWeek.Thursday;
                            break;
                        case "fri":
                            dayWeek = DayOfWeek.Friday;
                            break;
                        case "sat":
                            dayWeek = DayOfWeek.Saturday;
                            break;
                    }
                    int theHour = 0;
                    if (schedule.Weekday.Substring(schedule.Weekday.IndexOf(":") + 1, 1).ToLower() == "p") {
                        theHour = 12;
                    }

                    endDate = startDate.AddDays(maxNiceDoctorRangeDays);
                    for (DateTime bedTime = startDate; bedTime <= endDate; bedTime = bedTime.AddHours(12)) {
                        if (((bedTime.DayOfWeek == dayWeek) && (bedTime.Hour == theHour) && (bedTime != schedule.Exception)) || (bedTime == schedule.Addition)) {
                            availableDate.date[cnt] = bedTime;
                            cnt++;
                        }
                    }
                }
            }

            /*统计已预约人数*/
            availableDate.Count = cnt;
            for (int i = 0; i < cnt; i++) {
                DateTime date = availableDate.date[i];
                var appointments = (from ap in DEntities.Appointments
                                    where ((ap.Date == date) && (ap.DoctorID == doctor.DoctorID) && (ap.Status == null))
                                    select ap);
                availableDate.appointed[i] = appointments.Count();
            }
            Array.Resize(ref availableDate.date, cnt);
            Array.Resize(ref availableDate.appointed, cnt);

            return availableDate;
        }

        #endregion

        
        #region 查询科室最近一周预约的时间安排 GetSectionSchedule(string sectionID)

        public SectionScheduleEntity GetSectionSchedule(string sectionID) {
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            SectionScheduleEntity sectionScheduleEntity = new SectionScheduleEntity();

            /*确认Section存在*/
            Section section = (from se in DEntities.Sections
                               where se.SectionID == sectionID
                               select se).FirstOrDefault();
            if (section == null) {
                sectionScheduleEntity.ErrorMessage = "412 Invalid Section ID! @Data";
                return sectionScheduleEntity;
            }

            /*查找该Section的所有医生*/
            var doctors = (from dr in DEntities.Doctors
                           where dr.SectionID == section.SectionID
                           select dr);
            if (doctors == null) {
                sectionScheduleEntity.ErrorMessage = "413 No Doctors in Section" + sectionID + "! @Data";
                return sectionScheduleEntity;
            }

            DateTime startDate  = DateTime.Now.Date;
            DateTime endDate    = startDate.AddDays(8);

            int item = 0;
            int cnt = 0;
            AvailableDateEntity availableDate = null;
            sectionScheduleEntity.Detail = new SectionScheduleItemEntity[(doctors.Count()) * maxAvailableDataCount];
            foreach (var doctor in doctors) {
                availableDate = GetAvailableDate(doctor.DoctorID);
                for (item = 0; item < availableDate.Count; item++) {
                    if (availableDate.date[item] <= endDate) {
                        sectionScheduleEntity.Detail[cnt] = new SectionScheduleItemEntity();
                        sectionScheduleEntity.Detail[cnt].DoctorID  = doctor.DoctorID;
                        sectionScheduleEntity.Detail[cnt].Date      = availableDate.date[item];
                        sectionScheduleEntity.Detail[cnt].Appointed = availableDate.appointed[item];
                        sectionScheduleEntity.Detail[cnt].Capacity  = availableDate.Capacity;

                        cnt++;
                    }
                }
            }

            Array.Resize(ref sectionScheduleEntity.Detail, cnt);
            Array.Sort(sectionScheduleEntity.Detail, new MyComparer());
            sectionScheduleEntity.Count = cnt;

            if (sectionScheduleEntity.Count == 0) {
                sectionScheduleEntity.ErrorMessage = "414 No Schedule Info! @Data";
            }

            return sectionScheduleEntity;
        }

        private class MyComparer : IComparer<SectionScheduleItemEntity> {
            public int Compare(SectionScheduleItemEntity x, SectionScheduleItemEntity y) {
                return StringComparer.InvariantCulture.Compare(x.Date, y.Date);
            }
        }

        #endregion

        
        #region 预约医生某时间段 MakeAppointment(string userID, string doctorID, DateTime? date)

        const decimal registrationCost = 10;

        /*预约医生某时间段*/
        public string MakeAppointment(string userID, string doctorID, DateTime? date) {

            /*判断时间是否为空*/
            if (date == null) {
                return "421 Invalid Date! @Date";
            }

            /*判断UserID是否有效*/
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            User user = (from u in DEntities.Users
                         where u.UserID == userID
                         select u).FirstOrDefault();
            if (user == null) {
                return "422 Invalid UserID! @Data";
            }

            /*判断余额是否足够*/
            if (user.Balance < registrationCost) {
                return "424 Needs Enough Balance! @Data";
            }

            /*判断DoctorID是否有效，并获取可预约时间*/
            AvailableDateEntity availableDate = GetAvailableDate(doctorID);
            if (availableDate.ErrorMessage != null) {
                return "423 Invalid DoctorID! @Data";
            }

            /*预处理预约时间*/
            DateTime appointDate = (DateTime)date;
            appointDate = appointDate.Date;
            if (date >= appointDate.AddHours(12)) {
                appointDate = appointDate.AddHours(12);
            }

            /*判断是否可以预约*/
            bool isOK = false;
            int? rank = null;
            for (int i = 0; i < availableDate.Count; i++) {
                if (availableDate.date[i] == appointDate) {
                    if (availableDate.appointed[i] >= availableDate.Capacity) {
                        return "425 Already Fulled! @Data";
                    }
                    isOK = true;
                    rank = availableDate.appointed[i];
                    break;
                }
            }
            if (isOK == false) {
                return "426 Not Available! @Data";
            }
            

            /*判断是否重复预约*/
            Appointment checkApp = (from ap in DEntities.Appointments
                                    where ((ap.DoctorID == doctorID) && (ap.UserID == userID) && (ap.Date == appointDate))
                                    select ap).FirstOrDefault();
            if (checkApp != null) {
                return "427 Repeated Appointment!";
            }

            /*创建预约*/
            Appointment appointment     = new Appointment();
            appointment.AppointmentID   = Guid.NewGuid();
            appointment.DoctorID        = doctorID;
            appointment.UserID          = userID;
            appointment.Date            = appointDate;
            appointment.Rank            = rank + 1;

            try {
                DEntities.Appointments.AddObject(appointment);
                DEntities.SaveChanges();
            }
            catch {
                return "428 Appointment Denied! @Data";
            }

            /*查询医生*/
            Doctor doctor = (from dr in DEntities.Doctors
                             where dr.DoctorID == doctorID
                             select dr).FirstOrDefault();
            if (doctor == null) {
                return "423 Invalid DoctorID! @Data";
            }

            /*查询科室*/
            Section section = (from se in DEntities.Sections
                               where se.SectionID == doctor.SectionID
                               select se).FirstOrDefault();
            if (section == null) {
                return "429 Invalid Section! @Data";
            }

            /*查询医院*/
            Hospital hospital = (from ho in DEntities.Hospitals
                                 where ho.HospitalID == section.HospitalID
                                 select ho).FirstOrDefault();
            if (hospital == null) {
                return "430 Invalid Hospital! @Data";
            }

            /*创建账单*/
            Transaction newTransaction   = new Transaction();
            newTransaction.TransactionID = Guid.NewGuid();
            newTransaction.UserID = user.UserID;
            newTransaction.Date = DateTime.Now;
            newTransaction.Amount = registrationCost;
            newTransaction.Status = String.Format("A:{0}", appointment.AppointmentID);
            newTransaction.Detail = String.Format("R:{0}", hospital.HospitalID);
            try {
                DEntities.Transactions.AddObject(newTransaction);
                DEntities.SaveChanges();
            }
            catch {
                return "431 Transaction Denied! @Data";
            }

            user.Balance -= registrationCost;
            hospital.RegistrationBalance += registrationCost;
            DEntities.SaveChanges();

            return null;
        }

        #endregion

        
        #region 获取待去的预约记录 GetMyFutureAppointment(string userID)

        /*获取待去的预约记录*/
        public AllAppointmentEntity GetMyFutureAppointment(string userID) {
            AllAppointmentEntity allAppointmentEntity = new AllAppointmentEntity();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*判断UserID是否有效*/
            User user = (from u in DEntities.Users
                         where u.UserID == userID
                         select u).FirstOrDefault();

            /*获取符合要求的预约记录*/
            DateTime bedTime = DateTime.Now.Date;
            var appointments = from app in DEntities.Appointments
                               where ((app.UserID == userID) && (app.Date >= bedTime) && (app.Status == null))
                               orderby app.Date
                               select app;

            allAppointmentEntity.Count = appointments.Count();
            if (allAppointmentEntity.Count <= 0) {
                allAppointmentEntity.ErrorMessage = "433 No Future Appointment! @Data";
            }
            else {
                allAppointmentEntity.appointment = new AppointmentEntity[allAppointmentEntity.Count];
                int cnt = 0;
                foreach (var app in appointments) {
                    allAppointmentEntity.appointment[cnt] = new AppointmentEntity();
                    allAppointmentEntity.appointment[cnt].gGuid     = app.AppointmentID;
                    allAppointmentEntity.appointment[cnt].UserID    = app.UserID;
                    allAppointmentEntity.appointment[cnt].DoctorID  = app.DoctorID;
                    allAppointmentEntity.appointment[cnt].Date      = app.Date;
                    allAppointmentEntity.appointment[cnt].Rank      = app.Rank;
                    if (app.Status == null) {
                        allAppointmentEntity.appointment[cnt].Finished = false;
                    }
                    else {
                        allAppointmentEntity.appointment[cnt].Finished = true;
                    }
                    cnt++;
                }
            }

            return allAppointmentEntity;
        }

        #endregion

        
        #region 获取最近一次病历 GetLastCaseInfo(string userID, bool showICD)

        /*获取最近一次病历*/
        public CaseInfoEntity GetLastCaseInfo(string userID, bool showICD) {

            CaseInfoEntity caseInfoEntity = new CaseInfoEntity();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*获取最近一次病历*/
            CaseHistory caseInfo = (from c in DEntities.CaseHistories
                                    where ((c.ModifiedDate != null) && (c.UserID == userID))
                                    orderby c.CreatedDate descending
                                    select c).FirstOrDefault();
            if (caseInfo == null) {
                caseInfoEntity.ErrorMessage = "No Any Case! @Data";
                return caseInfoEntity;
            }

            /*复制病历部分信息*/
            caseInfoEntity.CaseID           = caseInfo.CaseID;
            caseInfoEntity.ExaminationID    = caseInfo.ExaminationID;
            caseInfoEntity.PrescriptionID   = caseInfo.PrescriptionID;
            caseInfoEntity.UserID           = caseInfo.UserID;
            caseInfoEntity.DoctorID         = caseInfo.DoctorID;
            caseInfoEntity.SectionID        = caseInfo.SectionID;
            caseInfoEntity.CountercheckDate = caseInfo.CountercheckDate;

            /*判断病历是否完成*/
            caseInfoEntity.Date = caseInfo.ModifiedDate;
            if (caseInfoEntity.Date == null) {
                caseInfoEntity.IsDraft = true;
            }
            else {
                caseInfoEntity.IsDraft = false;
            }

            /*针对ICD编码进行转写*/
            if (showICD == false) {
                caseInfoEntity.ChiefComplaint           = TranslateICD(caseInfo.ChiefComplaint);
                caseInfoEntity.TentativeDiagnosis       = TranslateICD(caseInfo.TentativeDiagnosis);
                caseInfoEntity.DifferentialDiagnosis    = TranslateICD(caseInfo.DifferentialDiagnosis);
                caseInfoEntity.TreatmentPlan            = TranslateICD(caseInfo.TreatmentPlan);
            }
            else {
                caseInfoEntity.ChiefComplaint           = caseInfo.ChiefComplaint;
                caseInfoEntity.TentativeDiagnosis       = caseInfo.TentativeDiagnosis;
                caseInfoEntity.DifferentialDiagnosis    = caseInfo.DifferentialDiagnosis;
                caseInfoEntity.TreatmentPlan            = caseInfo.TreatmentPlan;
            }

            return caseInfoEntity;
        }


        #endregion

        
        #region 获取某用户在特定时间区间内的所有已完成病历 GetCaseHistory(string userID, DateTime? startDate, DateTime? endDate, bool showICD)

        /*获取指定用户在某区间内所有完成的病历*/
        public CaseHistoryEntity GetCaseHistory(string userID, DateTime? startDate, DateTime? endDate, bool showICD) {

            CaseHistoryEntity caseHistoryEntity = new CaseHistoryEntity();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*根据时间和用户ID查找病历*/
            var cases = from c in DEntities.CaseHistories
                        where ((c.ModifiedDate != null) && (startDate <= c.ModifiedDate) && (c.ModifiedDate <= endDate) && (c.UserID == userID))
                        orderby c.ModifiedDate descending
                        select c;

            /*处理无结果的情况*/
            int caseCount = cases.Count();
            if (caseCount <= 0) {
                caseHistoryEntity.ErrorMessage = String.Format("No Case Histories Modified Between {0} and {1}! @Data", startDate, endDate);
                return caseHistoryEntity;
            }

            /*逐条进行转录*/
            int cnt = 0;
            caseHistoryEntity = new CaseHistoryEntity();
            caseHistoryEntity.Count = caseCount;
            caseHistoryEntity.caseInfoEntity = new CaseInfoEntity[caseCount];
            foreach (var c in cases) {
                caseHistoryEntity.caseInfoEntity[cnt] = new CaseInfoEntity();

                /*复制病历部分信息*/
                caseHistoryEntity.caseInfoEntity[cnt].CaseID = c.CaseID;
                caseHistoryEntity.caseInfoEntity[cnt].ExaminationID = c.ExaminationID;
                caseHistoryEntity.caseInfoEntity[cnt].PrescriptionID = c.PrescriptionID;
                caseHistoryEntity.caseInfoEntity[cnt].UserID = c.UserID;
                caseHistoryEntity.caseInfoEntity[cnt].DoctorID = c.DoctorID;
                caseHistoryEntity.caseInfoEntity[cnt].SectionID = c.SectionID;
                caseHistoryEntity.caseInfoEntity[cnt].CountercheckDate = c.CountercheckDate;
                caseHistoryEntity.caseInfoEntity[cnt].Date = c.ModifiedDate;

                /*针对ICD编码进行转写*/
                if (showICD == false) {
                    caseHistoryEntity.caseInfoEntity[cnt].ChiefComplaint = TranslateICD(c.ChiefComplaint);
                    caseHistoryEntity.caseInfoEntity[cnt].TentativeDiagnosis = TranslateICD(c.TentativeDiagnosis);
                    caseHistoryEntity.caseInfoEntity[cnt].DifferentialDiagnosis = TranslateICD(c.DifferentialDiagnosis);
                    caseHistoryEntity.caseInfoEntity[cnt].TreatmentPlan = TranslateICD(c.TreatmentPlan);
                }
                else {
                    caseHistoryEntity.caseInfoEntity[cnt].ChiefComplaint = c.ChiefComplaint;
                    caseHistoryEntity.caseInfoEntity[cnt].TentativeDiagnosis = c.TentativeDiagnosis;
                    caseHistoryEntity.caseInfoEntity[cnt].DifferentialDiagnosis = c.DifferentialDiagnosis;
                    caseHistoryEntity.caseInfoEntity[cnt].TreatmentPlan = c.TreatmentPlan;
                }

                cnt++;
            }

            return caseHistoryEntity;
        }

        #endregion

        
        #region 将文本中的ICD-10编码翻译为文字 TranslateICD(string context)

        /*将文本中的ICD-10编码翻译为文字*/
        private string TranslateICD(string context) {

            if (context == null) {
                return null;
            }

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            Disease disease = null;

            string answer = context;
            string code = null;
            string hugCode = null;

            int pos1 = answer.IndexOf("[[");
            int pos2 = 0;
            if (pos1 >= 0) {
                pos2 = answer.IndexOf("]]", pos1);
            }
            while ((pos1 >= 0) && (pos2 >= 0)) {
                code = answer.Substring(pos1 + 2, pos2 - pos1 - 2);
                hugCode = "[[" + code + "]]";
                disease = (from d in DEntities.Diseases
                           where d.DiseaseID == code
                           select d).FirstOrDefault();

                /*使用Replace方法，减少数据库查询以及不必要的String类生成*/
                if (disease == null) {
                    answer = answer.Replace(hugCode, code);
                }
                else {
                    answer = answer.Replace(hugCode, disease.Name);
                }

                pos1 = answer.IndexOf("[[");
                if (pos1 >= 0) {
                    pos2 = answer.IndexOf("]]", pos1);
                }
            }

            return answer;
        }

        #endregion

        
        #region 读取处方单 GetPrescriptionInfo(Guid gPrescriptionID)

        
        public PrescriptionInfoEntity GetPrescriptionInfo(Guid gPrescriptionID) {

            PrescriptionInfoEntity prescriptionInfoEntity = new PrescriptionInfoEntity();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            Prescription prescription = (from p in DEntities.Prescriptions
                                         where p.PrescriptionID == gPrescriptionID
                                         select p).FirstOrDefault();

            /*若处方不存在*/
            if (prescription == null) {
                prescriptionInfoEntity.ErrorMessage = "No Such Prescription! @Data";
                return prescriptionInfoEntity;
            }

            /*解析Detail域内容*/
            int cnt = 0;
            int pos = 0;
            string sPhysicID    = null;
            string sNumber      = null;
            string[] detail     = prescription.Detail.Split(';');
            prescriptionInfoEntity.physicID = new string[detail.Length];
            prescriptionInfoEntity.number = new int[detail.Length];

            foreach (string s in detail) {
                /*逐条解析Detail域内容*/
                pos = s.IndexOf(':');
                if (pos < 0) {
                    break;
                }

                sPhysicID = s.Substring(0, pos);
                sNumber = s.Substring(pos + 1);
                prescriptionInfoEntity.physicID[cnt] = sPhysicID;
                prescriptionInfoEntity.number[cnt] = Convert.ToInt32(sNumber);

                cnt++;
            }

            prescriptionInfoEntity.Count = cnt;
            prescriptionInfoEntity.PrescriptionID = gPrescriptionID;

            return prescriptionInfoEntity;
        }

        #endregion

        
        #region 读取化验单 GetExaminationInfo(string examinationID)

        /*读取化验单 GetExaminationInfo(string examinationID)*/
        public ExaminationInfoEntity GetExaminationInfo(Guid gExaminationID) {

            ExaminationInfoEntity examinationInfoEntity = new ExaminationInfoEntity();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            Examination examination = (from ex in DEntities.Examinations
                                       where ex.ExaminationID == gExaminationID
                                       select ex).FirstOrDefault();

            if (examination == null) {
                examinationInfoEntity.ErrorMessage = "No Such Examination Record! @Data";
                return examinationInfoEntity;
            }

            examinationInfoEntity.ExaminationID = gExaminationID;
            examinationInfoEntity.Date = examination.Date;
            examinationInfoEntity.Type = examination.Type;
            examinationInfoEntity.Text = examination.Text;
            examinationInfoEntity.Advice = examination.Advice;
            examinationInfoEntity.Image = examination.Image;

            return examinationInfoEntity;
        }

        #endregion

        
        #region 用户撰写邮件询问医生 MessageCompose(string senderID, string receiverID, string text)

        /*用户撰写邮件User to Doctor*/
        public string MessageCompose(string senderID, string receiverID, string text) {
            Message message = new Message();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*必要的ID确认，避免在数据库中留下垃圾信息*/
            User user = (from u in DEntities.Users
                         where u.UserID == senderID
                         select u).FirstOrDefault();
            if (user == null) {
                return "Invalid Sender UserID!";
            }

            Doctor doctor = (from d in DEntities.Doctors
                             where d.DoctorID == receiverID
                             select d).FirstOrDefault();
            if (doctor == null) {
                return "Invalid Receiver DoctorID!";
            }

            message.MessageID = Guid.NewGuid();
            message.Sender = user.UserID;
            message.Receiver = doctor.DoctorID;
            message.Type = "U2D";
            message.Date = DateTime.Now;
            message.Text = text;

            try {
                DEntities.Messages.AddObject(message);
                DEntities.SaveChanges();
            }
            catch {
                return "Sending Failed! @Data";
            }

            return null;
        }

        #endregion

        
        #region 用户收取收件箱 MessageInbox(string userID, DateTime? startDate, DateTime? endDate)

        /*用户收取收件箱*/
        public AllMessageEntity MessageInbox(string userID, DateTime? startDate, DateTime? endDate) {
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            AllMessageEntity allMessageEntity = new AllMessageEntity();

            /*提交查询...可优化掉，不会毁坏数据库信息*/
            User user = (from u in DEntities.Users
                         where u.UserID == userID
                         select u).FirstOrDefault();
            if (user == null) {
                allMessageEntity.ErrorMessage = "Invalid Receiver UserID! @Data";
                return allMessageEntity;
            }

            var messages = from m in DEntities.Messages
                           where ((m.Receiver == user.UserID) && (startDate <= m.Date) && (m.Date <= endDate))
                           orderby m.Date descending
                           select m;

            /*处理无结果的情况*/
            int messageCount = messages.Count();
            if (messageCount <= 0) {
                allMessageEntity.ErrorMessage = String.Format("User {0} Received No Messages Between {1} and {2}! @Data",
                                                                user.UserID, startDate, endDate);
                return allMessageEntity;
            }

            int cnt = 0;
            allMessageEntity.Count = messageCount;
            allMessageEntity.messageEntity = new MessageEntity[messageCount];
            foreach (var m in messages) {
                allMessageEntity.messageEntity[cnt] = new MessageEntity();
                allMessageEntity.messageEntity[cnt].MessageID = m.MessageID;
                allMessageEntity.messageEntity[cnt].Sender = m.Sender;
                allMessageEntity.messageEntity[cnt].Receiver = m.Receiver;
                allMessageEntity.messageEntity[cnt].Status = m.Type;
                allMessageEntity.messageEntity[cnt].Date = m.Date;
                allMessageEntity.messageEntity[cnt].Context = m.Text;
                cnt++;
            }

            return allMessageEntity;
        }

        #endregion

        
        #region 用户查看发件箱 MessageSent(string userID, DateTime? startDate, DateTime? endDate)

        /*用户查看发件箱*/
        public AllMessageEntity MessageSent(string userID, DateTime? startDate, DateTime? endDate) {
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            AllMessageEntity allMessageEntity = new AllMessageEntity();

            /*提交查询...可优化掉，不会毁坏数据库信息...*/
            User user = (from u in DEntities.Users
                         where u.UserID == userID
                         select u).FirstOrDefault();
            if (user == null) {
                allMessageEntity.ErrorMessage = "Invalid Receiver UserID! @Data";
                return allMessageEntity;
            }

            var messages = from m in DEntities.Messages
                           where ((m.Sender == user.UserID) && (startDate <= m.Date) && (m.Date <= endDate))
                           orderby m.Date descending
                           select m;

            /*处理无结果的情况*/
            int messageCount = messages.Count();
            if (messageCount <= 0) {
                allMessageEntity.ErrorMessage = String.Format("User {0} Sent No Messages Between {1} and {2}! @Data",
                                                                user.UserID, startDate, endDate);
                return allMessageEntity;
            }

            int cnt = 0;
            allMessageEntity.Count = messageCount;
            allMessageEntity.messageEntity = new MessageEntity[messageCount];
            foreach (var m in messages) {
                allMessageEntity.messageEntity[cnt] = new MessageEntity();
                allMessageEntity.messageEntity[cnt].MessageID = m.MessageID;
                allMessageEntity.messageEntity[cnt].Sender = m.Sender;
                allMessageEntity.messageEntity[cnt].Receiver = m.Receiver;
                allMessageEntity.messageEntity[cnt].Status = m.Type;
                allMessageEntity.messageEntity[cnt].Date = m.Date;
                allMessageEntity.messageEntity[cnt].Context = m.Text;
                cnt++;
            }

            return allMessageEntity;
        }

        #endregion

        
        #region 获取指定时间区间中用户所有的交易记录 GetTransactionHistory(string userID, DateTime? startDate, DateTime? endDate)

        /*获取指定时间区间中药房所有的交易记录*/
        public AllTransactionInfoEntity GetTransactionHistory(string userID, DateTime? startDate, DateTime? endDate) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            AllTransactionInfoEntity allTransactionInfoEntity = new AllTransactionInfoEntity();

            /*查询[startDate, endDate]区间中产生的交易记录*/
            var transactions = from t in DEntities.Transactions
                               where ((startDate <= t.Date) && (t.Date <= endDate) && (t.UserID == userID))
                               orderby t.Date descending
                               select t;

            int cnt = 0;
            int transactionCount = transactions.Count();

            if (transactionCount <= 0) {
                allTransactionInfoEntity.ErrorMessage = String.Format("444 User {0} Got No Transaction History Between {1} and {2}! @Data",
                                                                       userID, startDate, endDate);
            }
            else {
                allTransactionInfoEntity.Count = transactionCount;
                allTransactionInfoEntity.transactionInfoEntity = new TransactionInfoEntity[transactionCount];

                foreach (var t in transactions) {
                    allTransactionInfoEntity.transactionInfoEntity[cnt] = new TransactionInfoEntity();

                    User user = (from u in DEntities.Users
                                 where u.UserID == t.UserID
                                 select u).FirstOrDefault();

                    allTransactionInfoEntity.transactionInfoEntity[cnt].TransactionID = t.TransactionID;
                    //allTransactionInfoEntity.transactionInfoEntity[cnt].LastName = user.LastName;
                    //allTransactionInfoEntity.transactionInfoEntity[cnt].FirstName = user.FirstName;
                    //allTransactionInfoEntity.transactionInfoEntity[cnt].pharmacyID = t.pharmacyID;
                    allTransactionInfoEntity.transactionInfoEntity[cnt].Amount = t.Amount;
                    allTransactionInfoEntity.transactionInfoEntity[cnt].Date = t.Date;
                    //allTransactionInfoEntity.transactionInfoEntity[cnt].Action = t.Detail;

                    if (t.PharmacyID != null) {
                        allTransactionInfoEntity.transactionInfoEntity[cnt].PharmacyID = "P" + t.Pharmacy.Name; /* +FeelPharmacyName(t.PharmacyID); */ //P + P001
                        if (t.Detail == null) {
                            allTransactionInfoEntity.transactionInfoEntity[cnt].Action = "W" + t.Status.Substring(2); //W: Pharmacy Waiting For User to Take
                        }
                        else {
                            allTransactionInfoEntity.transactionInfoEntity[cnt].Action = "D" + t.Status.Substring(2); //D: Transaction Done.
                        }
                    }
                    else {
                        allTransactionInfoEntity.transactionInfoEntity[cnt].PharmacyID = "H" + FeelHospitalName(t.Detail.Substring(2)); //H + H001
                        allTransactionInfoEntity.transactionInfoEntity[cnt].Action = t.Status[0] + t.Status.Substring(2); //E, A: Examination or Appointment Fee
                    }

                    cnt++;
                }
            }

            return allTransactionInfoEntity;
        }

        #endregion


        #region Private 根据ID查询药房或医院的名字 FeelPharmacyName() & FeelHospitalName()

        private string FeelPharmacyName(string pharmacyID) {
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            Pharmacy pharmacy = (from ph in DEntities.Pharmacies
                                 where ph.PharmacyID == pharmacyID
                                 select ph).FirstOrDefault();
            if (pharmacy == null) {
                return "Unknown Pharmacy";
            }
            else {
                return pharmacy.Name;
            }
        }

        private string FeelHospitalName(string hospitalID) {
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            Hospital hospital = (from hp in DEntities.Hospitals
                                 where hp.HospitalID == hospitalID
                                 select hp).FirstOrDefault();
            if (hospital == null) {
                return "Unknown Hospital";
            }
            else {
                return hospital.Name;
            }
        }

        #endregion


        #region 查询处方单在特定药房的花费 GetPrescriptionCost(Guid gPrescriptionID, string pharmacyID)

        /*查询处方单在特定药房的花费，供User调用。*/
        public PrescriptionCostEntity GetPrescriptionCost(Guid gPrescriptionID, string pharmacyID) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            PrescriptionCostEntity prescriptionCostEntity = new PrescriptionCostEntity();

            /*查询药房是否存在*/
            Pharmacy pharmacy = (from pm in DEntities.Pharmacies
                                 where pm.PharmacyID == pharmacyID
                                 select pm).FirstOrDefault();
            if (pharmacy == null) {
                prescriptionCostEntity.ErrorMessage = "No Such Pharmacy! @Data";
                return prescriptionCostEntity;
            }

            /*获取处方信息*/
            Prescription prescription = (from p in DEntities.Prescriptions
                                         where p.PrescriptionID == gPrescriptionID
                                         select p).FirstOrDefault();
            
            /*若处方不存在*/
            if (prescription == null) {
                prescriptionCostEntity.ErrorMessage = "No Such Prescription! @Data";
                return prescriptionCostEntity;
            }

            /*获取所属病历信息*/
            CaseHistory caseHistory = (from c in DEntities.CaseHistories
                                       where c.PrescriptionID == gPrescriptionID
                                       select c).FirstOrDefault();
            
            /*若处方不属于任何病历*/
            if (caseHistory == null) {
                prescriptionCostEntity.ErrorMessage = "No Case Has This Prescription! @Data";
                return prescriptionCostEntity;
            }

            /*获取病历所属用户信息*/
            User user = (from u in DEntities.Users
                         where (u.UserID == caseHistory.UserID)
                         select u).FirstOrDefault();

            /*解析Detail域内容，并计算当前药店处方花费*/
            int cnt = 0;
            int pos = 0;
            Decimal? amount     = 0;
            string sPhysicID    = null;
            string sNumber      = null;
            string[] detail     = prescription.Detail.Split(';');

            prescriptionCostEntity.physicID = new string[detail.Length];
            prescriptionCostEntity.number = new int[detail.Length];
            prescriptionCostEntity.price = new Decimal?[detail.Length];

            foreach (string s in detail) {
                /*逐条解析Detail域内容*/
                pos = s.IndexOf(':');
                if (pos < 0) {
                    break;
                }

                sPhysicID = s.Substring(0, pos);
                sNumber = s.Substring(pos + 1);
                prescriptionCostEntity.physicID[cnt] = sPhysicID;
                prescriptionCostEntity.number[cnt] = Convert.ToInt32(sNumber);

                /*在药店数据库中查询该药品的价格*/
                PharmacyDatabase pharmacyDatabase = (from p in DEntities.PharmacyDatabases
                                                     where ((p.PhysicID == sPhysicID) && (p.PharmacyID == pharmacyID))
                                                     select p).FirstOrDefault();

                if (pharmacyDatabase == null) {
                    /*若药店缺少某种药物*/
                    prescriptionCostEntity.ErrorMessage = "218 Medicine " + sPhysicID + " missing! @Data";
                    return prescriptionCostEntity;
                }
                else if (pharmacyDatabase.Price == null) {
                    /*若药店某种药物价格缺失*/
                    prescriptionCostEntity.ErrorMessage = "219 Medicine " + sPhysicID + " unknown! @Data";
                    return prescriptionCostEntity;
                }
                else {
                    /*记录价格，并计算总价*/
                    prescriptionCostEntity.price[cnt] = pharmacyDatabase.Price;
                    amount += (pharmacyDatabase.Price) * (prescriptionCostEntity.number[cnt]);
                }

                cnt++;
            }

            prescriptionCostEntity.Count = cnt;
            prescriptionCostEntity.UserBalance = user.Balance;
            prescriptionCostEntity.PharmacyID = pharmacyID;
            prescriptionCostEntity.Amount = amount;

            return prescriptionCostEntity;
        }

        #endregion


        #region 为处方单付款 PayPrescription(Guid gPrescriptionID, string pharmacyID, string payPassword)

        /*为处方单付款*/
        public TransactionInfoEntity PayPrescription(Guid gPrescriptionID, string pharmacyID, string payPassword) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            TransactionInfoEntity transactionInfoEntity = new TransactionInfoEntity();

            /*获取处方信息*/
            Prescription prescription = (from p in DEntities.Prescriptions
                                         where p.PrescriptionID == gPrescriptionID
                                         select p).FirstOrDefault();

            /*若处方不存在*/
            if (prescription == null) {
                transactionInfoEntity.ErrorMessage = "No Such Prescription! @Data";
                return transactionInfoEntity;
            }

            /*判断是否已付款*/
            string sCheck = String.Format("P:{0}", gPrescriptionID.ToString());
            Transaction transaction = (from t in DEntities.Transactions
                                       where t.Status == sCheck
                                       select t).FirstOrDefault();
            if (transaction != null) {
                /*查询交易药房*/
                Pharmacy pharmacy = (from p in DEntities.Pharmacies
                                     where p.PharmacyID == transaction.PharmacyID
                                     select p).FirstOrDefault();

                /*是否与本药房交易*/
                if (pharmacy.PharmacyID == pharmacyID) {
                    /*药物是否已经领取*/
                    if (transaction.Detail == null) {
                        transactionInfoEntity.Amount = transaction.Amount;
                        transactionInfoEntity.ErrorMessage = String.Format("231 Already Paid at {0}, But Not Taken Yet! @Data", transaction.Date);
                    }
                    else {
                        transactionInfoEntity.Amount = transaction.Amount;
                        transactionInfoEntity.ErrorMessage = String.Format("232 Already Paid and Taken at {0}! @Data", transaction.Date);
                    }
                }
                else {
                    transactionInfoEntity.ErrorMessage = String.Format("233 Already Bought in {0}({1}) at {2}! @Data",
                                                                        pharmacy.Name, pharmacy.PharmacyID, transaction.Date);
                }
                return transactionInfoEntity;
            }

            /*获取所属病历信息*/
            CaseHistory caseHistory = (from c in DEntities.CaseHistories
                                       where c.PrescriptionID == gPrescriptionID
                                       select c).FirstOrDefault();

            /*若处方不属于任何病历*/
            if (caseHistory == null) {
                transactionInfoEntity.ErrorMessage = "226 This Prescription Belongs to No Case! @Data";
                return transactionInfoEntity;
            }

            /*获取病历所属用户信息*/
            User user = (from u in DEntities.Users
                         where ((u.UserID == caseHistory.UserID) && (u.PayPassword == payPassword))
                         select u).FirstOrDefault();

            /*若支付密码错误*/
            if (user == null) {
                transactionInfoEntity.ErrorMessage = "227 Wrong PayPassword! @Data";
                return transactionInfoEntity;
            }

            /*获取处方费用*/
            PrescriptionCostEntity prescriptionCostEntity = GetPrescriptionCost(gPrescriptionID, pharmacyID);
            if (prescriptionCostEntity.ErrorMessage != null) {
                transactionInfoEntity.ErrorMessage = prescriptionCostEntity.ErrorMessage;
                return transactionInfoEntity;
            }

            /*判断用户帐号是否允许支付*/
            if (user.Balance == null) {
                transactionInfoEntity.ErrorMessage = String.Format("228 User's Balance Not Available! @Data");
                return transactionInfoEntity;
            }

            /*判断用户是否有足够的余额支付*/
            if (user.Balance < prescriptionCostEntity.Amount) {
                transactionInfoEntity.ErrorMessage = String.Format("229 Needs RMB{0}, But Only RMB{1} Left! @Data", prescriptionCostEntity.Amount, user.Balance);
                return transactionInfoEntity;
            }

            /*支付密码正确，生成新交易*/
            Transaction newTransaction   = new Transaction();
            newTransaction.TransactionID = Guid.NewGuid();
            newTransaction.UserID = user.UserID;
            newTransaction.PharmacyID = pharmacyID;
            newTransaction.Date = DateTime.Now;
            newTransaction.Amount = prescriptionCostEntity.Amount;
            newTransaction.Status = sCheck;

            /*提交修改至数据库*/
            try {
                DEntities.Transactions.AddObject(newTransaction);
                DEntities.SaveChanges();
            }
            catch {
                transactionInfoEntity.ErrorMessage = "241 GUID Conflicts! @Data";
                return transactionInfoEntity;
            }

            /*从用户帐号扣款*/
            user.Balance -= prescriptionCostEntity.Amount;
            DEntities.SaveChanges();

            /*返回交易详情*/
            transactionInfoEntity.TransactionID = newTransaction.TransactionID;
            transactionInfoEntity.LastName = user.LastName;
            transactionInfoEntity.FirstName = user.FirstName;
            transactionInfoEntity.PharmacyID = newTransaction.PharmacyID;
            transactionInfoEntity.Date = newTransaction.Date;
            transactionInfoEntity.Amount = newTransaction.Amount;
            transactionInfoEntity.UserBalanceThen = user.Balance;
            if (newTransaction.Detail != null) {
                transactionInfoEntity.Action = newTransaction.Detail;
            }
            else {
                transactionInfoEntity.Action = "[Left]";
            }

            return transactionInfoEntity;
        }

        #endregion


        #region 获取病例的疾病温馨提示 GetDiseaseNotice(Guid gCaseID)

        private const int maxNoticeCount = 200;

        public NoticeEntity GetDiseaseNotice(Guid gCaseID) {
            NoticeEntity noticeEntity = new NoticeEntity();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*获取该病例*/
            CaseHistory caseInfo = (from cs in DEntities.CaseHistories
                                    where (cs.CaseID == gCaseID)
                                    select cs).FirstOrDefault();
            if (caseInfo == null) {
                noticeEntity.ErrorMessage = "Invalid CaseID! @Data";
                return noticeEntity;
            }

            /*判除未完成病例*/
            if (caseInfo.ModifiedDate == null) {
                noticeEntity.ErrorMessage = "Case Not Finished! @Data";
                return noticeEntity;
            }

            noticeEntity.notice = new string[maxNoticeCount];
            int cnt = 0;
            GetNoticeFromContext(caseInfo.TentativeDiagnosis, ref noticeEntity.notice, ref cnt);
            GetNoticeFromContext(caseInfo.TreatmentPlan, ref noticeEntity.notice, ref cnt);
            GetNoticeFromContext(caseInfo.ChiefComplaint, ref noticeEntity.notice, ref cnt);
            GetNoticeFromContext(caseInfo.DifferentialDiagnosis, ref noticeEntity.notice, ref cnt);

            Disease disease;
            string diseaseID;
            string wareSenseMessage = null;
            for (int i = 0; i < cnt; i++) {
                diseaseID = noticeEntity.notice[i];
                disease = (from d in DEntities.Diseases
                           where d.DiseaseID == diseaseID
                           select d).FirstOrDefault();
                if (disease != null) {
                    noticeEntity.notice[i] = disease.Notice;
                }

                /*调用WareSense执行部分*/
                if (wareSenseMessage == null) {
                    WareSense wareSense = (from ws in DEntities.WareSenses
                                           where ws.DiseaseID == diseaseID
                                           select ws).FirstOrDefault();
                    if (wareSense != null) {
                        if (wareSense.Text != null) {
                            wareSenseMessage = "[[WS]]" + wareSense.Text;
                        }
                    }
                }
            }

            /*针对WareSense扩大容量*/
            if (wareSenseMessage != null) {
                noticeEntity.notice[cnt] = wareSenseMessage;
                cnt++;
            }

            Array.Resize(ref noticeEntity.notice, cnt);
            noticeEntity.Count = cnt;

            return noticeEntity;
        }


        private void GetNoticeFromContext(string context, ref string[] notice, ref int cnt) {
            if (context == null) {
                return;
            }

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            Disease disease = null;

            string answer = context;
            string code = null;
            string hugCode = null;

            int pos1 = answer.IndexOf("[[");
            int pos2 = 0;
            if (pos1 >= 0) {
                pos2 = answer.IndexOf("]]", pos1);
            }
            while ((pos1 >= 0) && (pos2 >= 0)) {
                code = answer.Substring(pos1 + 2, pos2 - pos1 - 2);
                hugCode = "[[" + code + "]]";
                disease = (from d in DEntities.Diseases
                           where d.DiseaseID == code
                           select d).FirstOrDefault();

                /*使用Replace方法，减少数据库查询以及不必要的String类生成*/
                if (disease == null) {
                    answer = answer.Replace(hugCode, code);
                }
                else {
                    answer = answer.Replace(hugCode, disease.Name);
                    if ((Array.Find(notice, n => (n == code)) == null) && (disease.Notice != null)) {
                        if (cnt >= maxNoticeCount) {
                            return;
                        }
                        notice[cnt] = code;
                        cnt++;
                    }
                }

                pos1 = answer.IndexOf("[[");
                if (pos1 >= 0) {
                    pos2 = answer.IndexOf("]]", pos1);
                }
            }
        }

        #endregion


        #region 获取处方的用药温馨提示 GetPhysicNotice(Guid gPrescriptionID)

        public NoticeEntity GetPhysicNotice(Guid gPrescriptionID) {

            PrescriptionInfoEntity prescriptionInfoEntity = GetPrescriptionInfo(gPrescriptionID);
            NoticeEntity noticeEntity = new NoticeEntity();

            if (prescriptionInfoEntity.ErrorMessage != null) {
                noticeEntity.ErrorMessage = prescriptionInfoEntity.ErrorMessage;
                return noticeEntity;
            }

            noticeEntity.Count = prescriptionInfoEntity.Count;
            noticeEntity.notice = new string[noticeEntity.Count];

            int cnt = 0;
            for (int i = 0; i < noticeEntity.Count; i++) {
                if ((Array.Find(noticeEntity.notice, n => (n == prescriptionInfoEntity.physicID[i])) == null)) {
                    if (cnt < maxNoticeCount) {
                        noticeEntity.notice[cnt] = prescriptionInfoEntity.physicID[i];
                        cnt++;
                    }
                }
            }

            Array.Resize(ref noticeEntity.notice, cnt);
            noticeEntity.Count = cnt;

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            Physic physic;
            string sNotice;
            for (int i = 0; i < cnt; i++) {
                sNotice = noticeEntity.notice[i];
                physic = (from p in DEntities.Physics
                          where p.PhysicID == sNotice
                          select p).FirstOrDefault();
                if (physic != null) {
                    noticeEntity.notice[i] = physic.Notice;
                }
            }

            return noticeEntity;
        }

        #endregion


    }
}

