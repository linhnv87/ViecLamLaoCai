using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domains
{
    public class PaginationSetModel<T>
    {
        public int Page { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
        public IEnumerable<T> Items { get; set; }
        public int Count
        {
            get { return Items != null ? Items.Count() : 0; }
        }
        public PaginationSetModel(int? PageNumber, int? PageSize, IEnumerable<T> data)
        {
            this.TotalCount = data.Count();
            if(PageSize == null)
            {
                this.Page = 1;
                this.Items = data;
                this.TotalPage = 1;
            }
            else
            {
                if (PageNumber == null) this.Page = 1;
                else this.Page = (int)PageNumber;
                this.Items = data.Skip((int)PageSize * (this.Page - 1)).Take((int)PageSize).ToList();
                //this.TotalPage = data.Count() / (int)Page + data.Count() % (int)Page == 0 ? 0 : 1;
                this.TotalPage = (int)Math.Ceiling((double)data.Count() / (int)PageSize);

            }
        }
    }
}
