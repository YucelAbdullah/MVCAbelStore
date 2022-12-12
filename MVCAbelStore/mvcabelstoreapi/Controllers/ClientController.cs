using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCAbelStoreData;
using X.PagedList;

namespace mvcabelstoreapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext context;

        public ClientController
            (
            AppDbContext context
            )
        {
            this.context = context;
        }

        [HttpGet("rayons")]
        public async Task<IActionResult> GetRayons()
        {
            var model = await context.Rayons.Where(p => p.Enabled).Select(p => new { p.Id, p.Name }).ToListAsync();
            return Ok(model);
        }

        [HttpGet("rayons/{id}")]
        public async Task<IActionResult> GetRayon(Guid id)
        {
            var model = await context.Rayons
                .Where(p => p.Enabled).Select(p => new
                {
                    p.Id,
                    p.Name,
                    categories = p.Categories.Select(c => new { c.Id, c.Name, c.Products.Count })
                .ToListAsync()
                }).SingleOrDefaultAsync(p => p.Id == id);
            if (model == null)
            {
                return BadRequest(id);
            }
            return Ok(model);
        }

        [HttpGet("category/{id}/{page?}")]
        public async Task<IActionResult> GetCategory(Guid id, int? page)
        {
            var model = await context.Categories.Where(p => p.Enabled)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    RayonName = p.Rayon!.Name,
                    Product = p.Products.Where(c=>c.Enabled).Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Price,
                        c.DiscountedPrice,
                        c.DiscountRate
                    })
                    .Skip(((page ?? 1)-1)*10).Take(10)
                }).SingleOrDefaultAsync(p=>p.Id==id);
            if (model == null)
            {
                return BadRequest(id);
            }
            return Ok(model);
        }

    }
}
