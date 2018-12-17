using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContainerDB.Models
{
    public class ContainerUploadItem
    {
        public string ContainerID { get; set; }
        public string shipco { get; set; }
        public string size { get; set; }
        public int grade { get; set; }
        public string location { get; set; }
        public Boolean full { get; set; }
        public string status { get; set; }
        public string time { get; set; }
        public string type { get; set; }
    }
}
