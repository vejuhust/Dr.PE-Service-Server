using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrPEServer.DrPEServerEntities {

    //public class WareSenseMessageEntity {
    //}

    public class QueueStatusEntity {
        public string       ErrorMessage    { get; set; }
        public int?         Capacity        { get; set; }
        public int?         Process         { get; set; }
        public int?         Mine            { get; set; }
        public DateTime?    When            { get; set; }
    }

    public class UserInfoEntity {
        public string       ErrorMessage { get; set; }
        public string       UserID { get; set; }
        public string       LastName { get; set; }
        public string       FirstName { get; set; }
        public string       Nationality { get; set; }
        public string       Gender { get; set; }
        public string       ABO { get; set; }
        public string       Rh { get; set; }
        public string       Birthplace { get; set; }
        public DateTime?    Birthday { get; set; }
        public string       Deathplace { get; set; }
        public DateTime?    Deathday { get; set; }
        public Decimal?     Balance { get; set; }
        public DateTime?    LastLoginDate { get; set; }
        public string       City { get; set; }
        public string       Address { get; set; }
        public string       Phone { get; set; }
        public string       Email { get; set; }
        public string       EmergencyContactPerson { get; set; }
    }


    public class AvailableDateEntity {
        public string       ErrorMessage { get; set; }
        public int          Capacity { get; set; }
        public int          Count { get; set; }
        public DateTime []  date = null;
        public int []       appointed = null;
    }


    public class SectionScheduleItemEntity {
        public string   DoctorID { get; set; }
        public DateTime Date { get; set; }
        public int      Appointed { get; set; }
        public int      Capacity { get; set; }
    }


    public class SectionScheduleEntity {
        public string       ErrorMessage { get; set; }
        public int          Count { get; set; }
        public SectionScheduleItemEntity [] Detail = null;
    }


    public class AppointmentEntity {
        public Guid         gGuid { get; set; }
        public string       DoctorID { get; set; }
        public string       UserID { get; set; }
        public DateTime?    Date { get; set; }
        public int?         Rank { get; set; }
        public bool         Finished { get; set; }
    }


    public class AllAppointmentEntity {
        public string       ErrorMessage { get; set; }
        public int          Count { get; set; }
        public AppointmentEntity [] appointment = null;
    }


    public class NoticeEntity {
        public string       ErrorMessage { get; set; }
        public int          Count { get; set; }
        public string []    notice = null;
    }

}
