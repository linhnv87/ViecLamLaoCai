using Repositories;
using Services.DTO;
using Database.Models;
using Database.Models.Website;
using Database.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using AutoMapper;
using static Services.DTO.SSODTOs;
using Core;

namespace Services
{
    public interface IAppUserService
    {
        Task<Guid> UserSignUp(SignUpDTO payload);
        Task<ResponseTokenDTO> UserLogin(LogInDTO payload);
        Task<bool> ChangePassword(ChangePasswordDTO payload);
        Task<bool> LockUser(string userId);
        Task<bool> DeleteUser(string userId);
        Task<IEnumerable<UserInfoDTO>> GetAllInfoUser();
        Task<IEnumerable<UserInfoDTO>> GetAllSpecialistInfoUser();
        Task<UserInfoDTO> GetUserById(string id);
        Task<string> UpdateInfo(UserInfoDTO payload);

        Task<bool> ResetPassword(string userId);
        Task<AppUser> GetUserForSSO(UserInfoResponseData userInfoResult);
        Task<ResponseTokenDTO> GenerateToken(AppUser user);
        Task<IEnumerable<UserInfoDTO>> GetUsersByRoleAsync(string roleName);
        Task<List<AppUser>> GetUserWithRolesAsync();
        Task<RegistrationResponseDTO> RegisterBusiness(BusinessRegisterDTO payload);
        Task<RegistrationResponseDTO> RegisterCandidate(CandidateRegisterDTO payload);

    }
    public class AppUserService : IAppUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserInRoleRepository _userInRoleRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly IMapper _mapper;

        public readonly IDocumentRepository _documentRepository;
        public readonly IDocumentReviewRepository _reviewRepository;
        public readonly ICommentRepository _commentRepository;
        private readonly QLTTrContext _context;

        private string TokenSecret = "77D3624E-1F44-4F7E-A472-D5E449D69F2C-77D3624E-1F44-4F7E-A472-D5E449D69F2C";
        private int TokenExpiryDay = 1;
        
        // Role constants - using Core constants
        public AppUserService(IMapper mapper, IUserRepository userRepository, IRoleRepository roleRepository, IUserInRoleRepository userInRoleRepository, IFieldRepository fieldRepository,
            IDocumentRepository documentRepository, IDocumentReviewRepository reviewRepository, ICommentRepository commentRepository, QLTTrContext context)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userInRoleRepository = userInRoleRepository;
            _mapper = mapper;
            _fieldRepository = fieldRepository;
            _documentRepository = documentRepository;
            _reviewRepository = reviewRepository;
            _commentRepository = commentRepository;
            _context = context;
        }
        public async Task<Guid> UserSignUp(SignUpDTO payload)
        {
            if (await CheckSignUpExisted(payload.Username, payload.Email, payload.PhoneNumber)) throw new Exception("Tên đăng nhập, số điện thoại hoặc E-mail đã được sử dụng, vui lòng nhập lại thông tin");
            Guid newUserId = new Guid();
            string salt = await GenerateSalt();
            var userToCreate = new AppUser()
            {
                UserId = newUserId,
                UserName = payload.Username,
                UserFullName = payload.FullName,
                Email = payload.Email,
                PhoneNumber = payload.PhoneNumber,
                DepartmentId = payload.DepartmentId,
                UnitId = payload.UnitId,
                PasswordSalt = salt,
                FieldOfficer = payload.FieldIds != null && payload.FieldIds.Count > 0 ? String.Join(',', payload.FieldIds) : null,
                HashedPassword = await PasswordHashing(payload.Password, salt)
            };

            var userRoles = from p in payload.RoleIds select new AppUserInRole() { Id = 0, UserId = userToCreate.UserId, RoleId = p };

            try
            {
                await _userRepository.AddAsync(userToCreate);
                await _userRepository.SaveChanges();
                await _userInRoleRepository.AddRangeAsync(userRoles);
                await _userInRoleRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xảy ra khi thêm mới tài khoản: " + ex.Message);
            }
            return userToCreate.UserId;
        }

        public async Task<ResponseTokenDTO> UserLogin(LogInDTO payload)
        {
            Expression<Func<AppUser, bool>> baseFilter = f => true;
            baseFilter = baseFilter.And(x => x.UserName.ToLower() == payload.Username.ToLower());
            var userToAuthenticate = await _userRepository.GetSingleByCondition(baseFilter);
            if (userToAuthenticate == null)
            {
                throw new Exception("Đăng nhập không thành công. Vui lòng kiểm tra lại thông tin đăng nhập");
            }
            var inputPassword = await PasswordHashing(payload.Password, userToAuthenticate.PasswordSalt);
            if (inputPassword != userToAuthenticate.HashedPassword)
            {
                throw new Exception("Đăng nhập không thành công. Vui lòng kiểm tra lại thông tin đăng nhập");
            }
            if (userToAuthenticate.IsLockedout)
            {
                throw new Exception("Đăng nhập không thành công. Tài khoản đã bị khóa, vui lòng liên hệ quản trị viên");
            }
            return await GenerateToken(userToAuthenticate);
        }

        public async Task<bool> ChangePassword(ChangePasswordDTO payload)
        {
            var userToChange = await _userRepository.FirstOrDefaultAsync(x => x.UserId == payload.UserId);
            if (userToChange == null) throw new Exception("Thay đổi mật khẩu thất bại");
            if (await PasswordHashing(payload.OldPassword, userToChange.PasswordSalt) != userToChange.HashedPassword) throw new Exception("Không thể thay đổi mật khẩu - Xác thực tài khoản không thành công");
            var newSalt = await GenerateSalt();
            userToChange.PasswordSalt = newSalt;
            userToChange.HashedPassword = await PasswordHashing(payload.NewPassword, newSalt);
            _userRepository.Update(userToChange);
            await _userRepository.SaveChanges();
            return true;
        }

        public async Task<bool> ResetPassword(string userId)
        {
            const string samplePassword = "Tinhuy@123456";
            var userToChange = await _userRepository.FirstOrDefaultAsync(x => x.UserId.ToString() == userId);
            if (userToChange == null) throw new Exception("Thay đổi mật khẩu thất bại");
            var newSalt = await GenerateSalt();
            userToChange.PasswordSalt = newSalt;
            userToChange.HashedPassword = await PasswordHashing(samplePassword, newSalt);
            _userRepository.Update(userToChange);
            await _userRepository.SaveChanges();
            return true;
        }

        private async Task<bool> CheckSignUpExisted(string username, string email, string phone)
        {
            Expression<Func<AppUser, bool>> baseFilter = f => true;
            baseFilter = baseFilter.And(x => x.UserName.ToLower() == username.ToLower() || x.Email.ToLower() == email.ToLower() || x.PhoneNumber == phone);
            var existingUsername = await _userRepository.GetSingleByCondition(baseFilter);
            if (existingUsername != null) return true;


            return false;
        }

        private async Task<string> GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] saltBytes = new byte[16];
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        private async Task<string> PasswordHashing(string password, string salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                return Convert.ToBase64String(hash);
            }
        }

        public async Task<ResponseTokenDTO> GenerateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var roleIds = _userInRoleRepository.GetAll().Where(x => x.UserId == user.UserId).Select(x => x.RoleId).ToList();
            var roles = _roleRepository.GetAll().Where(x => roleIds.Contains(x.RoleId)).Select(x => x.RoleName).ToList();
            var fields = new List<int>();
            if(user.FieldOfficer != null && user.FieldOfficer != string.Empty)
            {
                var fieldIds = user.FieldOfficer.Split(',');
                foreach (var fieldId in fieldIds)
                {
                    var z = 0;
                    if(int.TryParse(fieldId, out z))
                    {
                        fields.Add(z);
                    }
                }
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSecret.ToString()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(TokenExpiryDay),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenResponse = new ResponseTokenDTO
            {
                AccessToken = tokenHandler.WriteToken(token),
                TokenExpiration = token.ValidTo,
                UserName = user.UserName,
                DisplayName = user.UserFullName,
                UserId = user.UserId,
                Roles = roles,
                FieldIds = fields,
                SelectedRole = roles[0]
            };

            return tokenResponse;
        }

        public async Task<IEnumerable<UserInfoDTO>> GetAllInfoUser()
        {
            var (infoUser, roles) =  await _userRepository.GetAllUserWithRolesAsync();
            if (infoUser == null)
            {
                throw new NotImplementedException();
            }
            var result = _mapper.Map<IEnumerable<UserInfoDTO>>(infoUser);
            foreach (var user in result)
            {
                var userRoles = roles.Where(x => x.UserId == user.UserId);
                user.Roles = _mapper.Map<List<RoleDTO>>(userRoles).ToList();
            }
            return result;
        }

        public async Task<IEnumerable<UserInfoDTO>> GetAllSpecialistInfoUser()
        {
            var specialistRoles = await _roleRepository.FirstOrDefaultAsync(x => x.RoleName != AppRoleNames.CHUYEN_VIEN && x.RoleName != AppRoleNames.VAN_THU);
            var userIds = await _userInRoleRepository.GetMulti(x => x.RoleId == specialistRoles.RoleId);
            var userInfos = (await _userRepository.GetMulti(x => userIds.Select(z => z.UserId).Contains(x.UserId))).ToList();
            if (userInfos == null)
            {
                throw new NotImplementedException();
            }
            var result = _mapper.Map<IEnumerable<UserInfoDTO>>(userInfos);
            return result;
        }

        public async Task<UserInfoDTO> GetUserById(string id)
        {            
            var data = await _userRepository.FirstOrDefaultAsync(x => x.UserId.ToString().ToLower().Trim() == id.ToLower().Trim());
            if (data == null) throw new Exception("Không tìm thấy thông tin tài khoản");
            var result = _mapper.Map<UserInfoDTO>(data);
            var rolesOfUsers = await _userInRoleRepository.GetMulti(x => x.UserId.ToString().ToLower().Trim() == id.ToLower().Trim());
            var roles = await _roleRepository.GetMulti(x => rolesOfUsers.Select(x => x.RoleId).Contains(x.RoleId));
            var fields = new List<int>();
            if(data.FieldOfficer != null && data.FieldOfficer != string.Empty)
            {
                var fieldIds = data.FieldOfficer.Split(',');
                foreach (var fieldId in fieldIds)
                {
                    var z = 0;
                    if (int.TryParse(fieldId, out z))
                    {
                        fields.Add(z);
                    }
                }
            }
            result.FieldIds = fields;
            result.Roles = _mapper.Map<List<RoleDTO>>(roles);
            return result;
        }

        public async Task<string> UpdateInfo(UserInfoDTO payload)
        {
            var data = await _userRepository.FirstOrDefaultAsync(x => x.UserId.ToString().ToLower().Trim() == payload.UserId.ToString().ToLower().Trim());
            if (data == null) throw new Exception("Không tìm thấy thông tin tài khoản");
            data.UserFullName = payload.UserFullName;
            data.PhoneNumber = payload.PhoneNumber;
            data.Email = payload.Email;
            data.FieldOfficer = payload.FieldIds != null && payload.FieldIds.Count > 0 ? String.Join(',', payload.FieldIds) : null;
            data.DepartmentId = payload.DepartmentId;
            data.UnitId = payload.UnitId;
            _userRepository.Update(data);
            await _userRepository.SaveChanges();

            var roles = await _userInRoleRepository.GetMulti(x => x.UserId.ToString().ToLower().Trim() == payload.UserId.ToString().ToLower().Trim());
            if(roles.Select(x => x.RoleId) != payload.Roles.Select(x => x.RoleId))
            {
                var rolesToAdd = new List<AppUserInRole>();
                foreach(var item in payload.Roles)
                {
                    rolesToAdd.Add(new AppUserInRole() { RoleId = item.RoleId, UserId = payload.UserId });
                }
                await _userInRoleRepository.AddRangeAsync(rolesToAdd);
                _userInRoleRepository.RemoveRange(roles);
                await _userInRoleRepository.SaveChanges();
            }
            return data.UserId.ToString();
        }

        public async Task<AppUser> GetUserForSSO(UserInfoResponseData userInfoResult)
        {
            var subPrefix = userInfoResult.sub?.Split('@')[0];
            Expression<Func<AppUser, bool>> filter = x =>
            x.UserName == userInfoResult.email ||
            x.UserName == userInfoResult.sub ||
            x.UserName == userInfoResult.preferred_username ||
            (subPrefix != null && x.UserName == subPrefix) ||
            (x.Email != null && x.Email == userInfoResult.email) ||
            (x.PhoneNumber != null && x.PhoneNumber == userInfoResult.sub);
            var foundUser = await _userRepository.GetSingleByCondition(filter);
            if (foundUser == null)
            {
                return null;
            }
            return foundUser;
        }

        public async Task<bool> LockUser(string userId)
        {
            var foundUser = await _userRepository.FirstOrDefaultAsync(x => x.UserId.ToString().ToLower() == userId);
            if (foundUser == null) throw new Exception("Thao tác không thành công - không tìm thấy tài khoản này");

            foundUser.IsLockedout = !foundUser.IsLockedout;
            _userRepository.Update(foundUser);
            await _userRepository.SaveChanges();
            return true;
        }

        public async Task<bool> DeleteUser(string userId)
        {
            var foundUser = await _userRepository.FirstOrDefaultAsync(x => x.UserId.ToString().ToLower() == userId);
            if (foundUser == null) throw new Exception("Thao tác không thành công - không tìm thấy tài khoản này");

            var defaultAccountId = "E9A5779E-F275-4072-7B34-08DBD51A6DD9";
            try
            {
                var defaultRole = await _roleRepository.FirstOrDefaultAsync(x => x.RoleName == "General Specialist");
                var mappedData = await _userInRoleRepository.GetMulti(x => x.RoleId == defaultRole.RoleId);
                var defaultAccount = await _userRepository.FirstOrDefaultAsync(x => mappedData.Select(y => y.UserId).Contains(x.UserId));
                defaultAccountId = defaultAccount.UserId.ToString();
            }
            catch
            {
                defaultAccountId = "E9A5779E-F275-4072-7B34-08DBD51A6DD9";
            }
            var documents = await _documentRepository.GetMulti(x => x.CreatedBy == Guid.Parse(userId));
            foreach(var item in documents)
            {
                item.CreatedBy = Guid.Parse(defaultAccountId);
                _documentRepository.Update(item);
                await _documentRepository.SaveChanges();
            }

            var approvals = await _reviewRepository.GetMulti(x => x.CreatedBy == Guid.Parse(userId));
            foreach (var item in approvals)
            {
                item.CreatedBy = Guid.Parse(defaultAccountId);
                _reviewRepository.Update(item);
                await _reviewRepository.SaveChanges();
            }

            var comments = await _commentRepository.GetMulti(x => x.CreatedBy == Guid.Parse(userId));
            
            if(comments.Count > 0)
            {
                _commentRepository.RemoveRange(comments);
                await _commentRepository.SaveChanges();
            }

            _userRepository.Remove(foundUser);
            await _userRepository.SaveChanges();
            return true;
        }

        public async Task<IEnumerable<UserInfoDTO>> GetUsersByRoleAsync(string roleName)
        {
            try
            {
                var data = await _userRepository.GetUsersByRoleAsync(roleName);
                var result = _mapper.Map<IEnumerable<UserInfoDTO>>(data);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<AppUser>> GetUserWithRolesAsync()
        {
            try
            {
                var data = await _userRepository.GetUserWithRolesAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RegistrationResponseDTO> RegisterBusiness(BusinessRegisterDTO payload)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(payload.Email) || string.IsNullOrEmpty(payload.Password) || 
                    string.IsNullOrEmpty(payload.CompanyName) || string.IsNullOrEmpty(payload.RepresentativeName))
                {
                    throw new Exception("Vui lòng điền đầy đủ thông tin bắt buộc");
                }

                if (payload.Password != payload.ConfirmPassword)
                {
                    throw new Exception("Mật khẩu nhập lại không khớp");
                }
                if (await CheckSignUpExisted(payload.Email, payload.Email, payload.Phone))
                {
                    throw new Exception("Email hoặc số điện thoại đã được sử dụng");
                }

                Guid newUserId = Guid.NewGuid();
                string salt = await GenerateSalt();
                
                var userToCreate = new AppUser()
                {
                    UserId = newUserId,
                    UserName = payload.Email,
                    UserFullName = payload.RepresentativeName,
                    Email = payload.Email,
                    PhoneNumber = payload.Phone,
                    PasswordSalt = salt,
                    HashedPassword = await PasswordHashing(payload.Password, salt),
                    IsApproved = false,
                    IsLockedout = false
                };

                var businessRoleId = await GetBusinessRoleId();
              
                var userRoles = new List<AppUserInRole> 
                { 
                    new AppUserInRole() { Id = 0, UserId = newUserId, RoleId = businessRoleId }
                };

                await _userRepository.AddAsync(userToCreate);
                await _userRepository.SaveChanges();
                await _userInRoleRepository.AddRangeAsync(userRoles);
                await _userInRoleRepository.SaveChanges();

                // Create Company record
                await CreateCompanyRecord(newUserId, payload);

                await transaction.CommitAsync();

                return new RegistrationResponseDTO
                {
                    UserId = newUserId,
                    Message = "Đăng ký doanh nghiệp thành công! Tài khoản của bạn đang chờ phê duyệt.",
                    RequiresEmailVerification = true,
                    RequiresApproval = true
                };
            }
            catch (Exception ex)
            {
                
                await transaction.RollbackAsync();
                throw new Exception("Lỗi đăng ký doanh nghiệp: " + ex.Message);
            }
        }

        public async Task<RegistrationResponseDTO> RegisterCandidate(CandidateRegisterDTO payload)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(payload.FullName) || string.IsNullOrEmpty(payload.Email) || 
                    string.IsNullOrEmpty(payload.Password) || string.IsNullOrEmpty(payload.Phone))
                {
                    throw new Exception("Vui lòng điền đầy đủ thông tin bắt buộc");
                }

                if (payload.Password != payload.ConfirmPassword)
                {
                    throw new Exception("Mật khẩu nhập lại không khớp");
                }

                // Check if user already exists
                if (await CheckSignUpExisted(payload.Email, payload.Email, payload.Phone))
                {
                    throw new Exception("Email hoặc số điện thoại đã được sử dụng");
                }

                // Create AppUser account
                Guid newUserId = Guid.NewGuid();
                string salt = await GenerateSalt();
                
                var userToCreate = new AppUser()
                {
                    UserId = newUserId,
                    UserName = payload.Email,
                    UserFullName = payload.FullName,
                    Email = payload.Email,
                    PhoneNumber = payload.Phone,
                    PasswordSalt = salt,
                    HashedPassword = await PasswordHashing(payload.Password, salt),
                    IsApproved = true,
                    IsLockedout = false
                };

                var candidateRoleId = await GetCandidateRoleId();
                Console.WriteLine($"Candidate role ID: {candidateRoleId} for user: {newUserId}");
                
                var userRoles = new List<AppUserInRole> 
                { 
                    new AppUserInRole() { Id = 0, UserId = newUserId, RoleId = candidateRoleId }
                };

                await _userRepository.AddAsync(userToCreate);
                await _userRepository.SaveChanges();
                await _userInRoleRepository.AddRangeAsync(userRoles);
                await _userInRoleRepository.SaveChanges();

                await CreateWorkerRecord(newUserId, payload);

                await transaction.CommitAsync();

                return new RegistrationResponseDTO
                {
                    UserId = newUserId,
                    Message = "Đăng ký ứng viên thành công! Bạn có thể đăng nhập ngay.",
                    RequiresEmailVerification = true,
                    RequiresApproval = false
                };
            }
            catch (Exception ex)
            {
                
                await transaction.RollbackAsync();
                throw new Exception("Lỗi đăng ký ứng viên: " + ex.Message);
            }
        }

        private async Task<Guid> GetBusinessRoleId()
        {
            var role = await _roleRepository.FirstOrDefaultAsync(x => x.RoleName == Core.AppRoleNames.DOANH_NGHIEP);
            if (role == null)
            {
                throw new Exception($"Role '{Core.AppRoleNames.DOANH_NGHIEP}' không tồn tại trong hệ thống. Vui lòng liên hệ quản trị viên.");
            }
            return role.RoleId;
        }

        private async Task<Guid> GetCandidateRoleId()
        {
            
            var role = await _roleRepository.FirstOrDefaultAsync(x => x.RoleName == Core.AppRoleNames.UNG_VIEN);
            if (role == null)
            {
                throw new Exception($"Role '{Core.AppRoleNames.UNG_VIEN}' không tồn tại trong hệ thống. Vui lòng liên hệ quản trị viên.");
            }
            return role.RoleId;
        }

        private async Task CreateCompanyRecord(Guid userId, BusinessRegisterDTO payload)
        {
            try
            {
                var company = new Company
                {
                    UserId = userId,
                    CompanyName = payload.CompanyName,
                    Description = payload.Description ?? "",
                    Industry = payload.Industry ?? "",
                    CompanySize = payload.CompanySize ?? "",
                    Website = payload.Website ?? "",
                    Address = payload.Address ?? "",
                    PhoneNumber = payload.Phone,
                    Email = payload.Email,
                    IsVerified = false,
                    ApprovalStatus = "Pending",
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi tạo hồ sơ Company: {ex.Message}");
            }
        }

        private async Task CreateWorkerRecord(Guid userId, CandidateRegisterDTO payload)
        {
            try
            {
                var worker = new Worker
                {
                    UserId = userId,
                    FullName = payload.FullName,
                    Email = payload.Email,
                    PhoneNumber = payload.Phone,
                    DateOfBirth = payload.DateOfBirth,
                    Gender = payload.Gender ?? "Không xác định",
                    Address = payload.Address ?? "",
                    DistrictId = payload.DistrictId,
                    CommuneId = payload.CommuneId,
                    EducationLevelId = payload.EducationLevelId,
                    CareerId = payload.CareerId,
                    WorkExperience = "Chưa có kinh nghiệm",
                    CurrentCompany = "Chưa cập nhật",
                    ExpectedSalary = null,
                    AvatarUrl = "assets/vieclamlaocai/img/default-avatar.png",
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _context.Workers.Add(worker);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi tạo hồ sơ Worker: {ex.Message}");
            }
        }

    }
}
