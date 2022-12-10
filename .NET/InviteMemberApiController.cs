using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NuGet.Protocol;
using Sabio.Models.Domain.InviteMembers;
using Sabio.Models.Enums;
using Sabio.Models.Requests;
using Sabio.Models.Requests.InviteMembers;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/invitemembers")]
    [ApiController]
    public class InviteMemberApiController : BaseApiController
    {
        private IInviteMemberService _inviteMemberService = null;
        private IUserService _userService = null;
        private IEmployeeService _employeeService = null;
        private IAuthenticationService<int> _authService = null;

        public InviteMemberApiController(IInviteMemberService service
            , IUserService userService
            , IEmployeeService employeeService
            , IAuthenticationService<int> authService
            , ILogger<InviteMemberApiController> logger) : base(logger)
        {
            _inviteMemberService = service;
            _userService = userService;
            _employeeService = employeeService;
            _authService = authService;
        }

        [HttpGet("token")]
        public ActionResult<ItemResponse<InviteMember>> GetByToken(string token)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                InviteMember member = _inviteMemberService.GetByToken(token);

                if(member == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {
                    response = new ItemResponse<InviteMember> { Item = member };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<int>> SignUpMember(UserEmployeeAddRequest model)
        {
            ObjectResult result = null;
            int userId = 0;
            int employeeId = 0;
            int currentUserId = 0;

            try
            {
                userId = _userService.CreateInvitedMember(model.User, (int)StatusTypes.Active);
                model.Employee.UserId = userId;
                currentUserId = _authService.GetCurrentUserId();
                int customerRoleId = (int)Roles.Customer;
                int customerOrgId = model.Employee.OrganizationId;
                _userService.AddUserOrgAndRole(userId, customerRoleId, customerOrgId);

                employeeId = _employeeService.Add(model.Employee, currentUserId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = userId };
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


    }
}