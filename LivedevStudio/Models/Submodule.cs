using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LivedevStudio.Models
{
    public class Submodule
    {
        public long CategoryId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public string ModuleCategory { get; set; }
        public bool IsSystem { get; set; }
        public Nullable<int> Sl { get; set; }
        public string Code { get; set; }
        public string DeveloperSyncId { get; set; }
        public Nullable<long> Entry_User_Id { get; set; }
        public string Entry_By { get; set; }
        public Nullable<System.DateTime> Entry_Date { get; set; }
        public string Update_By { get; set; }
        public Nullable<System.DateTime> Update_Date { get; set; }
        public string Check_By { get; set; }
        public Nullable<System.DateTime> Check_Date { get; set; }
        public string Approve_By { get; set; }
        public Nullable<System.DateTime> Approve_Date { get; set; }
        public string Delete_By { get; set; }
        public Nullable<System.DateTime> Delete_Date { get; set; }
        public bool Is_Approved { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }
        public int Work_Follow_Position { get; set; }
        public System.Guid Row_Id { get; set; }
        public string Cancel_By { get; set; }
        public Nullable<System.DateTime> Cancel_Date { get; set; }
        public bool Is_Cancel { get; set; }
    }
}