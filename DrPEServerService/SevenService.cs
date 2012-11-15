using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Drawing;

namespace DrPEServer.DrPEServerService {

    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
    public class SevenService : ISevenService {

        /*查询排队状况 MyQueueStatus(Guid gAppointment)*/
        public QueueStatus MyQueueStatus(string userID, string password, string sAppointment) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.MyQueueStatus(sAppointment);
            }
            else {
                QueueStatus queueStatus = new QueueStatus();
                queueStatus.ErrorMessage = userInfo.ErrorMessage;
                return queueStatus;
            }
        }

        /*获取该科室的Tips：提交科室ID，返回该科室发布的文章*/
        public AllVeranda GetSectionVeranda(string sectionID) {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.GetSectionVeranda(sectionID);
        }

        /*生成QR码：提交字符串，返回该字符串对应的QR码的Image*/
        public byte[] GuidToQRCode(string sGuid) {
            UserService userService = new UserService();
            return userService.GuidToQRCode(sGuid);
        }

        /*获取药房信息：提交城市名，返回该城市的所有药房的信息*/
        public AllPharmacyInfo GetAllPharmacyInfo(string city) {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.GetAllPharmacyInfo(city);
        }

        /*获取特定药房信息：提交PharmacyID，返回该药房的信息*/
        public PharmacyInfo GetPharmacyInfo(string pharmacyID) {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.GetPharmacyInfo(pharmacyID);
        }


        /*获取医院信息：提交城市名，返回该城市的所有医院的信息*/
        public AllHospitalInfo GetAllHospitalInfo(string city) {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.GetAllHospitalInfo(city);
        }


        /*获取特定医院信息：提交HospitalID*/
        public HospitalInfo GetHospitalInfo(string hospitalID) {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.GetHospitalInfo(hospitalID);
        }


        /*获取科室信息：提交医院编号，返回该医院的所有科室的信息*/
        public AllSectionInfo GetAllSectionInfo(string hospitalID) {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.GetAllSectionInfo(hospitalID);
        }


        /*获取特定科室信息：提交SectionID*/
        public SectionInfo GetSectionInfo(string sectionID) {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.GetSectionInfo(sectionID);
        }


        /*获取医师信息：提交科室编号，返回该科室的所有医师的信息*/
        public AllDoctorInfo GetAllDoctorInfo(string sectionID) {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.GetAllDoctorInfo(sectionID);
        }

        /*获取医师信息：提交医生编号，返回该医师的信息*/
        public DoctorInfo GetDoctorInfo(string doctorID) {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.GetDoctorInfo(doctorID);
        }

        /*获取药品信息：提交药品编号，返回该医师的信息*/
        public PhysicInfo GetPhysicInfo(string physicID) {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.GetPhysicInfo(physicID);
        }

        /*用户登录：若ID和Password均不为空，则转发至DL，将结果翻译为数据契约*/
        public UserInfo Login(string userID, string password) {
            UserService userService = new UserService();
            return userService.Login(userID, password);
        }


        /*获取用户资料：将不为空的ID请求转发至DL，并将反馈结果翻译为数据契约*/
        public UserInfo GetUserInfo(string userID, string password) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.GetUserInfo(userID);
            }
            else {
                return userInfo;
            }
        }


        /*取回所有药物编号和名称*/
        public AllPhysicInfo RetrievePhysicList() {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.RetrievePhysicList();
        }

        
        /*取回所有疾病编号和名称*/
        public AllDiseaseInfo RetrieveDiseaseList() {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.RetrieveDiseaseList();
        }


        /*搜索医师信息：提交关键词，返回含有该关键词的所有医师的信息*/
        public AllDoctorInfo FindDoctorByName(string keyword) {
            OpenAccessService openAccessService = new OpenAccessService();
            return openAccessService.FindDoctorByName(keyword);
        }


        /*查询科室最近一周预约的时间安排*/
        public SectionSchedule GetSectionSchedule(string sectionID) {
            UserService userService = new UserService();
            return userService.GetSectionSchedule(sectionID);
        }


        /*查询医生最近可以预约的时间段*/
        public AvailableDate GetAvailableDate(string doctorID) {
            UserService userService = new UserService();
            return userService.GetAvailableDate(doctorID);
        }


        /*预约医生某时间段*/
        public string MakeAppointment(string userID, string password, string doctorID, DateTime? date) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.MakeAppointment(doctorID, date);
            }
            else {
                return userInfo.ErrorMessage;
            }
        }


        /*获取待去的预约记录*/
        public AllAppointment GetMyFutureAppointment(string userID, string password) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.GetMyFutureAppointment();
            }
            else {
                AllAppointment allAppointment = new AllAppointment();
                allAppointment.ErrorMessage = userInfo.ErrorMessage;
                return allAppointment;
            }
        }


        /*获取指定时间区间中用户所有的交易记录*/
        public AllTransactionInfo GetTransactionHistory(string userID, string password, DateTime? startDate, DateTime? endDate) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.GetTransactionHistory(startDate, endDate);
            }
            else {
                AllTransactionInfo allTransactionInfo = new AllTransactionInfo();
                allTransactionInfo.ErrorMessage = userInfo.ErrorMessage;
                return allTransactionInfo;
            }
        }

     
        /*获取最近一次病历*/
        public CaseInfo GetLastCaseInfo(string userID, string password) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                /*不显示ICD-10编码*/
                return userService.GetLastCaseInfo(false);
            }
            else {
                CaseInfo    caseInfo =  new CaseInfo();
                caseInfo.ErrorMessage = userInfo.ErrorMessage;
                return caseInfo;
            }
        }

        /*获取指定用户在某区间内所有完成的病历*/
        public CaseHistory GetCaseHistory(string userID, string password, DateTime? startDate, DateTime? endDate) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                /*不显示ICD-10编码*/
                return userService.GetCaseHistory(startDate, endDate, false);
            }
            else {
                CaseHistory    caseHistory =  new CaseHistory();
                caseHistory.ErrorMessage = userInfo.ErrorMessage;
                return caseHistory;
            }
        }

        /*读取处方单*/
        public PrescriptionInfo GetPrescriptionInfo(string userID, string password, string sPrescriptionID) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.GetPrescriptionInfo(sPrescriptionID);
            }
            else {
                PrescriptionInfo    prescriptionInfo =  new PrescriptionInfo();
                prescriptionInfo.ErrorMessage = userInfo.ErrorMessage;
                return prescriptionInfo;
            }
        }

        /*读取化验单*/
        public ExaminationInfo GetExaminationInfo(string userID, string password, string sExaminationID) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.GetExaminationInfo(sExaminationID);
            }
            else {
                ExaminationInfo    examinationInfo =  new ExaminationInfo();
                examinationInfo.ErrorMessage = userInfo.ErrorMessage;
                return examinationInfo;
            }
        }

        /*用户撰写邮件User to Doctor*/
        public string MessageCompose(string userID, string password, string receiverID, string text) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.MessageCompose(receiverID, text);
            }
            else {
                return userInfo.ErrorMessage;
            }
        }

        /*用户收取收件箱*/
        public AllMessage MessageInbox(string userID, string password, DateTime? startDate, DateTime? endDate) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.MessageInbox(startDate, endDate);
            }
            else {
                AllMessage allMessage = new AllMessage();
                allMessage.ErrorMessage = userInfo.ErrorMessage;
                return allMessage;
            }
        }

        /*用户取回发件箱*/
        public AllMessage MessageSent(string userID, string password, DateTime? startDate, DateTime? endDate) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.MessageSent(startDate, endDate);
            }
            else {
                AllMessage allMessage = new AllMessage();
                allMessage.ErrorMessage = userInfo.ErrorMessage;
                return allMessage;
            }
        }

        /*查询处方单在特定药房的花费，供User调用。*/
        public PrescriptionCost GetPrescriptionCost(string userID, string password, string sPrescriptionID, string pharmacyID) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.GetPrescriptionCost(sPrescriptionID, pharmacyID);
            }
            else {
                PrescriptionCost prescriptionCost = new PrescriptionCost();
                prescriptionCost.ErrorMessage = userInfo.ErrorMessage;
                return prescriptionCost;
            }
        }

        /*为处方单付款*/
        public TransactionInfo PayPrescription(string userID, string password, string sPrescriptionID, string pharmacyID, string payPassword) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.PayPrescription(sPrescriptionID, pharmacyID, payPassword);
            }
            else {
                TransactionInfo transactionInfo = new TransactionInfo();
                transactionInfo.ErrorMessage = userInfo.ErrorMessage;
                return transactionInfo;
            }
        }


        /*获取病例的疾病温馨提示*/
        public Notice GetDiseaseNotice(string userID, string password, string sCaseID) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.GetDiseaseNotice(sCaseID);
            }
            else {
                Notice notice = new Notice();
                notice.ErrorMessage = userInfo.ErrorMessage;
                return notice;
            }
        }


        /*获取药品的疾病温馨提示*/
        public Notice GetPhysicNotice(string userID, string password, string sPrescriptionID) {
            UserService userService = new UserService();
            UserInfo userInfo = userService.Login(userID, password);
            if (userInfo.ErrorMessage == null) {
                return userService.GetPhysicNotice(sPrescriptionID);
            }
            else {
                Notice notice = new Notice();
                notice.ErrorMessage = userInfo.ErrorMessage;
                return notice;
            }
        }

    }
}

