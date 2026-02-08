using ECom.Core.DTO;
using ECom.Core.DTO.Auth;
using ECom.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Infrastructure.Repositories
{
    public class AuthRepository: IAuth
    {
        private readonly UserManager<AppUser> _userManager;
        public AuthRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
         
            if(registerDto == null)
            {
                return null;
            }
            if(await _userManager.FindByNameAsync(registerDto.UserName) is not null)
            {
                return "This Username already Registered";
            }

            if (await _userManager.FindByEmailAsync(registerDto.Email) is not null)
            {
                return "This Email already Registered";
            }

            AppUser user = new  ()
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
            };
             var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded is not true)
            {
                return result.Errors.ToList()[0].Description;
            }

            return "User Registered Successfully";
            
        }
    }
}
