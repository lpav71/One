using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using One.Data;
using One.Models;

namespace One.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly OneContext _context;

        public ProductsController(OneContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            List<Product> product = await _context.Product.ToListAsync();
            return new OkObjectResult(product);
        }

        // GET: api/Products/3
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // GET: api/Products/value/{value}
        [Route("/api/products/value/{value}")]
        [HttpGet]
        public ActionResult<Product> GetProduct(string value)
        {
            var product = _context.Product.Where(p => p.Value == value);
            int s = product.Count();

            if (s == 0)
            {
                return NotFound();
            }

            return new OkObjectResult(product);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(List<Dictionary<string, string>> products)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync($"TRUNCATE TABLE dbo.Product");

            SortedDictionary<int, string> sortProducts = new SortedDictionary<int, string>();
            for (int i = 0; i < products.Count; i++)
            {
                var d = products[i];
                sortProducts.Add(Convert.ToInt32(d.Keys.First()), d.Values.First());
            }
            foreach (KeyValuePair<int, string> s in sortProducts)
            {
                Product prod = new Product();
                prod.Code = s.Key;
                prod.Value = s.Value;
                _context.Product.Add(prod);
            }
            
            await _context.SaveChangesAsync();
            return new OkObjectResult(products);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
