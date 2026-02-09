using AutoMapper;
using ECom.API.Helper;
using ECom.Core.DTO.Auth;
using ECom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await work.AuthRepository.RegisterAsync(registerDto);
            if (result != "User Registered Successfully")
            {
                return BadRequest(new ResponseAPI(400,result));
            }
                return Ok(new ResponseAPI(200, result));
        }
    }
}
