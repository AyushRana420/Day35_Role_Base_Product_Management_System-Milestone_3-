using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Role_Base_Product_Management_System.Data;
using Role_Base_Product_Management_System.Models;

namespace Role_Base_Product_Management_System.Controllers
{
    [Authorize(Roles = "Admin,Manager")] // Ensure that only authenticated users can access actions in this controller
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IDataProtector _Protector;

        public ProductController(ApplicationDbContext db, IDataProtectionProvider provider)
        {
            _db = db;
            // Create a data protector with a specific purpose string
            _Protector = provider.CreateProtector("ProductIdProtector");
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _db.Products.ToListAsync();
            var vm = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = string.IsNullOrEmpty(p.EncryptedPrice) ? 0 : decimal.Parse(_Protector.Unprotect(p.EncryptedPrice))
            }).ToList();

            return View(vm);
        }

        // GET: /Product/Create
        [Authorize(Roles = "Admin")]//Only Admin can create products
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateProductModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var product = new Product
            {
                Name = model.Name,
                EncryptedPrice = _Protector.Protect(model.Price.ToString()!)
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Product \"{product.Name}\" has been successfully created!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Products/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();

            var model = new EditProductModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = decimal.Parse(_Protector.Unprotect(p.EncryptedPrice!))
            };
            return View(model);
        }

        // POST: /Products/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(EditProductModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var p = await _db.Products.FindAsync(model.Id);
            if (p == null) return NotFound();

            p.Name = model.Name;
            p.EncryptedPrice = _Protector.Protect(model.Price.ToString()!);
            _db.Products.Update(p);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Product \"{p.Name}\" has been successfully updated!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Products/Delete/5
        [Authorize(Roles = "Admin")] //Only Admin can delete products
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();

            var vm = new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = decimal.Parse(_Protector.Unprotect(p.EncryptedPrice!))
            };
            return View(vm);
        }

        // POST: /Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Explicitly check ModelState to satisfy analyzers
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();

            _db.Products.Remove(p);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Product \"{p.Name}\" has been successfully deleted!";
            return RedirectToAction(nameof(Index));
        }

    }
}