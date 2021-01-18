using AutoMapper;
using DataAccessLayer.ApplicationDbContext;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Services.ViewModels.CommonModel;

namespace Services.RepositoryPattern.UserLogin
{
    public class UserService : IUserService
    {
        #region Property
        private readonly CASVDbContext _cASVDbContext;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor
        public UserService(CASVDbContext cASVDbContext, IMapper mapper)
        {
            _cASVDbContext = cASVDbContext;
            _mapper = mapper;

        }
        #endregion

        #region Get User Roles
        /// <summary>
        /// Get User Roles from Db
        /// </summary>
        /// <returns></returns>
        public async Task<List<RolesModel>> GetUserRolesAsync()
        {
            try
            {
                var userRoles = await _cASVDbContext.userRoles.Where(c => c.IsActive.Equals(true)).ToListAsync();
                return _mapper.Map<List<RolesModel>>(userRoles);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Create User
        /// <summary>
        /// Create user to Db
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public async Task<string> CreateUserAsync(UserModel userModel)
        {
            try
            {
                var userExists = await _cASVDbContext.users.Where(c => c.Email.Equals(userModel.Email)).AnyAsync();
                if (userExists is false)
                {
                    User res = new User
                    {
                        FirstName = userModel.FirstName,
                        LastName = userModel.LastName,
                        PhoneNumber = userModel.PhoneNumber,
                        Password = userModel.Password,
                        Email = userModel.Email,
                        RoleId = await _cASVDbContext.userRoles.Where(c => c.IsActive.Equals(true) && c.RoleName.Equals(userModel.RoleName)).Select(x => x.RoleId).FirstOrDefaultAsync(),
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    };
                    await _cASVDbContext.users.AddAsync(res);
                    await _cASVDbContext.SaveChangesAsync();
                    return "User Created Success";
                }
                else return "Email already exists";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region User Login
        public async Task<bool> UserLoginAsync(LoginModel loginModel)
        {
            try
            {
                var response = await _cASVDbContext.users.Where(c => c.Email.Equals(loginModel.UserName) && c.Password.Equals(loginModel.Password) && c.IsActive.Equals(true)).AnyAsync();
                if (response is true) return true; else return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
