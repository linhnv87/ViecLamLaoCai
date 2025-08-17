using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class ChartRequests
    {
        public string[] labels { get; set; }
        public DatasetsChart datasets { get; set; }
    }
    public class DatasetsChart
    {
        public string label { get; set; }
        public bool fill { get; set; }
        public int borderWidth { get; set; }
        public int lineTension { get; set; }
        public bool spanGaps { get; set; }
        public string borderColor { get; set; }
        public int pointRadius { get; set; }
        public int pointHoverRadius { get; set; }
        public string pointColor { get; set; }
        public string pointBackgroundColor { get; set; }
        public int[] data { get; set; }
    }
    public class ChartDataByField
    {
        public string title { get; set; }
        public int data { get; set; }
    }
}