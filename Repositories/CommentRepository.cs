using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.BaseRepository;

namespace Repositories
{
    public interface ICommentRepository : IBaseRepository<TblComment>
    {

    }
    public class CommentRepository : BaseRepository<TblComment>, ICommentRepository
    {
        public CommentRepository(QLTTrContext context) : base(context)
        {
        }
    }
}
