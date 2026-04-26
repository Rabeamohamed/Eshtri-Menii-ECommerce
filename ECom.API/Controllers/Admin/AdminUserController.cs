using AutoMapper;
using ECom.Application.DTO.Admin.User;
using ECom.Application.Interfaces;
using ECom.Application.Services.Admin;
using ECom.Application.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace ECom.API.Controllers.Admin
{
    public class AdminUserController : AdminBaseController
    {
        private readonly IAdminUserService _userService;

        public AdminUserController(
            IUnitOfWork work,
            IMapper mapper,
            IAdminUserService userService)
            : base(work, mapper)
        {
            _userService = userService;
        }

        // GET: api/admin/adminuser/get-all
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // GET: api/admin/adminuser/get-by-id/{userId}
        [HttpGet("get-by-id/{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user is null)
                    return NotFound(new ResponseAPI(404, "User not found"));
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // PUT: api/admin/adminuser/block
        [HttpPut("block")]
        public async Task<IActionResult> BlockUser([FromBody] BlockUserDto dto)
        {
            try
            {
                var result = await _userService.BlockUserAsync(dto);
                return result.StatusCode switch
                {
                    200 => Ok(result),
                    404 => NotFound(result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // PUT: api/admin/adminuser/unblock/{userId}
        [HttpPut("unblock/{userId}")]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            try
            {
                var result = await _userService.UnblockUserAsync(userId);
                return result.StatusCode switch
                {
                    200 => Ok(result),
                    404 => NotFound(result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // POST: api/admin/adminuser/assign-role
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            try
            {
                var result = await _userService.AssignRoleAsync(dto);
                return result.StatusCode switch
                {
                    200 => Ok(result),
                    404 => NotFound(result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // DELETE: api/admin/adminuser/remove-role
        [HttpDelete("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDto dto)
        {
            try
            {
                var result = await _userService.RemoveRoleAsync(dto);
                return result.StatusCode switch
                {
                    200 => Ok(result),
                    404 => NotFound(result),
                    _ => BadRequest(result)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }

        // GET: api/admin/adminuser/roles
        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _userService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
        }
    }
}