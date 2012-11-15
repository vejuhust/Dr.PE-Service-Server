using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using DrPEServer.DrPEServerEntities;
using DrPEServer.DrPEServerLogic;
using System.IO;

namespace DrPEServer.DrPEServerService {

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
    public class UserService : IUserService {

        /*Logic Layer实例*/
        UserLogic userLogic = new UserLogic();

        #region 查询排队状况 MyQueueStatus(Guid gAppointment)

        /*查询排队状况 MyQueueStatus(Guid gAppointment)*/
        public QueueStatus MyQueueStatus(string sAppointment) {
            QueueStatus queueStatus = new QueueStatus();
            Guid gAppointment = Guid.Empty;
            try {
                gAppointment = new Guid(sAppointment);
            }
            catch {
                queueStatus.ErrorMessage = "480 Invalid Guid! @Service";
                return queueStatus;
            }
            QueueStatusEntity queueStatusEntity = userLogic.MyQueueStatus(gAppointment);
            TranslateQueueStatusEntityToQueueStatusContractData(queueStatusEntity, queueStatus);

            return queueStatus;
        }

        #endregion


        #region 翻译QueueStatusEntity至QueueStatus数据契约 TranslateQueueStatusEntityToQueueStatusContractData()

        /*翻译QueueStatusEntity至QueueStatus数据契约 TranslateQueueStatusEntityToQueueStatusContractData()*/
        private void TranslateQueueStatusEntityToQueueStatusContractData(QueueStatusEntity queueStatusEntity, QueueStatus queueStatus) {
            queueStatus.ErrorMessage    = queueStatusEntity.ErrorMessage;
            queueStatus.Capacity        = queueStatusEntity.Capacity;
            queueStatus.Process         = queueStatusEntity.Process;
            queueStatus.Mine            = queueStatusEntity.Mine;
            queueStatus.When            = queueStatusEntity.When;
        }

        #endregion


        #region 生成QR码 GuidToQRCode(string sGuid)

        /*将图片转字符串*/
        private byte[] BmpToJpegBuff(Image img) {
            ImageConverter converter = new ImageConverter();
            byte[] bmpSrc = (byte[])converter.ConvertTo(img, typeof(byte[]));
            MemoryStream ms = new MemoryStream(bmpSrc);
            MemoryStream msjpg = new MemoryStream();
            Bitmap myBitmap = new Bitmap(ms);
            myBitmap.Save(msjpg, ImageFormat.Jpeg);
            byte[] bjpeg = msjpg.GetBuffer();

            return bjpeg;
        }

        /*生成QR码 GuidToQRCode(string sGuid)*/
        public byte[] GuidToQRCode(string sGuid) {
            Image image = userLogic.GuidToQRCode(sGuid);
            return BmpToJpegBuff(image);
        }

        #endregion


        #region 用户登录 Login(string doctorID, string password)

        /*用户登录：若ID和Password均不为空，则转发至DL，将结果翻译为数据契约*/
        public UserInfo Login(string userID, string password) {

            UserInfoEntity userInfoEntity = null;

            if (userID == null) {
                userInfoEntity = new UserInfoEntity();
                userInfoEntity.ErrorMessage = "401 Empty UserID! @Service";
            }
            else if (password == null) {
                userInfoEntity = new UserInfoEntity();
                userInfoEntity.ErrorMessage = "402 Empty password! @Service";
            }
            else {
                userInfoEntity = userLogic.Login(userID, password);
            }

            UserInfo userInfo = new UserInfo();
            TranslateUserInfoEntityToUserInfoContractData(userInfoEntity, userInfo);

            return userInfo;
        }

        #endregion


        #region 获取用户资料 GetUserInfo(string doctorID)

        /*获取用户资料：将不为空的ID请求转发至DL，并将反馈结果翻译为数据契约*/
        public UserInfo GetUserInfo(string userID) {

            UserInfoEntity userInfoEntity = null;

            if (userID == null) {
                userInfoEntity = new UserInfoEntity();
                userInfoEntity.ErrorMessage = "Empty UserID! @Service";
            }
            else {
                userInfoEntity = userLogic.GetUserInfo(userID);
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

                userInfo.ErrorMessage           = userInfoEntity.ErrorMessage;

                userInfo.UserID                 = userInfoEntity.UserID;
                userInfo.LastName               = userInfoEntity.LastName;
                userInfo.FirstName              = userInfoEntity.FirstName;
                userInfo.Nationality            = userInfoEntity.Nationality;
                userInfo.Gender                 = userInfoEntity.Gender;
                userInfo.ABO                    = userInfoEntity.ABO;
                userInfo.Rh                     = userInfoEntity.Rh;
                userInfo.Birthplace             = userInfoEntity.Birthplace;
                userInfo.Birthday               = userInfoEntity.Birthday;
                userInfo.Deathplace             = userInfoEntity.Deathplace;
                userInfo.Deathday               = userInfoEntity.Deathday;
                userInfo.Balance                = userInfoEntity.Balance;
                userInfo.LastLoginDate          = userInfoEntity.LastLoginDate;
                userInfo.City                   = userInfoEntity.City;
                userInfo.Address                = userInfoEntity.Address;
                userInfo.Phone                  = userInfoEntity.Phone;
                userInfo.Email                  = userInfoEntity.Email;
                userInfo.EmergencyContactPerson = userInfoEntity.EmergencyContactPerson;
        }

        #endregion


        #region 查询科室最近一周预约的时间安排 GetSectionSchedule(string sectionID)

        /*查询科室最近一周预约的时间安排*/
        public SectionSchedule GetSectionSchedule(string sectionID) {

            SectionScheduleEntity sectionScheduleEntity = null;

            if (sectionID == null) {
                sectionScheduleEntity = new SectionScheduleEntity();
                sectionScheduleEntity.ErrorMessage = "411 Empty SectionID! @Service";
            }
            else {
                sectionScheduleEntity = userLogic.GetSectionSchedule(sectionID);
            }

            SectionSchedule sectionSchedule = new SectionSchedule();
            TranslateSectionScheduleEntityToSectionScheduleContractData(sectionScheduleEntity, sectionSchedule);

            return sectionSchedule;
        }

        #endregion


        #region 将SectionSchedule对应的Entity翻译为数据契约 TranslateSectionScheduleEntityToSectionScheduleContractData

        /*将SectionSchedule对应的Entity翻译为数据契约*/
        void TranslateSectionScheduleEntityToSectionScheduleContractData(
            SectionScheduleEntity   sectionScheduleEntity,
            SectionSchedule         sectionSchedule) {
                sectionSchedule.ErrorMessage = sectionScheduleEntity.ErrorMessage;
                sectionSchedule.Count = sectionScheduleEntity.Count;
                sectionSchedule.Detail = new SectionScheduleItem[sectionSchedule.Count];
                for (int i = 0; i < sectionSchedule.Count; i++) {
                    sectionSchedule.Detail[i] = new SectionScheduleItem();
                    sectionSchedule.Detail[i].DoctorID  = sectionScheduleEntity.Detail[i].DoctorID;
                    sectionSchedule.Detail[i].Date      = sectionScheduleEntity.Detail[i].Date;
                    sectionSchedule.Detail[i].Appointed = sectionScheduleEntity.Detail[i].Appointed;
                    sectionSchedule.Detail[i].Capacity  = sectionScheduleEntity.Detail[i].Capacity;
                }
        }

        #endregion


        #region 查询医生最近可以预约的时间段 GetAvailableDate(string doctorID)

        /*查询医生最近可以预约的时间段*/
        public AvailableDate GetAvailableDate(string doctorID) {
            AvailableDateEntity availableDateEntity = null;

            if (doctorID == null) {
                availableDateEntity = new AvailableDateEntity();
                availableDateEntity.ErrorMessage = "415 Empty DoctorID! @Service";
            }
            else {
                availableDateEntity = userLogic.GetAvailableDate(doctorID);
            }

            AvailableDate availableDate = new AvailableDate();
            TranslateAvailableDateEntityToAvailableDateContractData(availableDateEntity, availableDate);

            return availableDate;
        }

        #endregion


        #region 将AvailableDate对应的Entity翻译为数据契约 TranslateAvailableDateEntityToAvailableDateContractData

        /*将AvailableDate对应的Entity翻译为数据契约*/
        private void TranslateAvailableDateEntityToAvailableDateContractData(
            AvailableDateEntity availableDateEntity, 
            AvailableDate       availableDate) {
                availableDate.ErrorMessage = availableDateEntity.ErrorMessage;
                availableDate.Capacity = availableDateEntity.Capacity;
                availableDate.Count = availableDateEntity.Count;
                availableDate.date = availableDateEntity.date;
                availableDate.appointed = availableDateEntity.appointed;
        }

        #endregion


        #region 预约医生某时间段 MakeAppointment(string doctorID, DateTime? date)

        /*预约医生某时间段*/
        public string MakeAppointment(string doctorID, DateTime? date) {
            if (doctorID == null) {
                return "418 Empty DoctorID! @Service";
            }

            if (date == null) {
                return "419 Empty Date! @Service";
            }

            return userLogic.MakeAppointment(doctorID, date);
        }

        #endregion


        #region 用户获取待去的预约记录 GetMyFutureAppointment()

        /*获取待去的预约记录*/
        public AllAppointment GetMyFutureAppointment() {
            AllAppointmentEntity allAppointmentEntity = userLogic.GetMyFutureAppointment();
            AllAppointment allAppointment = new AllAppointment();
            TranslateAllAppointmentEntityToAllAppointmentContractData(allAppointmentEntity, allAppointment);
            return allAppointment;
        }

        #endregion


        #region 将AllAppointment的Entity翻译为对应的数据契约 TranslateAllAppointmentEntityToAllAppointmentContractData

        /*翻译AllAppointment的Entity为对应的数据契约*/
        private void TranslateAllAppointmentEntityToAllAppointmentContractData(
            AllAppointmentEntity    allAppointmentEntity,
            AllAppointment          allAppointment) {
                allAppointment.ErrorMessage = allAppointmentEntity.ErrorMessage;
                allAppointment.Count        = allAppointmentEntity.Count;
                allAppointment.appointment  = new Appointment[allAppointment.Count];
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


        #region 获取指定时间区间中用户所有的交易记录 GetTransactionHistory(string userID, DateTime? startDate, DateTime? endDate)

        /*获取指定时间区间中用户所有的交易记录*/
        public AllTransactionInfo GetTransactionHistory(DateTime? startDate, DateTime? endDate) {
            AllTransactionInfo allTransactionInfo = new AllTransactionInfo();
            if (startDate == null) {
                allTransactionInfo.ErrorMessage = "441 Empty Start Date! @Service";
                return allTransactionInfo;
            }
            else if (endDate == null) {
                allTransactionInfo.ErrorMessage = "442 Empty End Date! @Service";
                return allTransactionInfo;
            }
            else {
                AllTransactionInfoEntity allTransactionInfoEntity = userLogic.GetTransactionHistory(startDate, endDate);
                TranslateAllTransactionInfoEntityToAllTransactionInfoContractData(allTransactionInfoEntity, allTransactionInfo);
                return allTransactionInfo;
            }
        }

        #endregion


        #region 将AllTransactionInfo对应的Entity翻译为数据契约 TranslateAllTransactionInfoEntityToAllTransactionInfoContractData()

        /*将AllTransactionInfo对应的Entity翻译为数据契约，调用 TranslateTransactionInfoEntityToTransactionInfoContractData()方法*/
        private void TranslateAllTransactionInfoEntityToAllTransactionInfoContractData(
            AllTransactionInfoEntity allTransactionInfoEntity,
            AllTransactionInfo allTransactionInfo) {

            allTransactionInfo.ErrorMessage = allTransactionInfoEntity.ErrorMessage;
            allTransactionInfo.Count = allTransactionInfoEntity.Count;
            allTransactionInfo.transactionInfo = new TransactionInfo[allTransactionInfo.Count];

            for (int i = 0; i < allTransactionInfo.Count; i++) {
                allTransactionInfo.transactionInfo[i] = new TransactionInfo();
                TranslateTransactionInfoEntityToTransactionInfoContractData(allTransactionInfoEntity.transactionInfoEntity[i],
                                                                            allTransactionInfo.transactionInfo[i]);
            }
        }

        #endregion


        #region 将TransactionInfo对应的Entity翻译为数据契约 TranslateTransactionInfoEntityToTransactionInfoContractData()

        /*将TransactionInfo对应的Entity翻译为数据契约*/
        private void TranslateTransactionInfoEntityToTransactionInfoContractData(
            TransactionInfoEntity transactionInfoEntity,
            TransactionInfo transactionInfo) {
            transactionInfo.ErrorMessage = transactionInfoEntity.ErrorMessage;
            transactionInfo.TransactionID = transactionInfoEntity.TransactionID.ToString();
            transactionInfo.LastName = transactionInfoEntity.LastName;
            transactionInfo.FirstName = transactionInfoEntity.FirstName;
            transactionInfo.PharmacyID = transactionInfoEntity.PharmacyID;
            transactionInfo.Date = transactionInfoEntity.Date;
            transactionInfo.Amount = transactionInfoEntity.Amount;
            transactionInfo.UserBalanceThen = transactionInfoEntity.UserBalanceThen;
            transactionInfo.Action = transactionInfoEntity.Action;
        }

        #endregion


        #region 将PrescriptionCost对应的Entity翻译为数据契约 TranslatePrescriptionCostEntityToPrescriptionCostContractData()

        /*将PrescriptionCost对应的Entity翻译为数据契约*/
        private void TranslatePrescriptionCostEntityToPrescriptionCostContractData(
            PrescriptionCostEntity prescriptionCostEntity,
            PrescriptionCost prescriptionCost) {
            prescriptionCost.ErrorMessage = prescriptionCostEntity.ErrorMessage;
            prescriptionCost.Count = prescriptionCostEntity.Count;
            prescriptionCost.LastName = prescriptionCostEntity.LastName;
            prescriptionCost.FirstName = prescriptionCostEntity.FirstName;
            prescriptionCost.UserBalance = prescriptionCostEntity.UserBalance;
            prescriptionCost.Amount = prescriptionCostEntity.Amount;
            prescriptionCost.PharmacyID = prescriptionCostEntity.PharmacyID;
            prescriptionCost.physicID = new string[prescriptionCost.Count];
            prescriptionCost.number = new int[prescriptionCost.Count];
            prescriptionCost.price = new Decimal?[prescriptionCost.Count];
            for (int i = 0; i < prescriptionCost.Count; i++) {
                prescriptionCost.physicID[i] = prescriptionCostEntity.physicID[i];
                prescriptionCost.number[i] = prescriptionCostEntity.number[i];
                prescriptionCost.price[i] = prescriptionCostEntity.price[i];
            }
        }

        #endregion


        #region 获取最近一次病历 GetLastCaseInfo(bool showICD)

        /*获取最近一次病历*/
        public CaseInfo GetLastCaseInfo(bool showICD) {
            CaseInfoEntity  caseInfoEntity = userLogic.GetLastCaseInfo(showICD); 
            CaseInfo        caseInfo =  new CaseInfo();
            TranslateCaseInfoEntityToCaseInfoContractData(caseInfoEntity, caseInfo);
            return caseInfo;
        }

        #endregion


        #region 将CaseInfo对应的Entity翻译为数据契约 TranslateCaseInfoEntityToCaseInfoContractData()

        /*将CaseInfo对应的Entity翻译为数据契约*/
        private void TranslateCaseInfoEntityToCaseInfoContractData(
            CaseInfoEntity caseInfoEntity,
            CaseInfo caseInfo) {
            caseInfo.ErrorMessage           = caseInfoEntity.ErrorMessage;
            caseInfo.CaseID                 = caseInfoEntity.CaseID.ToString();
            caseInfo.ExaminationID          = caseInfoEntity.ExaminationID.ToString();
            caseInfo.PrescriptionID         = caseInfoEntity.PrescriptionID.ToString();
            caseInfo.UserID                 = caseInfoEntity.UserID;
            caseInfo.DoctorID               = caseInfoEntity.DoctorID;
            caseInfo.SectionID              = caseInfoEntity.SectionID;
            caseInfo.Date                   = caseInfoEntity.Date;
            caseInfo.ChiefComplaint         = caseInfoEntity.ChiefComplaint;
            caseInfo.TentativeDiagnosis     = caseInfoEntity.TentativeDiagnosis;
            caseInfo.DifferentialDiagnosis  = caseInfoEntity.DifferentialDiagnosis;
            caseInfo.TreatmentPlan          = caseInfoEntity.TreatmentPlan;
            caseInfo.CountercheckDate       = caseInfoEntity.CountercheckDate;
            caseInfo.IsDraft                = caseInfoEntity.IsDraft;
        }

        #endregion


        #region 获取指定用户在某区间内所有完成的病历 GetCaseHistory(DateTime? startDate, DateTime? endDate, bool showICD)

        /*获取指定用户在某区间内所有完成的病历*/
        public CaseHistory GetCaseHistory(DateTime? startDate, DateTime? endDate, bool showICD) {
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
                CaseHistoryEntity  caseHistoryEntity = userLogic.GetCaseHistory(startDate, endDate, showICD);
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
            PrescriptionInfoEntity prescriptionInfoEntity = userLogic.GetPrescriptionInfo(gPrescriptionID);
            TranslatePrescriptionInfoEntityToPrescriptionInfoContractData(prescriptionInfoEntity, prescriptionInfo);
            return prescriptionInfo;
        }

        #endregion


        #region 将PrescriptionInfo对应的Entity翻译为数据契约 TranslatePrescriptionInfoEntityToPrescriptionInfoContractData

        /*将PrescriptionInfo对应的Entity翻译为数据契约*/
        private void TranslatePrescriptionInfoEntityToPrescriptionInfoContractData(
            PrescriptionInfoEntity  prescriptionInfoEntity,
            PrescriptionInfo        prescriptionInfo) {
                prescriptionInfo.PrescriptionID = prescriptionInfoEntity.PrescriptionID.ToString();
                prescriptionInfo.ErrorMessage   = prescriptionInfoEntity.ErrorMessage;
                prescriptionInfo.Count          = prescriptionInfoEntity.Count;
                prescriptionInfo.number         = prescriptionInfoEntity.number;
                prescriptionInfo.physicID       = prescriptionInfoEntity.physicID;
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
            ExaminationInfoEntity examinationInfoEntity = userLogic.GetExaminationInfo(gExaminationID);
            TranslateExaminationInfoEntityToExaminationInfoContractData(examinationInfoEntity, examinationInfo);
            return examinationInfo;
        }

        #endregion


        #region 将ExaminationInfo对应的Entity翻译为数据契约 TranslateExaminationInfoEntityToExaminationInfoContractData()

        /*将ExaminationInfo对应的Entity翻译为数据契约*/
        private void TranslateExaminationInfoEntityToExaminationInfoContractData(
            ExaminationInfoEntity   examinationInfoEntity,
            ExaminationInfo         examinationInfo) {
                examinationInfo.ErrorMessage    = examinationInfoEntity.ErrorMessage;
                examinationInfo.ExaminationID   = examinationInfoEntity.ExaminationID.ToString();
                examinationInfo.Date            = examinationInfoEntity.Date;
                examinationInfo.Type            = examinationInfoEntity.Type;
                examinationInfo.Text            = examinationInfoEntity.Text;
                examinationInfo.Advice          = examinationInfoEntity.Advice;
                examinationInfo.Image           = examinationInfoEntity.Image;
        }

        #endregion


        #region 用户撰写邮件询问医生 MessageCompose(string senderID, string receiverID, string text)

        /*用户撰写邮件User to Doctor*/
        public string MessageCompose(string receiverID, string text) {
            if (receiverID == null) {
                return "Empty Receiver ID! @Service";
            }
            else if (text == null) {
                return "Empty Text! @Service";
            }
            else {
                return userLogic.MessageCompose(receiverID, text);
            }
        }

        #endregion


        #region 用户收取收件箱 MessageInbox(DateTime? startDate, DateTime? endDate)

        /*用户收取收件箱*/
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
                allMessageEntity = userLogic.MessageInbox(startDate, endDate);
            }
            AllMessage allMessage = new AllMessage();
            TranslateAllMessageEntityToAllMessageContractData(allMessageEntity, allMessage);

            return allMessage;
        }

        #endregion


        #region 将AllMessage的Entity翻译为数据契约 TranslateAllMessageEntityToAllMessageContractData

        private void TranslateAllMessageEntityToAllMessageContractData(
            AllMessageEntity    allMessageEntity,
            AllMessage          allMessage) {
                allMessage.ErrorMessage = allMessageEntity.ErrorMessage;
                allMessage.Count = allMessageEntity.Count;
                if (allMessage.Count > 0) {
                    allMessage.message = new Message[allMessage.Count];
                    for (int i = 0; i < allMessage.Count; i++) {
                        allMessage.message[i] = new Message();
                        allMessage.message[i].ErrorMessage  = allMessageEntity.messageEntity[i].ErrorMessage;
                        allMessage.message[i].Date          = allMessageEntity.messageEntity[i].Date;
                        allMessage.message[i].Sender        = allMessageEntity.messageEntity[i].Sender;
                        allMessage.message[i].Receiver      = allMessageEntity.messageEntity[i].Receiver;
                        allMessage.message[i].Status        = allMessageEntity.messageEntity[i].Status;
                        allMessage.message[i].Context       = allMessageEntity.messageEntity[i].Context;
                    }
                }
        }

        #endregion


        #region 用户取回发件箱 MessageSent(DateTime? startDate, DateTime? endDate)

        /*用户取回发件箱*/
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
                allMessageEntity = userLogic.MessageSent(startDate, endDate);
            }
            AllMessage allMessage = new AllMessage();
            TranslateAllMessageEntityToAllMessageContractData(allMessageEntity, allMessage);

            return allMessage;
        }

        #endregion


        #region 查询处方单在特定药房的花费 GetPrescriptionCost(string sPrescriptionID, string pharmacyID)

        /*查询处方单在特定药房的花费，供User调用。*/
        public PrescriptionCost GetPrescriptionCost(string sPrescriptionID, string pharmacyID) {
            PrescriptionCost prescriptionCost = new PrescriptionCost();
            Guid gPrescriptionID = Guid.Empty;
            try {
                gPrescriptionID = new Guid(sPrescriptionID);
            }
            catch {
                prescriptionCost.ErrorMessage = "Invalid ID! @Service";
                return prescriptionCost;
            }
            PrescriptionCostEntity prescriptionCostEntity = userLogic.GetPrescriptionCost(gPrescriptionID, pharmacyID);
            TranslatePrescriptionCostEntityToPrescriptionCostContractData(prescriptionCostEntity, prescriptionCost);

            return prescriptionCost;
        }

        #endregion


        #region 为处方单付款 PayPrescription(string sPrescriptionID, string pharmacyID, string payPassword)

        /*为处方单付款*/
        public TransactionInfo PayPrescription(string sPrescriptionID, string pharmacyID, string payPassword) {
            TransactionInfo transactionInfo = new TransactionInfo();
            Guid gPrescriptionID = Guid.Empty;
            try {
                gPrescriptionID = new Guid(sPrescriptionID);
            }
            catch {
                transactionInfo.ErrorMessage = "Invalid ID! @Service";
                return transactionInfo;
            }
            TransactionInfoEntity transactionInfoEntity = userLogic.PayPrescription(gPrescriptionID, pharmacyID, payPassword);
            TranslateTransactionInfoEntityToTransactionInfoContractData(transactionInfoEntity, transactionInfo);

            return transactionInfo;
        }

        #endregion


        #region 获取病例的疾病温馨提示 GetDiseaseNotice(string sCaseID)

        /*获取病例的疾病温馨提示*/
        public Notice GetDiseaseNotice(string sCaseID) {
            Notice notice = new Notice();
            Guid gCaseID = Guid.Empty;
            try {
                gCaseID = new Guid(sCaseID);
            }
            catch {
                notice.ErrorMessage = "Invalid ID! @Service";
                return notice;
            }
            NoticeEntity noticeEntity = userLogic.GetDiseaseNotice(gCaseID);
            TranslateNoticeEntityToNoticeInfoContractData(noticeEntity, notice);
            return notice;
        }

        #endregion


        #region 获取药品的疾病温馨提示 GetPhysicNotice(string sPrescriptionID)

        /*获取药品的疾病温馨提示*/
        public Notice GetPhysicNotice(string sPrescriptionID) {
            Notice notice = new Notice();
            Guid gPrescriptionID = Guid.Empty;
            try {
                gPrescriptionID = new Guid(sPrescriptionID);
            }
            catch {
                notice.ErrorMessage = "Invalid ID! @Service";
                return notice;
            }
            NoticeEntity noticeEntity = userLogic.GetPhysicNotice(gPrescriptionID);
            TranslateNoticeEntityToNoticeInfoContractData(noticeEntity, notice);
            return notice;
        }

        #endregion


        #region 将Notice的Entity翻译为数据契约 TranslateNoticeEntityToNoticeInfoContractData(NoticeEntity noticeEntity, Notice notice)

        /*将Notice的Entity翻译为数据契约*/
        private void TranslateNoticeEntityToNoticeInfoContractData(NoticeEntity noticeEntity, Notice notice) {
            notice.ErrorMessage = noticeEntity.ErrorMessage;
            notice.Count = noticeEntity.Count;
            notice.notice = noticeEntity.notice;
        }

        #endregion


    }
}
