using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MVCAbelStoreData;
using MVCAbelStoreWeb.Models;
using NETCore.MailKit.Core;
using System.Security.Claims;


namespace MVCAbelStoreWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;
        private readonly AppDbContext context;

        public AccountController(

            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IWebHostEnvironment environment,
            IConfiguration configuration,
            AppDbContext context


            )
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.emailService = emailService;
            this.environment = environment;
            this.configuration = configuration;
            this.context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel { RememberMe = false });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError("", "Geçersiz kullanıcı girişi");
                return View(model);
            }
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel { });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel model)
        {

            var user = new ApplicationUser
            {
                Name = model.UserName,
                UserName = model.UserName,
                Gender = model.Gender,
                BirthDate = model.BirthDate,
                EmailConfirmed = false

            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Members");
                await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("FullName", user.Name));
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(ConfirmEMail), "Account", new { id = user.Id, token = token }, Request.Scheme);
                var body = string.Format(System.IO.File.ReadAllText(Path.Combine(environment.WebRootPath, "template", "EMailConfirmation.html")), model.Name, link);


                await emailService.SendAsync(
                    mailTo: model.UserName,
                    subject: "MVCAbelStore EPosta Doğrulama Mesajı",
                    message: body,
                    isHtml: true
                    );


                return RedirectToAction("Index", "Home");
            }
            else
            {
                result.Errors.ToList().ForEach
                    (p => ModelState.AddModelError("", p.Description));

                return View(model);
            }


        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEMail(Guid Id, string token)
        {

            var user = await userManager.FindByIdAsync(Id.ToString());

            if (user is not null)
            {
                var result = await userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)

                    if (configuration.GetValue<bool>("AutoLogin"))
                        await signInManager.SignInAsync(user, isPersistent: false);
                return View("EmailConfirmed");
            }

            return View("InvalidConfirmation");


        }

        [HttpPost]
        public async Task<IActionResult> Comment(Comment model)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var product = await context.Products.FindAsync(model.ProductId);
            model.DateCreated = DateTime.Now;
            model.Enabled = false;
            if (product is null)
                return BadRequest();
            model.ApplicationUserId = userId;
            await context.Comments.AddAsync(model);
            await context.SaveChangesAsync();
            return RedirectToRoute("product", new { id = model.ProductId, name = product.Name.ToSafeUrlString() });
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> ShoppingCart()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = await context.Users.FindAsync(userId);
            return View(model);
        }


    }
}
