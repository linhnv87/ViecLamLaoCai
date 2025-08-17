using Database.Models;
using Microsoft.Data.SqlClient;
using Repositories.BaseRepository;
using Repositories.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories;

public interface ITblGroupDetailRepository : IBaseRepository<TblGroupDetails>
{
}

public class TblGroupDetailRepository : BaseRepository<TblGroupDetails>, ITblGroupDetailRepository
{
    private readonly QLTTrContext _context;

    public TblGroupDetailRepository(QLTTrContext context) : base(context)
    {
        _context = context;
    }

}