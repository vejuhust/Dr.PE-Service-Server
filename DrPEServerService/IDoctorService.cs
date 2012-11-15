using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


namespace DrPEServer.DrPEServerService {
    [ServiceContract]
    public interface IDoctorService {

        [OperationContract]
        string FinishAppointment(string sGuid);

        [OperationContract]
        string UserIDThroughAppointment(string sGuid);

        [OperationContract]
        DoctorInfo Login(string doctorID, string password);

        [OperationContract]
        AllAppointment GetMyAppointment();

        [OperationContract]
        UserInfo GetUserInfo(string userID);

        [OperationContract]
        CaseInfo CreateCase(CaseInfo caseInfo);

        [OperationContract]
        CaseInfo ModifyCase(CaseInfo newCase);

        [OperationContract]
        CaseHistory GetCaseHistory(string userID, DateTime? startDate, DateTime? endDate, bool showICD);

        [OperationContract]
        CaseInfo GetCaseInfo(string sCaseID, bool showICD);

        [OperationContract]
        PrescriptionInfo GetPrescriptionInfo(string sPrescriptionID);

        [OperationContract]
        ExaminationInfo GetExaminationInfo(string sExaminationID);

        [OperationContract]
        string CreatePrescription(PrescriptionInfo prescriptionInfo);

        [OperationContract]
        CaseHistory RetrieveDoctorCase(DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD);

        [OperationContract]
        CaseHistory RetrieveSectionCase(DateTime? startDate, DateTime? endDate, bool isDraft, bool showICD);

        [OperationContract]
        string MessageCompose(string receiverID, string text);

        [OperationContract]
        AllMessage MessageSent(DateTime? startDate, DateTime? endDate);

        [OperationContract]
        AllMessage MessageInbox(DateTime? startDate, DateTime? endDate);

        [OperationContract]
        string SetDailySchedule(DayOfWeek dayOfWeek, int am);

        [OperationContract]
        string SetAdditionSchedule(string doctorID, DateTime? date);

        [OperationContract]
        string SetExceptionSchedule(string doctorID, DateTime? date);

    }

    [DataContract]
    public class PrescriptionInfo {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public string PrescriptionID { get; set; }

        [DataMember(Order = 2)]
        public int Count { get; set; }

        [DataMember(Order = 3)]
        public string []    physicID = null;

        [DataMember(Order = 4)]
        public int []       number = null;
    }


    [DataContract]
    public class ExaminationInfo {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public string ExaminationID { get; set; }

        [DataMember(Order = 2)]
        public DateTime? Date { get; set; }

        [DataMember(Order = 3)]
        public string Type { get; set; }

        [DataMember(Order = 4)]
        public string Text { get; set; }

        [DataMember(Order = 5)]
        public string Advice { get; set; }

        [DataMember(Order = 6)]
        public byte [] Image = null;
    }


    [DataContract]
    public class CaseInfo {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public string CaseID { get; set; }

        [DataMember(Order = 2)]
        public string ExaminationID { get; set; }

        [DataMember(Order = 3)]
        public string PrescriptionID { get; set; }

        [DataMember(Order = 4)]
        public string UserID { get; set; }

        [DataMember(Order = 5)]
        public string DoctorID { get; set; }

        [DataMember(Order = 6)]
        public string SectionID { get; set; }

        [DataMember(Order = 7)]
        public DateTime? Date { get; set; }

        [DataMember(Order = 8)]
        public string ChiefComplaint { get; set; }

        [DataMember(Order = 9)]
        public string TentativeDiagnosis { get; set; }

        [DataMember(Order = 10)]
        public string DifferentialDiagnosis { get; set; }

        [DataMember(Order = 11)]
        public string TreatmentPlan { get; set; }

        [DataMember(Order = 12)]
        public DateTime? CountercheckDate { get; set; }

        [DataMember(Order = 13)]
        public bool IsDraft { get; set; }
    }


    [DataContract]
    public class CaseHistory {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public int Count { get; set; }

        [DataMember(Order = 2)]
        public CaseInfo [] caseInfo = null;
    }


    [DataContract]
    public class Message {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public string MessageID { get; set; }

        [DataMember(Order = 2)]
        public DateTime? Date { get; set; }

        [DataMember(Order = 3)]
        public string Sender { get; set; }

        [DataMember(Order = 4)]
        public string Receiver { get; set; }

        [DataMember(Order = 5)]
        public string Status { get; set; }

        [DataMember(Order = 6)]
        public string Context { get; set; }
    }


    [DataContract]
    public class AllMessage {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public int Count { get; set; }

        [DataMember(Order = 2)]
        public Message [] message = null;
    }

    [DataContract]
    public class Notice {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public int Count { get; set; }

        [DataMember(Order = 2)]
        public string [] notice = null;
    }

}
