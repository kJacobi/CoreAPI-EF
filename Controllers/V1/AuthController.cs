using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreAPI_EF.Contracts.V1;
using CoreAPI_EF.Contracts.V1.Requests;
using CoreAPI_EF.Contracts.V1.Responses;
using CoreAPI_EF.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoreAPI_EF.Controllers.V1
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost(ApiRoutes.Auth.Register)]
        public async Task<IActionResult> Register([FromBody] Req_Register request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Res_Auth
                {
                    Success = false,
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var registerResponse = await _authService.RegisterAsync(request.Username, request.Password, request.Email, request.Role);

            if (!registerResponse.Success)
            {
                return BadRequest(new Res_Auth
                {
                    Success = registerResponse.Success,
                    Errors = registerResponse.Errors
                });
            }

            return Ok(new Res_Auth
            {
                Success = registerResponse.Success
            });
        }


        [HttpPost(ApiRoutes.Auth.Login)]
        public async Task<IActionResult> Login([FromBody] Req_Login request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Res_Auth
                {
                    Success = false,
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var loginResponse = await _authService.LoginAsync(request.Username, request.Password);

            if (!loginResponse.Success)
            {
                return BadRequest(new Res_Auth
                {
                    Success = loginResponse.Success,
                    Errors = loginResponse.Errors
                });
            }

            return Ok(new Res_Token
            {
                Token = loginResponse.Token,
                RefreshToken = loginResponse.RefreshToken,
                Expires = loginResponse.Expires
            });
        }

        [HttpPost(ApiRoutes.Auth.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] Req_Refresh request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Res_Auth
                {
                    Success = false,
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var loginResponse = await _authService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!loginResponse.Success)
            {
                return BadRequest(new Res_Auth
                {
                    Success = loginResponse.Success,
                    Errors = loginResponse.Errors
                });
            }

            return Ok(new Res_Token
            {
                Token = loginResponse.Token,
                RefreshToken = loginResponse.RefreshToken,
                Expires = loginResponse.Expires
            });
        }
    }
}