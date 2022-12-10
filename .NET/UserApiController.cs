using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Users;
using Sabio.Models.Enums;
using Sabio.Models.Requests;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserApiController : BaseApiController
    {
        private IUserService _userService = null;
        private IAuthenticationService<int> _authService = null;
        private IEmailsService _emailsService = null;
        public UserApiController(IEmailsService emailsService, IUserService service, ILogger<UserApiController> logger, IAuthenticationService<int> authenticationService) : base(logger)
        {
            _userService = service;
            _emailsService = emailsService;
            _authService = authenticationService;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<ItemResponse<int>> Create(UserAddRequest model)
        {
            int iCode = 201;
            BaseResponse response = null;

            try
            {
                int statusTypeId = (int)StatusTypes.Active;
                int id = _userService.Create(model, statusTypeId);

                if (id > 0)
                {
                    string email = model.Email;
                    int tokenTypeId = (int)TokenType.NewUser;
                    string token = Guid.NewGuid().ToString();
                    _userService.AddUserToken(token, id, tokenTypeId);
                    _emailsService.SendConfirmEmail(token, email);
                    int customerRoleId = (int)Roles.Customer;
                    int customerOrgId = 100;//100 is orgId of 'Immersed'
                    _userService.AddUserOrgAndRole(id, customerRoleId, customerOrgId);
                }
                if (id == 0)
                {
                    iCode = 404;
                    response = new ErrorResponse("Application resource not found");
                }
                else
                {
                    response = new ItemResponse<int> { Item = id };
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Message.Contains("Cannot insert duplicate key"))
                {
                    iCode = 400;
                    response = new ErrorResponse("Email exists");
                }
                else
                {
                    iCode = 500;
                    response = new ErrorResponse(sqlEx.Message);
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }
            return StatusCode(iCode, response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<SuccessResponse>> Login(UserLoginRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            bool isValid = false;

            try
            {
                isValid = await _userService.LogInAsync(model.Email, model.Password);

                if (isValid)
                {
                    response = new SuccessResponse();
                }
                else
                {
                    code = 404;
                    response = new ErrorResponse("Credentials do not match anything in database");
                }
            }
            catch (Exception ex)
            {
                code = 500;
                Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("current")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<IUserAuthData>> GetCurrent()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                IUserAuthData user = _authService.GetCurrentUser();

                if (user == null)
                {
                    code = 404;
                    response = new ErrorResponse("No current user found");
                }
                else
                {
                    response = new ItemResponse<IUserAuthData> { Item = user };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("logout")]
        public async Task<ActionResult<SuccessResponse>> LogoutAsync()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                await _authService.LogOutAsync();
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<BaseUser>> GetById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                BaseUser user = _userService.GetById(id);

                if (user == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<BaseUser> { Item = user };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("email")]
        public ActionResult<ItemResponse<BaseUser>> GetIdByEmail(string address)
        {
            int code = 200;
            int id = 0;
            BaseResponse response = null;

            try
            {
                id = _userService.GetIdByEmail(address);
                BaseUser user = _userService.GetById(id);

                if (user == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<BaseUser> { Item = user };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPut("confirm")]
        [AllowAnonymous]
        public ActionResult<SuccessResponse> Confirm(string token, string email)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                _userService.ConfirmUser(token, email);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("userFromToken")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<int>> GetUserFromToken(int tokenTypeId, string token)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int id = _userService.GetUserFromToken(tokenTypeId, token);

                if (id == 0)
                {
                    code = 404;
                    response = new ErrorResponse("UserId not found");
                }
                else
                {
                    response = new ItemResponse<int> { Item = id };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpPost("changeorg")]
        public async Task<ActionResult<SuccessResponse>> ChangeOrg(int orgId)
        {
            int code = 200;
            BaseResponse response = null;
            bool isSuccessful = false;

            try
            {
                IUserAuthData currentUser = _authService.GetCurrentUser();
                isSuccessful = await _userService.ChangeCurrentOrg(currentUser, orgId);
                if (isSuccessful)
                {
                    response = new SuccessResponse();
                }
                else
                {
                    code = 404;
                    response = new ErrorResponse("Organization could not be changed");
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

        [HttpGet("statusTotals")]
        public ActionResult<ItemResponse<UserStatus>> GetAllUserStatus()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int id = _authService.GetCurrentUserId();
                UserStatus user = _userService.GetUserStatusTotals(id);

                if (user == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application resource not found.");
                }
                else
                {
                    response = new ItemResponse<UserStatus> { Item = user };
                }
            }
            catch(Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }
    }
}
