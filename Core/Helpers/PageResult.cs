using System.Linq;

namespace LamLaiBaiCuoiKhoa.Helpers
{

    public class PageResult<T>
    {
        public Pagination Pagination { get; set; }
        public IQueryable<T> Data { get; set; }   
        public PageResult(Pagination pagination, IQueryable<T> data)
        {
            Pagination = pagination;
            Data = data;
        }
        public static PageResult<T> ToPagedResult(Pagination pagination, IQueryable<T> query)
        {
            pagination.PageNumber = pagination.PageNumber < 1 ? 1 : pagination.PageNumber;

            int totalCount = query.Count(); 

            query = query.Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize).AsQueryable();

            return new PageResult<T>(pagination, query)
            {
                Pagination = { TotalCount = totalCount } 
            };
        }
    }
}
