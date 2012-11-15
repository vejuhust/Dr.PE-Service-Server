using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrPEServer.DrPEServerEntities;
using DrPEServer.DrPEServerDAL;

namespace DrPEServer.DrPEServerLogic {

    public class PharmacyLogic {

        /*登录标记*/
        private bool    confirmed           = false;
        private string  confirmedPharmacyID = "";

        /*Data Access Layer实例*/
        PharmacyDAO pharmacyDAO = new PharmacyDAO();

        #region Login(string pharmacyID, string password)

        /*药房登录: 向DAL提交查询，并推测错误代码*/
        public PharmacyInfoEntity Login(string pharmacyID, string password) {

            PharmacyInfoEntity pharmacyInfoEntity = null;

            if ((confirmed == true) && (confirmedPharmacyID == pharmacyID)) {
                pharmacyInfoEntity = new PharmacyInfoEntity();
                pharmacyInfoEntity.ErrorMessage = "203 " + pharmacyID + " Already Logged In! @Logic";
            }
            else {
                pharmacyInfoEntity = pharmacyDAO.Login(pharmacyID, password);

                if (pharmacyInfoEntity == null) {
                    pharmacyInfoEntity = new PharmacyInfoEntity();
                    pharmacyInfoEntity.ErrorMessage = "204 Wrong pharmacyID or Password! @Logic";
                }
                else {
                    confirmed = true;
                    confirmedPharmacyID = pharmacyInfoEntity.PharmacyID;
                }
            }

            return pharmacyInfoEntity;
        }


        #endregion


        #region GetTransactionHistory(DateTime? startDate, DateTime? endDate)

        /*获取指定时间区间中药房所有的交易记录，仅供Pharmacy调用*/
        public AllTransactionInfoEntity GetTransactionHistory(DateTime? startDate, DateTime? endDate) {

            AllTransactionInfoEntity allTransactionInfoEntity = null;

            if (confirmed == false) {
                allTransactionInfoEntity = new AllTransactionInfoEntity();
                allTransactionInfoEntity.ErrorMessage = "253 Not Logged in Yet! @Logic";
            }
            else {
                allTransactionInfoEntity = pharmacyDAO.GetTransactionHistory(confirmedPharmacyID, startDate, endDate);

                if (allTransactionInfoEntity == null) {
                    allTransactionInfoEntity = new AllTransactionInfoEntity();
                    allTransactionInfoEntity.ErrorMessage = String.Format("254 No Transaction Record of {0} Between {1} and {2}! @Logic",
                                                                           confirmedPharmacyID, startDate, endDate);
                }
            }

            return allTransactionInfoEntity;
        }



        #endregion


        #region GetPrescriptionCost(string sPrescriptionID, string password)

        /*查询处方单在特定药房的花费，仅供Pharmacy调用*/
        public PrescriptionCostEntity GetPrescriptionCost(string sPrescriptionID, string password) {
            PrescriptionCostEntity prescriptionCostEntity = null;

            if (confirmed == false) {
                prescriptionCostEntity = new PrescriptionCostEntity();
                prescriptionCostEntity.ErrorMessage = "213 Not Logged in Yet! @Logic";
            }
            else {
                prescriptionCostEntity = pharmacyDAO.GetPrescriptionCost(sPrescriptionID, confirmedPharmacyID, password);
            }

            return prescriptionCostEntity;
        }


        #endregion


        #region PayPrescription(string sPrescriptionID, string payPassword, string take)

        /*为处方单付款，仅供Pharmacy调用*/
        public TransactionInfoEntity PayPrescription(string sPrescriptionID, string payPassword, string take) {
            TransactionInfoEntity transactionInfoEntity = null;

            if (confirmed == false) {
                transactionInfoEntity = new TransactionInfoEntity();
                transactionInfoEntity.ErrorMessage = "223 Not Logged in Yet! @Logic";
            }
            else {
                transactionInfoEntity = pharmacyDAO.PayPrescription(sPrescriptionID, confirmedPharmacyID, payPassword, take);
            }

            return transactionInfoEntity;
        }

        #endregion

    }
}
