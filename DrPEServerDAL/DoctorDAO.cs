using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrPEServer.DrPEServerEntities;
using System.Data.SqlClient;
using System.Configuration;

namespace DrPEServer.DrPEServerDAL {

    public class DoctorDAO {

        #region 根据挂号信息查询UserID UserIDThroughAppointment(Guid gGuid)

        /*根据挂号信息查询UserID UserIDThroughAppointment(Guid gGuid)*/
        public string UserIDThroughAppointment(Guid gGuid) {
            /*数据库访问实例*/
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询符合要求的Appointment, 未做时间判断，是否销号判断*/
            Appointment appointment = (from ap in DEntities.Appointments
                                       where (ap.AppointmentID == gGuid)
                                       select ap).FirstOrDefault();

            /*返回UserID*/
            if (appointment == null) {
                return null;
            }
            else {
                return appointment.UserID;
            }
        }

        #endregion


        #region 进行就医消去挂号 FinishAppointment(Guid gGuid)

        /*进行就医消去挂号 FinishAppointment(Guid gGuid)*/
        public string FinishAppointment(Guid gGuid) {
            /*数据库访问实例*/
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询符合要求的Appointment, 未做时间判断*/
            Appointment appointment = (from ap in DEntities.Appointments
                                       where (ap.AppointmentID == gGuid)
                                       select ap).FirstOrDefault();

            /*若该预约不存在*/
            if (appointment == null) {
                return "No Such Appointment! @Data";
            }
            else {
                appointment.Status = "[Finished]";
            }

            /*完成消号过程*/
            try {
                DEntities.SaveChanges();
            }
            catch {
                return "Saving Denied! @Data";
            }

            return null;
        }

        #endregion


        #region 医生登录 Login(string doctorID, string password)

        /*医生登录：校验(ID,Password)是否与数据库记录匹配*/
        public DoctorInfoEntity Login(string doctorID, string password) {
            /*数据库访问实例*/
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询(DoctorID,password)对是否匹配*/
            Doctor doctor = (from d in DEntities.Doctors
                             where ((d.DoctorID == doctorID) && (d.Password == password))
                             select d).FirstOrDefault();

            /*将结果转写为Entity，仅转写必要登录信息*/
            DoctorInfoEntity doctorInfoEntity = null;
            if (doctor != null) {
                doctorInfoEntity = new DoctorInfoEntity() {
                    DoctorID = doctor.DoctorID,
                    LastName = doctor.LastName,
                    FirstName = doctor.FirstName,
                    SectionID = doctor.SectionID,
                    Designation = doctor.Designation,
                    LastLoginDate = doctor.LastLoginDate
                };

                /*更新该User的LastLoginDate域*/
                doctor.LastLoginDate = DateTime.Now;
                DEntities.SaveChanges();
            }

            return doctorInfoEntity;
        }

        #endregion

        
        #region 创建病历 CreateCase(CaseInfoEntity caseInfoEntity)

        /*创建病历并针对复查日期创建message*/
        public CaseInfoEntity CreateCase(CaseInfoEntity caseInfoEntity) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            CaseHistory newCase = new CaseHistory();
            CaseInfoEntity addedCase = new CaseInfoEntity();

            newCase.CaseID = Guid.NewGuid();
            newCase.ExaminationID = caseInfoEntity.ExaminationID;
            newCase.PrescriptionID = caseInfoEntity.PrescriptionID;
            newCase.UserID = caseInfoEntity.UserID;
            newCase.DoctorID = caseInfoEntity.DoctorID;

            Doctor doctor = (from d in DEntities.Doctors
                             where d.DoctorID == newCase.DoctorID
                             select d).FirstOrDefault();
            if (doctor == null) {
                addedCase.ErrorMessage = "Invalid DoctorID! @Data";
                return addedCase;
            }
            else {
                newCase.SectionID = doctor.SectionID;
            }

            newCase.CreatedDate = DateTime.Now;
            if (caseInfoEntity.IsDraft == false) {
                newCase.ModifiedDate = newCase.CreatedDate;
            }

            newCase.ChiefComplaint = caseInfoEntity.ChiefComplaint;
            newCase.TentativeDiagnosis = caseInfoEntity.TentativeDiagnosis;
            newCase.DifferentialDiagnosis = caseInfoEntity.DifferentialDiagnosis;
            newCase.TreatmentPlan = caseInfoEntity.TreatmentPlan;
            newCase.CountercheckDate = caseInfoEntity.CountercheckDate;

            try {
                DEntities.CaseHistories.AddObject(newCase);
                DEntities.SaveChanges();
            }
            catch {
                addedCase.ErrorMessage = "Invalid Case! @Data";
                return addedCase;
            }

            if (caseInfoEntity.CountercheckDate != null) {
                Section section = (from s in DEntities.Sections
                                   where s.SectionID == doctor.SectionID
                                   select s).FirstOrDefault();
                User user = (from u in DEntities.Users
                             where u.UserID == caseInfoEntity.UserID
                             select u).FirstOrDefault();
                DateTime lastVisit = (DateTime)newCase.CreatedDate;
                DateTime countercheckDate = (DateTime)newCase.CountercheckDate;
                DateTime currentTime = DateTime.Now;

                Message message = new Message();
                message.MessageID = Guid.NewGuid();
                message.Sender = section.HospitalID;
                message.Receiver = newCase.UserID;
                message.Date = countercheckDate.AddDays(-3).AddMinutes(currentTime.Minute).AddSeconds(currentTime.Second).AddMilliseconds(currentTime.Millisecond);
                message.Type = "H2U";
                message.Text = String.Format(
                    "Dear {0}.{1} {2},\nDuring your visit on {3}, Dr.{4} {5} ({6}) suggested you pay another visit on {7}. It might be a good idea to make an appointment before it's too late.\nSincerely,\nDr.PE",
                    (user.Gender.ToLower() == "female") ? "Ms" : "Mr",
                    user.LastName, user.FirstName,
                    lastVisit.ToLongDateString(),
                    doctor.LastName, doctor.FirstName,
                    section.Name,
                    countercheckDate.ToLongDateString());

                try {
                    DEntities.Messages.AddObject(message);
                    DEntities.SaveChanges();
                }
                catch {
                    addedCase.ErrorMessage = "Can't Create Appointment Inform! @Data";
                    return addedCase;
                }
            }

            addedCase.CaseID = newCase.CaseID;
            addedCase.ExaminationID = newCase.ExaminationID;
            addedCase.PrescriptionID = newCase.PrescriptionID;
            addedCase.UserID = newCase.UserID;
            addedCase.DoctorID = newCase.DoctorID;
            addedCase.SectionID = newCase.SectionID;
            addedCase.Date = newCase.CreatedDate;
            addedCase.ChiefComplaint = newCase.ChiefComplaint;
            addedCase.TentativeDiagnosis = newCase.TentativeDiagnosis;
            addedCase.DifferentialDiagnosis = newCase.DifferentialDiagnosis;
            addedCase.TreatmentPlan = newCase.TreatmentPlan;
            addedCase.CountercheckDate = newCase.CountercheckDate;
            if (newCase.ModifiedDate == null) {
                addedCase.IsDraft = true;
            }
            else {
                addedCase.IsDraft = false;
            }

            return addedCase;
        }

        #endregion

        
        #region 修改病历 ModifyCase(CaseInfoEntity newCase)

        /*修改病历*/
        public CaseInfoEntity ModifyCase(CaseInfoEntity newCase) {
            CaseHistory oldCase = new CaseHistory();
            CaseInfoEntity modifiedCase = new CaseInfoEntity();

            if (newCase.CaseID == null) {
                modifiedCase.ErrorMessage = "Case GUID Missing! @Data";
                return modifiedCase;
            }

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            oldCase = (from c in DEntities.CaseHistories
                       where ((c.CaseID == newCase.CaseID) && (c.DoctorID == newCase.DoctorID))
                       select c).FirstOrDefault();
            if (newCase == null) {
                modifiedCase.ErrorMessage = "Invalid Case GUID! @Data";
                return modifiedCase;
            }
            if (oldCase.ModifiedDate != null) {
                modifiedCase.ErrorMessage = "Modification Denied! @Data";
                return modifiedCase;
            }

            if (oldCase.ExaminationID == null) {
                oldCase.ExaminationID = newCase.ExaminationID;
            }
            if (oldCase.PrescriptionID == null) {
                oldCase.PrescriptionID = newCase.PrescriptionID;
            }

            if (oldCase.ChiefComplaint == null) {
                oldCase.ChiefComplaint = newCase.ChiefComplaint;
            }
            if (oldCase.TentativeDiagnosis == null) {
                oldCase.TentativeDiagnosis = newCase.TentativeDiagnosis;
            }
            if (oldCase.DifferentialDiagnosis == null) {
                oldCase.DifferentialDiagnosis = newCase.DifferentialDiagnosis;
            }
            if (oldCase.TreatmentPlan == null) {
                oldCase.TreatmentPlan = newCase.TreatmentPlan;
            }

            bool IsSent = oldCase.CountercheckDate.HasValue;
            if (IsSent == false) {
                oldCase.CountercheckDate = newCase.CountercheckDate;
            }

            if (newCase.IsDraft == false) {
                oldCase.ModifiedDate = DateTime.Now;
            }

            try {
                DEntities.SaveChanges();
            }
            catch {
                modifiedCase.ErrorMessage = "Invalid Case! @Data";
                return modifiedCase;
            }

            if ((IsSent == false) && (oldCase.CountercheckDate.HasValue)) {
                Doctor doctor = (from d in DEntities.Doctors
                                 where d.DoctorID == oldCase.DoctorID
                                 select d).FirstOrDefault();
                Section section = (from s in DEntities.Sections
                                   where s.SectionID == doctor.SectionID
                                   select s).FirstOrDefault();
                User user = (from u in DEntities.Users
                             where u.UserID == oldCase.UserID
                             select u).FirstOrDefault();
                DateTime lastVisit = (DateTime)oldCase.CreatedDate;
                DateTime countercheckDate = (DateTime)oldCase.CountercheckDate;
                DateTime currentTime = DateTime.Now;

                Message message = new Message();
                message.MessageID = Guid.NewGuid();
                message.Sender = section.HospitalID;
                message.Receiver = newCase.UserID;
                message.Date = countercheckDate.AddDays(-3).AddMinutes(currentTime.Minute).AddSeconds(currentTime.Second).AddMilliseconds(currentTime.Millisecond);
                message.Type = "H2U";
                message.Text = String.Format(
                    "Dear {0}.{1} {2},\nDuring your visit on {3}, Dr.{4} {5} ({6}) suggested you pay another visit on {7}. It might be a good idea to make an appointment before it's too late.\nSincerely,\nDr.PE",
                    (user.Gender.ToLower() == "female") ? "Ms" : "Mr",
                    user.LastName, user.FirstName,
                    lastVisit.ToLongDateString(),
                    doctor.LastName, doctor.FirstName,
                    section.Name,
                    countercheckDate.ToLongDateString());

                try {
                    DEntities.Messages.AddObject(message);
                    DEntities.SaveChanges();
                }
                catch {
                    modifiedCase.ErrorMessage = "Can't Create Appointment Inform! @Data";
                    return modifiedCase;
                }
            }

            modifiedCase.CaseID = oldCase.CaseID;
            modifiedCase.ExaminationID = oldCase.ExaminationID;
            modifiedCase.PrescriptionID = oldCase.PrescriptionID;
            modifiedCase.UserID = oldCase.UserID;
            modifiedCase.DoctorID = oldCase.DoctorID;
            modifiedCase.SectionID = oldCase.SectionID;
            modifiedCase.Date = oldCase.CreatedDate;
            modifiedCase.ChiefComplaint = oldCase.ChiefComplaint;
            modifiedCase.TentativeDiagnosis = oldCase.TentativeDiagnosis;
            modifiedCase.DifferentialDiagnosis = oldCase.DifferentialDiagnosis;
            modifiedCase.TreatmentPlan = oldCase.TreatmentPlan;
            modifiedCase.CountercheckDate = oldCase.CountercheckDate;

            return modifiedCase;
        }

        #endregion

        
        #region 创建处方单 CreatePrescription(PrescriptionInfoEntity prescriptionInfoEntity) RETURN[string]

        public string CreatePrescription(PrescriptionInfoEntity prescriptionInfoEntity) {
            Prescription  newPrescription = new Prescription();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            newPrescription.Detail = "";
            for (int i = 0; i < prescriptionInfoEntity.Count; i++) {
                string sPhysicID = prescriptionInfoEntity.physicID[i];
                Physic physic = (from p in DEntities.Physics
                                 where p.PhysicID == sPhysicID
                                 select p).FirstOrDefault();
                if (physic == null) {
                    return "Invalid PhysicID! @Data";
                }
                newPrescription.Detail += prescriptionInfoEntity.physicID[i] + ":" + prescriptionInfoEntity.number[i].ToString() + ";";
            }
            newPrescription.PrescriptionID = Guid.NewGuid();

            try {
                DEntities.Prescriptions.AddObject(newPrescription);
                DEntities.SaveChanges();
            }
            catch {
                return "Invalid Case! @Data";
            }

            return String.Format("EA{0}", newPrescription.PrescriptionID.ToString());
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

        
        #region 获取患者信息 GetUserInfo(string userID)

        /*获取用户\患者信息*/
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
                    UserID = user.UserID,
                    LastName = user.LastName,
                    FirstName = user.FirstName,
                    Nationality = user.Nationality,
                    Gender = user.Gender,
                    ABO = user.ABO,
                    Rh = user.Rh,
                    Birthplace = user.Birthplace,
                    Birthday = user.Birthday,
                    Deathplace = user.Deathplace,
                    Deathday = user.Deathday,
                    Balance = user.Balance,
                    //LastLoginDate = user.LastLoginDate,
                    City = user.City,
                    Address = user.Address,
                    Phone = user.Phone,
                    Email = user.Email,
                    EmergencyContactPerson = user.EmergencyContactPerson
                };
            }

            return userInfoEntity;
        }

        #endregion

        
        #region 获取指定病历 GetCaseInfo(Guid caseID, bool showICD) & 翻译ICD-10编码

        /*获取指定病历的信息*/
        public CaseInfoEntity GetCaseInfo(Guid caseID, bool showICD) {

            CaseInfoEntity caseInfoEntity = new CaseInfoEntity();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*根据编号查找该病历*/
            CaseHistory caseInfo = (from c in DEntities.CaseHistories
                                    where c.CaseID == caseID
                                    select c).FirstOrDefault();
            if (caseInfo == null) {
                caseInfoEntity.ErrorMessage = "Invalid Case GUID! @Data";
                return caseInfoEntity;
            }

            /*复制病历部分信息*/
            caseInfoEntity.CaseID = caseInfo.CaseID;
            caseInfoEntity.ExaminationID = caseInfo.ExaminationID;
            caseInfoEntity.PrescriptionID = caseInfo.PrescriptionID;
            caseInfoEntity.UserID = caseInfo.UserID;
            caseInfoEntity.DoctorID = caseInfo.DoctorID;
            caseInfoEntity.SectionID = caseInfo.SectionID;
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
                caseInfoEntity.ChiefComplaint = TranslateICD(caseInfo.ChiefComplaint);
                caseInfoEntity.TentativeDiagnosis = TranslateICD(caseInfo.TentativeDiagnosis);
                caseInfoEntity.DifferentialDiagnosis = TranslateICD(caseInfo.DifferentialDiagnosis);
                caseInfoEntity.TreatmentPlan = TranslateICD(caseInfo.TreatmentPlan);
            }
            else {
                caseInfoEntity.ChiefComplaint = caseInfo.ChiefComplaint;
                caseInfoEntity.TentativeDiagnosis = caseInfo.TentativeDiagnosis;
                caseInfoEntity.DifferentialDiagnosis = caseInfo.DifferentialDiagnosis;
                caseInfoEntity.TreatmentPlan = caseInfo.TreatmentPlan;
            }

            return caseInfoEntity;
        }


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

        
        #region 获取某医生撰写的病历 RetrieveDoctorCase(string doctorID, DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD)

        /*获取某医生在某区间内所有已完成的病历(isDraft为false时)，或未完成的病历*/
        public CaseHistoryEntity RetrieveDoctorCase(string doctorID, DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD) {

            CaseHistoryEntity caseHistoryEntity = new CaseHistoryEntity();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            IQueryable<CaseHistory> cases = null;

            /*根据时间和医生ID查找病历*/
            if (isDraft == false) {
                cases = from c in DEntities.CaseHistories
                        where ((c.ModifiedDate != null) && (startDate <= c.ModifiedDate) && (c.ModifiedDate <= endDate) && (c.DoctorID == doctorID))
                        orderby c.ModifiedDate
                        select c;
            }
            else {
                cases = from c in DEntities.CaseHistories
                        where ((c.ModifiedDate == null) && (startDate <= c.CreatedDate) && (c.CreatedDate <= endDate) && (c.DoctorID == doctorID))
                        orderby c.CreatedDate
                        select c;
            }

            /*处理无结果的情况*/
            int caseCount = cases.Count();
            if (caseCount <= 0) {
                caseHistoryEntity.ErrorMessage = String.Format("No Case Histories Created by Doctor {2} Between {0} and {1}! @Data",
                                                                startDate, endDate, doctorID);
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
                if (c.ModifiedDate == null) {
                    caseHistoryEntity.caseInfoEntity[cnt].Date = c.CreatedDate;
                    caseHistoryEntity.caseInfoEntity[cnt].IsDraft = true;
                }
                else {
                    caseHistoryEntity.caseInfoEntity[cnt].Date = c.ModifiedDate;
                    caseHistoryEntity.caseInfoEntity[cnt].IsDraft = false;
                }

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



        ///*获取某医生在某区间内所有未完成的病历*/
        //public CaseHistoryEntity RetrieveMyDraft(string doctorID, DateTime? startDate, DateTime? endDate) {

        //    return null;
        //}

        #endregion

        
        #region 获取某科室全体医生撰写的病历 RetrieveSectionCase(string sectionID, DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD)

        /*获取某科室在某区间内所有已完成的病历(isDraft为false时)，或未完成的病历*/
        public CaseHistoryEntity RetrieveSectionCase(string sectionID, DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD) {
            CaseHistoryEntity caseHistoryEntity = new CaseHistoryEntity();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            IQueryable<CaseHistory> cases = null;

            /*根据时间和科室ID查找病历*/
            if (isDraft == false) {
                cases = from c in DEntities.CaseHistories
                        where ((c.ModifiedDate != null) && (startDate <= c.ModifiedDate) && (c.ModifiedDate <= endDate) && (c.SectionID == sectionID))
                        orderby c.ModifiedDate
                        select c;
            }
            else {
                cases = from c in DEntities.CaseHistories
                        where ((c.ModifiedDate == null) && (startDate <= c.CreatedDate) && (c.CreatedDate <= endDate) && (c.SectionID == sectionID))
                        orderby c.CreatedDate
                        select c;
            }

            /*处理无结果的情况*/
            int caseCount = cases.Count();
            if (caseCount <= 0) {
                caseHistoryEntity.ErrorMessage = String.Format("No Case Histories Created in Section {2} Modified Between {0} and {1}! @Data",
                                                                startDate, endDate, sectionID);
                return caseHistoryEntity;
            }

            /*逐条进行转录*/
            int cnt = 0;
            //caseHistoryEntity = new CaseHistoryEntity();
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

        
        #region 医生撰写邮件回复用户 MessageCompose(string senderID, string receiverID, string text)

        /*医生撰写邮件Doctor to User*/
        public string MessageCompose(string senderID, string receiverID, string text) {
            Message message = new Message();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            Doctor doctor = (from d in DEntities.Doctors
                             where d.DoctorID == senderID
                             select d).FirstOrDefault();
            if (doctor == null) {
                return "Invalid Sender DoctorID!";
            }

            User user = (from u in DEntities.Users
                         where u.UserID == receiverID
                         select u).FirstOrDefault();
            if (user == null) {
                return "Invalid Receiver UserID!";
            }

            message.MessageID = Guid.NewGuid();
            message.Sender = doctor.DoctorID;
            message.Receiver = user.UserID;
            message.Type = "D2U";
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

        
        #region 医生收取收件箱 MessageInbox(string doctorID, DateTime? startDate, DateTime? endDate)


        /*医生收取收件箱*/
        public AllMessageEntity MessageInbox(string doctorID, DateTime? startDate, DateTime? endDate) {
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            AllMessageEntity allMessageEntity = new AllMessageEntity();

            /*提交查询*/
            Doctor doctor = (from d in DEntities.Doctors
                             where d.DoctorID == doctorID
                             select d).FirstOrDefault();
            if (doctor == null) {
                allMessageEntity.ErrorMessage = "Invalid Sender DoctorID! @Data";
                return allMessageEntity;
            }

            var messages = from m in DEntities.Messages
                           where ((m.Receiver == doctorID) && (startDate <= m.Date) && (m.Date <= endDate))
                           orderby m.Date descending
                           select m;

            /*处理无结果的情况*/
            int messageCount = messages.Count();
            if (messageCount <= 0) {
                allMessageEntity.ErrorMessage = String.Format("Doctor {0} Received No Messages Between {1} and {2}! @Data",
                                                                doctor.DoctorID, startDate, endDate);
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

        
        #region 医生查看发件箱 MessageSent(string doctorID, DateTime? startDate, DateTime? endDate)


        /*医生查看发件箱*/
        public AllMessageEntity MessageSent(string doctorID, DateTime? startDate, DateTime? endDate) {
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            AllMessageEntity allMessageEntity = new AllMessageEntity();

            /*提交查询*/
            Doctor doctor = (from d in DEntities.Doctors
                             where d.DoctorID == doctorID
                             select d).FirstOrDefault();
            if (doctor == null) {
                allMessageEntity.ErrorMessage = "Invalid Sender DoctorID! @Data";
                return allMessageEntity;
            }

            var messages = from m in DEntities.Messages
                           where ((m.Sender == doctorID) && (startDate <= m.Date) && (m.Date <= endDate))
                           orderby m.Date descending
                           select m;

            /*处理无结果的情况*/
            int messageCount = messages.Count();
            if (messageCount <= 0) {
                allMessageEntity.ErrorMessage = String.Format("Doctor {0} Sent No Messages Between {1} and {2}! @Data",
                                                                doctor.DoctorID, startDate, endDate);
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

        
        #region 医生设置日常预约信息 SetDailySchedule(string doctorID, DayOfWeek dayOfWeek, int am)

        public string SetDailySchedule(string doctorID, DayOfWeek dayOfWeek, int am) {
            string setting = null;

            switch (dayOfWeek) {
                case DayOfWeek.Sunday:
                    setting = "Sun";
                    break;
                case DayOfWeek.Monday:
                    setting = "Mon";
                    break;
                case DayOfWeek.Tuesday:
                    setting = "Tue";
                    break;
                case DayOfWeek.Wednesday:
                    setting = "Wed";
                    break;
                case DayOfWeek.Thursday:
                    setting = "Thu";
                    break;
                case DayOfWeek.Friday:
                    setting = "Fri";
                    break;
                case DayOfWeek.Saturday:
                    setting = "Sat";
                    break;
                default:
                    return "Invalid Weekday! @Data";
            }

            if (am > 0) {
                setting += ":A";
            }
            else {
                setting += ":P";
            }

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            Doctor doctor = (from d in DEntities.Doctors
                             where d.DoctorID == doctorID
                             select d).FirstOrDefault();
            if (doctor == null) {
                return "Invalid DoctorID!";
            }

            Schedule schedule = (from s in DEntities.Schedules
                                 where s.DoctorID == doctorID
                                 select s).FirstOrDefault();

            if (schedule == null) {
                schedule = new Schedule();
                schedule.DoctorID = doctor.DoctorID;
                schedule.Weekday = setting;
                schedule.LastCheck = DateTime.Now;
                DEntities.Schedules.AddObject(schedule);
            }
            else {
                schedule.Weekday = setting;
                schedule.LastCheck = DateTime.Now;
            }

            try {
                DEntities.SaveChanges();
            }
            catch {
                return "Set Daily Schedule Failed! @Data";
            }

            return null;
        }

        #endregion


        #region 医生设置额外预约信息 SetAdditionSchedule(string doctorID, DateTime? date)

        public string SetAdditionSchedule(string doctorID, DateTime? date) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            Doctor doctor = (from d in DEntities.Doctors
                             where d.DoctorID == doctorID
                             select d).FirstOrDefault();
            if (doctor == null) {
                return "Invalid DoctorID! @Data";
            }

            if (date == null) {
                return "Paremeter Date is Null! @Data";
            }

            DateTime newDate = (DateTime)date;
            DateTime dDate = newDate.Date;
            if (newDate.CompareTo(dDate.AddHours(12)) >= 0) {
                dDate = dDate.AddHours(12);
            }

            Schedule schedule = (from s in DEntities.Schedules
                                 where s.DoctorID == doctorID
                                 select s).FirstOrDefault();

            if (schedule == null) {
                schedule = new Schedule();
                schedule.DoctorID = doctor.DoctorID;
                schedule.Addition = dDate;
                schedule.LastCheck = DateTime.Now;
                DEntities.Schedules.AddObject(schedule);
            }
            else {
                if (schedule.Exception != null) {
                    if (((DateTime)schedule.Exception) == dDate) {
                        return "Conflict with Exceptional Date! @Data";
                    }
                }

                schedule.Addition = dDate;
                schedule.LastCheck = DateTime.Now;
            }

            try {
                DEntities.SaveChanges();
            }
            catch {
                return "Set Additional Schedule Failed! @Data";
            }

            return null;
        }

        #endregion


        #region 医生设置例外预约信息 SetExceptionSchedule(string doctorID, DateTime? date)

        public string SetExceptionSchedule(string doctorID, DateTime? date) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            Doctor doctor = (from d in DEntities.Doctors
                             where d.DoctorID == doctorID
                             select d).FirstOrDefault();
            if (doctor == null) {
                return "Invalid DoctorID! @Data";
            }

            if (date == null) {
                return "Paremeter Date is Null! @Data";
            }

            DateTime newDate = (DateTime)date;
            DateTime dDate = newDate.Date;
            if (newDate.CompareTo(dDate.AddHours(12)) >= 0) {
                dDate = dDate.AddHours(12);
            }

            Schedule schedule = (from s in DEntities.Schedules
                                 where s.DoctorID == doctorID
                                 select s).FirstOrDefault();

            if (schedule == null) {
                schedule = new Schedule();
                schedule.DoctorID = doctor.DoctorID;
                schedule.Exception = dDate;
                schedule.LastCheck = DateTime.Now;
                DEntities.Schedules.AddObject(schedule);
            }
            else {
                if (schedule.Addition != null) {
                    if (((DateTime)schedule.Addition) == dDate) {
                        return "Conflict with Additonal Date! @Data";
                    }
                }

                schedule.Exception = dDate;
                schedule.LastCheck = DateTime.Now;
            }

            try {
                DEntities.SaveChanges();
            }
            catch {
                return "Set Exceptional Schedule Failed! @Data";
            }

            return null;
        }

        #endregion

        
        #region 查看当前时间段多少人选择 GetMyAppointment(string doctorID)

        /*查看当前时间段多少人选择*/
        public AllAppointmentEntity GetMyAppointment(string doctorID) {
            AllAppointmentEntity allAppointmentEntity = new AllAppointmentEntity();
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            Doctor doctor = (from d in DEntities.Doctors
                             where d.DoctorID == doctorID
                             select d).FirstOrDefault();
            if (doctor == null) {
                allAppointmentEntity.ErrorMessage = "312 Invalid DoctorID! @Data";
                return allAppointmentEntity;
            }

            DateTime newDate = DateTime.Now;
            DateTime bedTime = newDate.Date;
            if (newDate.CompareTo(bedTime.AddHours(12)) >= 0) {
                bedTime = bedTime.AddHours(12);
            }

            var appointments = (from ap in DEntities.Appointments
                                where (ap.DoctorID == doctorID) && (ap.Date == bedTime)
                                orderby ap.Rank
                                select ap);

            int appointmentCount = appointments.Count();
            allAppointmentEntity.Count = appointmentCount;

            if (appointmentCount <= 0) {
                allAppointmentEntity.ErrorMessage = "313 No Appointment! @Data";
            }
            else {
                allAppointmentEntity.appointment = new AppointmentEntity[appointmentCount];
                int cnt = 0;
                foreach (var app in appointments) {
                    allAppointmentEntity.appointment[cnt]           = new AppointmentEntity();
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

    }
}
