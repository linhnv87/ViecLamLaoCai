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
    public interface IFieldService
    {
        Task<FieldDTO> Create(FieldDTO payload);
        Task<IEnumerable<FieldDTO>> GetAll();
        Task<FieldDTO> GetById(int id);
        Task<FieldDTO> Update(FieldDTO payload);
        Task<int> Delete(int id);
        Task<bool> UpdateOrder(List<UpdateOrderFieldsDTO> orderList);

    }

    public class FieldService : IFieldService
    {
        private readonly IFieldRepository _repository;
        private readonly IMapper _mapper;
        public FieldService(IFieldRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }
        public async Task<FieldDTO> Create(FieldDTO payload)
        {
           
            var data = _mapper.Map<TblField>(payload);
            try
            {
                int maxOrder = _repository.GetAll().Any()
                       ? _repository.GetAll().Max(x => x.Order)
                       : 0;
                data.Order = maxOrder + 1;
                await _repository.AddAsync(data);
                await _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<FieldDTO>(data);
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
                //_repository.Remove(foundItem);
                foundItem.Deleted = true;
                _repository.Update(foundItem);
                await _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return id;
        }

        public async Task<IEnumerable<FieldDTO>> GetAll()
        {
            var data = _repository.GetAll().Where(x => x.Deleted == false).OrderBy(x => x.Order);
            return _mapper.Map<IEnumerable<FieldDTO>>(data);
        }

        public async Task<FieldDTO> GetById(int id)
        {
            var data = await _repository.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                throw new Exception("Item not found");
            }
            return _mapper.Map<FieldDTO>(data);
        }

        public async Task<FieldDTO> Update(FieldDTO payload)
        {
            var data = await _repository.FirstOrDefaultAsync(x => x.Id == payload.Id);
            if (data == null)
            {
                throw new Exception($"{payload.Id} was not found");
            }
            data.Title = payload.Title;
            data.Active = payload.Active;
            await _repository.SaveChanges();
            return _mapper.Map<FieldDTO>(data);
        }
        public async Task<bool> UpdateOrder(List<UpdateOrderFieldsDTO> orderList)
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
