using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrPEServer.DrPEServerEntities {

    public class PrescriptionInfoEntity {
        public string       ErrorMessage { get; set; }
        public Guid?        PrescriptionID { get; set; }
        public int          Count { get; set; }
        public string []    physicID = null;
        public int []       number = null;
    }


    public class ExaminationInfoEntity {
        public string       ErrorMessage { get; set; }
        public Guid?        ExaminationID { get; set; }
        public DateTime?    Date { get; set; }
        public string       Type { get; set; }
        public string       Text { get; set; }
        public string       Advice { get; set; }
        public byte []      Image = null;
    }


    public class CaseInfoEntity {
        public string       ErrorMessage { get; set; }
        public Guid?        CaseID { get; set; }
        public Guid?        ExaminationID { get; set; }
        public Guid?        PrescriptionID { get; set; }
        public string       UserID { get; set; }
        public string       DoctorID { get; set; }
        public string       SectionID { get; set; }
        public DateTime?    Date { get; set; }
        public string       ChiefComplaint { get; set; }
        public string       TentativeDiagnosis { get; set; }
        public string       DifferentialDiagnosis { get; set; }
        public string       TreatmentPlan { get; set; }
        public DateTime?    CountercheckDate { get; set; }
        public bool         IsDraft { get; set; }
    }


    public class CaseHistoryEntity {
        public string   ErrorMessage { get; set; }
        public int      Count { get; set; }
        public CaseInfoEntity [] caseInfoEntity = null;
    }


    public class MessageEntity {
        public string       ErrorMessage { get; set; }
        public Guid?        MessageID { get; set; }
        public DateTime?    Date { get; set; }
        public string       Sender { get; set; }
        public string       Receiver { get; set; }
        //public string       Subject { get; set; }
        public string       Status { get; set; }
        public string       Context { get; set; }
    }


    public class AllMessageEntity {
        public string       ErrorMessage { get; set; }
        public int          Count { get; set; }
        public MessageEntity [] messageEntity = null;
    }



}
