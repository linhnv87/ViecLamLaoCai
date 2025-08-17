using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class pieData
    {
        public string[] labels { get; set; }
        public List<datasetspie> datasets { get; set; }
    }
    public class datasetspie
    {
        public int[] data { get; set; }
        public string[] backgroundColor { get; set; }

    }
}
