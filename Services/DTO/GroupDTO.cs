using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO;

public class GroupDTO
{
    public string GroupName { get; set; }
    public bool IsActive { get; set; }
    public bool IsSMS { get; set; }
    public IEnumerable<GroupDetailDTO> GroupDetails { get; set; }
}

public class GroupDetailDTO
{
    public Guid UserId { get; set; }
}