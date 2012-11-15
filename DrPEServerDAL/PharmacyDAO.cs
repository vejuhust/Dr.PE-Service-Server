using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrPEServer.DrPEServerEntities;
using System.Data.SqlClient;
using System.Configuration;

namespace DrPEServer.DrPEServerDAL {

    public class PharmacyDAO {

        #region 药房登录 Login(string pharmacyID, string password)

        /*药房登录: 向数据库提交select查询，若成功则并更新LastLoginDate域，将结果转写为Entity*/
        public PharmacyInfoEntity Login(string pharmacyID, string password) {

            /*数据库访问实例*/
            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询(pharmacyID,password)对是否匹配*/
            Pharmacy pharmacy = (from p in DEntities.Pharmacies
                                 where ((p.PharmacyID == pharmacyID) && (p.Password == password))
                                 select p).FirstOrDefault();

            /*将结果转写为Entity，仅转写必要登录信息*/
            PharmacyInfoEntity pharmacyInfoEntity = null;
            if (pharmacy != null) {
                pharmacyInfoEntity = new PharmacyInfoEntity() {
                    PharmacyID      = pharmacyID,
                    Name            = pharmacy.Name,
                    LastLoginDate   = pharmacy.LastLoginDate
                };

                /*更新该Pharmacy的LastLoginDate域*/
                pharmacy.LastLoginDate = DateTime.Now;
                DEntities.SaveChanges();
            }

            return pharmacyInfoEntity;
        }

        #endregion


        #region 获取指定时间区间中药房所有的交易记录 GetTransactionHistory(string pharmacyID, DateTime? startDate, DateTime? endDate)

        /*获取指定时间区间中药房所有的交易记录*/
        public AllTransactionInfoEntity GetTransactionHistory(string pharmacyID, DateTime? startDate, DateTime? endDate) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();

            /*查询[startDate, endDate]区间中产生的交易记录*/
            var transactions = from t in DEntities.Transactions
                               where ((startDate <= t.Date) && (t.Date <= endDate) && (t.PharmacyID == pharmacyID))
                               orderby t.Date descending
                               select t;

            int cnt = 0;
            int transactionCount = transactions.Count();

            AllTransactionInfoEntity allTransactionInfoEntity = null;
            if (transactionCount > 0) {
                allTransactionInfoEntity = new AllTransactionInfoEntity();
                allTransactionInfoEntity.Count = transactionCount;
                allTransactionInfoEntity.transactionInfoEntity = new TransactionInfoEntity[transactionCount];

                foreach (var t in transactions) {
                    allTransactionInfoEntity.transactionInfoEntity[cnt] = new TransactionInfoEntity();

                    User user = (from u in DEntities.Users
                                 where u.UserID == t.UserID
                                 select u).FirstOrDefault();

                    allTransactionInfoEntity.transactionInfoEntity[cnt].TransactionID   = t.TransactionID;
                    allTransactionInfoEntity.transactionInfoEntity[cnt].LastName        = user.LastName;
                    allTransactionInfoEntity.transactionInfoEntity[cnt].FirstName       = user.FirstName;
                    allTransactionInfoEntity.transactionInfoEntity[cnt].PharmacyID      = pharmacyID;
                    allTransactionInfoEntity.transactionInfoEntity[cnt].Amount          = t.Amount;
                    allTransactionInfoEntity.transactionInfoEntity[cnt].Date            = t.Date;

                    if (t.Detail == null) {
                        allTransactionInfoEntity.transactionInfoEntity[cnt].Action = "[Pending]";
                    }
                    else {
                        allTransactionInfoEntity.transactionInfoEntity[cnt].Action = "[Taken]";
                    }

                    cnt++;
                }
            }

            return allTransactionInfoEntity;
        }

        #endregion


        #region 查询处方单在特定药房的花费 GetPrescriptionCost(string sPrescriptionID, string pharmacyID, string password)

        /*查询处方单在特定药房的花费，供User和Pharmacy调用。*/
        public PrescriptionCostEntity GetPrescriptionCost(string sPrescriptionID, string pharmacyID, string password) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            PrescriptionCostEntity prescriptionCostEntity = new PrescriptionCostEntity();

            /*获取处方信息*/
            Guid gPrescriptionID = Guid.Empty;
            try {
                gPrescriptionID = new Guid(sPrescriptionID);
            }
            catch {
                prescriptionCostEntity.ErrorMessage = "214 Wrong Prescription GUID! @Data";
                return prescriptionCostEntity;
            }
            Prescription prescription = (from p in DEntities.Prescriptions
                                         where p.PrescriptionID == gPrescriptionID
                                         select p).FirstOrDefault();

            /*若处方不存在*/
            if (prescription == null) {
                prescriptionCostEntity.ErrorMessage = "215 No Such Prescription! @Data";
                return prescriptionCostEntity;
            }

            /*获取所属病历信息*/
            CaseHistory caseHistory = (from c in DEntities.CaseHistories
                                       where c.PrescriptionID == gPrescriptionID
                                       select c).FirstOrDefault();

            /*若处方不属于任何病历*/
            if (caseHistory == null) {
                prescriptionCostEntity.ErrorMessage = "216 No Case Has This Prescription! @Data";
                return prescriptionCostEntity;
            }

            /*获取病历所属用户信息*/
            User user = (from u in DEntities.Users
                         where ((u.UserID == caseHistory.UserID) && (u.Password == password))
                         select u).FirstOrDefault();

            /*若密码错误*/
            if (user == null) {
                prescriptionCostEntity.ErrorMessage = "217 Wrong Password! @Data";
                return prescriptionCostEntity;
            }

            /*填入用户名字信息*/
            prescriptionCostEntity.LastName = user.LastName;
            prescriptionCostEntity.FirstName = user.FirstName;

            /*解析Detail域内容，并计算当前药店处方花费*/
            int cnt = 0;
            int pos = 0;
            Decimal? amount     = 0;
            string sPhysicID    = null;
            string sNumber      = null;
            string[] detail     = prescription.Detail.Split(';');

            prescriptionCostEntity.physicID = new string[detail.Length];
            prescriptionCostEntity.number   = new int[detail.Length];
            prescriptionCostEntity.price    = new Decimal?[detail.Length];

            foreach (string s in detail) {
                /*逐条解析Detail域内容*/
                pos = s.IndexOf(':');
                if (pos < 0) {
                    break;
                }

                sPhysicID = s.Substring(0, pos);
                sNumber = s.Substring(pos + 1);
                prescriptionCostEntity.physicID[cnt] = sPhysicID;
                prescriptionCostEntity.number[cnt] = Convert.ToInt32(sNumber);

                /*在药店数据库中查询该药品的价格*/
                PharmacyDatabase pharmacyDatabase = (from p in DEntities.PharmacyDatabases
                                                     where ((p.PhysicID == sPhysicID) && (p.PharmacyID == pharmacyID))
                                                     select p).FirstOrDefault();

                if (pharmacyDatabase == null) {
                    /*若药店缺少某种药物*/
                    prescriptionCostEntity.ErrorMessage = "218 Medicine " + sPhysicID + " missing! @Data";
                    return prescriptionCostEntity;
                }
                else if (pharmacyDatabase.Price == null) {
                    /*若药店某种药物价格缺失*/
                    prescriptionCostEntity.ErrorMessage = "219 Medicine " + sPhysicID + " unknown! @Data";
                    return prescriptionCostEntity;
                }
                else {
                    /*记录价格，并计算总价*/
                    prescriptionCostEntity.price[cnt] = pharmacyDatabase.Price;
                    amount += (pharmacyDatabase.Price) * (prescriptionCostEntity.number[cnt]);
                }

                cnt++;
            }

            prescriptionCostEntity.Count        = cnt;
            prescriptionCostEntity.UserBalance  = user.Balance;
            prescriptionCostEntity.PharmacyID   = pharmacyID;
            prescriptionCostEntity.Amount       = amount;

            return prescriptionCostEntity;
        }

        #endregion


        #region 为处方单付款 PayPrescription(string sPrescriptionID, string pharmacyID, string payPassword, string take)

        /*为处方单付款*/
        public TransactionInfoEntity PayPrescription(string sPrescriptionID, string pharmacyID, string payPassword, string take) {

            DrPEDatabaseEntities DEntities = new DrPEDatabaseEntities();
            TransactionInfoEntity transactionInfoEntity = new TransactionInfoEntity();

            /*获取处方信息*/
            Guid gPrescriptionID = Guid.Empty;
            try {
                gPrescriptionID = new Guid(sPrescriptionID);
            }
            catch {
                transactionInfoEntity.ErrorMessage = "224 Wrong Prescription GUID! @Data";
                return transactionInfoEntity;
            }
            Prescription prescription = (from p in DEntities.Prescriptions
                                         where p.PrescriptionID == gPrescriptionID
                                         select p).FirstOrDefault();

            /*若处方不存在*/
            if (prescription == null) {
                transactionInfoEntity.ErrorMessage = "225 No Such Prescription! @Data";
                return transactionInfoEntity;
            }

            /*获取所属病历信息*/
            CaseHistory caseHistory = (from c in DEntities.CaseHistories
                                       where c.PrescriptionID == gPrescriptionID
                                       select c).FirstOrDefault();

            /*若处方不属于任何病历*/
            if (caseHistory == null) {
                transactionInfoEntity.ErrorMessage = "226 This Prescription Belongs to No Case! @Data";
                return transactionInfoEntity;
            }

            /*获取病历所属用户信息*/
            User user = (from u in DEntities.Users
                         where ((u.UserID == caseHistory.UserID) && (u.PayPassword == payPassword))
                         select u).FirstOrDefault();

            /*若支付密码错误*/
            if (user == null) {
                transactionInfoEntity.ErrorMessage = "227 Wrong PayPassword! @Data";
                return transactionInfoEntity;
            }

            /*判断是否已付款*/
            string sCheck = String.Format("P:{0}", gPrescriptionID.ToString());
            Transaction transaction = (from t in DEntities.Transactions
                                       where t.Status == sCheck
                                       select t).FirstOrDefault();
            if (transaction != null) {
                /*查询交易药房*/
                Pharmacy pharmacy = (from p in DEntities.Pharmacies
                                     where p.PharmacyID == transaction.PharmacyID
                                     select p).FirstOrDefault();

                /*是否与本药房交易*/
                if (pharmacy.PharmacyID == pharmacyID) {
                    /*药物是否已经领取*/
                    if (transaction.Detail == null) {
                        transactionInfoEntity.Amount = transaction.Amount;
                        transactionInfoEntity.ErrorMessage = String.Format("231 Already Paid at {0}, But Not Taken Yet! @Data", transaction.Date);

                        /*进行领取操作*/
                        transaction.Detail = "[Taken]";
                        DEntities.SaveChanges();
                    }
                    else {
                        transactionInfoEntity.Amount = transaction.Amount;
                        transactionInfoEntity.ErrorMessage = String.Format("232 Already Paid and Taken at {0}! @Data", transaction.Date);
                    }
                }
                else {
                    transactionInfoEntity.ErrorMessage = String.Format("233 Already Bought in {0}({1}) at {2}! @Data",
                                                                        pharmacy.Name, pharmacy.PharmacyID, transaction.Date);
                }
                return transactionInfoEntity;
            }

            /*获取处方费用*/
            PrescriptionCostEntity prescriptionCostEntity = GetPrescriptionCost(sPrescriptionID, pharmacyID, user.Password);
            if (prescriptionCostEntity.ErrorMessage != null) {
                transactionInfoEntity.ErrorMessage = prescriptionCostEntity.ErrorMessage;
                return transactionInfoEntity;
            }

            /*判断用户帐号是否允许支付*/
            if (user.Balance == null) {
                transactionInfoEntity.ErrorMessage = String.Format("228 User's Balance Not Available! @Data");
                return transactionInfoEntity;
            }

            /*判断用户是否有足够的余额支付*/
            if (user.Balance < prescriptionCostEntity.Amount) {
                transactionInfoEntity.ErrorMessage = String.Format("229 Needs RMB{0}, But Only RMB{1} Left! @Data", prescriptionCostEntity.Amount, user.Balance);
                return transactionInfoEntity;
            }

            /*支付密码正确，生成新交易*/
            Transaction newTransaction      = new Transaction();
            newTransaction.TransactionID    = Guid.NewGuid();
            newTransaction.UserID           = user.UserID;
            newTransaction.PharmacyID       = pharmacyID;
            newTransaction.Date             = DateTime.Now;
            newTransaction.Amount           = prescriptionCostEntity.Amount;
            newTransaction.Status           = sCheck;
            if (take != null) {
                newTransaction.Detail = "[Taken]";
            }

            /*提交修改至数据库*/
            try {
                DEntities.Transactions.AddObject(newTransaction);
                DEntities.SaveChanges();
            }
            catch {
                transactionInfoEntity.ErrorMessage = "241 GUID Conflicts! @Data";
                return transactionInfoEntity;
            }

            /*从用户帐号扣款*/
            user.Balance -= prescriptionCostEntity.Amount;
            DEntities.SaveChanges();

            /*将款项加入药房*/
            Pharmacy newPharmacy = (from ph in DEntities.Pharmacies
                                    where ph.PharmacyID == pharmacyID
                                    select ph).FirstOrDefault();
            if (newPharmacy.Balance == null) {
                newPharmacy.Balance = prescriptionCostEntity.Amount;
            }
            else {
                newPharmacy.Balance += prescriptionCostEntity.Amount;
            }
            DEntities.SaveChanges();


            /*返回交易详情*/
            transactionInfoEntity.TransactionID     = newTransaction.TransactionID;
            transactionInfoEntity.LastName          = user.LastName;
            transactionInfoEntity.FirstName         = user.FirstName;
            transactionInfoEntity.PharmacyID        = newTransaction.PharmacyID;
            transactionInfoEntity.Date              = newTransaction.Date;
            transactionInfoEntity.Amount            = newTransaction.Amount;
            transactionInfoEntity.UserBalanceThen   = user.Balance;
            if (newTransaction.Detail != null) {
                transactionInfoEntity.Action = newTransaction.Detail;
            }
            else {
                transactionInfoEntity.Action = "[Left]";
            }

            return transactionInfoEntity;
        }

        #endregion


    }
}
