using Database.Models;
using Database;
using Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{

    public interface ITblDocumentApprovalRepository : IBaseRepository<TblDocumentApproval>
    {

    }

    public class TblDocumentApprovalRepository : BaseRepository<TblDocumentApproval>, ITblDocumentApprovalRepository
    {
        public TblDocumentApprovalRepository(QLTTrContext context) : base(context)
        {

        }
    }
}
