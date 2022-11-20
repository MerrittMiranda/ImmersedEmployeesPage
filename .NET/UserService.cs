using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sabio.Services
{
    public class UserService : IUserService
    {
        private IAuthenticationService<int> _authenticationService;
        private IDataProvider _dataProvider;

        public UserService(IAuthenticationService<int> authSerice, IDataProvider dataProvider)
        {
            _authenticationService = authSerice;
            _dataProvider = dataProvider;
        }

        public async Task<bool> LogInAsync(string email, string password)
        {
            bool isSuccessful = false;

            IUserAuthData response = GetCurrent(email, password);

            if (response != null)
            {
                await _authenticationService.LogInAsync(response);
                isSuccessful = true;
            }

            return isSuccessful;
        }

        public async Task<bool> LogInTest(string email, string password, int id, string[] roles = null)
        {
            bool isSuccessful = false;
            var testRoles = new[] { "User", "Super", "Content Manager" };

            var allRoles = roles == null ? testRoles : testRoles.Concat(roles);

            IUserAuthData response = new UserBase
            {
                Id = id
                ,
                Name = email
                ,
                Roles = allRoles
                ,
                TenantId = "Acme Corp UId"
            };

            Claim fullName = new Claim("CustomClaim", "Sabio Bootcamp");
            await _authenticationService.LogInAsync(response, new Claim[] { fullName });

            return isSuccessful;
        }

        public int Create(UserAddRequest model, int statusTypeId)
        {
            int userId = 0;
            string password = model.Password;
            string salt = BCrypt.BCryptHelper.GenerateSalt();
            string hashedPassword = BCrypt.BCryptHelper.HashPassword(password, salt);
            string procName = "[dbo].[Users_Insert]";

            _dataProvider.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col, hashedPassword);
                    col.AddWithValue("@statusTypeId", statusTypeId);
                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;
                    col.Add(idOut);
                },
            returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@Id"].Value;
                int.TryParse(oId.ToString(), out userId);
            }
            );

            return userId;
        }

        public IUserAuthData GetCurrent(string email, string password)
        {
            string procName = "[dbo].[Users_Select_AuthData]";
            UserBase user = null;
            AuthUser authUser = null;

            _dataProvider.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Email", email);
            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {

                if (set == 0)
                {
                    int startingIndex = 0;

                    authUser = new AuthUser();
                    user = new UserBase();

                    authUser.Id = reader.GetSafeInt32(startingIndex++);
                    authUser.Email = reader.GetSafeString(startingIndex++);
                    authUser.Password = reader.GetSafeString(startingIndex++);

                    bool isValidCredentials = BCrypt.BCryptHelper.CheckPassword(password, authUser.Password);

                    if (isValidCredentials)
                    {
                        user.Id = authUser.Id;
                        user.Name = authUser.Email;
                        user.TenantId = "Immersed";
                    }
                }
                if (set == 1)
                {
                    if (authUser.Roles == null)
                    {
                        authUser.Roles = new List<string>();
                    }
                    var role = reader.GetSafeString(0);
                    authUser.Roles.Add(role);
                }
                user.Roles = authUser.Roles;
            }
            );

            return user;
        }

        public BaseUser GetById(int id)
        {
            string procName = "[dbo].[Users_Select_ById]";
            BaseUser baseUser = null;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);

            },
            delegate (IDataReader reader, short set)
            {
                baseUser = MapSingleUser(reader);
            }
            );

            return baseUser;
        }

        public int GetIdByEmail(string email)
        {
            string procName = "[dbo].[Users_SelectId_ByEmail]";
            int baseUserId = 0;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Email", email);
            },
            delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                baseUserId = reader.GetSafeInt32(startingIndex++);
            });

            return baseUserId;
        }

        public void AddUserToken(string token, int userId, int tokenTypeId)
        {
            string procName = "[dbo].[UserTokens_Insert]";

            _dataProvider.ExecuteNonQuery(procName
                , inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Token", token);
                    col.AddWithValue("@UserId", userId);
                    col.AddWithValue("@TokenTypeId", tokenTypeId);
                }
                );
        }

        public void AddUserRole(int userId, int roleId)
        {
            string procName = "[dbo].[UserRoles_Insert]";

            _dataProvider.ExecuteNonQuery(procName
                , inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@UserId", userId);
                    col.AddWithValue("@RoleId", roleId);
                }
                );
        }

        public void ConfirmUser(string token, string email)
        {
            string procName = "[dbo].[Users_Confirm]";

            _dataProvider.ExecuteNonQuery(procName
                , inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Email", email);
                    col.AddWithValue("@Token", token);
                }
                );
        }

        public int GetUserFromToken(int tokenTypeId, string token)
        {
            int userId = 0;

            string procName = "[dbo].[UserTokens_Select_ByTokenAndTokenTypeId]";

            _dataProvider.ExecuteCmd(procName
                , inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@TokenTypeId", tokenTypeId);
                    col.AddWithValue("@Token", token);
                },
                delegate (IDataReader reader, short set)
                {
                    userId = reader.GetSafeInt32(0);
                }
                );
                return userId;
        }

        private static BaseUser MapSingleUser(IDataReader reader)
        {
            BaseUser aUser = new BaseUser();

            int startingIndex = 0;

            aUser.Id = reader.GetSafeInt32(startingIndex++);
            aUser.Email = reader.GetSafeString(startingIndex++);
            aUser.FirstName = reader.GetSafeString(startingIndex++);
            aUser.LastName = reader.GetSafeString(startingIndex++);
            aUser.Mi = reader.GetSafeString(startingIndex++);
            aUser.AvatarUrl = reader.GetSafeString(startingIndex++);
            return aUser;
        }

        private static void AddCommonParams(UserAddRequest model, SqlParameterCollection col, string password)
        {
            col.AddWithValue("@Email", model.Email);
            col.AddWithValue("@FirstName", model.FirstName == null ? DBNull.Value : model.FirstName);
            col.AddWithValue("@LastName", model.LastName == null ? DBNull.Value : model.LastName);
            col.AddWithValue("@Mi", model.Mi == null ? DBNull.Value : model.Mi);
            col.AddWithValue("@AvatarUrl", model.AvatarUrl == null ? DBNull.Value : model.AvatarUrl);
            col.AddWithValue("@Password", password);
        }
    }
}