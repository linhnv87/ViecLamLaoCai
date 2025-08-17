using AutoMapper;
using Database.Models;
using Repositories;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IRoleSerice
    {
        Task<IEnumerable<AppRole>> GetAllRoles();
        Task<RoleDTO> Create(RoleDTO payload);
        Task<RoleDTO> GetById(string id);
        Task<RoleDTO> Update(RoleDTO payload);
        Task<string> Delete(string id);
    }
    public class RoleService : IRoleSerice
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        public RoleService(IRoleRepository roleRepository, IMapper mapper) 
        { 
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppRole>> GetAllRoles()
        {
            var result = _roleRepository.GetAll().Where(x => x.Deleted == false);
            if (result == null)
            {
                throw new NotImplementedException();
            }
            return result;
        }
        public async Task<RoleDTO> Create(RoleDTO payload)
        {

            var data = _mapper.Map<AppRole>(payload);
            try
            {
                await _roleRepository.AddAsync(data);
                await _roleRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<RoleDTO>(data);
        }

        public async Task<string> Delete(string id)
        {
            var foundItem = _roleRepository.FirstOrDefault(x => x.RoleId.ToString() == id);
            if (foundItem == null)
            {
                throw new Exception("Item not found");
            }
            try
            {
                //_roleRepository.Remove(foundItem);
                foundItem.Deleted = true;
                _roleRepository.Update(foundItem);
                await _roleRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return id;
        }

        public async Task<IEnumerable<RoleDTO>> GetAll()
        {
            var data = _roleRepository.GetAll().Where(x => x.Deleted == false);
            return _mapper.Map<IEnumerable<RoleDTO>>(data);
        }

        public async Task<RoleDTO> GetById(string id)
        {
            var data = await _roleRepository.FirstOrDefaultAsync(x => x.RoleId.ToString() == id);
            if (data == null)
            {
                throw new Exception("Item not found");
            }
            return _mapper.Map<RoleDTO>(data);
        }

        public async Task<RoleDTO> Update(RoleDTO payload)
        {
            var data = await _roleRepository.FirstOrDefaultAsync(x => x.RoleId == payload.RoleId);
            if (data == null)
            {
                throw new Exception($"{payload.RoleId} was not found");
            }
            data.Description = payload.Description;
            data.RoleName = payload.RoleName;
            await _roleRepository.SaveChanges();
            return _mapper.Map<RoleDTO>(data);
        }
    }
}
