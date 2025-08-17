using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers;

public class ExportStream
{
    public MemoryStream Stream { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
}
