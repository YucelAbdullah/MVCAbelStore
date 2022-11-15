using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MVCAbelStoreWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Administrators, ProductManagers")]

    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
