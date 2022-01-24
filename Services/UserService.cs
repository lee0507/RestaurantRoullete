using Microsoft.AspNetCore.Mvc;
using Swilago.Data;
using Swilago.Interfaces;
using Swilago.Models;

namespace Swilago.Services
{
    public class UserService : ControllerBase, IUserService
    {
        private readonly IConfiguration _config;
        private readonly BreakContext _context;

        public UserService(IConfiguration config, BreakContext context)
        {
            _config = config;
            _context = context;
        }

        public IActionResult SetAccessUserInfo(UserAccessInfo info)
        {
            if (info == null)
                return BadRequest();

            

            return NoContent();
        }
    }
}
