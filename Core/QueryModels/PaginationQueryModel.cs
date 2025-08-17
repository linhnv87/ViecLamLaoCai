using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.QueryModels
{
    public class PaginationQueryModel
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int? SortOrder { get; set; }//1: asc, 0: desc
    }

    public class StudentQueryModel:PaginationQueryModel
    {
        public int? OrganizationId { get; set; }
        public string? FullName { get; set; }
        public DateTime? DoBFrom { get; set; }
        public DateTime? DoBTo { get; set; }
        public DateTime? RegisterFrom { get; set; }
        public DateTime? RegisterTo { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }        
    }

    public class SubjectQueryModel : PaginationQueryModel
    {
        public int? OrganizationId { get; set; }
        public int? SubjectGroupId { get; set;}

    }

    public class SubjectGroupQueryModel : PaginationQueryModel
    {
        public int? OrganizationId { get; set; }
    }


    public class EmployeeQueryModel : PaginationQueryModel
    {
        public int? OrganizationId { get; set; }
        public string? FullName { get; set; }
        public DateTime? DoBFrom { get; set; }
        public DateTime? DoBTo { get; set; }
        public DateTime? RegisterFrom { get; set; }
        public DateTime? RegisterTo { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
    }
}
