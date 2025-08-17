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
    public interface IDepartmentService
    {
        IEnumerable<TblDeparments> GetAllDeparments();
        Task<IEnumerable<UserInfoDTO>> GetAllInfoUserByDepartmentId(int departmentId);
        Task<DeparmentsDTO> Create(DeparmentsDTO payload);
        Task<DeparmentsDTO> GetById(int id);
        Task<DeparmentsDTO> Update(DeparmentsDTO payload);
        Task<int> Delete(int id);
    }

    public class DepartmentService : IDepartmentService
    {
        private readonly ITblDepartmentRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public DepartmentService(
            ITblDepartmentRepository repository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public IEnumerable<TblDeparments> GetAllDeparments()
        {
            var result = _repository.GetAll().ToList();
            if (result == null)
            {
                throw new NotImplementedException();
            }
            return result;
        }

        public async Task<IEnumerable<UserInfoDTO>> GetAllInfoUserByDepartmentId(int departmentId)
        {
            var (infoUser, userInRoles) = await _repository.GetUsersByDepartmentIdAsync(departmentId);

            if (infoUser == null)
            {
                throw new NotImplementedException();
            }

            var result = _mapper.Map<IEnumerable<UserInfoDTO>>(infoUser);

            foreach (var item in result)
            {
                var userInRole = userInRoles.Where(x => x.UserId == item.UserId);
                if (userInRole != null)
                {
                    item.Roles = userInRole.Select(r => new RoleDTO { RoleId = r.RoleId, RoleName = r.RoleName, Description = r.Description }).ToList();
                }
            }

            return result;
        }
        public async Task<DeparmentsDTO> Create(DeparmentsDTO payload)
        {

            var data = _mapper.Map<TblDeparments>(payload);
            try
            {
                await _repository.AddAsync(data);
                await _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<DeparmentsDTO>(data);
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
        public async Task<DeparmentsDTO> GetById(int id)
        {
            var data = await _repository.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                throw new Exception("Item not found");
            }
            return _mapper.Map<DeparmentsDTO>(data);
        }

        public async Task<DeparmentsDTO> Update(DeparmentsDTO payload)
        {
            var data = await _repository.FirstOrDefaultAsync(x => x.Id == payload.Id);
            if (data == null)
            {
                throw new Exception($"{payload.Id} was not found");
            }
            data.DepartmentName = payload.DepartmentName;
            data.IsActive = payload.IsActive;
            await _repository.SaveChanges();
            return _mapper.Map<DeparmentsDTO>(data);
        }
    }
}
