using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrPEServer.DrPEServerEntities;
using DrPEServer.DrPEServerDAL;

namespace DrPEServer.DrPEServerLogic {

    public class DoctorLogic {

        /*Data Access Layer实例*/
        DoctorDAO doctorDAO = new DoctorDAO();
        OpenAccessDAO openAccessDAO = new OpenAccessDAO();

        /*登录标记*/
        private bool    confirmed           = false;
        private string  confirmedDoctorID   = "";


        #region 进行就医消去挂号 FinishAppointment(Guid gGuid)

        /*进行就医消去挂号 FinishAppointment(Guid gGuid)*/
        public string FinishAppointment(Guid gGuid) {
            if (confirmed == false) {
                return "Not Logged In Yet! @Logic";
            }
            else {
                return doctorDAO.FinishAppointment(gGuid);
            }
        }

        #endregion


        #region 根据挂号信息查询UserID UserIDThroughAppointment(Guid gGuid)

        /*根据挂号信息查询UserID UserIDThroughAppointment(Guid gGuid)*/
        public string UserIDThroughAppointment(Guid gGuid) {
            if (confirmed == false) {
                return null;
            }
            else {
                return doctorDAO.UserIDThroughAppointment(gGuid);
            }
        }

        #endregion


        #region 医生登录 Login(string doctorID, string password)

        /*医生登录：接受并转发ID和Password，探测或解读错误*/
        public DoctorInfoEntity Login(string doctorID, string password) {

            DoctorInfoEntity doctorInfoEntity = null;

            if ((confirmed == true) && (confirmedDoctorID == doctorID)) {
                doctorInfoEntity = new DoctorInfoEntity();
                doctorInfoEntity.ErrorMessage = "303 " + doctorID + " Already Logged In! @Logic";
            }
            else {
                doctorInfoEntity = doctorDAO.Login(doctorID, password);

                if (doctorInfoEntity == null) {
                    doctorInfoEntity = new DoctorInfoEntity();
                    doctorInfoEntity.ErrorMessage = "304 Wrong DoctorID or Password! @Logic";
                }
                else {
                    confirmed = true;
                    confirmedDoctorID = doctorInfoEntity.DoctorID;
                }
            }

            return doctorInfoEntity;
        }

        #endregion


        #region 查看当前时间段多少人选择 GetMyAppointment()
        
        /*查看当前时间段多少人选择*/
        public AllAppointmentEntity GetMyAppointment() {
            if (confirmed == false) {
                AllAppointmentEntity allAppointmentEntity = new AllAppointmentEntity();
                allAppointmentEntity.ErrorMessage = "311 Not Logged in Yet! @Logic";
                return allAppointmentEntity;
            }
            else {
                return doctorDAO.GetMyAppointment(confirmedDoctorID);
            }
        }

        #endregion


        #region 获取患者信息 GetUserInfo(string userID)

        /*获取用户\患者信息*/
        public UserInfoEntity GetUserInfo(string userID) {
            UserInfoEntity userInfoEntity = null;
            if (confirmed == false) {
                userInfoEntity = new UserInfoEntity();
                userInfoEntity.ErrorMessage = "315 Not Logged in Yet! @Logic";
            }
            else {
                userInfoEntity = doctorDAO.GetUserInfo(userID);

                if (userInfoEntity == null) {
                    userInfoEntity = new UserInfoEntity();
                    userInfoEntity.ErrorMessage = "316 Wrong UserID! @Logic";
                }
            }

            return userInfoEntity;
        }

        #endregion


        #region 创建病历 CreateCase(CaseInfoEntity caseInfoEntity)

        /*创建病历并针对复查日期创建message*/
        public CaseInfoEntity CreateCase(CaseInfoEntity caseInfoEntity) {
            if (confirmed == false) {
                CaseInfoEntity newCaseInfoEntity = new CaseInfoEntity();
                newCaseInfoEntity.ErrorMessage = "XXX Not Logged in Yet! @Logic";
                return newCaseInfoEntity;
            }
            else {
                //caseInfoEntity.ErrorMessage = null;
                caseInfoEntity.DoctorID = confirmedDoctorID;
                return doctorDAO.CreateCase(caseInfoEntity);
            }
        }

        #endregion


        #region 修改病历 ModifyCase(CaseInfoEntity newCase)

        /*修改病历*/
        public CaseInfoEntity ModifyCase(CaseInfoEntity newCase) {
            if (confirmed == false) {
                CaseInfoEntity newCaseInfoEntity = new CaseInfoEntity();
                newCaseInfoEntity.ErrorMessage = "XXX Not Logged in Yet! @Logic";
                return newCaseInfoEntity;
            }
            else {
                newCase.DoctorID = confirmedDoctorID;
                return doctorDAO.ModifyCase(newCase);
            }
        }

        #endregion


        #region 获取指定病历的信息 GetCaseInfo(bool showICD)

        /*获取指定病历的信息*/
        public CaseInfoEntity GetCaseInfo(Guid caseID, bool showICD) {
            if (confirmed == false) {
                CaseInfoEntity caseInfoEntity = new CaseInfoEntity();
                caseInfoEntity.ErrorMessage = "xxx Not Logged in Yet! @Logic";
                return caseInfoEntity;
            }
            else {
                return doctorDAO.GetCaseInfo(caseID, showICD);
            }
        }

        #endregion


        #region 获取某用户在特定时间区间内的所有已完成病历 GetCaseHistory(DateTime? startDate, DateTime? endDate, bool showICD)

        /*获取指定用户在某区间内所有完成的病历*/
        public CaseHistoryEntity GetCaseHistory(string userID, DateTime? startDate, DateTime? endDate, bool showICD) {
            if (confirmed == false) {
                CaseHistoryEntity caseHistoryEntity = new CaseHistoryEntity();
                caseHistoryEntity.ErrorMessage = "有意义 Not Logged in Yet! @Logic";
                return caseHistoryEntity;
            }
            else {
                return doctorDAO.GetCaseHistory(userID, startDate, endDate, showICD);
            }
        }

        #endregion


        #region 读取处方单 GetPrescriptionInfo(Guid gPrescriptionID)

        /*读取处方单*/
        public PrescriptionInfoEntity GetPrescriptionInfo(Guid gPrescriptionID) {
            if (confirmed == false) {
                PrescriptionInfoEntity prescriptionInfoEntity = new PrescriptionInfoEntity();
                prescriptionInfoEntity.ErrorMessage = "1x1 Not Logged in Yet! @Logic";
                return prescriptionInfoEntity;
            }
            else {
                return doctorDAO.GetPrescriptionInfo(gPrescriptionID);
            }
        }

        #endregion


        #region 读取化验单 GetExaminationInfo(string examinationID)

        /*读取化验单 GetExaminationInfo(string examinationID)*/
        public ExaminationInfoEntity GetExaminationInfo(Guid gExaminationID) {
            if (confirmed == false) {
                ExaminationInfoEntity examinationInfoEntity = new ExaminationInfoEntity();
                examinationInfoEntity.ErrorMessage = "1x3 Not Logged in Yet! @Logic";
                return examinationInfoEntity;
            }
            else {
                return doctorDAO.GetExaminationInfo(gExaminationID);
            }
        }

        #endregion


        #region 创建处方单 CreatePrescription(PrescriptionInfoEntity prescriptionInfoEntity)

        public string CreatePrescription(PrescriptionInfoEntity prescriptionInfoEntity) {
            if (confirmed == false) {
                return "XX2 Not Logged in Yet! @Logic";
            }
            else {
                return doctorDAO.CreatePrescription(prescriptionInfoEntity);
            }
        }

        #endregion


        #region 获取某医生撰写的病历 RetrieveDoctorCase(DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD)

        /*获取某医生在某区间内所有已完成的病历(isDraft为false时)，或未完成的病历*/
        public CaseHistoryEntity RetrieveDoctorCase(DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD) {
            if (confirmed == false) {
                CaseHistoryEntity caseHistoryEntity = new CaseHistoryEntity();
                caseHistoryEntity.ErrorMessage = "尚未 Not Logged in Yet! @Logic";
                return caseHistoryEntity;
            }
            else {
                return doctorDAO.RetrieveDoctorCase(confirmedDoctorID, startDate, endDate, isDraft, showICD);
            }
        }

        #endregion


        #region 获取某科室全体医生撰写的病历 RetrieveSectionCase(string sectionID, DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD)

        /*获取某科室在某区间内所有已完成的病历(isDraft为false时)，或未完成的病历*/
        public CaseHistoryEntity RetrieveSectionCase(DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD) {
            if (confirmed == false) {
                CaseHistoryEntity caseHistoryEntity = new CaseHistoryEntity();
                caseHistoryEntity.ErrorMessage = "尚x未 Not Logged in Yet! @Logic";
                return caseHistoryEntity;
            }
            else {
                DoctorInfoEntity doctorInfoEntity = openAccessDAO.GetDoctorInfo(confirmedDoctorID);
                return doctorDAO.RetrieveSectionCase(doctorInfoEntity.SectionID, startDate, endDate, isDraft, showICD);
            }
        }

        #endregion


        #region 医生撰写邮件回复用户 MessageCompose(string receiverID, string text)

        /*医生撰写邮件Doctor to User*/
        public string MessageCompose(string receiverID, string text) {
            if (confirmed == false) {
                return "Not Logged in Yet! @Logic";
            }
            else {
                return doctorDAO.MessageCompose(confirmedDoctorID, receiverID, text);
            }
        }

        #endregion


        #region 医生收取收件箱 MessageInbox(DateTime? startDate, DateTime? endDate)

        /*医生收取收件箱*/
        public AllMessageEntity MessageInbox(DateTime? startDate, DateTime? endDate) {
            if (confirmed == false) {
                AllMessageEntity allMessageEntity = new AllMessageEntity();
                allMessageEntity.ErrorMessage = "Not Logged in Yet! @Logic";
                return allMessageEntity;
            }
            else {
                return doctorDAO.MessageInbox(confirmedDoctorID, startDate, endDate);
            }
        }

        #endregion


        #region 医生查看发件箱 MessageSent(DateTime? startDate, DateTime? endDate)

        /*医生查看发件箱*/
        public AllMessageEntity MessageSent(DateTime? startDate, DateTime? endDate) {
            if (confirmed == false) {
                AllMessageEntity allMessageEntity = new AllMessageEntity();
                allMessageEntity.ErrorMessage = "Not Logged in Yet! @Logic";
                return allMessageEntity;
            }
            else {
                return doctorDAO.MessageSent(confirmedDoctorID, startDate, endDate);
            }
        }

        #endregion
    
        
        #region 医生设置日常预约信息 SetDailySchedule(DayOfWeek dayOfWeek, int am)

        public string SetDailySchedule(DayOfWeek dayOfWeek, int am) {
            return doctorDAO.SetDailySchedule(confirmedDoctorID, dayOfWeek, am);
        }

        #endregion


        #region 医生设置额外预约信息 SetAdditionSchedule(DateTime? date)

        public string SetAdditionSchedule(DateTime? date) {
            return doctorDAO.SetAdditionSchedule(confirmedDoctorID, date);
        }
        #endregion


        #region 医生设置例外预约信息 SetExceptionSchedule(DateTime? date)

        public string SetExceptionSchedule(DateTime? date) {
            return doctorDAO.SetExceptionSchedule(confirmedDoctorID, date);
        }
        #endregion

    }
}
