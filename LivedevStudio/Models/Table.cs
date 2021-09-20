using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LivedevStudio.Models
{
    public class Table
    {
        public long TableId { get; set; }
        public string TableCaption { get; set; }

        public Table(long tableid, string tablecaption)
        {
            TableId = tableid;
            TableCaption = tablecaption;
        }
    }
}