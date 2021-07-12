using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Data
    {
        public String Response { get; set;}

        public Boolean IsAscending { get; set; }

        public String SortedResponse { get; set; }
        
        public String SortList { get; set; }

        public int TimeTaken { get; set; }
    }
}
