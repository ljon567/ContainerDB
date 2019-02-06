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
        public string ISO { get; set; }
        public int grade { get; set; }
        public string location { get; set; }
        public string status { get; set; }
        public string time { get; set; }
        public string booking { get; set; }
        public string vessel { get; set; }
        public string loadPort { get; set; }
        public string weight { get; set; }
        public string category { get; set; }
        public string seal { get; set; }
        public string commodity { get; set; }
        public string temperature { get; set; }
        public string hazard { get; set; }
    }
}
