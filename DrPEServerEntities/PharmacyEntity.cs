using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrPEServer.DrPEServerEntities {

    public class PrescriptionCostEntity {
        public string       ErrorMessage { get; set; }
        public int          Count { get; set; }
        public string       LastName { get; set; }
        public string       FirstName { get; set; }
        public Decimal?     UserBalance { get; set; }
        public Decimal?     Amount { get; set; }
        public string       PharmacyID { get; set; }
        public string []    physicID = null;
        public int []       number = null;
        public Decimal? []  price = null;
    }


    public class TransactionInfoEntity {
        public string       ErrorMessage { get; set; }
        public Guid         TransactionID { get; set; }
        public string       LastName { get; set; }
        public string       FirstName { get; set; }
        public string       PharmacyID { get; set; }
        public DateTime?    Date { get; set; }
        public Decimal?     Amount { get; set; }
        public Decimal?     UserBalanceThen { get; set; }
        public string       Action { get; set; }
    }


    public class AllTransactionInfoEntity {
        public string       ErrorMessage { get; set; }
        public int          Count { get; set; }
        public TransactionInfoEntity [] transactionInfoEntity = null;
    }


    public class PharmacyInfoEntity {
        public string       ErrorMessage { get; set; }
        public string       PharmacyID { get; set; }
        public string       Name { get; set; }
        public string       City { get; set; }
        public string       Address { get; set; }
        public double?      Latitude { get; set; }
        public double?      Longitude { get; set; }
        public string       HospitalID { get; set; }
        public string       Phone { get; set; }
        public string       Fax { get; set; }
        public DateTime?    LastLoginDate { get; set; }
    }

    public class AllPharmacyInfoEntity {
        public string ErrorMessage { get; set; }
        public int Count { get; set; }
        public PharmacyInfoEntity[] pharmacyInfoEntity = null;
    }

}
