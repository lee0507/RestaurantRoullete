using Microsoft.AspNetCore.Mvc;
using Swilago.Interfaces;
using Swilago.Models;

namespace Swilago.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        // 로그인 정보Token 받기 : /User/PostUserAccessInfo
        [HttpPost]
        [Route("PostUserAccessInfo")]
        public IActionResult SetAccessUserInfo([FromBody] UserAccessInfo info)
        {
            return _service.SetAccessUserInfo(info);
        }
    }
}
