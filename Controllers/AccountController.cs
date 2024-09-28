using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroFlex.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {

            Response.Cookies.Delete("AuthToken"); // e.g., JWT token cookie

            return Ok(new { message = "Logout successful" });
        }
    }
}
