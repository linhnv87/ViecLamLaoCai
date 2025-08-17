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
    public interface ITblDocumentTypesService
    {
        IEnumerable<TblDocumentTypesDTO> GetAllDocumentTypes();
        Task<TblDocumentTypesDTO> Create(TblDocumentTypesDTO payload);
        Task<TblDocumentTypesDTO> GetById(int id);
        Task<TblDocumentTypesDTO> Update(TblDocumentTypesDTO payload);
        Task<int> Delete(int id);
        Task<bool> UpdateOrder(List<UpdateOrderDocumentTypesDTO> orderList);
    }

    public class TblDocumentTypesService : ITblDocumentTypesService
    {
        private readonly IDocumentTypesRepository _repository;
        private readonly IMapper _mapper;

        public TblDocumentTypesService(
            IDocumentTypesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IEnumerable<TblDocumentTypesDTO> GetAllDocumentTypes()
        {
            try
            {
                var data = _repository.GetAll().OrderBy(x => x.Order);

                return _mapper.Map<IEnumerable<TblDocumentTypes>, IEnumerable<TblDocumentTypesDTO>>(data);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TblDocumentTypesDTO> Create(TblDocumentTypesDTO payload)
        {

            var data = _mapper.Map<TblDocumentTypes>(payload);
            try
            {
                int maxOrder = _repository.GetAll().Any()
                       ? _repository.GetAll().Max(x => x.Order)
                       : 0;
                data.Order = maxOrder + 1; 
                await _repository.AddAsync(data);
                await _repository.SaveChanges();

                return _mapper.Map<TblDocumentTypesDTO>(data);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<TblDocumentTypesDTO>(data);
        }

        public async Task<int> Delete(int id)
        {
            var foundItem = _repository.FirstOrDefault(x => x.Id == id);
            if (foundItem == null)
            {
                throw new Exception("Item not found");
            }
            try
            {
                _repository.Remove(foundItem);
                await _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return id;
        }

        public async Task<TblDocumentTypesDTO> GetById(int id)
        {
            var data = await _repository.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                throw new Exception("Item not found");
            }
            return _mapper.Map<TblDocumentTypesDTO>(data);
        }

        public async Task<TblDocumentTypesDTO> Update(TblDocumentTypesDTO payload)
        {
            var data = await _repository.FirstOrDefaultAsync(x => x.Id == payload.Id);
            if (data == null)
            {
                throw new Exception($"{payload.Id} was not found");
            }
            data.Name = payload.Name;
            data.Description = payload.Description;
            await _repository.SaveChanges();
            return _mapper.Map<TblDocumentTypesDTO>(data);
        }

        public async Task<bool> UpdateOrder(List<UpdateOrderDocumentTypesDTO> orderList)
        {
            if (orderList == null || !orderList.Any())
            {
                throw new Exception("Order list is empty or null");
            }
            try
            {
                foreach (var item in orderList)
                {
                    var entity = await _repository.FirstOrDefaultAsync(x => x.Id == item.Id);
                    if (entity != null)
                    {
                        entity.Order = item.Order;
                        _repository.Update(entity);
                    }
                }
              
                await _repository.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
