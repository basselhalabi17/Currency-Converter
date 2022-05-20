using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CurrencyConverter.Controllers
{
    public class clientdata
    {
        public string Email { get; set; }
        public string password { get; set; }
    }
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(string email, string Password)
        {
            if (email!=null && Password!=null)
            {
                var result = await signInManager.PasswordSignInAsync(
                   email, Password, false, false);

                if (result.Succeeded)
                {
                    return StatusCode(200, new message { msg = "Login Successful" });
                }

                return StatusCode(404, new message { msg = "Login Attempt Failed" });
            }

            return StatusCode(404, new message { msg = "Some fields are required" });
        }
       
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(string email, string Password, string ConfirmPassword)
        {
            if (email != null && Password != null && ConfirmPassword != null)
            {
                if (Password == ConfirmPassword)
                {
                    // Copy data from RegisterViewModel to IdentityUser
                    var user = new IdentityUser
                    {
                        UserName = email,
                        Email = email
                    };

                    // Store user data in AspNetUsers database table
                    var result = await userManager.CreateAsync(user, Password);

                    // If user is successfully created, sign-in the user using
                    // SignInManager and redirect to index action of HomeController
                    if (result.Succeeded)
                    {
                        clientdata f = new clientdata
                        {
                            Email = email,
                            password = Password
                        };
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return new OkObjectResult(f);
                    }
                }

                return StatusCode(404, new message { msg = "Passwords don't match" });
            }
            else
            {
                return StatusCode(404, new message { msg = "Some fields are required" });
            }
        }
    }
}
