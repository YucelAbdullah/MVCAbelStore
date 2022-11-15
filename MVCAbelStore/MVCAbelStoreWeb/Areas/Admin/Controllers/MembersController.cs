using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCAbelStoreData;
using System.Data;

namespace MVCAbelStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrators , ProductManagers")]

    public class MembersController : Controller
    {
        private readonly AppDbContext context;

        public MembersController(
            AppDbContext context
            )
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            var model = await context.Users.ToListAsync();
            return View();
        }
    }
}
