using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain.Employees;
using Sabio.Models.Requests.Employees;
using Sabio.Models.Requests.InviteMembers;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class EmployeeService : IEmployeeService
    {

        IDataProvider _data = null;

        public EmployeeService(IDataProvider data)
        {
            _data = data;
        }

        public void Terminate(int id, int currentUserId)

        {
            string procName = "[dbo].[Employees_Terminate_ById]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", id);
                collection.AddWithValue("@ModifiedBy", currentUserId);
            },
            returnParameters: null);
        }

        public void Update(EmployeeUpdateRequest model, int currentUserId)

        {
            string procName = "[dbo].[Employees_Update]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", model.Id);
                collection.AddWithValue("@Phone", model.Phone);
                collection.AddWithValue("@StartDate", model.StartDate);
                collection.AddWithValue("@ModifiedBy", currentUserId);
            },
            returnParameters: null);
        }

        public int Add(EmployeeAddRequest model, int currentUserId)
        
        {
            int id = 0;

            string procName = "[dbo].[Employees_Insert]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
               
                collection.AddWithValue("@UserId", model.UserId);
                collection.AddWithValue("@OrganizationId", model.OrganizationId);
                collection.AddWithValue("@Phone", model.Phone);
                collection.AddWithValue("@StartDate", model.StartDate);
                collection.AddWithValue("@CreatedBy", currentUserId);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);

                idOut.Direction = ParameterDirection.Output;

                collection.Add(idOut);
            },
            returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object objectId = returnCollection["@Id"].Value;

                Int32.TryParse(objectId.ToString(), out id);
            });

            return id;
        }

        public int InsertMember(InviteMembersAddRequest model, int userRoleTypeId, int tokenTypeId, string token, int currentUserId)
        {
            int id = 0;

            string procName = "[dbo].[InviteMembers_Insert]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@FirstName", model.FirstName);
                collection.AddWithValue("@LastName", model.LastName);
                collection.AddWithValue("@Email", model.Email);
                collection.AddWithValue("@UserRoleTypeId", userRoleTypeId);
                collection.AddWithValue("@OrganizationId", model.OrganizationId);
                collection.AddWithValue("@TokenTypeId", tokenTypeId);
                collection.AddWithValue("@Token", token);
                collection.AddWithValue("@CreatedBy", currentUserId);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);

                idOut.Direction = ParameterDirection.Output;

                collection.Add(idOut);
            },
            returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object objectId = returnCollection["@Id"].Value;

                Int32.TryParse(objectId.ToString(), out id);
            });

            return id;
        }

        public Employee GetEmployee(int id)

        {
            string procName = "[dbo].[Employees_Select_ByIdV2]";

            Employee employee = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            },
            delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                employee = MapSingleEmployee(reader, ref startingIndex);
            });

            return employee;
        }

        public Paged<Employee> GetPaginatedOrgs(int id, int pageIndex, int pageSize)

        {
            Paged<Employee> pagedList = null;

            List<Employee> list = null;

            string procName = "[dbo].[Employees_Select_ByOrganizationIdV2]";

            int totalCount = 0;

            _data.ExecuteCmd(procName, (param) =>
            {
                param.AddWithValue("@OrganizationId", id);
                param.AddWithValue("@PageIndex", pageIndex);
                param.AddWithValue("@PageSize", pageSize);
            },
            (reader, recordSetIndex) =>
            {
                int startingIndex = 0;
                Employee employee = MapSingleEmployee(reader, ref startingIndex);
                totalCount = reader.GetSafeInt32(startingIndex++);

                if (list == null)
                {
                    list = new List<Employee>();
                }

                list.Add(employee);
            });

            if (list != null)
            {
                pagedList = new Paged<Employee>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;


        }

        public Paged<Employee> SearchPaginated(int pageIndex, int pageSize, int orgsId, string query)
        {
            Paged<Employee> pagedList = null;

            List<Employee> list = null;

            string procName = "[dbo].[Employees_SearchV2]";

            int totalCount = 0;

            _data.ExecuteCmd(procName, (param) =>
            {
                param.AddWithValue("@PageIndex", pageIndex);
                param.AddWithValue("@PageSize", pageSize);
                param.AddWithValue("@OrganizationId", orgsId);
                param.AddWithValue("@Query", query);
            },
            (reader, recordSetIndex) =>
            {
                int startingIndex = 0;
                Employee employee = MapSingleEmployee(reader, ref startingIndex);
                totalCount = reader.GetSafeInt32(startingIndex++);

                if (list == null)
                {
                    list = new List<Employee>();
                }

                list.Add(employee);
            });

            if (list != null)
            {
                pagedList = new Paged<Employee>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }

        private static Employee MapSingleEmployee(IDataReader reader, ref int startingIndex)
        {
            Employee anEmployee = new Employee();
            anEmployee.Organization = new BaseOrganization();

            anEmployee.EmployeeId = reader.GetSafeInt32(startingIndex++);
            anEmployee.UserId = reader.GetSafeInt32(startingIndex++);
            anEmployee.FirstName = reader.GetSafeString(startingIndex++);
            anEmployee.LastName = reader.GetSafeString(startingIndex++);
            anEmployee.Mi = reader.GetSafeString(startingIndex++);
            anEmployee.AvatarUrl = reader.GetSafeString(startingIndex++);
            anEmployee.Email = reader.GetSafeString(startingIndex++);
            anEmployee.Phone = reader.GetSafeString(startingIndex++);
            anEmployee.IsActive = reader.GetSafeBool(startingIndex++);
            anEmployee.Organization.Id = reader.GetSafeInt32(startingIndex++);
            anEmployee.Organization.Name = reader.GetSafeString(startingIndex++);
            anEmployee.Organization.LogoUrl = reader.GetSafeString(startingIndex++);
            anEmployee.Organization.BusinessPhone = reader.GetSafeString(startingIndex++);
            anEmployee.Organization.SiteUrl = reader.GetSafeString(startingIndex++);
            anEmployee.StartDate = reader.GetSafeDateTime(startingIndex++);
            anEmployee.EndDate = reader.GetSafeDateTime(startingIndex++);
            anEmployee.DateCreated = reader.GetSafeDateTime(startingIndex++);
            anEmployee.DateModified = reader.GetSafeDateTime(startingIndex++);

            return anEmployee;
        }


    }
}