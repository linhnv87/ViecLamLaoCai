using AutoMapper;
using Core.QueryModels;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ISMSLogService
    {
        Task<IEnumerable<SMSLogGroupDTO>> GetAllLogsWithUserNames(SMSLogQueryModel queries);
    }
    public class SMSLogService : ISMSLogService
    {
        private readonly ISMSLogRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserInRoleRepository _userInRoleRepository;
        public SMSLogService(ISMSLogRepository repository, IMapper mapper, IUserRepository userRepository, IRoleRepository roleRepository, IUserInRoleRepository userInRoleRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userInRoleRepository = userInRoleRepository;
        }
        public async Task<IEnumerable<SMSLogGroupDTO>> GetAllLogsWithUserNames(SMSLogQueryModel queries)
        {
            try
            {
                var rolePriority = new List<string> { "bi-thu", "pho-bi-thu", "ban-chap-hanh", "ban-thuong-vu","chanh-van-phong","pho-chanh-van-phong","truong-phong", "pho-truong-phong", "chuyen-vien" };
                var logsQuery = from smsLog in _repository.GetAll()
                                join appUser in _userRepository.GetAll()
                                on smsLog.PhoneNumber equals appUser.PhoneNumber into userGroup
                                from user in userGroup.DefaultIfEmpty()
                                join userRole in _userInRoleRepository.GetAll()
                                on user.UserId equals userRole.UserId into roleGroup
                                from userRole in roleGroup.DefaultIfEmpty()
                                join role in _roleRepository.GetAll()
                                on userRole.RoleId equals role.RoleId into roleGroupFull
                                from role in roleGroupFull.DefaultIfEmpty()
                                where smsLog.DocId == queries.DocId
                                      && smsLog.Type == queries.Type
                                      && (queries.IsSucceeded == null || smsLog.IsSucceeded == queries.IsSucceeded)
                                      && smsLog.SubmitCount.HasValue
                                select new
                                {
                                    smsLog.Id,
                                    smsLog.PhoneNumber,
                                    smsLog.ErrorMessage,
                                    smsLog.SubmitCount,
                                    ReceiverName = user != null ? user.UserFullName : string.Empty,
                                    MessageType = smsLog.Type == 0 ? "Thêm mới" : smsLog.Type == 1 ? "Nhắc nhở" : string.Empty,
                                    Status = smsLog.IsSucceeded ? "Thành Công" : "Thất Bại",
                                    SentTime = smsLog.Created,
                                    RoleName = role != null ? role.RoleName : string.Empty
                                };
                var data = (await logsQuery.ToListAsync());
                var filteredData = data.Select(x => new
                {
                    x.Id,
                    x.PhoneNumber,
                    x.ErrorMessage,
                    x.SubmitCount,
                    x.ReceiverName,
                    x.MessageType,
                    x.Status,
                    x.SentTime,
                    x.RoleName,
                    RolePriorityIndex = rolePriority.IndexOf(x.RoleName)
                }).GroupBy(x => new { x.PhoneNumber, x.SubmitCount })
                  .Select(g => g.Where(x => x.SentTime == g.Max(y => y.SentTime)).OrderBy(x => x.RolePriorityIndex).First())
                  .OrderBy(x => x.RolePriorityIndex).ToList();
                var groupedData = filteredData
                    .OrderBy(x => x.SubmitCount)
                    .GroupBy(x => x.SubmitCount)
                    .Select(group => new SMSLogGroupDTO
                    {
                        SubmitCount = group.Key.Value,
                        SMSLogs = group
                            .OrderBy(x => rolePriority.IndexOf(x.RoleName))
                            .Select(x => new SMSLogDTO
                            {
                                Id = x.Id,
                                PhoneNumber = x.PhoneNumber?.Replace("\r", "").Replace("\n", "").Trim(),
                                ReceiverName = x.ReceiverName,
                                MessageType = x.MessageType,
                                Status = x.Status,
                                SentTime = x.SentTime,
                                ErrorMessage = x.ErrorMessage,
                                RoleName = x.RoleName
                            })
                            .ToList()
                    })
                    .ToList();
                return groupedData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
