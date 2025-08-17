using Repositories;
using Services.DTO;
using Database.Models;
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

        private string TokenSecret = "77D3624E-1F44-4F7E-A472-D5E449D69F2C-77D3624E-1F44-4F7E-A472-D5E449D69F2C";
        private int TokenExpiryDay = 1;
        public AppUserService(IMapper mapper,IUserRepository userRepository, IRoleRepository roleRepository, IUserInRoleRepository userInRoleRepository, IFieldRepository fieldRepository
            ,IDocumentRepository documentRepository, IDocumentReviewRepository reviewRepository, ICommentRepository commentRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userInRoleRepository = userInRoleRepository;
            _mapper = mapper;
            _fieldRepository = fieldRepository;
            _documentRepository = documentRepository;
            _reviewRepository = reviewRepository;
            _commentRepository = commentRepository;
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
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000))
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

    }
}
