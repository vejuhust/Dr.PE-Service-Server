using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DrPEServer.DrPEServerEntities;
using DrPEServer.DrPEServerLogic;

namespace DrPEServer.DrPEServerService {

    class DoctorService : IDoctorService {
        /*Logic Layer实例*/
        DoctorLogic doctorLogic = new DoctorLogic();


        #region 进行就医消去挂号 FinishAppointment(string sGuid)

        /*进行就医消去挂号 FinishAppointment(string sGuid)*/
        public string FinishAppointment(string sGuid) {
            Guid gGuid = Guid.Empty;
            try {
                gGuid = new Guid(sGuid);
            }
            catch {
                return "Invalid sGuid! @Service";
            }
            return doctorLogic.FinishAppointment(gGuid);
        }

        #endregion


        #region 根据挂号信息查询UserID UserIDThroughAppointment(string sGuid)

        public string UserIDThroughAppointment(string sGuid) {
            Guid gGuid = Guid.Empty;
            try {
                gGuid = new Guid(sGuid);
            }
            catch {
                return null;
            }
            return doctorLogic.UserIDThroughAppointment(gGuid);
        }

        #endregion


        #region 医生登录 Login(string doctorID, string password)

        /*医生登录：若ID和Password均不为空，则转发至DL，将结果翻译为数据契约*/
        public DoctorInfo Login(string doctorID, string password) {

            DoctorInfoEntity doctorInfoEntity = null;

            if (doctorID == null) {
                doctorInfoEntity = new DoctorInfoEntity();
                doctorInfoEntity.ErrorMessage = "301 Empty DoctorID! @Service";
            }
            else if (password == null) {
                doctorInfoEntity = new DoctorInfoEntity();
                doctorInfoEntity.ErrorMessage = "302 Empty password! @Service";
            }
            else {
                doctorInfoEntity = doctorLogic.Login(doctorID, password);
            }

            DoctorInfo doctorInfo = new DoctorInfo();
            TranslateDoctorInfoEntityToDoctorInfoContractData(doctorInfoEntity, doctorInfo);

            return doctorInfo;
        }

        #endregion


        #region 将DoctorInfo对应的Entity翻译为数据契约 TranslateDoctorInfoEntityToDoctorInfoContractData()

        /*将DoctorInfo对应的Entity翻译为数据契约*/
        private void TranslateDoctorInfoEntityToDoctorInfoContractData(
            DoctorInfoEntity doctorInfoEntity,
            DoctorInfo doctorInfo) {
            doctorInfo.ErrorMessage = doctorInfoEntity.ErrorMessage;

            doctorInfo.SectionID = doctorInfoEntity.SectionID;
            doctorInfo.DoctorID = doctorInfoEntity.DoctorID;
            doctorInfo.UserID = doctorInfoEntity.UserID;
            doctorInfo.LastName = doctorInfoEntity.LastName;
            doctorInfo.FirstName = doctorInfoEntity.FirstName;
            doctorInfo.Designation = doctorInfoEntity.Designation;
            doctorInfo.Resume = doctorInfoEntity.Resume;
            doctorInfo.Phone = doctorInfoEntity.Phone;
            doctorInfo.Fax = doctorInfoEntity.Fax;
            doctorInfo.Email = doctorInfoEntity.Email;
            doctorInfo.Vocation = doctorInfoEntity.Vocation;
            doctorInfo.Portrait = doctorInfoEntity.Portrait;
            doctorInfo.LastLoginDate = doctorInfoEntity.LastLoginDate;
        }

        #endregion


        #region 查看当前时间段多少人选择 GetMyAppointment()

        /*查看当前时间段多少人选择*/
        public AllAppointment GetMyAppointment() {
            AllAppointmentEntity allAppointmentEntity = doctorLogic.GetMyAppointment();
            AllAppointment allAppointment = new AllAppointment();
            TranslateAllAppointmentEntityToAllAppointmentContractData(allAppointmentEntity, allAppointment);
            return allAppointment;
        }

        #endregion


        #region 翻译AllAppointment的Entity为对应的数据契约 TranslateAllAppointmentEntityToAllAppointmentContractData

        /*翻译AllAppointment的Entity为对应的数据契约*/
        private void TranslateAllAppointmentEntityToAllAppointmentContractData(
            AllAppointmentEntity allAppointmentEntity,
            AllAppointment allAppointment) {
            allAppointment.ErrorMessage = allAppointmentEntity.ErrorMessage;
            allAppointment.Count = allAppointmentEntity.Count;
            allAppointment.appointment = new Appointment[allAppointment.Count];
            for (int i = 0; i < allAppointment.Count; i++) {
                allAppointment.appointment[i] = new Appointment();
                allAppointment.appointment[i].sGuid     = allAppointmentEntity.appointment[i].gGuid.ToString();
                allAppointment.appointment[i].Date      = allAppointmentEntity.appointment[i].Date;
                allAppointment.appointment[i].UserID    = allAppointmentEntity.appointment[i].UserID;
                allAppointment.appointment[i].DoctorID  = allAppointmentEntity.appointment[i].DoctorID;
                allAppointment.appointment[i].Rank      = allAppointmentEntity.appointment[i].Rank;
                allAppointment.appointment[i].Finished  = allAppointmentEntity.appointment[i].Finished;
            }
        }

        #endregion


        #region 获取用户资料 GetUserInfo(string doctorID)

        /*获取用户资料：将不为空的ID请求转发至DL，并将反馈结果翻译为数据契约*/
        public UserInfo GetUserInfo(string userID) {

            UserInfoEntity userInfoEntity = null;

            if (userID == null) {
                userInfoEntity = new UserInfoEntity();
                userInfoEntity.ErrorMessage = "314 Empty UserID! @Service";
            }
            else {
                userInfoEntity = doctorLogic.GetUserInfo(userID);
            }

            UserInfo userInfo = new UserInfo();
            TranslateUserInfoEntityToUserInfoContractData(userInfoEntity, userInfo);

            return userInfo;
        }

        #endregion


        #region 将UserInfo对应的Entity翻译为数据契约 TranslateUserInfoEntityToUserInfoContractData

        /*将UserInfo对应的Entity翻译为数据契约*/
        private void TranslateUserInfoEntityToUserInfoContractData(
            UserInfoEntity userInfoEntity,
            UserInfo userInfo) {

            userInfo.ErrorMessage = userInfoEntity.ErrorMessage;

            userInfo.UserID = userInfoEntity.UserID;
            userInfo.LastName = userInfoEntity.LastName;
            userInfo.FirstName = userInfoEntity.FirstName;
            userInfo.Nationality = userInfoEntity.Nationality;
            userInfo.Gender = userInfoEntity.Gender;
            userInfo.ABO = userInfoEntity.ABO;
            userInfo.Rh = userInfoEntity.Rh;
            userInfo.Birthplace = userInfoEntity.Birthplace;
            userInfo.Birthday = userInfoEntity.Birthday;
            userInfo.Deathplace = userInfoEntity.Deathplace;
            userInfo.Deathday = userInfoEntity.Deathday;
            userInfo.Balance = userInfoEntity.Balance;
            userInfo.LastLoginDate = userInfoEntity.LastLoginDate;
            userInfo.City = userInfoEntity.City;
            userInfo.Address = userInfoEntity.Address;
            userInfo.Phone = userInfoEntity.Phone;
            userInfo.Email = userInfoEntity.Email;
            userInfo.EmergencyContactPerson = userInfoEntity.EmergencyContactPerson;
        }

        #endregion


        #region 创建病历并针对复查日期创建message

        /*创建病历并针对复查日期创建message*/
        public CaseInfo CreateCase(CaseInfo caseInfo) {
            if (caseInfo == null) {
                CaseInfo newCaseInfo = new CaseInfo();
                newCaseInfo.ErrorMessage = "Empty Case Info! @Service";
                return newCaseInfo;
            }
            else {
                caseInfo.ErrorMessage = null;
                CaseInfoEntity caseInfoEntity = new CaseInfoEntity();
                TranslateCaseInfoToCaseInfoEntityContractData(caseInfo, caseInfoEntity);
                if (caseInfoEntity.ErrorMessage != null) {
                    caseInfo.ErrorMessage = caseInfoEntity.ErrorMessage;
                }
                else {
                    CaseInfoEntity newCaseInfoEntity = doctorLogic.CreateCase(caseInfoEntity);
                    caseInfo = new CaseInfo();
                    TranslateCaseInfoEntityToCaseInfoContractData(newCaseInfoEntity, caseInfo);
                }
                return caseInfo;
            }
        }

        #endregion


        #region 将CaseInfo对应的数据契约翻译为数据契约Entity TranslateCaseInfoEntityToCaseInfoContractData()

        /*将CaseInfo对应的数据契约翻译为Entity*/
        private void TranslateCaseInfoToCaseInfoEntityContractData(
            CaseInfo        caseInfo,
            CaseInfoEntity  caseInfoEntity) {
            caseInfoEntity.ErrorMessage         = caseInfo.ErrorMessage;
            if (caseInfo.CaseID != null) {
                try {
                    caseInfoEntity.CaseID = new Guid(caseInfo.CaseID);
                }
                catch {
                    caseInfoEntity.ErrorMessage = "Invalid CaseID! @Service";
                }
            }
            if (caseInfo.ExaminationID != null) {
                 try {
                     caseInfoEntity.ExaminationID = new Guid(caseInfo.ExaminationID);
                }
                catch {
                    caseInfoEntity.ErrorMessage = "Invalid ExaminationID! @Service";
                }
            }
            if (caseInfo.PrescriptionID != null) {
                try {
                    caseInfoEntity.PrescriptionID = new Guid(caseInfo.PrescriptionID);
                }
                catch {
                    caseInfoEntity.ErrorMessage = "Invalid PrescriptionID! @Service";
                }
            }
            caseInfoEntity.UserID               = caseInfo.UserID;
            caseInfoEntity.DoctorID             = caseInfo.DoctorID;
            caseInfoEntity.SectionID            = caseInfo.SectionID;
            caseInfoEntity.Date                 = caseInfo.Date;
            caseInfoEntity.ChiefComplaint       = caseInfo.ChiefComplaint;
            caseInfoEntity.TentativeDiagnosis   = caseInfo.TentativeDiagnosis;
            caseInfoEntity.DifferentialDiagnosis = caseInfo.DifferentialDiagnosis;
            caseInfoEntity.TreatmentPlan        = caseInfo.TreatmentPlan;
            caseInfoEntity.CountercheckDate     = caseInfo.CountercheckDate;
            caseInfoEntity.IsDraft              = caseInfo.IsDraft;
        }

        #endregion


        #region 将CaseInfo对应的Entity翻译为数据契约 TranslateCaseInfoEntityToCaseInfoContractData()

        /*将CaseInfo对应的Entity翻译为数据契约*/
        private void TranslateCaseInfoEntityToCaseInfoContractData(
            CaseInfoEntity caseInfoEntity,
            CaseInfo caseInfo) {
            caseInfo.ErrorMessage       = caseInfoEntity.ErrorMessage;
            if (caseInfoEntity.CaseID != null) {
                caseInfo.CaseID = caseInfoEntity.CaseID.ToString();
            }
            if (caseInfoEntity.ExaminationID != null) {
                caseInfo.ExaminationID = caseInfoEntity.ExaminationID.ToString();
            }
            if (caseInfoEntity.PrescriptionID != null) {
                caseInfo.PrescriptionID = caseInfoEntity.PrescriptionID.ToString();
            }
            caseInfo.UserID             = caseInfoEntity.UserID;
            caseInfo.DoctorID           = caseInfoEntity.DoctorID;
            caseInfo.SectionID          = caseInfoEntity.SectionID;
            caseInfo.Date               = caseInfoEntity.Date;
            caseInfo.ChiefComplaint     = caseInfoEntity.ChiefComplaint;
            caseInfo.TentativeDiagnosis = caseInfoEntity.TentativeDiagnosis;
            caseInfo.DifferentialDiagnosis = caseInfoEntity.DifferentialDiagnosis;
            caseInfo.TreatmentPlan      = caseInfoEntity.TreatmentPlan;
            caseInfo.CountercheckDate   = caseInfoEntity.CountercheckDate;
            caseInfo.IsDraft            = caseInfoEntity.IsDraft;
        }

        #endregion


        #region 修改病历 ModifyCase(CaseInfoEntity newCase)

        /*修改病历*/
        public CaseInfo ModifyCase(CaseInfo newCase) {
            if (newCase == null) {
                CaseInfo newCaseInfo = new CaseInfo();
                newCaseInfo.ErrorMessage = "Empty Case Info! @Service";
                return newCaseInfo;
            }
            else {
                newCase.ErrorMessage = null;
                CaseInfoEntity caseInfoEntity = new CaseInfoEntity();
                TranslateCaseInfoToCaseInfoEntityContractData(newCase, caseInfoEntity);
                if (caseInfoEntity.ErrorMessage != null) {
                    newCase.ErrorMessage = caseInfoEntity.ErrorMessage;
                }
                else {
                    CaseInfoEntity newCaseInfoEntity = doctorLogic.ModifyCase(caseInfoEntity);
                    newCase = new CaseInfo();
                    TranslateCaseInfoEntityToCaseInfoContractData(newCaseInfoEntity, newCase);
                }
                return newCase;
            }
        }

        #endregion


        #region 获取指定用户在某区间内所有完成的病历 GetCaseHistory(DateTime? startDate, DateTime? endDate, bool showICD)

        /*获取指定用户在某区间内所有完成的病历*/
        public CaseHistory GetCaseHistory(string userID, DateTime? startDate, DateTime? endDate, bool showICD) {
            CaseHistory   caseHistory = new CaseHistory();
            if (startDate == null) {
                caseHistory.ErrorMessage = "ccc Empty Start Date! @Service";
                return caseHistory;
            }
            else if (endDate == null) {
                caseHistory.ErrorMessage = "yyy Empty End Date! @Service";
                return caseHistory;
            }
            else {
                CaseHistoryEntity  caseHistoryEntity = doctorLogic.GetCaseHistory(userID, startDate, endDate, showICD);
                TranslateCaseHistoryEntityToCaseHistoryContractData(caseHistoryEntity, caseHistory);
                return caseHistory;
            }
        }

        #endregion


        #region 将CaseHistory对应的Entity翻译为数据契约 TranslateCaseHistoryEntityToCaseHistoryContractData()

        /*将CaseHistory对应的Entity翻译为数据契约*/
        private void TranslateCaseHistoryEntityToCaseHistoryContractData(
            CaseHistoryEntity caseHistoryEntity,
            CaseHistory caseHistory) {
            caseHistory.ErrorMessage = caseHistoryEntity.ErrorMessage;
            caseHistory.Count = caseHistoryEntity.Count;

            if (caseHistory.Count > 0) {
                caseHistory.caseInfo = new CaseInfo[caseHistory.Count];
                for (int i = 0; i < caseHistory.Count; i++) {
                    caseHistory.caseInfo[i] = new CaseInfo();
                    TranslateCaseInfoEntityToCaseInfoContractData(
                        caseHistoryEntity.caseInfoEntity[i],
                        caseHistory.caseInfo[i]);
                }
            }
        }

        #endregion


        #region 获取指定病历的信息 GetLastCaseInfo(string sCaseID, bool showICD)

        /*获取指定病历的信息*/
        public CaseInfo GetCaseInfo(string sCaseID, bool showICD) {
            CaseInfo    caseInfo =  new CaseInfo();
            Guid        gCaseID;
            try {
                gCaseID = new Guid(sCaseID);
            }
            catch {
                caseInfo.ErrorMessage = "Invalid ID! @Service";
                return caseInfo;
            }

            CaseInfoEntity  caseInfoEntity = doctorLogic.GetCaseInfo(gCaseID, showICD);
            TranslateCaseInfoEntityToCaseInfoContractData(caseInfoEntity, caseInfo);
            return caseInfo;
        }

        #endregion


        #region 读取处方单 GetPrescriptionInfo(Guid gPrescriptionID)

        /*读取处方单*/
        public PrescriptionInfo GetPrescriptionInfo(string sPrescriptionID) {
            PrescriptionInfo prescriptionInfo = new PrescriptionInfo();
            Guid gPrescriptionID = Guid.Empty;
            try {
                gPrescriptionID = new Guid(sPrescriptionID);
            }
            catch {
                prescriptionInfo.ErrorMessage = "Invalid ID! @Service";
                return prescriptionInfo;
            }
            PrescriptionInfoEntity prescriptionInfoEntity = doctorLogic.GetPrescriptionInfo(gPrescriptionID);
            TranslatePrescriptionInfoEntityToPrescriptionInfoContractData(prescriptionInfoEntity, prescriptionInfo);
            return prescriptionInfo;
        }

        #endregion


        #region 将PrescriptionInfo对应的Entity翻译为数据契约 TranslatePrescriptionInfoEntityToPrescriptionInfoContractData

        /*将PrescriptionInfo对应的Entity翻译为数据契约*/
        private void TranslatePrescriptionInfoEntityToPrescriptionInfoContractData(
            PrescriptionInfoEntity prescriptionInfoEntity,
            PrescriptionInfo prescriptionInfo) {
            prescriptionInfo.PrescriptionID = prescriptionInfoEntity.PrescriptionID.ToString();
            prescriptionInfo.ErrorMessage = prescriptionInfoEntity.ErrorMessage;
            prescriptionInfo.Count = prescriptionInfoEntity.Count;
            prescriptionInfo.number = prescriptionInfoEntity.number;
            prescriptionInfo.physicID = prescriptionInfoEntity.physicID;
        }

        #endregion


        #region 读取化验单 GetExaminationInfo(string sExaminationID)

        /*读取化验单*/
        public ExaminationInfo GetExaminationInfo(string sExaminationID) {
            ExaminationInfo examinationInfo = new ExaminationInfo();
            Guid gExaminationID = Guid.Empty;
            try {
                gExaminationID = new Guid(sExaminationID);
            }
            catch {
                examinationInfo.ErrorMessage = "Invalid ID! @Service";
                return examinationInfo;
            }
            ExaminationInfoEntity examinationInfoEntity = doctorLogic.GetExaminationInfo(gExaminationID);
            TranslateExaminationInfoEntityToExaminationInfoContractData(examinationInfoEntity, examinationInfo);
            return examinationInfo;
        }

        #endregion


        #region 将ExaminationInfo对应的Entity翻译为数据契约 TranslateExaminationInfoEntityToExaminationInfoContractData()

        /*将ExaminationInfo对应的Entity翻译为数据契约*/
        private void TranslateExaminationInfoEntityToExaminationInfoContractData(
            ExaminationInfoEntity examinationInfoEntity,
            ExaminationInfo examinationInfo) {
            examinationInfo.ErrorMessage = examinationInfoEntity.ErrorMessage;
            examinationInfo.ExaminationID = examinationInfoEntity.ExaminationID.ToString();
            examinationInfo.Date = examinationInfoEntity.Date;
            examinationInfo.Type = examinationInfoEntity.Type;
            examinationInfo.Text = examinationInfoEntity.Text;
            examinationInfo.Advice = examinationInfoEntity.Advice;
            examinationInfo.Image = examinationInfoEntity.Image;
        }

        #endregion


        #region 创建处方单 CreatePrescription(PrescriptionInfoEntity prescriptionInfoEntity)

        public string CreatePrescription(PrescriptionInfo prescriptionInfo) {
            if (prescriptionInfo == null) {
                return "Empty Prescription! @Service";
            }
            else {
                prescriptionInfo.ErrorMessage = null;
                PrescriptionInfoEntity prescriptionInfoEntity = new PrescriptionInfoEntity();
                TranslatePrescriptionInfoDataContractToPrescriptionInfoEntity(prescriptionInfo, prescriptionInfoEntity);
                return doctorLogic.CreatePrescription(prescriptionInfoEntity);
            }
        }

        #endregion


        #region 将PrescriptionInfo的数据契约翻译为Entity TranslatePrescriptionInfoDataContractToPrescriptionInfoEntity

        private void TranslatePrescriptionInfoDataContractToPrescriptionInfoEntity(
            PrescriptionInfo        prescriptionInfo,
            PrescriptionInfoEntity  prescriptionInfoEntity) {
                prescriptionInfoEntity.ErrorMessage = prescriptionInfo.ErrorMessage;
                prescriptionInfoEntity.Count        = prescriptionInfo.Count;
                prescriptionInfoEntity.physicID     = prescriptionInfo.physicID;
                prescriptionInfoEntity.number       = prescriptionInfo.number;
        }

        #endregion


        #region 获取某医生撰写的病历 RetrieveDoctorCase(DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD)

        /*获取某医生在某区间内所有已完成的病历(isDraft为false时)，或未完成的病历*/
        public CaseHistory RetrieveDoctorCase(DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD) {
            CaseHistory caseHistory = new CaseHistory();
            if (startDate == null) {
                caseHistory.ErrorMessage = "xcc Empty Start Date! @Service";
                return caseHistory;
            }
            else if (endDate == null) {
                caseHistory.ErrorMessage = "yxy Empty End Date! @Service";
                return caseHistory;
            }
            else {
                CaseHistoryEntity  caseHistoryEntity = doctorLogic.RetrieveDoctorCase(startDate, endDate, isDraft, showICD);
                TranslateCaseHistoryEntityToCaseHistoryContractData(caseHistoryEntity, caseHistory);
                return caseHistory;
            }
        }

        #endregion


        #region 获取某科室全体医生撰写的病历 RetrieveSectionCase(string sectionID, DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD)

        /*获取某科室在某区间内所有已完成的病历(isDraft为false时)，或未完成的病历*/
        public CaseHistory RetrieveSectionCase(DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD) {
            CaseHistory caseHistory = new CaseHistory();
            if (startDate == null) {
                caseHistory.ErrorMessage = "xxc Empty Start Date! @Service";
                return caseHistory;
            }
            else if (endDate == null) {
                caseHistory.ErrorMessage = "ysy Empty End Date! @Service";
                return caseHistory;
            }
            else {
                CaseHistoryEntity  caseHistoryEntity = doctorLogic.RetrieveSectionCase(startDate, endDate, isDraft, showICD);
                TranslateCaseHistoryEntityToCaseHistoryContractData(caseHistoryEntity, caseHistory);
                return caseHistory;
            }
        }

        #endregion


        #region 医生撰写邮件回复用户 MessageCompose(string receiverID, string text)

        /*医生撰写邮件Doctor to User*/
        public string MessageCompose(string receiverID, string text) {
            if (receiverID == null) {
                return "Empty Receiver ID! @Service";
            }
            else if (text == null) {
                return "Empty Text! @Service";
            }
            else {
                return doctorLogic.MessageCompose(receiverID, text);
            }
        }

        #endregion


        #region 医生收取收件箱 MessageInbox(DateTime? startDate, DateTime? endDate)

        /*医生收取收件箱*/
        public AllMessage MessageInbox(DateTime? startDate, DateTime? endDate) {
            AllMessageEntity allMessageEntity = null;

            if (startDate == null) {
                allMessageEntity = new AllMessageEntity();
                allMessageEntity.ErrorMessage = "Start Date! @Service";
            }
            else if (endDate == null) {
                allMessageEntity = new AllMessageEntity();
                allMessageEntity.ErrorMessage = "End Date! @Service";
            }
            else {
                allMessageEntity = doctorLogic.MessageInbox(startDate, endDate);
            }
            AllMessage allMessage = new AllMessage();
            TranslateAllMessageEntityToAllMessageContractData(allMessageEntity, allMessage);

            return allMessage;
        }

        #endregion


        #region 医生查看发件箱 MessageSent(DateTime? startDate, DateTime? endDate)

        /*医生查看发件箱*/
        public AllMessage MessageSent(DateTime? startDate, DateTime? endDate) {
            AllMessageEntity allMessageEntity = null;

            if (startDate == null) {
                allMessageEntity = new AllMessageEntity();
                allMessageEntity.ErrorMessage = "Start Date! @Service";
            }
            else if (endDate == null) {
                allMessageEntity = new AllMessageEntity();
                allMessageEntity.ErrorMessage = "End Date! @Service";
            }
            else {
                allMessageEntity = doctorLogic.MessageSent(startDate, endDate);
            }
            AllMessage allMessage = new AllMessage();
            TranslateAllMessageEntityToAllMessageContractData(allMessageEntity, allMessage);

            return allMessage;
        }

        #endregion


        #region 将AllMessage的Entity翻译为数据契约 TranslateAllMessageEntityToAllMessageContractData

        private void TranslateAllMessageEntityToAllMessageContractData(
            AllMessageEntity allMessageEntity,
            AllMessage allMessage) {
            allMessage.ErrorMessage = allMessageEntity.ErrorMessage;
            allMessage.Count = allMessageEntity.Count;
            if (allMessage.Count > 0) {
                allMessage.message = new Message[allMessage.Count];
                for (int i = 0; i < allMessage.Count; i++) {
                    allMessage.message[i] = new Message();
                    allMessage.message[i].ErrorMessage = allMessageEntity.messageEntity[i].ErrorMessage;
                    allMessage.message[i].Date = allMessageEntity.messageEntity[i].Date;
                    allMessage.message[i].Sender = allMessageEntity.messageEntity[i].Sender;
                    allMessage.message[i].Receiver = allMessageEntity.messageEntity[i].Receiver;
                    allMessage.message[i].Status = allMessageEntity.messageEntity[i].Status;
                    allMessage.message[i].Context = allMessageEntity.messageEntity[i].Context;
                }
            }
        }

        #endregion


        #region 医生设置日常预约信息 SetDailySchedule(DayOfWeek dayOfWeek, int am)

        public string SetDailySchedule(DayOfWeek dayOfWeek, int am) {
            return doctorLogic.SetDailySchedule(dayOfWeek, am);
        }

        #endregion


        #region 医生设置额外预约信息 SetAdditionSchedule(DateTime? date)

        public string SetAdditionSchedule(string doctorID, DateTime? date) {
            if (date == null) {
                return "Empty Date! @Service";
            }
            else {
                return doctorLogic.SetAdditionSchedule(date);
            }
        }
        #endregion


        #region 医生设置例外预约信息 SetExceptionSchedule(DateTime? date)

        public string SetExceptionSchedule(string doctorID, DateTime? date) {
            if (date == null) {
                return "Empty Date! @Service";
            }
            else {
                return doctorLogic.SetExceptionSchedule(date);
            }
        }
        #endregion
    }
}
