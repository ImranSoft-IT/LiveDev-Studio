using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LivedevStudio.Models
{
    public class Column
    {
        public long ColumnId { get; set; }
        public string ColumnDisplayName { get; set; }
        public int LinkTableId { get; set; }
       

        public Column(long columnId, string columnDisplayName)
        {
            ColumnId = columnId;
            ColumnDisplayName = columnDisplayName;
        }

        public Column(int linktabId)
        {
            LinkTableId = linktabId;
        }
    }

    public class ColumnSlno
    {
        public ColumnSlno(long columnId, decimal slNo, string colName, int colSize, int reportSlno)
        {
            ColumnId = columnId;
            SlNo = slNo;
            ColName = colName;
            ColSize = colSize;
            ReportSlno = reportSlno;
        }

        public long ColumnId { get; set; }
        public decimal SlNo { get; set; }
        public string ColName { get; set; }
        public int ColSize { get; set; }
        public int ReportSlno { get; set; }

    }

    public class PanelColSlno
    {
        
        public PanelColSlno(long columnId, decimal pnlColsln, int pnlColMerge)
        {
            ColumnId = columnId;
            PnlColsln = pnlColsln;
            PnlColMerge = pnlColMerge;
        }

        public long ColumnId { get; set; }
        public decimal PnlColsln { get; set; }
        public int PnlColMerge { get; set; }
    }
}