using Microsoft.AspNetCore.Mvc;
using Swilago.Models;

namespace Swilago.Interfaces
{
    public interface IUserService
    {
        IActionResult SetAccessUserInfo(UserAccessInfo info);
    }
}
