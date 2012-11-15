using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrPEServer.DrPEServerEntities {

    public class VerandaEntity {
        public string Title { get; set; }
        public byte[] Image { get; set; }
        public string Text { get; set; }
    }

    public class AllVerandaEntity {
        public string ErrorMessage { get; set; }
        public int Count { get; set; }
        public VerandaEntity[] verandaEntity = null;
    }

    public class HospitalInfoEntity {
        public string ErrorMessage { get; set; }
        public string City { get; set; }
        public string HospitalID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Type { get; set; }
        public string Grade { get; set; }
        public string Features { get; set; }
        public string Website { get; set; }
        public int? Bed { get; set; }
    }

    public class AllHospitalInfoEntity {
        public string ErrorMessage { get; set; }
        public int Count { get; set; }
        public HospitalInfoEntity[] hospitalInfoEntity = null;
    }

    public class SectionInfoEntity {
        public string ErrorMessage { get; set; }
        public string HospitalID { get; set; }
        public string SectionID { get; set; }
        public string Place { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
    }

    public class AllSectionInfoEntity {
        public string ErrorMessage { get; set; }
        public int Count { get; set; }
        public SectionInfoEntity[] sectionInfoEntity = null;
    }

    public class DoctorInfoEntity {
        public string ErrorMessage { get; set; }
        public string SectionID { get; set; }
        public string DoctorID { get; set; }
        public string UserID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Designation { get; set; }
        public string Resume { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Vocation { get; set; }
        public byte[] Portrait { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

    public class AllDoctorInfoEntity {
        public string ErrorMessage { get; set; }
        public int Count { get; set; }
        public DoctorInfoEntity[] doctorInfoEntity = null;
    }

    public class PhysicInfoEntity {
        public string ErrorMessage { get; set; }
        public string PhysicID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Method { get; set; }
        public string Notice { get; set; }
    }

    public class AllPhysicInfoEntity {
        public string ErrorMessage { get; set; }
        public int Count { get; set; }
        public PhysicInfoEntity[] physicInfoEntity = null;
    }

    public class DiseaseInfoEntity {
        public string ErrorMessage { get; set; }
        public string DiseaseID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Notice { get; set; }
    }

    public class AllDiseaseInfoEntity {
        public string ErrorMessage { get; set; }
        public int Count { get; set; }
        public DiseaseInfoEntity[] diseaseInfoEntity = null;
    }
}
