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
    public interface INotificationService
    {
        Task<int> CreateNotifications(int type, int docId, string? commentUserId);
        Task<List<NotificationDTO>> GetNotificationsForUser(string userId);
        Task<int> UpdateNotificationStatus(int id);
        Task<int> CheckNewAlert(string userId);
    }
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserInRoleRepository _userInRoleRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        public NotificationService(INotificationRepository _notificationRepository,
        IUserRepository _userRepository,
        IRoleRepository _roleRepository,
        IUserInRoleRepository _userInRoleRepository,
        IDocumentRepository _documentRepository,
        IMapper _mapper)
        {
            this._notificationRepository = _notificationRepository;
            this._userRepository = _userRepository;
            this._roleRepository = _roleRepository;
            this._userInRoleRepository = _userInRoleRepository;
            this._documentRepository = _documentRepository;
            this._mapper = _mapper;

        }
        public async Task<int> CreateNotifications(int type, int docId, string? commentUserId)
        {
            /**
            Type 1: User send document to approve => Create Notifications for Approvers, General Specialist
            Type 2: General Specialist remind approver to perform actions => Create Notifications for Approvers
            Type 3: Approver comments on document => Create Notifications for document's author
            Type 4: Automation Status Update => Create Notifications for document's author, General Specialist
            */
            try
            {
                var document = await _documentRepository.FirstOrDefaultAsync(x => x.Id == docId);
                if (document == null) throw new Exception("Not Found");

                var approverRole = await _roleRepository.FirstOrDefaultAsync(x => x.RoleName.ToLower() == "Approver".ToLower() || x.RoleName.ToLower() == "General Approver".ToLower());
                if (approverRole == null) throw new Exception("Không có cán bộ duyệt - Thêm cán bộ duyệt vào hệ thống");

                var generalRole = await _roleRepository.FirstOrDefaultAsync(x => x.RoleName.ToLower() == "General Specialist".ToLower());
                if (generalRole == null) throw new Exception("Không có chuyên viên tổng hợp - Thêm chuyên viên tổng hợp vào hệ thông");

                var approverList = (await _userInRoleRepository.GetMulti(x => x.RoleId == approverRole.RoleId)).Select(x => x.UserId).ToList();
                var generalList = (await _userInRoleRepository.GetMulti(x => x.RoleId == generalRole.RoleId)).Select(x => x.UserId).ToList();
                var authorUser = await _userRepository.FirstOrDefaultAsync(x => x.UserId == document.CreatedBy);
                var commentUserName = "";
                if (commentUserId != null)
                {
                    var commentUser = await _userRepository.FirstOrDefaultAsync(x => x.UserId.ToString().ToLower() == commentUserId.ToLower());
                    if (commentUser != null)
                    {
                        commentUserName = commentUser.UserFullName;
                    }
                }

                var currentTime = DateTime.Now;
                var dateEndApproval = (DateTime)document.DateEndApproval!;

                var notificationList = new List<TblNotification>();


                //if (type == 1)
                //{                    
                //    foreach (var item in approverList)
                //    {                        
                //        notificationList.Add(new TblNotification()
                //        {
                //            //Id = 0
                //            ForUserId = item,
                //            NotificationContent = $"[Chuyên viên gửi duyệt] Tờ trình [{document.Title}] đã được gửi duyệt bởi chuyên viên [{authorUser.UserFullName}] " +
                //            $"lúc [{currentTime.Hour} giờ {currentTime.Minute} phút - ngày {currentTime.Day}/{currentTime.Month}/{currentTime.Year}]. " +
                //            $"Hạn xử lý cho đến hết ngày [{dateEndApproval.Day}/{dateEndApproval.Month}/{dateEndApproval.Year}]",
                //            NotificationLink = $"/admin/to-trinh/{document.StatusCode}/document-detail/{docId}",
                //            Type = type,
                //            Watched = false
                //        });
                //    }
                //    foreach (var item in generalList)
                //    {
                //        notificationList.Add(new TblNotification()
                //        {
                //            //Id = 0
                //            ForUserId = item,
                //            NotificationContent = $"[Chuyên viên gửi duyệt] Tờ trình [{document.Title}] đã được gửi duyệt bởi chuyên viên [{authorUser.UserFullName}] " +
                //            $"lúc [{currentTime.Hour} giờ {currentTime.Minute} phút - ngày {currentTime.Day}/{currentTime.Month}/{currentTime.Year}]. " +
                //            $"Hạn xử lý cho đến hết ngày [{dateEndApproval.Day}/{dateEndApproval.Month}/{dateEndApproval.Year}]",
                //            NotificationLink = $"/admin/to-trinh/{document.StatusCode}/document-detail/{docId}",
                //            Type = type,
                //            Watched = false
                //        });
                //    }
                //}
                //else if (type == 2)
                //{                    
                //    foreach (var item in approverList)
                //    {                        
                //        notificationList.Add(new TblNotification()
                //        {
                //            //Id = 0
                //            ForUserId = item,
                //            NotificationContent = $"[Nhắc nhở xử lý] lúc {currentTime.Hour} giờ {currentTime.Minute} phút - ngày {currentTime.Day}/{currentTime.Month}/{currentTime.Year}] " +
                //        $"Tờ trình [{document.Title}] đã được gửi duyệt bởi chuyên viên [{authorUser.UserFullName}]. " +
                //        $"Hạn xử lý cho đến hết ngày [{dateEndApproval.Day}/{dateEndApproval.Month}/{dateEndApproval.Year}]",
                //            NotificationLink = $"/admin/to-trinh/{document.StatusCode}/document-detail/{docId}",
                //            Type = type,
                //            Watched = false
                //        });
                //    }
                //}
                //else if (type == 3)
                //{                    
                //    notificationList.Add(new TblNotification()
                //    {
                //        //Id = 0
                //        ForUserId = authorUser.UserId,
                //        NotificationContent = $"[Ý kiến từ cán bộ] " +
                //        $"Tờ trình [{document.Title}] nhận được ý kiến từ cán bộ [{commentUserName}] " +
                //        $"lúc [{currentTime.Hour} giờ {currentTime.Minute} phút - ngày {currentTime.Day}/{currentTime.Month}/{currentTime.Year}] ",
                //        NotificationLink = $"/admin/to-trinh/{document.StatusCode}/document-detail/{docId}",
                //        Type = type,
                //        Watched = false
                //    });
                //}
                //else if (type == 4)
                //{
                //    var status = document.StatusCode == 4 ? "đã được duyệt"
                //                : document.StatusCode == 5 ? "không được duyệt"
                //                : document.StatusCode == 7 ? "đã quá hạn duyệt" : "";
                                        
                //    notificationList.Add(new TblNotification()
                //    {
                //        //Id = 0
                //        ForUserId = authorUser.UserId,
                //        NotificationContent = $"[Cập nhật trạng thái tờ trình] " +
                //        $"Tờ trình [{document.Title}] {status}",
                //        NotificationLink = $"/admin/to-trinh/{document.StatusCode}/document-detail/{docId}",
                //        Type = type,
                //        Watched = false
                //    });
                //    foreach (var item in approverList)
                //    {                        
                //        notificationList.Add(new TblNotification()
                //        {
                //            //Id = 0
                //            ForUserId = item,
                //            NotificationContent = $"[Cập nhật trạng thái tờ trình] " +
                //        $"Tờ trình [{document.Title}] {status}",
                //            NotificationLink = $"/admin/to-trinh/{document.StatusCode}/document-detail/{docId}",
                //            Type = type,
                //            Watched = false
                //        });
                //    }
                //}

                await _notificationRepository.AddRangeAsync(notificationList);
                await _notificationRepository.SaveChanges();
                return notificationList.Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }            
        }

        public async Task<List<NotificationDTO>> GetNotificationsForUser(string userId)
        {
            try
            {
                var data = await _notificationRepository.GetMulti(x => x.ForUserId != null && x.ForUserId.ToString()!.ToLower() == userId.ToLower());
                return _mapper.Map<List<NotificationDTO>>(data.OrderByDescending(x => x.Id));
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdateNotificationStatus(int id)
        {
            try
            {
                var data = await _notificationRepository.FirstOrDefaultAsync(x => x.Id == id);
                if (data == null) throw new Exception("Not found");
                data.Watched = true;
                _notificationRepository.Update(data);
                await _notificationRepository.SaveChanges();
                return data.Id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> CheckNewAlert(string userId)
        {
            try
            {
                var data = await _notificationRepository.GetMulti(x => x.ForUserId != null && x.ForUserId.ToString()!.ToLower() == userId.ToLower());
                return data.Count;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
