using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inventory.Models;
using Inventory.Models.ProductModel;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace Inventory.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Name,Description,Quantity,Price")] Product product, IFormFile imageFile)
        {
            // Remove ImageUrl from ModelState to prevent validation errors
            ModelState.Remove("ImageUrl");
            // Handle file upload
            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageUrl", "Just allow .jpg, .jpeg or .png files");
                }
            }
            else
                ModelState.AddModelError("ImageUrl", "Product image is required.");

            if (_context.Products.Any(p => p.Code == product.Code))
            {
                ModelState.AddModelError("Code", "A product with this code already exists.");
            }

            if (ModelState.IsValid)
            {
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                // Create unique filename
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                product.ImageUrl = "/images/products/" + fileName;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = product.Id });
            }
            return View(product);
        }

        // GET: Product/Edit/5
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

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Name,Description,Quantity,Price")] Product product, IFormFile imageFile)
        {
            // Remove ImageUrl from ModelState to prevent validation errors
            ModelState.Remove("ImageUrl");
            ModelState.Remove("imageFile");
            if (id != product.Id)
            {
                return NotFound();
            }
            // Get the existing product to preserve data not in the form
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Handle file upload
            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("imageUrl", "Just allow .jpg, .jpeg or .png files");
                }
            }

            if (string.IsNullOrWhiteSpace(product.Code))
            {
                ModelState.AddModelError("Code", "Code is required.");
            }
            else if (_context.Products.Any(p => p.Code == product.Code && p.Id != product.Id))
            {
                ModelState.AddModelError("Code", "Code must be unique.");
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                    // Create unique filename
                    var fileName = Guid.NewGuid().ToString() + fileExtension;
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Update the existing product with form data
                    existingProduct.ImageUrl = "/images/products/" + fileName;
                }
                existingProduct.Code = product.Code;
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Price = product.Price;


                try
                {
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
                return RedirectToAction(nameof(Details), new { id = product.Id });;
            }
            return View(existingProduct);
        }

        // GET: Product/Delete/5
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

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int productId, int amount)
        {
            if (amount <= 0)
            {
                return Json(new { success = false, message = "Amount must be greater than zero." });
            }
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }
            product.Quantity += amount;
            await _context.SaveChangesAsync();
            return Json(new { success = true, newQuantity = product.Quantity });
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int productId, int amount)
        {
            if (amount <= 0)
            {
                return Json(new { success = false, message = "Amount must be greater than zero." });
            }
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }
            if (product.Quantity < amount)
            {
                return Json(new { success = false, message = "Not enough quantity to decrease." });
            }
            product.Quantity -= amount;
            await _context.SaveChangesAsync();
            return Json(new { success = true, newQuantity = product.Quantity });
        }
    }
}
