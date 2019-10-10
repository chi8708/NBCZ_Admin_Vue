using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBCZ.Web.Api
{
    public class PageDataReq
    {
      //  int pageNum = 1, int pageSize = 10, string field = "id", string order = " desc "

        public PageDataReq() 
        {
            pageNum = 1;
            pageSize = 10;
        }
        public int pageNum { get; set; } 
        public int pageSize { get; set; }

        public string field { get; set; }
        public string order { get; set; }

        public Dictionary<string, object> query{ get; set; }
}
}
