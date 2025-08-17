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
    public interface IUnitService
    {
        IEnumerable<TblUnit> GetAll();
      
        Task<UnitDTO> Create(UnitDTO payload);
        Task<UnitDTO> GetById(int id);
        Task<UnitDTO> Update(UnitDTO payload);
        Task<int> Delete(int id);
    }

    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UnitService(
            IUnitRepository repository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public IEnumerable<TblUnit> GetAll()
        {
            var result = _repository.GetAll().ToList();
            if (result == null)
            {
                throw new NotImplementedException();
            }
            return result;
        }

        public async Task<UnitDTO> Create(UnitDTO payload)
        {

            var data = _mapper.Map<TblUnit>(payload);
            try
            {
                await _repository.AddAsync(data);
                await _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<UnitDTO>(data);
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
        public async Task<UnitDTO> GetById(int id)
        {
            var data = await _repository.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                throw new Exception("Item not found");
            }
            return _mapper.Map<UnitDTO>(data);
        }

        public async Task<UnitDTO> Update(UnitDTO payload)
        {
            var data = await _repository.FirstOrDefaultAsync(x => x.Id == payload.Id);
            if (data == null)
            {
                throw new Exception($"{payload.Id} was not found");
            }
            data.Name = payload.Name;
            data.IsActive = payload.IsActive;
            await _repository.SaveChanges();
            return _mapper.Map<UnitDTO>(data);
        }
    }
}
