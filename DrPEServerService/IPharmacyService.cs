using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DrPEServer.DrPEServerService {

    [ServiceContract]
    public interface IPharmacyService {

        [OperationContract]
        PharmacyInfo Login(string pharmacyID, string password);

        [OperationContract]
        PrescriptionCost GetPrescriptionCost(string sPrescriptionID, string password);

        [OperationContract]
        TransactionInfo PayPrescription(string sPrescriptionID, string payPassword);

        [OperationContract]
        AllTransactionInfo GetTransactionHistory(DateTime? startDate, DateTime? endDate);
    }


    [DataContract]
    public class PrescriptionCost {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public string LastName { get; set; }

        [DataMember (Order = 2)]
        public string FirstName { get; set; }

        [DataMember (Order = 3)]
        public string PharmacyID { get; set; }

        [DataMember (Order = 4)]
        public Decimal? Amount { get; set; }

        [DataMember (Order = 5)]
        public Decimal? UserBalance { get; set; }

        [DataMember (Order = 6)]
        public int Count { get; set; }

        [DataMember (Order = 7)]
        public string [] physicID = null;

        [DataMember (Order = 8)]
        public int [] number = null;

        [DataMember (Order = 9)]
        public Decimal? [] price = null;
    }


    [DataContract]
    public class TransactionInfo {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public string TransactionID { get; set; }

        [DataMember (Order = 2)]
        public string LastName { get; set; }

        [DataMember (Order = 3)]
        public string FirstName { get; set; }

        [DataMember (Order = 4)]
        public string PharmacyID { get; set; }

        [DataMember (Order = 5)]
        public DateTime? Date { get; set; }

        [DataMember (Order = 6)]
        public Decimal? Amount { get; set; }

        [DataMember (Order = 7)]
        public Decimal? UserBalanceThen { get; set; }

        [DataMember (Order = 8)]
        public string Action { get; set; }
    }


    [DataContract]
    public class AllTransactionInfo {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public int Count { get; set; }

        [DataMember (Order = 2)]
        public TransactionInfo [] transactionInfo = null;
    }


    [DataContract]
    public class PharmacyInfo {

        [DataMember (Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember (Order = 1)]
        public string PharmacyID { get; set; }

        [DataMember (Order = 2)]
        public string Name { get; set; }

        [DataMember (Order = 3)]
        public string City { get; set; }

        [DataMember (Order = 4)]
        public string Address { get; set; }

        [DataMember (Order = 5)]
        public double? Latitude { get; set; }

        [DataMember (Order = 6)]
        public double? Longitude { get; set; }

        [DataMember (Order = 7)]
        public string HospitalID { get; set; }

        [DataMember (Order = 8)]
        public string Phone { get; set; }

        [DataMember (Order = 9)]
        public string Fax { get; set; }

        [DataMember (Order = 10)]
        public DateTime? LastLoginDate { get; set; }
    }


    [DataContract]
    public class AllPharmacyInfo {

        [DataMember(Order = 0)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 1)]
        public int Count { get; set; }

        [DataMember(Order = 2)]
        public PharmacyInfo[] pharmacyInfo = null;
    }

}
