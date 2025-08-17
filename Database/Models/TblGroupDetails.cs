using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models;

public partial class TblGroupDetails
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public int GroupId { get; set; }
}
