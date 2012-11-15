using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Drawing;

namespace DrPEServer.DrPEServerService {
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IUserService {

        [OperationContract]
        QueueStatus MyQueueStatus(string sAppointment);

        [OperationContract]
        byte[] GuidToQRCode(string sGuid);

        [OperationContract]
        UserInfo Login(string userID, string password);

        [OperationContract]
        UserInfo GetUserInfo(string userID);

        [OperationContract]
        SectionSchedule GetSectionSchedule(string sectionID);

        [OperationContract]
        AvailableDate GetAvailableDate(string doctorID);

        [OperationContract]
        string MakeAppointment(string doctorID, DateTime? date);

        [OperationContract]
        AllAppointment GetMyFutureAppointment();

        [OperationContract]
        AllTransactionInfo GetTransactionHistory(DateTime? startDate, DateTime? endDate);

        [OperationContract]
        CaseInfo GetLastCaseInfo(bool showICD);

        [OperationContract]
        CaseHistory GetCaseHistory(DateTime? startDate, DateTime? endDate, bool showICD);

        [OperationContract]
        PrescriptionInfo GetPrescriptionInfo(string sPrescriptionID);

        [OperationContract]
        ExaminationInfo GetExaminationInfo(string sExaminationID);

        [OperationContract]
        string MessageCompose(string receiverID, string text);

        [OperationContract]
        AllMessage MessageInbox(DateTime? startDate, DateTime? endDate);

        [OperationContract]
        AllMessage MessageSent(DateTime? startDate, DateTime? endDate);

        [OperationContract]
        TransactionInfo PayPrescription(string sPrescriptionID, string pharmacyID, string payPassword);

        [OperationContract]
        PrescriptionCost GetPrescriptionCost(string sPrescriptionID, string pharmacyID);

        [OperationContract]
        Notice GetDiseaseNotice(string sCaseID);

        [OperationContract]
        Notice GetPhysicNotice(string sPrescriptionID);

    }


    [DataContract]
    public class QueueStatus {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public int? Capacity { get; set; }

        [DataMember(Order = 2)]
        public int? Process { get; set; }

        [DataMember(Order = 3)]
        public int? Mine { get; set; }

        [DataMember(Order = 4)]
        public DateTime? When { get; set; }
    }


    [DataContract]
    public class UserInfo {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public string UserID { get; set; }

        [DataMember (Order = 2)]
        public string LastName { get; set; }

        [DataMember (Order = 3)]
        public string FirstName { get; set; }

        [DataMember (Order = 4)]
        public string Nationality { get; set; }

        [DataMember (Order = 5)]
        public string Gender { get; set; }

        [DataMember (Order = 6)]
        public string ABO { get; set; }

        [DataMember (Order = 7)]
        public string Rh { get; set; }

        [DataMember (Order = 8)]
        public string Birthplace { get; set; }

        [DataMember (Order = 9)]
        public DateTime? Birthday { get; set; }

        [DataMember (Order = 10)]
        public string Deathplace { get; set; }

        [DataMember (Order = 11)]
        public DateTime? Deathday { get; set; }

        [DataMember (Order = 12)]
        public Decimal? Balance { get; set; }

        [DataMember (Order = 13)]
        public DateTime? LastLoginDate { get; set; }

        [DataMember (Order = 14)]
        public string City { get; set; }

        [DataMember (Order = 15)]
        public string Address { get; set; }

        [DataMember (Order = 16)]
        public string Phone { get; set; }

        [DataMember (Order = 17)]
        public string Email { get; set; }

        [DataMember (Order = 18)]
        public string EmergencyContactPerson { get; set; }
    }

    [DataContract]
    public class AvailableDate {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public int Capacity { get; set; }

        [DataMember(Order = 2)]
        public int Count { get; set; }

        [DataMember(Order = 3)]
        public DateTime []  date = null;

        [DataMember(Order = 4)]
        public int [] appointed = null;
    }

    [DataContract]
    public class SectionScheduleItem {

        [DataMember (Order = 0)]
        public string DoctorID { get; set; }

        [DataMember (Order = 1)]
        public DateTime Date { get; set; }

        [DataMember (Order = 2)]
        public int Appointed { get; set; }

        [DataMember (Order = 3)]
        public int Capacity { get; set; }
    }


    [DataContract]
    public class SectionSchedule {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public int Count { get; set; }

        [DataMember (Order = 2)]
        public SectionScheduleItem [] Detail = null;
    }


    [DataContract]
    public class Appointment {

        [DataMember(Order = 0)]
        public string sGuid { get; set; }

        [DataMember(Order = 1)]
        public string DoctorID { get; set; }

        [DataMember(Order = 2)]
        public string UserID { get; set; }

        [DataMember(Order = 3)]
        public DateTime? Date { get; set; }

        [DataMember(Order = 4)]
        public int? Rank { get; set; }

        [DataMember(Order = 5)]
        public bool Finished { get; set; }
    }


    [DataContract]
    public class AllAppointment {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public int Count { get; set; }

        [DataMember(Order = 2)]
        public Appointment [] appointment = null;
    }

}
