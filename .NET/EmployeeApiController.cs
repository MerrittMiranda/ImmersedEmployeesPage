using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Sabio.Models.Domain.Employees;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System.Data.SqlClient;
using System;
using Sabio.Models;
using Sabio.Models.Requests.Employees;
using Microsoft.AspNetCore.Authorization;
using Sabio.Models.Requests;
using Sabio.Models.Enums;
using Sabio.Services.Interfaces;
using Sabio.Models.Requests.InviteMembers;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeApiController : BaseApiController
    {
        private IEmployeeService _employeeService = null;
        private IAuthenticationService<int> _authService = null;
        private IUserService _userService = null;

        public EmployeeApiController(IEmployeeService service
            , IUserService userService
            , ILogger<EmployeeApiController> logger
            , IAuthenticationService<int> authService) : base(logger)
        {
            _employeeService = service;
            _authService = authService;
            _userService = userService;
        }


        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Employee>> GetById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Employee employee = _employeeService.GetEmployee(id);

                if (employee == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Employee> { Item = employee };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }

            return StatusCode(code, response);
        }


        [HttpGet("organization/{orgsId:int}")]
        public ActionResult<ItemResponse<Paged<Employee>>> GetPaginatedByOrgsId(int orgsId, int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Employee> page = _employeeService.GetPaginatedOrgs(orgsId, pageIndex, pageSize);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Employee>> { Item = page };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }


        [HttpGet("organization/{orgsId:int}/search")]
        public ActionResult<ItemResponse<Paged<Employee>>> SearchPaginated(int pageIndex, int pageSize, int orgsId, string query)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Employee> page = _employeeService.SearchPaginated(pageIndex, pageSize, orgsId, query);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Employee>> { Item = page };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }


        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(EmployeeAddRequest model)
        {
            ObjectResult result = null;
            int currentUserId = 0;
            int id = 0;

            try
            {
                currentUserId = _authService.GetCurrentUserId();
                id = _employeeService.Add(model, currentUserId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);


            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }

            return result;
        }

        [HttpPost("invitemember")]
        public ActionResult<ItemResponse<int>> InsertMember(InviteMembersAddRequest model)
        {
            ObjectResult result = null;
            int currentUserId = 0;
            int id = 0;

            try
            {
                currentUserId = _authService.GetCurrentUserId();
                int userRoleTypeId = (int)Roles.Customer;
                string token = Guid.NewGuid().ToString();
                int tokenTypeId = (int)TokenType.EmployeeInvite;

                id = _employeeService.InsertMember(model, userRoleTypeId, tokenTypeId, token, currentUserId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);

            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }

            return result;
        }


        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(EmployeeUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            int currentUserId = _authService.GetCurrentUserId();

            try
            {
                _employeeService.Update(model, currentUserId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }


        [HttpPut("terminate/{id:int}")]
        public ActionResult<SuccessResponse> Terminate(int id)
        {
            int code = 200;
            BaseResponse response = null;
            int currentUserId = _authService.GetCurrentUserId();

            try
            {
                _employeeService.Terminate(id, currentUserId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }
    }
}

