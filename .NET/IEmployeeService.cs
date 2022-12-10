using Sabio.Models;
using Sabio.Models.Domain.Employees;
using Sabio.Models.Requests.Employees;
using Sabio.Models.Requests.InviteMembers;

namespace Sabio.Services.Interfaces
{
    public interface IEmployeeService
    {
        int Add(EmployeeAddRequest model, int currentUserId);
        int InsertMember(InviteMembersAddRequest model, int userRoleTypeId, int tokenTypeId, string token, int currentUserId);
        Employee GetEmployee(int id);
        Paged<Employee> GetPaginatedOrgs(int id, int pageIndex, int pageSize);
        Paged<Employee> SearchPaginated(int pageIndex, int pageSize, int orgsId, string query);
        void Terminate(int id, int currentUserId);
        void Update(EmployeeUpdateRequest model, int currentUserId);
    }
}