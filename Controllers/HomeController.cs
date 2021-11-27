using DomainModel.Data;
using DomainModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DomainModelContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(DomainModelContext context, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddBook()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);
            _context.Database.EnsureCreated();

            ApplicationUser bor = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Email.Contains("mur"));

            Book newbook = new Book() { BookID = 0, Title = "Braywatch", Author = "Ross O'Carroll-Kelly", ISBN = "1234567895643", AvgRating = 4, Genres = Genre.Comedy};
            await _context.BookTable.AddAsync(newbook);

            Catalogue cat = new Catalogue();
            cat.book = newbook;
            cat.Owner = bor;
            await _context.catalogues.AddAsync(cat);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> addTest()
        {
            Test mytest = new Test() { TestID = 0, FName = "Work!" };
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> seedDB()
        {
            var roleStore = new RoleStore<IdentityRole>(_context); //Pass the instance of your DbContext here
            var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
            }
            // creating Creating Member role
            if (!await roleManager.RoleExistsAsync("Member"))
            {
                await roleManager.CreateAsync(new IdentityRole("Member"));
            }

            //Here we create the Admin super user who will maintain the website
            var adm = new Administrator { };
            adm.UserName = "niamh.morrin@gmail.com";
            adm.Email = "niamh.morrin@gmail.com";
            adm.EmailConfirmed = true;
            string userPWD = "NiamhyB1!";
            var chkUser = await _userManager.CreateAsync(adm, userPWD);
            //Add default User to Role Admin
            if (chkUser.Succeeded)
            {
                var result1 = await _userManager.AddToRoleAsync(adm, "Administrator");
                var result2 = await _userManager.AddToRoleAsync(adm, "Member");
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
