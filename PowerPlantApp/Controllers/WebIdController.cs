using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerPlantApp.Context;
using PowerPlantApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;  // EPPlus için gerekli
using System.IO;      // Bellek akışı için gerekli


namespace PowerPlantApp.Controllers
{
    public class WebIdController : Controller
    {
        private readonly PowerPlantContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WebIdController(PowerPlantContext context , UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult ExportToExcel()
        {
            var powerPlants = _context.PowerPlants.ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Güç Santralleri");
                
                //başlık
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Ad";
                worksheet.Cells[1, 3].Value = "Açıklama";
                worksheet.Cells[1, 4].Value = "Tip";
                
                //row 2 vericez
                var row = 2;
                foreach (var plant in powerPlants)
                {
                    worksheet.Cells[row, 1].Value = plant.Id;
                    worksheet.Cells[row, 2].Value = plant.Name;
                    worksheet.Cells[row, 3].Value = plant.Description;
                    worksheet.Cells[row, 4].Value = plant.Type;
                    row++;
                    
                }
                
                // Dosyayı geri döndürme
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                
                string excelName = $"GucSantralleri-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);

            }
        }



        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile excelFile)
        {
            if (excelFile != null && excelFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // 2'den başlıyor çünkü ilk satır başlık
                        {
                            var powerPlant = new PowerPlant
                            {
                                Name = worksheet.Cells[row, 1].Value?.ToString(),
                                Description = worksheet.Cells[row, 2].Value?.ToString(),
                                Type = worksheet.Cells[row, 3].Value?.ToString()
                            };

                            _context.PowerPlants.Add(powerPlant);
                        }

                        await _context.SaveChangesAsync(); 
                    }
                }
            }

            return RedirectToAction("Index"); 
        }

      

        // email iletme metodu
        private async Task SetUserEmailInViewBag()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.UserEmail = user.Email;
            }
        }
        
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> List()
        { 
            await SetUserEmailInViewBag();
            var powerPlants = _context.PowerPlants.ToList();
            return View(powerPlants);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            await SetUserEmailInViewBag();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(PowerPlant powerPlant)
        {
            await SetUserEmailInViewBag();
            if (ModelState.IsValid)
            {
                _context.PowerPlants.Add(powerPlant);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(powerPlant);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            
            var powerPlant = _context.PowerPlants.Find(id);
            if (powerPlant == null)
            {
                return NotFound();
            }
            return View(powerPlant);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var powerPlant = _context.PowerPlants.Find(id);
            if (powerPlant != null)
            {
                _context.PowerPlants.Remove(powerPlant);
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            await SetUserEmailInViewBag();
            var powerPlant = _context.PowerPlants.Find(id);
            if (powerPlant == null)
            {
                return NotFound();
            }
            return View(powerPlant);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(PowerPlant powerPlant)
        {
            await SetUserEmailInViewBag();
            if (ModelState.IsValid)
            {
                _context.Entry(powerPlant).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            return View(powerPlant);
        }
    }
}
