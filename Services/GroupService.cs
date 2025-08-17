using AutoMapper;
using Database.Models;
using DocumentFormat.OpenXml.Office2016.Excel;
using Repositories;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services;

public interface IGroupService
{
    Task<IEnumerable<TblGroups>> GetAllGroupsAsync();
    Task<IEnumerable<UserInfoDTO>> GetAllInfoUserByGroupId(int groupId);
    Task<int> CreateGroupAsync(GroupDTO group);
    Task<int> UpdateGroupAsync(int groupId, GroupDTO group);
    Task<int> UpdateStatusGroupAsync(int groupId);
    Task<int> DeleteGroupAsync(int groupId);
}

public class GroupService : IGroupService
{
    private readonly ITblGroupRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly ITblGroupDetailRepository _groupDetailRepository;
    private readonly IMapper _mapper;

    public GroupService(
        ITblGroupRepository repository,
        IUserRepository userRepository,
        ITblGroupDetailRepository groupDetailRepository,
        IMapper mapper)
    {
        _repository = repository;
        _userRepository = userRepository;
        _groupDetailRepository = groupDetailRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TblGroups>> GetAllGroupsAsync()
    {
        var result = _repository.GetAll().ToList();
        if (result == null)
        {
            throw new NotImplementedException();
        }
        return result ?? new List<TblGroups>();
    }

    public async Task<IEnumerable<UserInfoDTO>> GetAllInfoUserByGroupId(int groupId)
    {
        var (infoUser, userInRoles) = await _repository.GetUsersByGroupIdAsync(groupId);

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

    public async Task<int> CreateGroupAsync(GroupDTO group)
    {
        var groupEntity = _mapper.Map<TblGroups>(group);
        await _repository.AddAsync(groupEntity);
        var newGroup = await _repository.SaveChangesAsync(groupEntity);

        var groupDetails = group.GroupDetails.Select(x => new TblGroupDetails
        {
            GroupId = newGroup.Id,
            UserId = x.UserId
        });

        await _groupDetailRepository.AddRangeAsync(groupDetails);
        await _groupDetailRepository.SaveChanges();

        return 1;
    }

    public async Task<int> UpdateGroupAsync(int groupId, GroupDTO group)
    {
        var currentGroup = await _repository.GetSingleByCondition(g => g.Id == groupId);
        if (currentGroup == null)
        {
            throw new NotImplementedException();
        }

        currentGroup.GroupName = group.GroupName;
        currentGroup.IsActive = group.IsActive;
        currentGroup.IsSMS = group.IsSMS;
        _repository.Update(currentGroup);
        await _repository.SaveChanges();

        var currentGroupDetails = await _groupDetailRepository.FindAsync(gd => gd.GroupId == groupId);
        _groupDetailRepository.RemoveRange(currentGroupDetails);

        var groupDetails = group.GroupDetails.Select(x => new TblGroupDetails
        {
            GroupId = currentGroup.Id,
            UserId = x.UserId
        });

        await _groupDetailRepository.AddRangeAsync(groupDetails);
        await _groupDetailRepository.SaveChanges();

        return 1;
    }

    public async Task<int> UpdateStatusGroupAsync(int groupId)
    {
        var currentGroup = await _repository.GetSingleByCondition(g => g.Id == groupId);
        if (currentGroup == null)
        {
            throw new NotImplementedException();
        }

        currentGroup.IsActive = !currentGroup.IsActive;
        _repository.Update(currentGroup);
        await _repository.SaveChanges();

        return 1;
    }

    public async Task<int> DeleteGroupAsync(int groupId)
    {
        var currentGroup = await _repository.GetSingleByCondition(g => g.Id == groupId);
        if (currentGroup == null)
        {
            throw new NotImplementedException();
        }

        var currentGroupDetails = await _groupDetailRepository.FindAsync(gd => gd.GroupId == groupId);
        _groupDetailRepository.RemoveRange(currentGroupDetails);
        await _groupDetailRepository.SaveChanges();

        _repository.Remove(currentGroup);
        await _repository.SaveChanges();

        return 1;
    }
}
