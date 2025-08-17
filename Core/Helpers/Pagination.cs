using System.Linq;

namespace LamLaiBaiCuoiKhoa.Helpers
{

    public class Pagination
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage
        {
            get
            {
                if (this.PageSize == 0) return 0;
                var Total = this.TotalCount / this.PageSize;
                if (this.TotalCount % this.PageSize > 0) Total += 1;
                return Total;
            }
        }
        public Pagination()
        {
            PageNumber = 1;
            PageSize = -1;
        }
    }
}
