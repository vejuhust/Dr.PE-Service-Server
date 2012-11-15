using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrPEServer.DrPEServerEntities;
using DrPEServer.DrPEServerDAL;

using System.Drawing;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ThoughtWorks.QRCode.Codec.Util;

namespace DrPEServer.DrPEServerLogic {

    public class UserLogic {

        /*Data Access Layer实例*/
        UserDAO userDAO = new UserDAO();

        /*登录标记*/
        private bool    confirmed       = false;
        private string  confirmedUserID = "";

        #region 查询排队状况 MyQueueStatus(Guid gAppointment)

        /*查询排队状况 MyQueueStatus(Guid gAppointment)*/
        public QueueStatusEntity MyQueueStatus(Guid gAppointment) {

            QueueStatusEntity queueStatusEntity = null;

            if (confirmed == false) {
                queueStatusEntity = new QueueStatusEntity();
                queueStatusEntity.ErrorMessage = "477 Not Logged in Yet! @Logic";
            }
            else {
                /*根据逻辑处理*/
                queueStatusEntity = userDAO.MyQueueStatus(gAppointment);
                if (queueStatusEntity.ErrorMessage == null) {
                    if (queueStatusEntity.Mine == queueStatusEntity.Process) {
                        queueStatusEntity.ErrorMessage = "478 You are Watching the Doctor! @Logic";
                    }
                    else {
                        if (queueStatusEntity.Process > queueStatusEntity.Mine) {
                            queueStatusEntity.ErrorMessage = "479 Sorry, You are Late! @Logic";
                        }
                        else {
                            int leftCount = ((int)queueStatusEntity.Mine) - ((int)queueStatusEntity.Process);
                            queueStatusEntity.When = DateTime.Now.AddMinutes(leftCount * 10);
                        }
                    }
                }
            }

            return queueStatusEntity;

        }

        #endregion


        #region 生成QR码 GuidToQRCode(string sGuid)
        /*生成QR码 GuidToQRCode(string sGuid)*/

        public Image GuidToQRCode(string sGuid) {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            Image image = qrCodeEncoder.Encode(sGuid);

            return image;
        }

        #endregion 


        #region 用户登录 Login(string doctorID, string password)

        /*用户登录：接受并转发ID和Password，探测或解读错误*/
        public UserInfoEntity Login(string userID, string password) {

            UserInfoEntity userInfoEntity = null;

            if ((confirmed == true) && (confirmedUserID == userID)) {
                userInfoEntity = new UserInfoEntity();
                userInfoEntity.ErrorMessage = "403 User" + userID + " Already Logged In! @Logic";
            }
            else {
                userInfoEntity = userDAO.Login(userID, password);

                if (userInfoEntity == null) {
                    userInfoEntity = new UserInfoEntity();
                    userInfoEntity.ErrorMessage = "404 Wrong UserID or password! @Logic";
                }
                else {
                    confirmed       = true;
                    confirmedUserID = userInfoEntity.UserID;
                }
            }

            return userInfoEntity;
        }

        #endregion


        #region 获取用户资料 GetUserInfo(string doctorID)

        /*获取用户资料：判断是否登录、授权，向DAL提交请求并解读错误*/
        public UserInfoEntity GetUserInfo(string userID) {

            UserInfoEntity userInfoEntity = null;

            if (confirmed == false) {
                userInfoEntity = new UserInfoEntity();
                userInfoEntity.ErrorMessage = "Not Logged in Yet! @Logic";
            }
            else if (confirmedUserID != userID) {
                userInfoEntity = new UserInfoEntity();
                userInfoEntity.ErrorMessage = "Not Authorized to Get This Profile! @Logic";
            }
            else {
                userInfoEntity = userDAO.GetUserInfo(userID);

                if (userInfoEntity == null) {
                    userInfoEntity = new UserInfoEntity();
                    userInfoEntity.ErrorMessage = "Wrong UserID! @Logic";
                }
            }

            return userInfoEntity;
        }

        #endregion


        #region 查询医生最近可以预约的时间段 GetAvailableDate(string doctorID)

        /*查询医生最近可以预约的时间段*/
        public AvailableDateEntity GetAvailableDate(string doctorID) {
            AvailableDateEntity availableDateEntity = userDAO.GetAvailableDate(doctorID);
            if (availableDateEntity.Count == 0) {
                availableDateEntity.ErrorMessage = "416 No Available Time! @Logic";
            }
            return userDAO.GetAvailableDate(doctorID);
        }

        #endregion


        #region 查询科室最近一周预约的时间安排 GetSectionSchedule(string sectionID)

        public SectionScheduleEntity GetSectionSchedule(string sectionID) {
            return userDAO.GetSectionSchedule(sectionID);
        }

        #endregion


        #region 预约医生某时间段 MakeAppointment(string doctorID, DateTime? date)

        /*预约医生某时间段*/
        public string MakeAppointment(string doctorID, DateTime? date) {
            if (confirmed == false) {
                return "420 Not Logged in Yet! @Logic";
            }
            else {
                return userDAO.MakeAppointment(confirmedUserID, doctorID, date);
            }
        }

        #endregion


        #region 获取待去的预约记录 GetMyFutureAppointment()

        /*获取待去的预约记录*/
        public AllAppointmentEntity GetMyFutureAppointment() {
            if (confirmed == false) {
                AllAppointmentEntity allAppointmentEntity = new AllAppointmentEntity();
                allAppointmentEntity.ErrorMessage = "432 Not Logged in Yet! @Logic";
                return allAppointmentEntity;
            }
            else {
                return userDAO.GetMyFutureAppointment(confirmedUserID);
            }
        }

        #endregion


        #region 获取指定时间区间中用户所有的交易记录 GetTransactionHistory(string userID, DateTime? startDate, DateTime? endDate)

        /*获取指定时间区间中药房所有的交易记录*/
        public AllTransactionInfoEntity GetTransactionHistory(DateTime? startDate, DateTime? endDate) {
            if (confirmed == false) {
                AllTransactionInfoEntity allTransactionInfoEntity = new AllTransactionInfoEntity();
                allTransactionInfoEntity.ErrorMessage = "443 Not Logged in Yet! @Logic";
                return allTransactionInfoEntity;
            }
            else {
                return userDAO.GetTransactionHistory(confirmedUserID, startDate, endDate);
            }
        }

        #endregion


        #region 获取最近一次病历 GetLastCaseInfo(bool showICD)

        /*获取最近一次病历*/
        public CaseInfoEntity GetLastCaseInfo(bool showICD) {
            if (confirmed == false) {
                CaseInfoEntity caseInfoEntity = new CaseInfoEntity();
                caseInfoEntity.ErrorMessage = "xxx Not Logged in Yet! @Logic";
                return caseInfoEntity;
            }
            else {
                return userDAO.GetLastCaseInfo(confirmedUserID, showICD);
            }
        }

        #endregion


        #region 获取病例的疾病温馨提示 GetDiseaseNotice(Guid gCaseID)

        /*获取病例的疾病温馨提示*/
        public NoticeEntity GetDiseaseNotice(Guid gCaseID) {
            if (confirmed == false) {
                NoticeEntity noticeEntity = new NoticeEntity();
                noticeEntity.ErrorMessage = "xzf Not Logged in Yet! @Logic";
                return noticeEntity;
            }
            else {
                return userDAO.GetDiseaseNotice(gCaseID);
            }
        }

        #endregion


        #region 获取药品的疾病温馨提示 GetPhysicNotice(Guid gPrescriptionID)

        /*获取药品的疾病温馨提示*/
        public NoticeEntity GetPhysicNotice(Guid gPrescriptionID) {
            if (confirmed == false) {
                NoticeEntity noticeEntity = new NoticeEntity();
                noticeEntity.ErrorMessage = "x信息f Not Logged in Yet! @Logic";
                return noticeEntity;
            }
            else {
                return userDAO.GetPhysicNotice(gPrescriptionID);
            }
        }

        #endregion


        #region 获取某用户在特定时间区间内的所有已完成病历 GetCaseHistory(DateTime? startDate, DateTime? endDate, bool showICD)

        /*获取指定用户在某区间内所有完成的病历*/
        public CaseHistoryEntity GetCaseHistory(DateTime? startDate, DateTime? endDate, bool showICD) {
            if (confirmed == false) {
                CaseHistoryEntity caseHistoryEntity = new CaseHistoryEntity();
                caseHistoryEntity.ErrorMessage = "有意义 Not Logged in Yet! @Logic";
                return caseHistoryEntity;
            }
            else {
                return userDAO.GetCaseHistory(confirmedUserID, startDate, endDate, showICD);
            }
        }

        #endregion


        #region 读取处方单 GetPrescriptionInfo(Guid gPrescriptionID)

        /*读取处方单*/
        public PrescriptionInfoEntity GetPrescriptionInfo(Guid gPrescriptionID) {
            if (confirmed == false) {
                PrescriptionInfoEntity prescriptionInfoEntity = new PrescriptionInfoEntity();
                prescriptionInfoEntity.ErrorMessage = "11 Not Logged in Yet! @Logic";
                return prescriptionInfoEntity;
            }
            else {
                return userDAO.GetPrescriptionInfo(gPrescriptionID);
            }
        }

        #endregion


        #region 读取化验单 GetExaminationInfo(string examinationID)

        /*读取化验单 GetExaminationInfo(string examinationID)*/
        public ExaminationInfoEntity GetExaminationInfo(Guid gExaminationID) {
            if (confirmed == false) {
                ExaminationInfoEntity examinationInfoEntity = new ExaminationInfoEntity();
                examinationInfoEntity.ErrorMessage = "13 Not Logged in Yet! @Logic";
                return examinationInfoEntity;
            }
            else {
                return userDAO.GetExaminationInfo(gExaminationID);
            }
        }

        #endregion


        #region 用户撰写邮件询问医生 MessageCompose(string receiverID, string text)

        /*用户撰写邮件User to Doctor*/
        public string MessageCompose(string receiverID, string text) {
            if (confirmed == false) {
                return "Not Logged in Yet! @Logic";
            }
            else {
                return userDAO.MessageCompose(confirmedUserID, receiverID, text);
            }
        }

        #endregion


        #region 用户收取收件箱 MessageInbox(DateTime? startDate, DateTime? endDate)

        /*用户收取收件箱*/
        public AllMessageEntity MessageInbox(DateTime? startDate, DateTime? endDate) {
            if (confirmed == false) {
                AllMessageEntity allMessageEntity = new AllMessageEntity();
                allMessageEntity.ErrorMessage = "Not Logged in Yet! @Logic";
                return allMessageEntity;
            }
            else {
                return userDAO.MessageInbox(confirmedUserID, startDate, endDate);
            }
        }

        #endregion


        #region 用户查看发件箱 MessageSent(DateTime? startDate, DateTime? endDate)

        /*用户查看发件箱*/
        public AllMessageEntity MessageSent(DateTime? startDate, DateTime? endDate) {
            if (confirmed == false) {
                AllMessageEntity allMessageEntity = new AllMessageEntity();
                allMessageEntity.ErrorMessage = "Not Logged in Yet! @Logic";
                return allMessageEntity;
            }
            else {
                return userDAO.MessageSent(confirmedUserID, startDate, endDate);
            }
        }

        #endregion
    
    
        #region 查询处方单在特定药房的花费 GetPrescriptionCost(Guid gPrescriptionID, string pharmacyID)

        /*查询处方单在特定药房的花费，供User调用。*/
        public PrescriptionCostEntity GetPrescriptionCost(Guid gPrescriptionID, string pharmacyID) {
            if (confirmed == false) {
                PrescriptionCostEntity prescriptionCostEntity = new PrescriptionCostEntity();
                prescriptionCostEntity.ErrorMessage = "Not Logged in Yet! @Logic";
                return prescriptionCostEntity;
            }
            else {
                return userDAO.GetPrescriptionCost(gPrescriptionID, pharmacyID);
            }
        }

        #endregion


        #region 为处方单付款 PayPrescription(Guid gPrescriptionID, string pharmacyID, string payPassword)

        /*为处方单付款*/
        public TransactionInfoEntity PayPrescription(Guid gPrescriptionID, string pharmacyID, string payPassword) {
            if (confirmed == false) {
                TransactionInfoEntity transactionInfoEntity = new TransactionInfoEntity();
                transactionInfoEntity.ErrorMessage = "Not Logged in Yet! @Logic";
                return transactionInfoEntity;
            }
            else {
                return userDAO.PayPrescription(gPrescriptionID, pharmacyID, payPassword);
            }
        }

        #endregion

    }
}
