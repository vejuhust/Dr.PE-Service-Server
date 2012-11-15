using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DrPEServer.DrPEServerEntities;
using DrPEServer.DrPEServerLogic;

namespace DrPEServer.DrPEServerService {

    public class PharmacyService : IPharmacyService {

        /*Logic Layer实例*/
        PharmacyLogic pharmacyLogic = new PharmacyLogic();


        /*药房登录：若ID和Password均不为空，则转发至DL，将结果翻译为数据契约*/
        public PharmacyInfo Login(string pharmacyID, string password) {
            PharmacyInfoEntity pharmacyInfoEntity = null;

            if (pharmacyID == null) {
                pharmacyInfoEntity = new PharmacyInfoEntity();
                pharmacyInfoEntity.ErrorMessage = "201 Empty PharmacyID! @Service";
            }
            else if (password == null) {
                pharmacyInfoEntity = new PharmacyInfoEntity();
                pharmacyInfoEntity.ErrorMessage = "202 Empty Password! @Service";
            }
            else {
                pharmacyInfoEntity = pharmacyLogic.Login(pharmacyID, password);
            }

            PharmacyInfo pharmacyInfo = new PharmacyInfo();
            TranslatePharmacyInfoEntityToPharmacyInfoContractData(pharmacyInfoEntity, pharmacyInfo);

            return pharmacyInfo;
        }


        /*获取指定时间区间中药房所有的交易记录*/
        public AllTransactionInfo GetTransactionHistory(DateTime? startDate, DateTime? endDate) {

            AllTransactionInfoEntity allTransactionInfoEntity = null;

            if (startDate == null) {
                allTransactionInfoEntity = new AllTransactionInfoEntity();
                allTransactionInfoEntity.ErrorMessage = "251 Empty StartDate! @Service";
            }
            else if (endDate == null) {
                allTransactionInfoEntity = new AllTransactionInfoEntity();
                allTransactionInfoEntity.ErrorMessage = "252 Empty EndDate! @Service";
            }
            else {
                allTransactionInfoEntity = pharmacyLogic.GetTransactionHistory(startDate, endDate);
            }

            AllTransactionInfo allTransactionInfo = new AllTransactionInfo();
            TranslateAllTransactionInfoEntityToAllTransactionInfoContractData(allTransactionInfoEntity, allTransactionInfo);

            return allTransactionInfo;
        }


        /*查询处方单在特定药房的花费*/
        public PrescriptionCost GetPrescriptionCost(string sPrescriptionID, string password) {

            PrescriptionCostEntity prescriptionCostEntity = null;

            if (sPrescriptionID == null) {
                prescriptionCostEntity = new PrescriptionCostEntity();
                prescriptionCostEntity.ErrorMessage = "211 Empty PrescriptionID! @Service";
            }
            else if (password == null) {
                prescriptionCostEntity = new PrescriptionCostEntity();
                prescriptionCostEntity.ErrorMessage = "212 Empty User Password! @Service";
            }
            else {
                prescriptionCostEntity = pharmacyLogic.GetPrescriptionCost(sPrescriptionID, password);
            }

            PrescriptionCost prescriptionCost = new PrescriptionCost();
            TranslatePrescriptionCostEntityToPrescriptionCostContractData(prescriptionCostEntity, prescriptionCost);

            return prescriptionCost;
        }


        /*为处方单付款*/
        public TransactionInfo PayPrescription(string sPrescriptionID, string payPassword) {

            TransactionInfoEntity transactionInfoEntity = null;

            if (sPrescriptionID == null) {
                transactionInfoEntity = new TransactionInfoEntity();
                transactionInfoEntity.ErrorMessage = "221 Empty PrescriptionID! @Service";
            }
            else if (payPassword == null) {
                transactionInfoEntity = new TransactionInfoEntity();
                transactionInfoEntity.ErrorMessage = "222 Empty User PayPassword! @Service";
            }
            else {
                transactionInfoEntity = pharmacyLogic.PayPrescription(sPrescriptionID, payPassword, "Yes We Can!");
            }

            TransactionInfo transactionInfo = new TransactionInfo();
            TranslateTransactionInfoEntityToTransactionInfoContractData(transactionInfoEntity, transactionInfo);

            return transactionInfo;
        }


        /*将PrescriptionCost对应的Entity翻译为数据契约*/
        private void TranslatePrescriptionCostEntityToPrescriptionCostContractData(
            PrescriptionCostEntity  prescriptionCostEntity,
            PrescriptionCost        prescriptionCost) {
                prescriptionCost.ErrorMessage   = prescriptionCostEntity.ErrorMessage;
                prescriptionCost.Count          = prescriptionCostEntity.Count;
                prescriptionCost.LastName       = prescriptionCostEntity.LastName;
                prescriptionCost.FirstName      = prescriptionCostEntity.FirstName;
                prescriptionCost.UserBalance    = prescriptionCostEntity.UserBalance;
                prescriptionCost.Amount           = prescriptionCostEntity.Amount;
                prescriptionCost.PharmacyID     = prescriptionCostEntity.PharmacyID;
                prescriptionCost.physicID       = new string[prescriptionCost.Count];
                prescriptionCost.number         = new int[prescriptionCost.Count];
                prescriptionCost.price          = new Decimal?[prescriptionCost.Count];
                for (int i = 0; i < prescriptionCost.Count; i++) {
                    prescriptionCost.physicID[i]    = prescriptionCostEntity.physicID[i];
                    prescriptionCost.number[i]      = prescriptionCostEntity.number[i];
                    prescriptionCost.price[i]       = prescriptionCostEntity.price[i];
                }
        }


        /*将AllTransactionInfo对应的Entity翻译为数据契约，调用 TranslateTransactionInfoEntityToTransactionInfoContractData()方法*/
        private void TranslateAllTransactionInfoEntityToAllTransactionInfoContractData(
            AllTransactionInfoEntity    allTransactionInfoEntity, 
            AllTransactionInfo          allTransactionInfo) {

                allTransactionInfo.ErrorMessage     = allTransactionInfoEntity.ErrorMessage;
                allTransactionInfo.Count            = allTransactionInfoEntity.Count;
                allTransactionInfo.transactionInfo  = new TransactionInfo[allTransactionInfo.Count];    

                for (int i = 0; i < allTransactionInfo.Count; i++) {
                    allTransactionInfo.transactionInfo[i] = new TransactionInfo();
                    TranslateTransactionInfoEntityToTransactionInfoContractData(allTransactionInfoEntity.transactionInfoEntity[i],
                                                                                allTransactionInfo.transactionInfo[i]);
                }
        }


        /*将TransactionInfo对应的Entity翻译为数据契约*/
        private void TranslateTransactionInfoEntityToTransactionInfoContractData(
            TransactionInfoEntity   transactionInfoEntity,
            TransactionInfo         transactionInfo) {
                transactionInfo.ErrorMessage    = transactionInfoEntity.ErrorMessage;
                transactionInfo.TransactionID   = transactionInfoEntity.TransactionID.ToString();
                transactionInfo.LastName        = transactionInfoEntity.LastName;
                transactionInfo.FirstName       = transactionInfoEntity.FirstName;
                transactionInfo.PharmacyID      = transactionInfoEntity.PharmacyID;
                transactionInfo.Date            = transactionInfoEntity.Date;
                transactionInfo.Amount          = transactionInfoEntity.Amount;
                transactionInfo.UserBalanceThen = transactionInfoEntity.UserBalanceThen;
                transactionInfo.Action          = transactionInfoEntity.Action;
        }


        /*将PharmacyInfo对应的Entity翻译为数据契约*/
        private void TranslatePharmacyInfoEntityToPharmacyInfoContractData(
            PharmacyInfoEntity pharmacyInfoEntity,
            PharmacyInfo pharmacyInfo) {
                pharmacyInfo.ErrorMessage   = pharmacyInfoEntity.ErrorMessage;
                pharmacyInfo.PharmacyID     = pharmacyInfoEntity.PharmacyID;
                pharmacyInfo.Name           = pharmacyInfoEntity.Name;
                pharmacyInfo.City           = pharmacyInfoEntity.City;
                pharmacyInfo.Address        = pharmacyInfoEntity.Address;
                pharmacyInfo.Latitude       = pharmacyInfoEntity.Latitude;
                pharmacyInfo.Longitude      = pharmacyInfoEntity.Longitude;
                pharmacyInfo.HospitalID     = pharmacyInfoEntity.HospitalID;
                pharmacyInfo.Phone          = pharmacyInfoEntity.Phone;
                pharmacyInfo.Fax            = pharmacyInfoEntity.Fax;
                pharmacyInfo.LastLoginDate  = pharmacyInfoEntity.LastLoginDate;
        }

    }
}
