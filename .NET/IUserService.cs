using Sabio.Models;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public interface IUserService
    {
        Task<bool> LogInAsync(string email, string password);

        Task<bool> LogInTest(string email, string password, int id, string[] roles = null);
        
        int Create(UserAddRequest model, int statusTypeId);

        int CreateInvitedMember(UserAddRequest model, int statusTypeId);

        IUserAuthData GetCurrent(string email, string password);

        public BaseUser GetById(int id);

        int GetIdByEmail(string email);

        void AddUserToken(string token, int userId, int tokenTypeId);

        void AddUserOrgAndRole(int userId, int roleId, int orgId);

        void ConfirmUser(string token, string email);

        int GetUserFromToken(int tokenTypeId, string token);
        UserStatus GetUserStatusTotals(int id);
        Task<bool> ChangeCurrentOrg(IUserAuthData currentUser, int orgId);
    }
}