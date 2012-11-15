using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DrPEServer.DrPEServerEntities;
using DrPEServer.DrPEServerLogic;

namespace DrPEServer.DrPEServerService {

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
    class OpenAccessService : IOpenAccessService {

        OpenAccessLogic openAccessLogic = new OpenAccessLogic();

        #region 获取该科室的Tips GetSectionVeranda(string sectionID)

        /*获取该科室的Tips */
        public AllVeranda GetSectionVeranda(string sectionID) {

            AllVerandaEntity allVerandaEntity = null;

            if (sectionID == null) {
                allVerandaEntity = new AllVerandaEntity();
                allVerandaEntity.ErrorMessage = "Empty SectionID! @Service";
            }
            else {
                allVerandaEntity = openAccessLogic.GetSectionVeranda(sectionID);
            }
            AllVeranda allVeranda = new AllVeranda();
            TranslateAllVerandaEntityToAllVerandaContractData(allVerandaEntity, allVeranda);

            return allVeranda;
        }

        #endregion


        #region 获取药房信息 GetAllPharmacyInfo(string city)

        /*获取药房信息：提交城市名，返回该城市的所有药房的信息*/
        public AllPharmacyInfo GetAllPharmacyInfo(string city) {

            AllPharmacyInfoEntity allPharmacyInfoEntity = null;

            if (city == null) {
                allPharmacyInfoEntity = new AllPharmacyInfoEntity();
                allPharmacyInfoEntity.ErrorMessage = "104 Empty City! @Service";
            }
            else {
                allPharmacyInfoEntity = openAccessLogic.GetAllPharmacyInfo(city);
            }
            AllPharmacyInfo allPharmacyInfo = new AllPharmacyInfo();
            TranslateAllPharmacyInfoEntityToAllPharmacyInfoContractData(allPharmacyInfoEntity, allPharmacyInfo);

            return allPharmacyInfo;
        }

        #endregion


        #region 获取特定药房信息 GetPharmacyInfo(string pharmacyID)

        /*获取特定药房信息：提交PharmacyID，返回该药房的信息*/
        public PharmacyInfo GetPharmacyInfo(string pharmacyID) {

            PharmacyInfoEntity pharmacyInfoEntity = null;

            if (pharmacyID == null) {
                pharmacyInfoEntity = new PharmacyInfoEntity();
                pharmacyInfoEntity.ErrorMessage = "116 Empty pharmacyID! @Service";
            }
            else {
                pharmacyInfoEntity = openAccessLogic.GetPharmacyInfo(pharmacyID);
            }
            PharmacyInfo pharmacyInfo = new PharmacyInfo();
            TranslatePharmacyInfoEntityToPharmacyInfoContractData(pharmacyInfoEntity, pharmacyInfo);

            return pharmacyInfo;
        }

        #endregion


        #region 获取医院信息 GetAllHospitalInfo(string city)

        /*获取医院信息：提交城市名，返回该城市的所有医院的信息*/
        public AllHospitalInfo GetAllHospitalInfo(string city) {

            AllHospitalInfoEntity allHospitalInfoEntity = null;

            if (city == null) {
                allHospitalInfoEntity = new AllHospitalInfoEntity();
                allHospitalInfoEntity.ErrorMessage = "101 Empty City! @Service";
            }
            else {
                allHospitalInfoEntity = openAccessLogic.GetAllHospitalInfo(city);
            }
            AllHospitalInfo allHospitalInfo = new AllHospitalInfo();
            TranslateAllHospitalInfoEntityToAllHospitalInfoContractData(allHospitalInfoEntity, allHospitalInfo);

            return allHospitalInfo;
        }

        #endregion


        #region 获取科室信息 GetAllSectionInfo(string hospitalID)

        /*获取科室信息：提交医院编号，返回该医院的所有科室的信息*/
        public AllSectionInfo GetAllSectionInfo(string hospitalID) {

            AllSectionInfoEntity allSectionInfoEntity = null;

            if (hospitalID == null) {
                allSectionInfoEntity = new AllSectionInfoEntity();
                allSectionInfoEntity.ErrorMessage = "102 Empty HospitalID! @Service";
            }
            else {
                allSectionInfoEntity = openAccessLogic.GetAllSectionInfo(hospitalID);
            }
            AllSectionInfo allSectionInfo = new AllSectionInfo();
            TranslateAllSectionInfoEntityToAllSectionInfoContractData(allSectionInfoEntity, allSectionInfo);

            return allSectionInfo;
        }

        #endregion


        #region 获取医师信息 GetAllDoctorInfo(string sectionID)

        /*获取医师信息：提交科室编号，返回该科室的所有医师的信息*/
        public AllDoctorInfo GetAllDoctorInfo(string sectionID) {

            AllDoctorInfoEntity allDoctorInfoEntity = null;

            if (sectionID == null) {
                allDoctorInfoEntity = new AllDoctorInfoEntity();
                allDoctorInfoEntity.ErrorMessage = "103 Empty SectionID! @Service";
            }
            else {
                allDoctorInfoEntity = openAccessLogic.GetAllDoctorInfo(sectionID);
            }
            AllDoctorInfo allDoctorInfo = new AllDoctorInfo();
            TranslateAllDoctorInfoEntityToAllDoctorInfoContractData(allDoctorInfoEntity, allDoctorInfo);

            return allDoctorInfo;
        }

        #endregion


        #region 搜索科室信息 FindSectionByName(string keyword)

        /*搜索科室信息：提交关键词，返回含有该关键词的所有科室的信息*/
        public AllSectionInfo FindSectionByName(string keyword) {

            AllSectionInfoEntity allSectionInfoEntity = null;

            if (keyword == null) {
                allSectionInfoEntity = new AllSectionInfoEntity();
                allSectionInfoEntity.ErrorMessage = "121 Empty Keyword of SectionName! @Service";
            }
            else {
                allSectionInfoEntity = openAccessLogic.FindSectionByName(keyword);
            }
            AllSectionInfo allSectionInfo = new AllSectionInfo();
            TranslateAllSectionInfoEntityToAllSectionInfoContractData(allSectionInfoEntity, allSectionInfo);

            return allSectionInfo;
        }

        #endregion


        #region 搜索医师信息 FindDoctorByName(string keyword)

        /*搜索医师信息：提交关键词，返回含有该关键词的所有医师的信息*/
        public AllDoctorInfo FindDoctorByName(string keyword) {

            AllDoctorInfoEntity allDoctorInfoEntity = null;

            if (keyword == null) {
                allDoctorInfoEntity = new AllDoctorInfoEntity();
                allDoctorInfoEntity.ErrorMessage = "122 Empty Keyword of DoctorName! @Service";
            }
            else {
                allDoctorInfoEntity = openAccessLogic.FindDoctorByName(keyword);
            }
            AllDoctorInfo allDoctorInfo = new AllDoctorInfo();
            TranslateAllDoctorInfoEntityToAllDoctorInfoContractData(allDoctorInfoEntity, allDoctorInfo);

            return allDoctorInfo;
        }

        #endregion


        #region 获取特定医院信息 GetHospitalInfo(string hospitalID)

        /*获取特定医院信息：提交HospitalID，返回该医院的信息*/
        public HospitalInfo GetHospitalInfo(string hospitalID) {

            HospitalInfoEntity hospitalInfoEntity = null;

            if (hospitalID == null) {
                hospitalInfoEntity = new HospitalInfoEntity();
                hospitalInfoEntity.ErrorMessage = "111 Empty hospitalID! @Service";
            }
            else {
                hospitalInfoEntity = openAccessLogic.GetHospitalInfo(hospitalID);
            }
            HospitalInfo hospitalInfo = new HospitalInfo();
            TranslateHospitalInfoEntityToHospitalInfoContractData(hospitalInfoEntity, hospitalInfo);

            return hospitalInfo;
        }

        #endregion


        #region 获取特定科室信息 GetSectionInfo(string sectionID)

        /*获取特定科室信息：提交SectionID，返回该科室的信息*/
        public SectionInfo GetSectionInfo(string sectionID) {

            SectionInfoEntity sectionInfoEntity = null;

            if (sectionID == null) {
                sectionInfoEntity = new SectionInfoEntity();
                sectionInfoEntity.ErrorMessage = "112 Empty sectionID! @Service";
            }
            else {
                sectionInfoEntity = openAccessLogic.GetSectionInfo(sectionID);
            }
            SectionInfo sectionInfo = new SectionInfo();
            TranslateSectionInfoEntityToSectionInfoContractData(sectionInfoEntity, sectionInfo);

            return sectionInfo;
        }

        #endregion


        #region 获取特定医生信息 GetDoctorInfo(string doctorID)

        /*获取特定医师信息：提交DoctorID，返回该医师的信息*/
        public DoctorInfo GetDoctorInfo(string doctorID) {

            DoctorInfoEntity doctorInfoEntity = null;

            if (doctorID == null) {
                doctorInfoEntity = new DoctorInfoEntity();
                doctorInfoEntity.ErrorMessage = "113 Empty doctorID! @Service";
            }
            else {
                doctorInfoEntity = openAccessLogic.GetDoctorInfo(doctorID);
            }
            DoctorInfo doctorInfo = new DoctorInfo();
            TranslateDoctorInfoEntityToDoctorInfoContractData(doctorInfoEntity, doctorInfo);

            return doctorInfo;
        }

        #endregion


        #region 获取所有药物编号和名称 RetrievePhysicList()

        /*取回所有药物编号和名称*/
        public AllPhysicInfo RetrievePhysicList() {
            AllPhysicInfoEntity allPhysicInfoEntity = openAccessLogic.RetrievePhysicList();
            AllPhysicInfo allPhysicInfo = new AllPhysicInfo();
            TranslateAllPhysicInfoEntityToAllPhysicInfoContractData(allPhysicInfoEntity, allPhysicInfo);
            return allPhysicInfo;
        }

        #endregion


        #region 获取所有疾病编号和名称 RetrieveDiseaseList()

        /*取回所有疾病编号和名称*/
        public AllDiseaseInfo RetrieveDiseaseList() {
            AllDiseaseInfoEntity allDiseaseInfoEntity = openAccessLogic.RetrieveDiseaseList();
            AllDiseaseInfo allDiseaseInfo = new AllDiseaseInfo();
            TranslateAllDiseaseInfoEntityToAllDiseaseInfoContractData(allDiseaseInfoEntity, allDiseaseInfo);
            return allDiseaseInfo;
        }

        #endregion


        #region 获取特定疾病信息 GetDiseaseInfo(string diseaseID)

        /*获取特定疾病信息：输入DiseaseID，返回该疾病的完整信息*/
        public DiseaseInfo GetDiseaseInfo(string diseaseID) {

            DiseaseInfoEntity diseaseInfoEntity = null;

            if (diseaseID == null) {
                diseaseInfoEntity = new DiseaseInfoEntity();
                diseaseInfoEntity.ErrorMessage = "115 Empty diseaseID! @Service";
            }
            else {
                diseaseInfoEntity = openAccessLogic.GetDiseaseInfo(diseaseID);
            }
            DiseaseInfo diseaseInfo = new DiseaseInfo();
            TranslateDiseaseInfoEntityToDiseaseInfoContractData(diseaseInfoEntity, diseaseInfo);

            return diseaseInfo;
        }

        #endregion 


        #region 获取特定药品信息 GetPhysicInfo(string physicID)

        /*获取特定药品信息：输入PhysicID，返回该药品的完整信息*/
        public PhysicInfo GetPhysicInfo(string physicID) {

            PhysicInfoEntity physicInfoEntity = null;

            if (physicID == null) {
                physicInfoEntity = new PhysicInfoEntity();
                physicInfoEntity.ErrorMessage = "114 Empty physicID! @Service";
            }
            else {
                physicInfoEntity = openAccessLogic.GetPhysicInfo(physicID);
            }
            PhysicInfo physicInfo = new PhysicInfo();
            TranslatePhysicInfoEntityToPhysicInfoContractData(physicInfoEntity, physicInfo);

            return physicInfo;
        }

        #endregion


        #region 搜索药物信息 FindPhysicByName(string keyword)

        /*搜索药物信息：提交关键词，返回含有该关键词的所有药物的信息*/
        public AllPhysicInfo FindPhysicByName(string keyword) {
            AllPhysicInfoEntity allPhysicInfoEntity = null;

            if (keyword == null) {
                allPhysicInfoEntity = new AllPhysicInfoEntity();
                allPhysicInfoEntity.ErrorMessage = "123 Empty Keyword of Physic Name! @Service";
            }
            else {
                allPhysicInfoEntity = openAccessLogic.FindPhysicByName(keyword);
            }
            AllPhysicInfo allPhysicInfo = new AllPhysicInfo();
            TranslateAllPhysicInfoEntityToAllPhysicInfoContractData(allPhysicInfoEntity, allPhysicInfo);

            return allPhysicInfo;
        }

        #endregion


        #region 搜索疾病信息 FindDiseaseByName(string keyword)

        /*搜索疾病信息：提交关键词，返回含有该关键词的所有疾病的信息*/
        public AllDiseaseInfo FindDiseaseByName(string keyword) {
            AllDiseaseInfoEntity allDiseaseInfoEntity = null;

            if (keyword == null) {
                allDiseaseInfoEntity = new AllDiseaseInfoEntity();
                allDiseaseInfoEntity.ErrorMessage = "124 Empty Keyword of Disease Name! @Service";
            }
            else {
                allDiseaseInfoEntity = openAccessLogic.FindDiseaseByName(keyword);
            }
            AllDiseaseInfo allDiseaseInfo = new AllDiseaseInfo();
            TranslateAllDiseaseInfoEntityToAllDiseaseInfoContractData(allDiseaseInfoEntity, allDiseaseInfo);

            return allDiseaseInfo;
        }

        #endregion


        #region 将AllVeranda对应的Entity翻译为数据契约 TranslateAllVerandaEntityToAllVerandaContractData(allVerandaEntity, allVeranda)

        private void TranslateAllVerandaEntityToAllVerandaContractData(
            AllVerandaEntity allVerandaEntity,
            AllVeranda allVeranda) {
                int cnt = 0;
                allVeranda.ErrorMessage = allVerandaEntity.ErrorMessage;
                allVeranda.Count = allVerandaEntity.Count;
                if (allVeranda.Count > 0) {
                    allVeranda.veranda = new Veranda[allVeranda.Count];
                    for (cnt = 0; cnt < allVeranda.Count; cnt++) {
                        allVeranda.veranda[cnt] = new Veranda();
                        allVeranda.veranda[cnt].Title = allVerandaEntity.verandaEntity[cnt].Title;
                        allVeranda.veranda[cnt].Image = allVerandaEntity.verandaEntity[cnt].Image;
                        allVeranda.veranda[cnt].Text = allVerandaEntity.verandaEntity[cnt].Text;
                    }
                }
        }

        #endregion


        #region 将AllPharmacyInfo对应的Entity翻译为数据契约 TranslatePharmacyInfoEntityToPharmacyInfoContractData()

        /*将AllPharmacyInfo对应的Entity翻译为数据契约，调用TranslatePharmacyInfoEntityToPharmacyInfoContractData()*/
        private void TranslateAllPharmacyInfoEntityToAllPharmacyInfoContractData(
            AllPharmacyInfoEntity allPharmacyInfoEntity,
            AllPharmacyInfo allPharmacyInfo) {

            int cnt = 0;

            allPharmacyInfo.ErrorMessage = allPharmacyInfoEntity.ErrorMessage;
            allPharmacyInfo.Count = allPharmacyInfoEntity.Count;

            if (allPharmacyInfo.Count > 0) {
                allPharmacyInfo.pharmacyInfo = new PharmacyInfo[allPharmacyInfo.Count];
                for (cnt = 0; cnt < allPharmacyInfo.Count; cnt++) {
                    allPharmacyInfo.pharmacyInfo[cnt] = new PharmacyInfo();
                    TranslatePharmacyInfoEntityToPharmacyInfoContractData(
                        allPharmacyInfoEntity.pharmacyInfoEntity[cnt],
                        allPharmacyInfo.pharmacyInfo[cnt]);
                }
            }
        }

        #endregion


        #region 将PharmacyInfo对应的Entity翻译为数据契约 TranslatePharmacyInfoEntityToPharmacyInfoContractData()

        /*将PharmacyInfo对应的Entity翻译为数据契约*/
        private void TranslatePharmacyInfoEntityToPharmacyInfoContractData(
            PharmacyInfoEntity pharmacyInfoEntity,
            PharmacyInfo pharmacyInfo) {
            pharmacyInfo.ErrorMessage = pharmacyInfoEntity.ErrorMessage;
            pharmacyInfo.PharmacyID = pharmacyInfoEntity.PharmacyID;
            pharmacyInfo.Name = pharmacyInfoEntity.Name;
            pharmacyInfo.City = pharmacyInfoEntity.City;
            pharmacyInfo.Address = pharmacyInfoEntity.Address;
            pharmacyInfo.Latitude = pharmacyInfoEntity.Latitude;
            pharmacyInfo.Longitude = pharmacyInfoEntity.Longitude;
            pharmacyInfo.HospitalID = pharmacyInfoEntity.HospitalID;
            pharmacyInfo.Phone = pharmacyInfoEntity.Phone;
            pharmacyInfo.Fax = pharmacyInfoEntity.Fax;
            pharmacyInfo.LastLoginDate = pharmacyInfoEntity.LastLoginDate;
        }

        #endregion


        #region 将AllPhysicInfo对应的Entity翻译为数据契约 TranslateAllPhysicInfoEntityToAllPhysicInfoContractData()

        /*将AllPhysicInfo对应的Entity翻译为数据契约，调用TranslatePhysicInfoEntityToPhysicInfoContractData()*/
        private void TranslateAllPhysicInfoEntityToAllPhysicInfoContractData(
            AllPhysicInfoEntity allPhysicInfoEntity,
            AllPhysicInfo       allPhysicInfo) {

            int cnt = 0;

            allPhysicInfo.ErrorMessage  = allPhysicInfoEntity.ErrorMessage;
            allPhysicInfo.Count         = allPhysicInfoEntity.Count;

            if (allPhysicInfo.Count > 0) {
                allPhysicInfo.physicInfo = new PhysicInfo[allPhysicInfo.Count];
                for (cnt = 0; cnt < allPhysicInfo.Count; cnt++) {
                    allPhysicInfo.physicInfo[cnt] = new PhysicInfo();
                    TranslatePhysicInfoEntityToPhysicInfoContractData(
                        allPhysicInfoEntity.physicInfoEntity[cnt],
                        allPhysicInfo.physicInfo[cnt]);
                }
            }
        }

        #endregion


        #region 将PhysicInfo对应的Entity翻译为数据契约 TranslatePhysicInfoEntityToPhysicInfoContractData()

        /*将PhysicInfo对应的Entity翻译为数据契约*/
        private void TranslatePhysicInfoEntityToPhysicInfoContractData(
            PhysicInfoEntity    physicInfoEntity,
            PhysicInfo          physicInfo) {

                physicInfo.ErrorMessage = physicInfoEntity.ErrorMessage;
                physicInfo.PhysicID     = physicInfoEntity.PhysicID;
                physicInfo.Name         = physicInfoEntity.Name;
                physicInfo.Description  = physicInfoEntity.Description;
                physicInfo.Method       = physicInfoEntity.Method;
                physicInfo.Notice       = physicInfoEntity.Notice;
        }

        #endregion


        #region 将AllDiseaseInfo对应的Entity翻译为数据契约 TranslateAllDiseaseInfoEntityToAllDiseaseInfoContractData()

        /*将AllDiseaseInfo对应的Entity翻译为数据契约，调用TranslateDiseaseInfoEntityToDiseaseInfoContractData()*/
        private void TranslateAllDiseaseInfoEntityToAllDiseaseInfoContractData(
            AllDiseaseInfoEntity allDiseaseInfoEntity,
            AllDiseaseInfo allDiseaseInfo) {

            int cnt = 0;

            allDiseaseInfo.ErrorMessage = allDiseaseInfoEntity.ErrorMessage;
            allDiseaseInfo.Count = allDiseaseInfoEntity.Count;

            if (allDiseaseInfo.Count > 0) {
                allDiseaseInfo.diseaseInfo = new DiseaseInfo[allDiseaseInfo.Count];
                for (cnt = 0; cnt < allDiseaseInfo.Count; cnt++) {
                    allDiseaseInfo.diseaseInfo[cnt] = new DiseaseInfo();
                    TranslateDiseaseInfoEntityToDiseaseInfoContractData(
                        allDiseaseInfoEntity.diseaseInfoEntity[cnt],
                        allDiseaseInfo.diseaseInfo[cnt]);
                }
            }
        }

        #endregion


        #region 将DiseaseInfo对应的Entity翻译为数据契约 TranslateDiseaseInfoEntityToDiseaseInfoContractData

        /*将DiseaseInfo对应的Entity翻译为数据契约*/
        private void TranslateDiseaseInfoEntityToDiseaseInfoContractData(
            DiseaseInfoEntity diseaseInfoEntity,
            DiseaseInfo diseaseInfo) {

            diseaseInfo.ErrorMessage = diseaseInfoEntity.ErrorMessage;
            diseaseInfo.DiseaseID = diseaseInfoEntity.DiseaseID;
            diseaseInfo.Name = diseaseInfoEntity.Name;
            diseaseInfo.Description = diseaseInfoEntity.Description;
            //diseaseInfo.Method = diseaseInfoEntity.Method;
            diseaseInfo.Notice = diseaseInfoEntity.Notice;
        }

        #endregion


        #region 将AllHospitalInfo对应的Entity翻译为数据契约 TranslateAllHospitalInfoEntityToAllHospitalInfoContractData()

        /*将AllHospitalInfo对应的Entity翻译为数据契约，调用TranslateHospitalInfoEntityToHospitalInfoContractData()*/
        private void TranslateAllHospitalInfoEntityToAllHospitalInfoContractData(
            AllHospitalInfoEntity   allHospitalInfoEntity,
            AllHospitalInfo         allHospitalInfo) {

                int cnt = 0;

                allHospitalInfo.ErrorMessage    = allHospitalInfoEntity.ErrorMessage;
                allHospitalInfo.Count           = allHospitalInfoEntity.Count;

                if (allHospitalInfo.Count > 0) {
                    allHospitalInfo.hospitalInfo = new HospitalInfo[allHospitalInfo.Count];
                    for (cnt = 0; cnt < allHospitalInfo.Count; cnt++) {
                        allHospitalInfo.hospitalInfo[cnt] = new HospitalInfo();
                        TranslateHospitalInfoEntityToHospitalInfoContractData(
                            allHospitalInfoEntity.hospitalInfoEntity[cnt],
                            allHospitalInfo.hospitalInfo[cnt]);
                    }
                }
        }

        #endregion


        #region 将HospitalInfo对应的Entity翻译为数据契约 TranslateHospitalInfoEntityToHospitalInfoContractData()

        /*将HospitalInfo对应的Entity翻译为数据契约*/
        private void TranslateHospitalInfoEntityToHospitalInfoContractData(
            HospitalInfoEntity  hospitalInfoEntity,
            HospitalInfo        hospitalInfo) {

                hospitalInfo.ErrorMessage   = hospitalInfoEntity.ErrorMessage;

                hospitalInfo.City           = hospitalInfoEntity.City;
                hospitalInfo.HospitalID     = hospitalInfoEntity.HospitalID;
                hospitalInfo.Name           = hospitalInfoEntity.Name;
                hospitalInfo.Address        = hospitalInfoEntity.Address;
                hospitalInfo.Latitude       = hospitalInfoEntity.Latitude;
                hospitalInfo.Longitude      = hospitalInfoEntity.Longitude;
                hospitalInfo.Type           = hospitalInfoEntity.Type;
                hospitalInfo.Grade          = hospitalInfoEntity.Grade;
                hospitalInfo.Features       = hospitalInfoEntity.Features;
                hospitalInfo.Website        = hospitalInfoEntity.Website;
                hospitalInfo.Bed            = hospitalInfoEntity.Bed;
        }

        #endregion


        #region 将AllSectionInfo对应的Entity翻译为数据契约 TranslateAllSectionInfoEntityToAllSectionInfoContractData()

        /*将AllSectionInfo对应的Entity翻译为数据契约，调用TranslateSectionInfoEntityToSectionInfoContractData()*/
        private void TranslateAllSectionInfoEntityToAllSectionInfoContractData(
            AllSectionInfoEntity    allSectionInfoEntity,
            AllSectionInfo          allSectionInfo) {

                int cnt = 0;

                allSectionInfo.ErrorMessage     = allSectionInfoEntity.ErrorMessage;
                allSectionInfo.Count            = allSectionInfoEntity.Count;

                if (allSectionInfo.Count > 0) {
                    allSectionInfo.sectionInfo = new SectionInfo[allSectionInfo.Count];
                    for (cnt = 0; cnt < allSectionInfo.Count; cnt++) {
                        allSectionInfo.sectionInfo[cnt] = new SectionInfo();
                        TranslateSectionInfoEntityToSectionInfoContractData(
                            allSectionInfoEntity.sectionInfoEntity[cnt],
                            allSectionInfo.sectionInfo[cnt]);
                    }
                }
        }

        #endregion


        #region 将SectionInfo对应的Entity翻译为数据契约 TranslateSectionInfoEntityToSectionInfoContractData()

        /*将SectionInfo对应的Entity翻译为数据契约*/
        private void TranslateSectionInfoEntityToSectionInfoContractData(
           SectionInfoEntity    sectionInfoEntity,
           SectionInfo          sectionInfo) {

               sectionInfo.ErrorMessage = sectionInfoEntity.ErrorMessage;

               sectionInfo.HospitalID   = sectionInfoEntity.HospitalID;
               sectionInfo.SectionID    = sectionInfoEntity.SectionID;
               sectionInfo.Place        = sectionInfoEntity.Place;
               sectionInfo.Name         = sectionInfoEntity.Name;
               sectionInfo.Phone        = sectionInfoEntity.Phone;
               sectionInfo.Fax          = sectionInfoEntity.Fax;
        }

        #endregion


        #region 将AllDoctorInfo对应的Entity翻译为数据契约 TranslateAllDoctorInfoEntityToAllDoctorInfoContractData()

        /*将AllDoctorInfo对应的Entity翻译为数据契约，调用TranslateDoctorInfoEntityToDoctorInfoContractData()*/
        private void TranslateAllDoctorInfoEntityToAllDoctorInfoContractData(
            AllDoctorInfoEntity     allDoctorInfoEntity,
            AllDoctorInfo           allDoctorInfo) {

                int cnt = 0;

                allDoctorInfo.ErrorMessage  = allDoctorInfoEntity.ErrorMessage;
                allDoctorInfo.Count         = allDoctorInfoEntity.Count;

                if (allDoctorInfo.Count > 0) {
                    allDoctorInfo.doctorInfo = new DoctorInfo[allDoctorInfo.Count];
                    for (cnt = 0; cnt < allDoctorInfo.Count; cnt++) {
                        allDoctorInfo.doctorInfo[cnt] = new DoctorInfo();
                        TranslateDoctorInfoEntityToDoctorInfoContractData(
                            allDoctorInfoEntity.doctorInfoEntity[cnt],
                            allDoctorInfo.doctorInfo[cnt]);
                    }
                }
        }

        #endregion


        #region 将DoctorInfo对应的Entity翻译为数据契约 TranslateDoctorInfoEntityToDoctorInfoContractData()

        /*将DoctorInfo对应的Entity翻译为数据契约*/
        private void TranslateDoctorInfoEntityToDoctorInfoContractData(
            DoctorInfoEntity    doctorInfoEntity,
            DoctorInfo          doctorInfo) {
                doctorInfo.ErrorMessage = doctorInfoEntity.ErrorMessage;

                doctorInfo.SectionID    = doctorInfoEntity.SectionID;
                doctorInfo.DoctorID     = doctorInfoEntity.DoctorID;
                doctorInfo.UserID       = doctorInfoEntity.UserID;
                doctorInfo.LastName     = doctorInfoEntity.LastName;
                doctorInfo.FirstName    = doctorInfoEntity.FirstName;
                doctorInfo.Designation  = doctorInfoEntity.Designation;
                doctorInfo.Resume       = doctorInfoEntity.Resume;
                doctorInfo.Phone        = doctorInfoEntity.Phone;
                doctorInfo.Fax          = doctorInfoEntity.Fax;
                doctorInfo.Email        = doctorInfoEntity.Email;
                doctorInfo.Vocation     = doctorInfoEntity.Vocation;
                doctorInfo.Portrait     = doctorInfoEntity.Portrait;
        }

        #endregion

    }
}
