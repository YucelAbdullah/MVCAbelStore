using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MVCAbelStoreData;
using MVCAbelStoreWeb.Models;
using NETCore.MailKit.Core;
using Org.BouncyCastle.Asn1.Cms;
using System.Diagnostics;
using System.Security.Claims;

namespace MVCAbelStoreWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;
        private readonly AppDbContext context;

        public HomeController(
            ILogger<HomeController> logger,
            IEmailService emailService,
            IConfiguration configuration,
            AppDbContext context



            )

        {
            _logger = logger;
            this.emailService = emailService;
            this.configuration = configuration;
            this.context = context;
        }

        public async Task<IActionResult>  Index()
        {
            ViewBag.Featured = await context.Products.OrderBy(p => p.DiscountedPrice).Take(5).ToListAsync();
            ViewBag.BestSeller =await context.Products.OrderBy(p => p.OrderItems.Count).Take(5).ToListAsync();
            ViewBag.ShowCase = await context.Products.OrderBy(p => p.Id).Take(9).ToListAsync();
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ContactUs(ContactUsViewModel model)
        {
            await emailService.SendAsync(
                configuration.GetValue<string>("EMailSettings:SenderEmail"),
                $"Ziyaretçi Mesajı ({model.Name}",
                $" Gönderen:\t\t{model.Name}\n Telefon:\t\t{model.PhoneNumner ?? "Telefon Belirtilmemiş"}\n Mesaj:\t\t{model.Email}\n {model.Message}"
                );
            TempData["messageSend"] = true;
            return View(new ContactUsViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> Category(Guid id, int? page)

        {
            var model = await context.Categories.FindAsync(id);
            ViewBag.Page = page;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Product(Guid id)

        {
            var model = await context.Products.FindAsync(id);
            ViewBag.creditCards = new[]
            {
                new CreditCardViewModel
                {
                    Code="bonus",
                    Installments= new[]{
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1.03m, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1.04m, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1.06m, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1.09m, Exist=true},
                        new InstallmentViewModel{Rate=1.15m, Exist=true}
                     }.ToList()

                },

                new CreditCardViewModel
                {
                    Code="world",
                    Installments= new[]{
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1.03m, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1.04m, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1.06m, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1.09m, Exist=true},
                        new InstallmentViewModel{Rate=1.15m, Exist=true}
                     }.ToList()

                },

                new CreditCardViewModel
                {
                    Code="maximum",
                    Installments= new[]{
                        new InstallmentViewModel{Rate=1, Exist=true},
                        new InstallmentViewModel{Rate=1.03m, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=true},
                        new InstallmentViewModel{Rate=1.04m, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1.06m, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1.09m, Exist=true},
                        new InstallmentViewModel{Rate=1.15m, Exist=true}
                     }.ToList()

                },
                new CreditCardViewModel
                {
                    Code="axess",
                    Installments= new[]{
                        new InstallmentViewModel{Rate=1, Exist=true},
                        new InstallmentViewModel{Rate=1.03m, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=true},
                        new InstallmentViewModel{Rate=1.04m, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1.06m, Exist=true},
                        new InstallmentViewModel{Rate=1, Exist=false},
                        new InstallmentViewModel{Rate=1.09m, Exist=true},
                        new InstallmentViewModel{Rate=1.15m, Exist=true}
                     }.ToList()

                },
            };
            return View(model);
        }


        [HttpGet, Authorize]
        public async Task<IActionResult> AddToCart(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var product = await context.Products.FindAsync(id);
            var ShoppingCartItem = new ShoppingCartItem
            {
                ApplicationUserId = userId,
                Quantity = 1,
                ProductId = id,
                DateCreated = DateTime.Now,
                Enabled = true,

            };
            await context.ShoppingCartItems.AddAsync(ShoppingCartItem);

            await context.SaveChangesAsync();

            TempData["addedToCart"] = true;

            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpGet, Authorize]
        public async Task<IActionResult> RemoveFromCart(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var item = await context.
                ShoppingCartItems.
                OrderBy(p => p.DateCreated).LastOrDefaultAsync(p => p.ApplicationUserId == userId && p.ProductId == id);
            if (item == null)
            {
                return BadRequest();
            }
            context.ShoppingCartItems.Remove(item);
            await context.SaveChangesAsync();

            return RedirectToAction("ShoppingCart", "Account");
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> RemoveAllFromCart(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var items = await context.
                ShoppingCartItems.
                Where(p => p.ApplicationUserId == userId && p.ProductId == id).ToListAsync();
            if (items == null)
            {
                return BadRequest();
            }
            context.ShoppingCartItems.RemoveRange(items);
            await context.SaveChangesAsync();

            return RedirectToAction("ShoppingCart", "Account");
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> ClearCart()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var items = await context.
                ShoppingCartItems.
                Where(p => p.ApplicationUserId == userId).ToListAsync();
            if (items == null)
            {
                return BadRequest();
            }
            context.ShoppingCartItems.RemoveRange(items);
            await context.SaveChangesAsync();

            return RedirectToAction("ShoppingCart", "Account");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}