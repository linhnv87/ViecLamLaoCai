using AutoMapper;
using Database.Models;
using Repositories;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICategoryStatusesService
    {
        IEnumerable<CategoryStatusesDTO> GetAllStatuses();
        Task<bool> ValidateStatusAsync(string fromStatusCode, string toStatusCode);
    }

    public class CategoryStatusesService : ICategoryStatusesService
    {
        private readonly IStatusesRepository _repository;
        private readonly IMapper _mapper;

        public CategoryStatusesService(
            IStatusesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IEnumerable<CategoryStatusesDTO> GetAllStatuses()
        {
            try
            {
                var data = _repository.GetAll();

                return _mapper.Map<IEnumerable<TblStatuses>, IEnumerable<CategoryStatusesDTO>>(data);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ValidateStatusAsync(string fromStatusCode, string toStatusCode)
        {
            try
            {
                var fromStatus = await _repository.GetSingleByCondition(x => x.StatusCode == fromStatusCode);
                var toStatus = await _repository.GetSingleByCondition(x => x.StatusCode == toStatusCode);

                return fromStatus != null && toStatus != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
