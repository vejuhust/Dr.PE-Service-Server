using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


namespace DrPEServer.DrPEServerService {


    [ServiceContract]
    public interface IOpenAccessService {

        [OperationContract]
        AllVeranda GetSectionVeranda(string sectionID);

        [OperationContract]
        AllHospitalInfo GetAllHospitalInfo(string city);

        [OperationContract]
        AllSectionInfo GetAllSectionInfo(string hospitalID);

        [OperationContract]
        AllDoctorInfo GetAllDoctorInfo(string sectionID);

        [OperationContract]
        AllSectionInfo FindSectionByName(string keyword);

        [OperationContract]
        AllDoctorInfo FindDoctorByName(string keyword);

        [OperationContract]
        HospitalInfo GetHospitalInfo(string hospitalID);

        [OperationContract]
        SectionInfo GetSectionInfo(string sectionID);

        [OperationContract]
        DoctorInfo GetDoctorInfo(string doctorID);

        [OperationContract]
        AllPhysicInfo RetrievePhysicList();

        [OperationContract]
        AllDiseaseInfo RetrieveDiseaseList();

        [OperationContract]
        PhysicInfo GetPhysicInfo(string physicID);

        [OperationContract]
        DiseaseInfo GetDiseaseInfo(string diseaseID);

        [OperationContract]
        AllPhysicInfo FindPhysicByName(string keyword);

        [OperationContract]
        AllDiseaseInfo FindDiseaseByName(string keyword);

        [OperationContract]
        AllPharmacyInfo GetAllPharmacyInfo(string city);

        [OperationContract]
        PharmacyInfo GetPharmacyInfo(string pharmacyInfo);
    }


    [DataContract]
    public class HospitalInfo {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public string City { get; set; }

        [DataMember (Order = 2)]
        public string HospitalID { get; set; }

        [DataMember (Order = 3)]
        public string Name { get; set; }

        [DataMember (Order = 4)]
        public string Address { get; set; }

        [DataMember (Order = 5)]
        public double? Latitude { get; set; }

        [DataMember (Order = 6)]
        public double? Longitude { get; set; }

        [DataMember (Order = 7)]
        public string Type { get; set; }

        [DataMember (Order = 8)]
        public string Grade { get; set; }

        [DataMember (Order = 9)]
        public string Features { get; set; }

        [DataMember (Order = 10)]
        public string Website { get; set; }

        [DataMember (Order = 11)]
        public int? Bed { get; set; }
    }

    [DataContract]
    public class AllHospitalInfo {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public int Count { get; set; }

        [DataMember (Order = 2)]
        public HospitalInfo[] hospitalInfo = null;
    }

    [DataContract]
    public class Veranda {

        [DataMember(Order = 0)]
        public string Title { get; set; }

        [DataMember(Order = 1)]
        public byte[] Image { get; set; }

        [DataMember(Order = 2)]
        public string Text { get; set; }
    }

    [DataContract]
    public class AllVeranda {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public int Count { get; set; }

        [DataMember(Order = 2)]
        public Veranda[] veranda = null;
    }

    [DataContract]
    public class SectionInfo {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public string HospitalID { get; set; }

        [DataMember (Order = 2)]
        public string SectionID { get; set; }

        [DataMember (Order = 3)]
        public string Place { get; set; }

        [DataMember (Order = 4)]
        public string Name { get; set; }

        [DataMember (Order = 5)]
        public string Phone { get; set; }

        [DataMember (Order = 6)]
        public string Fax { get; set; }
    }

    [DataContract]
    public class AllSectionInfo {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public int Count { get; set; }

        [DataMember (Order = 2)]
        public SectionInfo[] sectionInfo = null;
    }




    [DataContract]
    public class DoctorInfo {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public string SectionID { get; set; }

        [DataMember (Order = 2)]
        public string DoctorID { get; set; }

        [DataMember (Order = 3)]
        public string UserID { get; set; }

        [DataMember (Order = 4)]
        public string LastName { get; set; }

        [DataMember (Order = 5)]
        public string FirstName { get; set; }

        [DataMember (Order = 6)]
        public string Designation { get; set; }

        [DataMember (Order = 7)]
        public string Resume { get; set; }

        [DataMember (Order = 8)]
        public string Phone { get; set; }

        [DataMember (Order = 9)]
        public string Fax { get; set; }

        [DataMember (Order = 10)]
        public string Email { get; set; }

        [DataMember (Order = 11)]
        public string Vocation { get; set; }

        [DataMember (Order = 12)]
        public byte[] Portrait { get; set; }

        [DataMember (Order = 13)]
        public DateTime? LastLoginDate { get; set; }
    }

    [DataContract]
    public class AllDoctorInfo {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public int Count { get; set; }

        [DataMember (Order = 2)]
        public DoctorInfo[] doctorInfo = null;
    }




    [DataContract]
    public class PhysicInfo {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public string PhysicID { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string Description { get; set; }

        [DataMember(Order = 4)]
        public string Method { get; set; }

        [DataMember(Order = 5)]
        public string Notice { get; set; }
    }

    [DataContract]
    public class AllPhysicInfo {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public int Count { get; set; }

        [DataMember(Order = 2)]
        public PhysicInfo[] physicInfo = null;
    }


    [DataContract]
    public class DiseaseInfo {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public string DiseaseID { get; set; }

        [DataMember (Order = 2)]
        public string Name { get; set; }

        [DataMember (Order = 3)]
        public string Description { get; set; }

        [DataMember (Order = 4)]
        public string Notice { get; set; }
    }

    [DataContract]
    public class AllDiseaseInfo {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public int Count { get; set; }

        [DataMember(Order = 2)]
        public DiseaseInfo[] diseaseInfo = null;
    }

}
