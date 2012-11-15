using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Drawing;

namespace DrPEServer.DrPEServerService {

    [ServiceContract]
    public interface ISevenService {

        [OperationContract]
        QueueStatus MyQueueStatus(string userID, string password, string sAppointment);

        [OperationContract]
        AllVeranda GetSectionVeranda(string sectionID);

        [OperationContract]
        byte[] GuidToQRCode(string sGuid);

        [OperationContract]
        AllPharmacyInfo GetAllPharmacyInfo(string city);

        [OperationContract]
        PharmacyInfo GetPharmacyInfo(string pharmacyID);

        [OperationContract]
        AllHospitalInfo GetAllHospitalInfo(string city);

        [OperationContract]
        HospitalInfo GetHospitalInfo(string hospitalID);

        [OperationContract]
        AllSectionInfo GetAllSectionInfo(string hospitalID);

        [OperationContract]
        SectionInfo GetSectionInfo(string sectionID);

        [OperationContract]
        AllDoctorInfo GetAllDoctorInfo(string sectionID);

        [OperationContract]
        DoctorInfo GetDoctorInfo(string doctorID);

        [OperationContract]
        UserInfo Login(string userID, string password);

        [OperationContract]
        UserInfo GetUserInfo(string userID, string password);

        [OperationContract]
        PhysicInfo GetPhysicInfo(string physicID);

        [OperationContract]
        AllPhysicInfo RetrievePhysicList();

        [OperationContract]
        AllDiseaseInfo RetrieveDiseaseList();

        [OperationContract]
        AllDoctorInfo FindDoctorByName(string keyword);

        [OperationContract]
        SectionSchedule GetSectionSchedule(string sectionID);

        [OperationContract]
        AvailableDate GetAvailableDate(string doctorID);

        [OperationContract]
        string MakeAppointment(string userID, string password, string doctorID, DateTime? date);

        [OperationContract]
        AllAppointment GetMyFutureAppointment(string userID, string password);

        [OperationContract]
        AllTransactionInfo GetTransactionHistory(string userID, string password, DateTime? startDate, DateTime? endDate);

        [OperationContract]
        CaseInfo GetLastCaseInfo(string userID, string password);

        [OperationContract]
        CaseHistory GetCaseHistory(string userID, string password, DateTime? startDate, DateTime? endDate);

        [OperationContract]
        PrescriptionInfo GetPrescriptionInfo(string userID, string password, string sPrescriptionID);

        [OperationContract]
        ExaminationInfo GetExaminationInfo(string userID, string password, string sExaminationID);

        [OperationContract]
        string MessageCompose(string userID, string password, string receiverID, string text);

        [OperationContract]
        AllMessage MessageInbox(string userID, string password, DateTime? startDate, DateTime? endDate);

        [OperationContract]
        AllMessage MessageSent(string userID, string password, DateTime? startDate, DateTime? endDate);

        [OperationContract]
        PrescriptionCost GetPrescriptionCost(string userID, string password, string sPrescriptionID, string pharmacyID);

        [OperationContract]
        TransactionInfo PayPrescription(string userID, string password, string sPrescriptionID, string pharmacyID, string payPassword);

        [OperationContract]
        Notice GetPhysicNotice(string userID, string password, string sPrescriptionID);

        [OperationContract]
        Notice GetDiseaseNotice(string userID, string password, string sCaseID);

    }


}
