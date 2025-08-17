using Database.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IDocumentTypesRepository : IBaseRepository<TblDocumentTypes>
    {
    }

    public class DocumentTypesRepository : BaseRepository<TblDocumentTypes>, IDocumentTypesRepository
    {
        public DocumentTypesRepository(QLTTrContext context) : base(context)
        {
        }
    }
}
