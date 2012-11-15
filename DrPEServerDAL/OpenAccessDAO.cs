using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrPEServer.DrPEServerEntities;
using System.Data.SqlClient;
using System.Configuration;

namespace DrPEServer.DrPEServerDAL {

    public class OpenAccessDAO {

        /*获取该科室的Tips*/
        public AllVerandaEntity GetSectionVeranda(string sectionID) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            var verandas = from ve in DEntities.VerandaLonelies
                           where ve.SectionID == sectionID
                           orderby ve.ID descending
                           select ve;

            int cnt = 0;
            int verandaCount = verandas.Count();

            AllVerandaEntity allVerandaEntity = null;
            if (verandaCount > 0) {
                allVerandaEntity = new AllVerandaEntity();
                allVerandaEntity.Count = verandaCount;
                allVerandaEntity.verandaEntity = new VerandaEntity[verandaCount];

                foreach (var ve in verandas) {
                    allVerandaEntity.verandaEntity[cnt] = new VerandaEntity();
                    allVerandaEntity.verandaEntity[cnt].Title   = ve.Title;
                    allVerandaEntity.verandaEntity[cnt].Image   = ve.Image;
                    allVerandaEntity.verandaEntity[cnt].Text    = ve.Text;
                    cnt++;
                }
            }

            return allVerandaEntity;
        }


        /*获取药房信息：提交城市名，返回该城市的所有药房的信息*/
        public AllPharmacyInfoEntity GetAllPharmacyInfo(string city) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            var pharmacies = from ph in DEntities.Pharmacies
                             where ph.City == city
                             orderby ph.PharmacyID
                             select ph;

            int cnt = 0;
            int pharmacyCount = pharmacies.Count();

            AllPharmacyInfoEntity allPharmacyInfoEntity = null;
            if (pharmacyCount > 0) {
                allPharmacyInfoEntity = new AllPharmacyInfoEntity();
                allPharmacyInfoEntity.Count = pharmacyCount;
                allPharmacyInfoEntity.pharmacyInfoEntity = new PharmacyInfoEntity[pharmacyCount];

                foreach (var pharmacy in pharmacies) {
                    allPharmacyInfoEntity.pharmacyInfoEntity[cnt] = new PharmacyInfoEntity();
                    allPharmacyInfoEntity.pharmacyInfoEntity[cnt].PharmacyID    = pharmacy.PharmacyID;
                    allPharmacyInfoEntity.pharmacyInfoEntity[cnt].Name          = pharmacy.Name;
                    allPharmacyInfoEntity.pharmacyInfoEntity[cnt].City          = pharmacy.City;
                    allPharmacyInfoEntity.pharmacyInfoEntity[cnt].Address       = pharmacy.Address;
                    allPharmacyInfoEntity.pharmacyInfoEntity[cnt].Latitude      = pharmacy.Latitude;
                    allPharmacyInfoEntity.pharmacyInfoEntity[cnt].Longitude     = pharmacy.Longitude;
                    allPharmacyInfoEntity.pharmacyInfoEntity[cnt].HospitalID    = pharmacy.HospitalID;
                    allPharmacyInfoEntity.pharmacyInfoEntity[cnt].Phone         = pharmacy.Phone;
                    allPharmacyInfoEntity.pharmacyInfoEntity[cnt].Fax           = pharmacy.Fax;

                    cnt++;
                }
            }

            return allPharmacyInfoEntity;
        }


        /*获取特定药房信息：提交PharmacyID，返回该药房的信息*/
        public PharmacyInfoEntity GetPharmacyInfo(string pharmacyID) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询PharmacyID是否存在*/
            Pharmacy pharmacy = (from p in DEntities.Pharmacies
                                 where p.PharmacyID == pharmacyID
                                 select p).FirstOrDefault();

            PharmacyInfoEntity pharmacyInfoEntity = null;
            if (pharmacy != null) {
                pharmacyInfoEntity = new PharmacyInfoEntity();

                pharmacyInfoEntity.PharmacyID = pharmacy.PharmacyID;
                pharmacyInfoEntity.Name = pharmacy.Name;
                pharmacyInfoEntity.City = pharmacy.City;
                pharmacyInfoEntity.Address = pharmacy.Address;
                pharmacyInfoEntity.Latitude = pharmacy.Latitude;
                pharmacyInfoEntity.Longitude = pharmacy.Longitude;
                pharmacyInfoEntity.HospitalID = pharmacy.HospitalID;
                pharmacyInfoEntity.Phone = pharmacy.Phone;
                pharmacyInfoEntity.Fax = pharmacy.Fax;
            }

            return pharmacyInfoEntity;
        }


        /*获取医院信息：提交城市名，返回该城市的所有医院的信息*/
        public AllHospitalInfoEntity GetAllHospitalInfo(string city) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询City域匹配的所有Hospital记录*/
            var hospitals = from h in DEntities.Hospitals
                            where h.City == city
                            orderby h.HospitalID
                            select h;

            int cnt = 0;
            int hospitalCount = hospitals.Count();
            
            AllHospitalInfoEntity allHospitalInfoEntity = null;
            if (hospitalCount > 0) {
                allHospitalInfoEntity                       = new AllHospitalInfoEntity();
                allHospitalInfoEntity.Count                 = hospitalCount;
                allHospitalInfoEntity.hospitalInfoEntity    = new HospitalInfoEntity[hospitalCount];

                foreach (var h in hospitals) {
                    allHospitalInfoEntity.hospitalInfoEntity[cnt] = new HospitalInfoEntity();

                    allHospitalInfoEntity.hospitalInfoEntity[cnt].HospitalID    = h.HospitalID;
                    allHospitalInfoEntity.hospitalInfoEntity[cnt].Name          = h.Name;
                    allHospitalInfoEntity.hospitalInfoEntity[cnt].City          = h.City;
                    allHospitalInfoEntity.hospitalInfoEntity[cnt].Address       = h.Address;
                    allHospitalInfoEntity.hospitalInfoEntity[cnt].Latitude      = h.Latitude;
                    allHospitalInfoEntity.hospitalInfoEntity[cnt].Longitude     = h.Longitude;
                    allHospitalInfoEntity.hospitalInfoEntity[cnt].Type          = h.Type;
                    allHospitalInfoEntity.hospitalInfoEntity[cnt].Grade         = h.Grade;
                    allHospitalInfoEntity.hospitalInfoEntity[cnt].Features      = h.Features;
                    allHospitalInfoEntity.hospitalInfoEntity[cnt].Website       = h.Website;
                    allHospitalInfoEntity.hospitalInfoEntity[cnt].Bed           = h.Bed;

                    cnt++;
                }
            }

            return allHospitalInfoEntity;
        }


        /*获取科室信息：提交医院编号，返回该医院的所有科室的信息*/
        public AllSectionInfoEntity GetAllSectionInfo(string hospitalID) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询HospitalID域匹配的所有Section记录*/
            var sections = from s in DEntities.Sections
                           where s.HospitalID == hospitalID
                           orderby s.SectionID
                           select s;

            int cnt = 0;
            int sectionCount = sections.Count();

            AllSectionInfoEntity allSectionInfoEntity = null;
            if (sectionCount > 0) {
                allSectionInfoEntity                    = new AllSectionInfoEntity();
                allSectionInfoEntity.Count              = sectionCount;
                allSectionInfoEntity.sectionInfoEntity  = new SectionInfoEntity[sectionCount];

                foreach (var s in sections) {
                    allSectionInfoEntity.sectionInfoEntity[cnt] = new SectionInfoEntity();

                    allSectionInfoEntity.sectionInfoEntity[cnt].HospitalID  = s.HospitalID;
                    allSectionInfoEntity.sectionInfoEntity[cnt].SectionID   = s.SectionID;
                    allSectionInfoEntity.sectionInfoEntity[cnt].Place       = s.Place;
                    allSectionInfoEntity.sectionInfoEntity[cnt].Name        = s.Name;
                    allSectionInfoEntity.sectionInfoEntity[cnt].Phone       = s.Phone;
                    allSectionInfoEntity.sectionInfoEntity[cnt].Fax         = s.Fax;

                    cnt++;
                }
            }

            return allSectionInfoEntity;
        }


        /*获取医师信息：提交科室编号，返回该科室的所有医师的信息*/
        public AllDoctorInfoEntity GetAllDoctorInfo(string sectionID) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询SectionID域匹配的所有Doctor记录*/
            var doctors = from d in DEntities.Doctors
                          where d.SectionID == sectionID
                          orderby d.DoctorID
                          select d;

            int cnt = 0;
            int doctorCount = doctors.Count();

            AllDoctorInfoEntity allDoctorInfoEntity = null;
            if (doctorCount > 0) {

                allDoctorInfoEntity = new AllDoctorInfoEntity();
                allDoctorInfoEntity.Count = doctorCount;
                allDoctorInfoEntity.doctorInfoEntity = new DoctorInfoEntity[doctorCount];

                foreach (var d in doctors) {
                    allDoctorInfoEntity.doctorInfoEntity[cnt] = new DoctorInfoEntity();

                    allDoctorInfoEntity.doctorInfoEntity[cnt].SectionID     = d.SectionID;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].DoctorID      = d.DoctorID;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].UserID        = d.UserID;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].LastName      = d.LastName;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].FirstName     = d.FirstName;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Designation   = d.Designation;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Resume        = d.Resume;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Phone         = d.Phone;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Fax           = d.Fax;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Email         = d.Email;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Vocation      = d.Vocation;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Portrait      = d.Portrait;

                    cnt++;
                }
            }

            return allDoctorInfoEntity;
        }


        /*搜索科室信息：提交关键词，返回含有该关键词的所有科室的信息*/
        public AllSectionInfoEntity FindSectionByName(string keyword) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询Name域包含keyword的所有Section记录*/
            keyword = keyword.ToLower();
            var sections = from s in DEntities.Sections
                           where s.Name.ToLower().Contains(keyword)
                           //orderby s.sectionID
                           select s;

            int cnt = 0;
            int sectionCount = sections.Count();

            AllSectionInfoEntity allSectionInfoEntity = null;
            if (sectionCount > 0) {
                allSectionInfoEntity = new AllSectionInfoEntity();
                allSectionInfoEntity.Count = sectionCount;
                allSectionInfoEntity.sectionInfoEntity = new SectionInfoEntity[sectionCount];

                foreach (var s in sections) {
                    allSectionInfoEntity.sectionInfoEntity[cnt] = new SectionInfoEntity();

                    allSectionInfoEntity.sectionInfoEntity[cnt].HospitalID  = s.HospitalID;
                    allSectionInfoEntity.sectionInfoEntity[cnt].SectionID   = s.SectionID;
                    allSectionInfoEntity.sectionInfoEntity[cnt].Place       = s.Place;
                    allSectionInfoEntity.sectionInfoEntity[cnt].Name        = s.Name;
                    allSectionInfoEntity.sectionInfoEntity[cnt].Phone       = s.Phone;
                    allSectionInfoEntity.sectionInfoEntity[cnt].Fax         = s.Fax;

                    cnt++;
                }
            }

            return allSectionInfoEntity;
        }


        /*搜索医师信息：提交关键词，返回含有该关键词的所有医师的信息*/
        public AllDoctorInfoEntity FindDoctorByName(string keyword) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询Name域包含keyword的所有Doctor记录*/
            keyword = keyword.ToLower();
            var doctors = from d in DEntities.Doctors
                          where (d.LastName + " " + d.FirstName).ToLower().Contains(keyword)
                             || (d.FirstName + " " + d.LastName).ToLower().Contains(keyword)
                          //orderby d.DoctorID
                          select d;

            int cnt = 0;
            int doctorCount = doctors.Count();

            AllDoctorInfoEntity allDoctorInfoEntity = null;
            if (doctorCount > 0) {

                allDoctorInfoEntity = new AllDoctorInfoEntity();
                allDoctorInfoEntity.Count = doctorCount;
                allDoctorInfoEntity.doctorInfoEntity = new DoctorInfoEntity[doctorCount];

                foreach (var d in doctors) {
                    allDoctorInfoEntity.doctorInfoEntity[cnt] = new DoctorInfoEntity();

                    allDoctorInfoEntity.doctorInfoEntity[cnt].SectionID     = d.SectionID;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].DoctorID      = d.DoctorID;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].UserID        = d.UserID;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].LastName      = d.LastName;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].FirstName     = d.FirstName;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Designation   = d.Designation;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Resume        = d.Resume;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Phone         = d.Phone;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Fax           = d.Fax;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Email         = d.Email;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Vocation      = d.Vocation;
                    allDoctorInfoEntity.doctorInfoEntity[cnt].Portrait      = d.Portrait;

                    cnt++;
                }
            }

            return allDoctorInfoEntity;
        }


        /*获取特定医院信息：提交HospitalID，返回该医院的信息*/
        public HospitalInfoEntity GetHospitalInfo(string hospitalID) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询HospitalID域匹配的Hospital记录*/
            var hospital = (from h in DEntities.Hospitals
                            where h.HospitalID == hospitalID
                            select h).FirstOrDefault();

            HospitalInfoEntity hospitalInfoEntity = null;
            if (hospital != null) {
                hospitalInfoEntity = new HospitalInfoEntity();

                hospitalInfoEntity.HospitalID   = hospital.HospitalID;
                hospitalInfoEntity.Name         = hospital.Name;
                hospitalInfoEntity.City         = hospital.City;
                hospitalInfoEntity.Address      = hospital.Address;
                hospitalInfoEntity.Latitude     = hospital.Latitude;
                hospitalInfoEntity.Longitude    = hospital.Longitude;
                hospitalInfoEntity.Type         = hospital.Type;
                hospitalInfoEntity.Grade        = hospital.Grade;
                hospitalInfoEntity.Features     = hospital.Features;
                hospitalInfoEntity.Website      = hospital.Website;
                hospitalInfoEntity.Bed          = hospital.Bed;
            }

            return hospitalInfoEntity;
        }


        /*获取特定科室信息：提交SectionID，返回该科室的信息*/
        public SectionInfoEntity GetSectionInfo(string sectionID) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询SectionID域匹配的Section记录*/
            var section = (from s in DEntities.Sections
                           where s.SectionID == sectionID
                           select s).FirstOrDefault();

            SectionInfoEntity sectionInfoEntity = null;
            if (section != null) {
                sectionInfoEntity = new SectionInfoEntity();

                sectionInfoEntity.HospitalID    = section.HospitalID;
                sectionInfoEntity.SectionID     = section.SectionID;
                sectionInfoEntity.Place         = section.Place;
                sectionInfoEntity.Name          = section.Name;
                sectionInfoEntity.Phone         = section.Phone;
                sectionInfoEntity.Fax           = section.Fax;
            }

            return sectionInfoEntity;
        }


        /*获取特定医师信息：提交DoctorID，返回该医师的信息*/
        public DoctorInfoEntity GetDoctorInfo(string doctorID) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询Doctor域为doctorID的Doctor记录*/
            var doctor = (from d in DEntities.Doctors
                          where d.DoctorID == doctorID
                          select d).FirstOrDefault();

            DoctorInfoEntity doctorInfoEntity = null;
            if (doctor != null) {
                doctorInfoEntity = new DoctorInfoEntity();

                doctorInfoEntity.SectionID      = doctor.SectionID;
                doctorInfoEntity.DoctorID       = doctor.DoctorID;
                doctorInfoEntity.UserID         = doctor.UserID;
                doctorInfoEntity.LastName       = doctor.LastName;
                doctorInfoEntity.FirstName      = doctor.FirstName;
                doctorInfoEntity.Designation    = doctor.Designation;
                doctorInfoEntity.Resume         = doctor.Resume;
                doctorInfoEntity.Phone          = doctor.Phone;
                doctorInfoEntity.Fax            = doctor.Fax;
                doctorInfoEntity.Email          = doctor.Email;
                doctorInfoEntity.Vocation       = doctor.Vocation;
                doctorInfoEntity.Portrait       = doctor.Portrait;
            }

            return doctorInfoEntity;
        }


        /*取回所有药物编号和名称*/
        public AllPhysicInfoEntity RetrievePhysicList() {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询所有的药物记录*/
            var physics = from p in DEntities.Physics
                          orderby p.Name
                          select p;

            int cnt = 0;
            int physicCount = physics.Count();

            AllPhysicInfoEntity allPhysicInfoEntity = null;
            if (physicCount > 0) {
                allPhysicInfoEntity                     = new AllPhysicInfoEntity();
                allPhysicInfoEntity.Count               = physicCount;
                allPhysicInfoEntity.physicInfoEntity    = new PhysicInfoEntity[physicCount];

                foreach (var p in physics) {
                    allPhysicInfoEntity.physicInfoEntity[cnt] = new PhysicInfoEntity();

                    allPhysicInfoEntity.physicInfoEntity[cnt].PhysicID  = p.PhysicID;
                    allPhysicInfoEntity.physicInfoEntity[cnt].Name      = p.Name;

                    cnt++;
                }
            }

            return allPhysicInfoEntity;
        }


        /*取回所有疾病编号和名称*/
        public AllDiseaseInfoEntity RetrieveDiseaseList() {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询所有的疾病记录*/
            var diseases = from d in DEntities.Diseases
                           orderby d.DiseaseID
                           select d;

            int cnt = 0;
            int diseaseCount = diseases.Count();

            AllDiseaseInfoEntity allDiseaseInfoEntity = null;
            if (diseaseCount > 0) {
                allDiseaseInfoEntity                    = new AllDiseaseInfoEntity();
                allDiseaseInfoEntity.Count              = diseaseCount;
                allDiseaseInfoEntity.diseaseInfoEntity  = new DiseaseInfoEntity[diseaseCount];

                foreach (var d in diseases) {
                    allDiseaseInfoEntity.diseaseInfoEntity[cnt] = new DiseaseInfoEntity();

                    allDiseaseInfoEntity.diseaseInfoEntity[cnt].DiseaseID   = d.DiseaseID;
                    allDiseaseInfoEntity.diseaseInfoEntity[cnt].Name        = d.Name;

                    cnt++;
                }
            }

            return allDiseaseInfoEntity;
        }


        /*获取特定疾病信息：输入DiseaseID，返回该疾病的完整信息*/
        public DiseaseInfoEntity GetDiseaseInfo(string diseaseID) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            var disease = (from d in DEntities.Diseases
                           where d.DiseaseID == diseaseID
                           select d).FirstOrDefault();

            DiseaseInfoEntity diseaseInfoEntity = null;
            if (disease != null) {
                diseaseInfoEntity = new DiseaseInfoEntity();

                diseaseInfoEntity.DiseaseID     = disease.DiseaseID;
                diseaseInfoEntity.Name          = disease.Name;
                diseaseInfoEntity.Description   = disease.Description;
                diseaseInfoEntity.Notice        = disease.Notice;
            }

            return diseaseInfoEntity;
        }


        /*获取特定药品信息：输入PhysicID，返回该药品的完整信息*/
        public PhysicInfoEntity GetPhysicInfo(string physicID) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            var physic = (from d in DEntities.Physics
                          where d.PhysicID == physicID
                          select d).FirstOrDefault();

            PhysicInfoEntity physicInfoEntity = null;
            if (physic != null) {
                physicInfoEntity = new PhysicInfoEntity();

                physicInfoEntity.PhysicID       = physic.PhysicID;
                physicInfoEntity.Name           = physic.Name;
                physicInfoEntity.Description    = physic.Description;
                physicInfoEntity.Method         = physic.Method;
                physicInfoEntity.Notice         = physic.Notice;
            }

            return physicInfoEntity;
        }


        /*搜索药物信息：提交关键词，返回含有该关键词的所有药物的信息*/
        public AllPhysicInfoEntity FindPhysicByName(string keyword) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            keyword = keyword.ToLower();

            /*查询所有的药物记录*/
            var physics = from p in DEntities.Physics
                          where p.Name.ToLower().Contains(keyword)
                          orderby p.PhysicID
                          select p;

            int cnt = 0;
            int physicCount = physics.Count();

            AllPhysicInfoEntity allPhysicInfoEntity = null;
            if (physicCount > 0) {
                allPhysicInfoEntity                     = new AllPhysicInfoEntity();
                allPhysicInfoEntity.Count               = physicCount;
                allPhysicInfoEntity.physicInfoEntity    = new PhysicInfoEntity[physicCount];

                foreach (var p in physics) {
                    allPhysicInfoEntity.physicInfoEntity[cnt] = new PhysicInfoEntity();

                    allPhysicInfoEntity.physicInfoEntity[cnt].PhysicID      = p.PhysicID;
                    allPhysicInfoEntity.physicInfoEntity[cnt].Name          = p.Name;
                    allPhysicInfoEntity.physicInfoEntity[cnt].Description   = p.Description;
                    allPhysicInfoEntity.physicInfoEntity[cnt].Method        = p.Method;
                    allPhysicInfoEntity.physicInfoEntity[cnt].Notice        = p.Notice;

                    cnt++;
                }
            }

            return allPhysicInfoEntity;
        }


        /*搜索疾病信息：提交关键词，返回含有该关键词的所有疾病的信息*/
        public AllDiseaseInfoEntity FindDiseaseByName(string keyword) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            keyword = keyword.ToLower();

            /*查询所有的疾病记录*/
            var diseases = from d in DEntities.Diseases
                           where d.Name.ToLower().Contains(keyword)
                           orderby d.DiseaseID
                           select d;

            int cnt = 0;
            int diseaseCount = diseases.Count();

            AllDiseaseInfoEntity allDiseaseInfoEntity = null;
            if (diseaseCount > 0) {
                allDiseaseInfoEntity                    = new AllDiseaseInfoEntity();
                allDiseaseInfoEntity.Count              = diseaseCount;
                allDiseaseInfoEntity.diseaseInfoEntity  = new DiseaseInfoEntity[diseaseCount];

                foreach (var d in diseases) {
                    allDiseaseInfoEntity.diseaseInfoEntity[cnt] = new DiseaseInfoEntity();

                    allDiseaseInfoEntity.diseaseInfoEntity[cnt].DiseaseID   = d.DiseaseID;
                    allDiseaseInfoEntity.diseaseInfoEntity[cnt].Name        = d.Name;
                    allDiseaseInfoEntity.diseaseInfoEntity[cnt].Description = d.Description;
                    allDiseaseInfoEntity.diseaseInfoEntity[cnt].Notice      = d.Notice;

                    cnt++;
                }
            }

            return allDiseaseInfoEntity;
        }

    }
}
