using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataProtection.Web.Models;
using Microsoft.AspNetCore.DataProtection;

namespace DataProtection.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DenemeContext _context;
        private readonly IDataProtector _dataProtector;

        public ProductsController(DenemeContext context, IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            // diğer protector nesnelerini birbirinden ayırmak için uniqe name verilir. Şifrelemeyi uniqe name ile yapar
            _dataProtector = dataProtectionProvider.CreateProtector("ProductsControllerProtector");  
        }
         
        public async Task<IActionResult> Index()
        
        
        {
            var list = await _context.Products.ToListAsync();

            var timeLimitedProtector =  _dataProtector.ToTimeLimitedDataProtector();                    // süre koyabilmek için ToTimeLimitedDataProtector metodu gerekli

            ViewBag.RemainingTime = 30;

            list.ForEach(x =>
            {
                //x.EncryptedId = _dataProtector.Protect(x.Id.ToString());  // şifreledik
                x.EncryptedId = timeLimitedProtector.Protect(x.Id.ToString(),TimeSpan.FromSeconds(30));  // şifreledik 1 dk içinde çözülebilir. Aksi durumda şifreli data çözülemez 
            });

            return View(list);
        }
         
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeLimitedProtector = _dataProtector.ToTimeLimitedDataProtector();
            //int decryptedId = int.Parse(_dataProtector.Unprotect(id));   // şifreyi çözdük
            int decryptedId = int.Parse(timeLimitedProtector.Unprotect(id));   // şifreyi çözdük

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == decryptedId);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
         
        public IActionResult Create()
        {
            return View();
        }

  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Date,Category")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
         
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            } 
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Price,Date,Category")] Product product)
        {

            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            int decryptedId = int.Parse(_dataProtector.Unprotect(id));   // şifreyi çözdük
            product.Id = decryptedId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
