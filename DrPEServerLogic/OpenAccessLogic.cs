using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrPEServer.DrPEServerEntities;
using DrPEServer.DrPEServerDAL;

namespace DrPEServer.DrPEServerLogic {

    public class OpenAccessLogic {

        OpenAccessDAO openAccessDAO = new OpenAccessDAO();

        #region 获取该科室的Tips GetSectionVeranda(string sectionID)

        /*获取该科室的Tips */
        public AllVerandaEntity GetSectionVeranda(string sectionID) {

            AllVerandaEntity allVerandaEntity = openAccessDAO.GetSectionVeranda(sectionID);

            if (allVerandaEntity == null) {
                allVerandaEntity = new AllVerandaEntity();
                allVerandaEntity.ErrorMessage = "None Articles! @Logic";
            }

            return allVerandaEntity;
        }

        #endregion


        #region GetAllPharmacyInfo(string city)

        /*获取药房信息：提交城市名，返回该城市的所有药房的信息*/
        public AllPharmacyInfoEntity GetAllPharmacyInfo(string city) {

            AllPharmacyInfoEntity allPharmacyInfoEntity = openAccessDAO.GetAllPharmacyInfo(city);

            if (allPharmacyInfoEntity == null) {
                allPharmacyInfoEntity = new AllPharmacyInfoEntity();
                allPharmacyInfoEntity.ErrorMessage = "134 No Pharmacies in " + city + "! @Logic";
            }

            return allPharmacyInfoEntity;
        }

        #endregion


        #region  GetPharmacyInfo(string pharmacyID)

        /*获取特定药房信息：提交PharmacyID，返回该药房的信息*/
        public PharmacyInfoEntity GetPharmacyInfo(string pharmacyID) {

            PharmacyInfoEntity pharmacyInfoEntity = openAccessDAO.GetPharmacyInfo(pharmacyID);

            if (pharmacyInfoEntity == null) {
                pharmacyInfoEntity = new PharmacyInfoEntity();
                pharmacyInfoEntity.ErrorMessage = "146 No Pharmacy of " + pharmacyID + "! @Logic";
            }

            return pharmacyInfoEntity;
        }

        #endregion


        #region GetAllHospitalInfo(string city)

        /*获取医院信息：提交城市名，返回该城市的所有医院的信息*/
        public AllHospitalInfoEntity GetAllHospitalInfo(string city) {

            AllHospitalInfoEntity allHospitalInfoEntity = openAccessDAO.GetAllHospitalInfo(city);

            if (allHospitalInfoEntity == null) {
                allHospitalInfoEntity = new AllHospitalInfoEntity();
                allHospitalInfoEntity.ErrorMessage = "131 No Hospitals in " + city + "! @Logic";
            }

            return allHospitalInfoEntity;
        }

        #endregion


        #region GetAllSectionInfo(string hospitalID)


        /*获取科室信息：提交医院编号，返回该医院的所有科室的信息*/
        public AllSectionInfoEntity GetAllSectionInfo(string hospitalID) {

            AllSectionInfoEntity allSectionInfoEntity = openAccessDAO.GetAllSectionInfo(hospitalID);

            if (allSectionInfoEntity == null) {
                allSectionInfoEntity = new AllSectionInfoEntity();
                allSectionInfoEntity.ErrorMessage = "132 No Sections in " + hospitalID + "! @Logic";
            }

            return allSectionInfoEntity;
        }

        #endregion


        #region GetAllDoctorInfo(string sectionID)


        /*获取医师信息：提交科室编号，返回该科室的所有医师的信息*/
        public AllDoctorInfoEntity GetAllDoctorInfo(string sectionID) {

            AllDoctorInfoEntity allDoctorInfoEntity = openAccessDAO.GetAllDoctorInfo(sectionID);

            if (allDoctorInfoEntity == null) {
                allDoctorInfoEntity = new AllDoctorInfoEntity();
                allDoctorInfoEntity.ErrorMessage = "133 No Doctors in " + sectionID + "! @Logic";
            }

            return allDoctorInfoEntity;
        }

        #endregion


        #region  FindSectionByName(string keyword)


        /*搜索科室信息：提交关键词，返回含有该关键词的所有科室的信息*/
        public AllSectionInfoEntity FindSectionByName(string keyword) {

            AllSectionInfoEntity allSectionInfoEntity = openAccessDAO.FindSectionByName(keyword);

            if (allSectionInfoEntity == null) {
                allSectionInfoEntity = new AllSectionInfoEntity();
                allSectionInfoEntity.ErrorMessage = "151 No Sections of " + keyword + "! @Logic";
            }

            return allSectionInfoEntity;
        }

        #endregion


        #region  FindDoctorByName(string keyword)


        /*搜索医师信息：提交关键词，返回含有该关键词的所有医师的信息*/
        public AllDoctorInfoEntity FindDoctorByName(string keyword) {
            AllDoctorInfoEntity allDoctorInfoEntity = openAccessDAO.FindDoctorByName(keyword);

            if (allDoctorInfoEntity == null) {
                allDoctorInfoEntity = new AllDoctorInfoEntity();
                allDoctorInfoEntity.ErrorMessage = "152 No Doctors of " + keyword + "! @Logic";
            }

            return allDoctorInfoEntity;
        }

        #endregion


        #region GetHospitalInfo(string hospitalID)


        /*获取特定医院信息：提交HospitalID，返回该医院的信息*/
        public HospitalInfoEntity GetHospitalInfo(string hospitalID) {

            HospitalInfoEntity hospitalInfoEntity = openAccessDAO.GetHospitalInfo(hospitalID);

            if (hospitalInfoEntity == null) {
                hospitalInfoEntity = new HospitalInfoEntity();
                hospitalInfoEntity.ErrorMessage = "141 No Hospital of " + hospitalID + "! @Logic";
            }

            return hospitalInfoEntity;
        }

        #endregion


        #region GetSectionInfo(string sectionID)


        /*获取特定科室信息：提交SectionID，返回该科室的信息*/
        public SectionInfoEntity GetSectionInfo(string sectionID) {

            SectionInfoEntity sectionInfoEntity = openAccessDAO.GetSectionInfo(sectionID);

            if (sectionInfoEntity == null) {
                sectionInfoEntity = new SectionInfoEntity();
                sectionInfoEntity.ErrorMessage = "142 No Section of " + sectionID + "! @Logic";
            }

            return sectionInfoEntity;
        }

        #endregion


        #region GetDoctorInfo(string doctorID)


        /*获取特定医师信息：提交DoctorID，返回该医师的信息*/
        public DoctorInfoEntity GetDoctorInfo(string doctorID) {

            DoctorInfoEntity doctorInfoEntity = openAccessDAO.GetDoctorInfo(doctorID);

            if (doctorInfoEntity == null) {
                doctorInfoEntity = new DoctorInfoEntity();
                doctorInfoEntity.ErrorMessage = "143 No Doctor of " + doctorID + "! @Logic";
            }

            return doctorInfoEntity;
        }

        #endregion


        #region RetrievePhysicList()

        /*取回所有药物编号和名称*/
        public AllPhysicInfoEntity RetrievePhysicList() {
            AllPhysicInfoEntity allPhysicInfoEntity = openAccessDAO.RetrievePhysicList();

            if (allPhysicInfoEntity == null) {
                allPhysicInfoEntity = new AllPhysicInfoEntity();
                allPhysicInfoEntity.ErrorMessage = "161 No Physic Records! @Logic";
            }

            return allPhysicInfoEntity;
        }


        #endregion


        #region RetrieveDiseaseList()

        /*取回所有疾病编号和名称*/
        public AllDiseaseInfoEntity RetrieveDiseaseList() {
            AllDiseaseInfoEntity allDiseaseInfoEntity = openAccessDAO.RetrieveDiseaseList();

            if (allDiseaseInfoEntity == null) {
                allDiseaseInfoEntity = new AllDiseaseInfoEntity();
                allDiseaseInfoEntity.ErrorMessage = "162 No Disease Records! @Logic";
            }

            return allDiseaseInfoEntity;
        }

        #endregion


        #region GetDiseaseInfo(string diseaseID)


        /*获取特定疾病信息：输入DiseaseID，返回该疾病的完整信息*/
        public DiseaseInfoEntity GetDiseaseInfo(string diseaseID) {

            DiseaseInfoEntity diseaseInfoEntity = openAccessDAO.GetDiseaseInfo(diseaseID);

            if (diseaseInfoEntity == null) {
                diseaseInfoEntity = new DiseaseInfoEntity();
                diseaseInfoEntity.ErrorMessage = "145 No Disease of " + diseaseID + "! @Logic";
            }

            return diseaseInfoEntity;
        }


        #endregion


        #region  GetPhysicInfo(string physicID)

        /*获取特定药品信息：输入PhysicID，返回该药品的完整信息*/
        public PhysicInfoEntity GetPhysicInfo(string physicID) {

            PhysicInfoEntity physicInfoEntity = openAccessDAO.GetPhysicInfo(physicID);

            if (physicInfoEntity == null) {
                physicInfoEntity = new PhysicInfoEntity();
                physicInfoEntity.ErrorMessage = "144 No Physic of " + physicID + "! @Logic";
            }

            return physicInfoEntity;
        }

        #endregion


        #region  FindPhysicByName(string keyword)


        /*搜索药物信息：提交关键词，返回含有该关键词的所有药物的信息*/
        public AllPhysicInfoEntity FindPhysicByName(string keyword) {
            AllPhysicInfoEntity allPhysicInfoEntity = openAccessDAO.FindPhysicByName(keyword);

            if (allPhysicInfoEntity == null) {
                allPhysicInfoEntity = new AllPhysicInfoEntity();
                allPhysicInfoEntity.ErrorMessage = "153 No Physic of " + keyword +"! @Logic";
            }

            return allPhysicInfoEntity;
        }

        #endregion


        #region  FindDiseaseByName(string keyword)


        /*搜索疾病信息：提交关键词，返回含有该关键词的所有疾病的信息*/
        public AllDiseaseInfoEntity FindDiseaseByName(string keyword) {
            AllDiseaseInfoEntity allDiseaseInfoEntity = openAccessDAO.FindDiseaseByName(keyword);

            if (allDiseaseInfoEntity == null) {
                allDiseaseInfoEntity = new AllDiseaseInfoEntity();
                allDiseaseInfoEntity.ErrorMessage = "154 No Disease of " + keyword + "! @Logic";
            }

            return allDiseaseInfoEntity;
        }

        #endregion

    }
}

