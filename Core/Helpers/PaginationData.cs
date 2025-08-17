using LamLaiBaiCuoiKhoa.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers;

public class PaginationV2
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public int TotalCount { get; set; }
    public int TotalPage => (int)Math.Ceiling((double)TotalCount / PageSize);

    public PaginationV2(int pageNumber, int pageSize, int totalCount)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}

public class PaginationData<T>
{
    public PaginationV2 Pagination { get; set; }
    public IEnumerable<T> Data { get; set; }
    public PaginationData(PaginationV2 pagination, IEnumerable<T> data)
    {
        Pagination = pagination;
        Data = data;
    }

    public static PaginationData<T> Create(List<T>? source, int pageNumber, int pageSize, int totalCount)
    {
        var pagination = new PaginationV2(pageNumber, pageSize, totalCount);
        var data = source != null ? source.ToList() : new List<T>();

        return new PaginationData<T>(pagination, data);
    }
}
