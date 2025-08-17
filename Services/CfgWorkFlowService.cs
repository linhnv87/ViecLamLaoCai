using AutoMapper;
using Database.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repositories;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICfgWorkFlowService
    {
        Task<IEnumerable<ReviewOrderByIdDTO>> GetAllAsync();
        Task<ReviewOrderByIdDTO?> GetWorkFlowByUser(WorkFlowRequestDTO request);
        Task<ReviewOrderByIdDTO?> CreateWorkflowAsync(ReviewOrderByIdDTO request);
        Task<IEnumerable<UserInfoDTO>> GetUsersByWorkflowId(int workflowId);
        Task<int> DeleteWorkflowAndUsersAsync(int workflowId);
        Task<int> UpdateWorkflowAndUserAsync(int workflowId, UpdateWorkflowRequestDTO model);
    }

    public class CfgWorkFlowService : ICfgWorkFlowService
    {
        private readonly ICfgWorkFlowRepository _repository;
        private readonly ICfgWorkFlowUser _cfgWorkFlowUser;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserInRoleRepository _userInRoleRepository;
        private readonly IRoleRepository _roleRepository;

        public CfgWorkFlowService(
            ICfgWorkFlowRepository repository,
            ICfgWorkFlowUser cfgWorkFlowUser, 
            IMapper mapper,
            IUserRepository userRepository,
            IUserInRoleRepository userInRoleRepository,
            IRoleRepository roleRepository)
        {
            _repository = repository;
            _cfgWorkFlowUser = cfgWorkFlowUser;
            _mapper = mapper;
            _userRepository = userRepository;
            _userInRoleRepository = userInRoleRepository;
            _roleRepository = roleRepository;
        }

        public async Task<ReviewOrderByIdDTO?> CreateWorkflowAsync(ReviewOrderByIdDTO request)
        {
            var existingWorkflow = await _repository.AnyAsync(w => w.State == request.State && w.StatusId == request.StatusId && w.UserId == request.UserId.ToString());
            if (existingWorkflow)
            {
                return null;
            }
            var newWorkflow = _mapper.Map<ReviewOrderByIdDTO, CfgWorkFlow>(request);
            await _repository.AddAsync(newWorkflow);
            var newWorkflowEntity = await _repository.SaveChangesAsync(newWorkflow);

            var users = _mapper.Map<IEnumerable<CfgUsersWorkFlowVModelDTO>, IEnumerable<CfgWorkFlowUser>>(request.UsersDto);
            foreach (var user in users)
            {
                user.WorkflowId = newWorkflow.Id;
            }
            await _cfgWorkFlowUser.AddRangeAsync(users);
            await _cfgWorkFlowUser.SaveChanges();

            return _mapper.Map<ReviewOrderByIdDTO>(newWorkflowEntity);
        }

        public async Task<IEnumerable<ReviewOrderByIdDTO>> GetAllAsync()
        {
            var data = _repository.GetAll().AsEnumerable();
            var dataDto = _mapper.Map<IEnumerable<CfgWorkFlow>, IEnumerable<ReviewOrderByIdDTO>>(data);
            return dataDto;
        }
        public async Task<IEnumerable<UserInfoDTO>> GetUsersByWorkflowId(int workflowId)
        {
            var users = await (from wfUser in _cfgWorkFlowUser.GetAll()
                               join user in _userRepository.GetAll()
                                   on wfUser.UserId equals user.UserId.ToString() into userGroup
                               from user in userGroup.DefaultIfEmpty()
                               join userRole in _userInRoleRepository.GetAll()
                                   on user.UserId equals userRole.UserId into userRoleGroup
                               from userRole in userRoleGroup.DefaultIfEmpty()
                               join role in _roleRepository.GetAll()
                                   on userRole.RoleId equals role.RoleId into roleGroup
                               from role in roleGroup.DefaultIfEmpty()
                               where wfUser.WorkflowId == workflowId
                               group role by new
                               {
                                   user.UserId,
                                   user.UserName,
                                   user.UserFullName,
                                   user.Email,
                                   user.PhoneNumber,
                                   user.CreateDate,
                                   user.LastLoginDate,
                                   user.IsApproved,
                                   user.IsLockedout,
                                   user.DepartmentId
                               } into userRolesGroup
                               select new UserInfoDTO
                               {
                                   UserId = userRolesGroup.Key.UserId,
                                   UserName = userRolesGroup.Key.UserName,
                                   UserFullName = userRolesGroup.Key.UserFullName,
                                   Email = userRolesGroup.Key.Email,
                                   PhoneNumber = userRolesGroup.Key.PhoneNumber,
                                   CreateDate = userRolesGroup.Key.CreateDate,
                                   LastloginDate = userRolesGroup.Key.LastLoginDate,
                                   IsApproved = userRolesGroup.Key.IsApproved,
                                   IsLockedout = userRolesGroup.Key.IsLockedout,
                                   DepartmentId = userRolesGroup.Key.DepartmentId,
                                   Roles = userRolesGroup
                                           .Where(role => role != null)
                                           .Select(role => new RoleDTO
                                           {
                                               RoleId = role.RoleId,
                                               RoleName = role.RoleName,
                                               Description = role.Description,
                                             
                                               Deleted = role.Deleted
                                           }).ToList(),
                               }).ToListAsync();
            return users;
        }
        public async Task<ReviewOrderByIdDTO?> GetWorkFlowByUser(WorkFlowRequestDTO request)
        {
            var (wordFlows, users, roles) = await _repository.GetWrokFlowByUserAsync(request.UserId, request.StatusId, request.State);
            var worfFlowDto = _mapper.Map<ReviewOrderByIdDTO>(wordFlows.FirstOrDefault());

            if (worfFlowDto != null)
            {
                var userDtos = _mapper.Map<List<CfgUsersWorkFlowVModelDTO>>(users);

                foreach (var userDto in userDtos)
                {
                    var userRoles = roles.Where(x => x.UserId == userDto.UserId);
                    userDto.Roles = _mapper.Map<List<CfgRolesModelDTO>>(userRoles);
                }

                worfFlowDto.UsersDto = userDtos;
            }

            return worfFlowDto;
        }
        public async Task<int> UpdateWorkflowAndUserAsync(int workflowId, UpdateWorkflowRequestDTO model)
        {
            var currentWorkflow = await _repository.GetSingleByCondition(wf => wf.Id == workflowId);
            if (currentWorkflow == null)
            {
                throw new NotImplementedException();
            }
            var existingWorkflow = await _repository.AnyAsync(w => w.State == model.State && w.StatusId == model.StatusId && w.UserId == model.UserId.ToString() && w.Id != workflowId);
            if (existingWorkflow)
            {
                throw new InvalidOperationException("Đã có cấu hình luồng này rồi");
            }
            currentWorkflow.Name = model.Name;
            currentWorkflow.StatusId = model.StatusId;
            currentWorkflow.UserId = model.UserId;
            currentWorkflow.State = model.State;

            _repository.Update(currentWorkflow);
            await _repository.SaveChanges();
            var currentWorkflowUsers = await _cfgWorkFlowUser.FindAsync(wu => wu.WorkflowId == workflowId);
            _cfgWorkFlowUser.RemoveRange(currentWorkflowUsers);
            var newUsers = model.usersDto.Select(u => new CfgWorkFlowUser
            {
                WorkflowId = workflowId,
                UserId = u.UserId,
                IsDefault = false, 
                CfgWorkFlow = currentWorkflow
            }).ToList();
            await _cfgWorkFlowUser.AddRangeAsync(newUsers);
            await _cfgWorkFlowUser.SaveChanges();

            return 1;
        }
        public async Task<int> DeleteWorkflowAndUsersAsync(int workflowId)
        {
            var currentWorkflow = await _repository.GetSingleByCondition(wf => wf.Id == workflowId);
            if (currentWorkflow == null)
            {
                throw new NotImplementedException("Workflow not found!");
            }
            var currentWorkflowUsers = await _cfgWorkFlowUser.FindAsync(wu => wu.WorkflowId == workflowId);
            _cfgWorkFlowUser.RemoveRange(currentWorkflowUsers);
            await _cfgWorkFlowUser.SaveChanges();
            _repository.Remove(currentWorkflow);
            await _repository.SaveChanges();

            return 1;
        }


    }
}
