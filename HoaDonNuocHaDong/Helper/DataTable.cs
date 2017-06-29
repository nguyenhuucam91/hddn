using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Helper
{
    
    public class DataTableData
    {
        public int TOTAL_ROWS = 995;
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<DataItem> data { get; set; }
    }

    public class DataItem
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string DoB { get; set; }
    }
}