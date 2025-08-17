using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class ReviewOrderUserDetailVModelDTO
    {
        public required string UserId { get; set; }
        public bool IsDefault { get; set; }
    }

    public class ReviewOrderUserDetailByIdDTO : ReviewOrderUserDetailVModelDTO
    {
        public int Id { get; set; }
    }
}
