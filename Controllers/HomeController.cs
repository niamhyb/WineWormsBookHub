using DomainModel.Data;
using DomainModel.Models;
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

        
        public async Task<IActionResult> AddBook()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            Member person = (Member)await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);
            _context.Database.EnsureCreated();

            Book newbook = new Book() { BookID = 0, Title = "Treasure Island", Author = "R.L. Stevenson", ISBN = "2234567891235", AvgRating = 5, Genres = Genre.Adventure };
            await _context.BookTable.AddAsync(newbook);

            Reservation r = new Reservation() { borrower = person, DateReserved = DateTime.Now, ReadingOrder = 1, reservationID = 0 };
            await _context.reservations.AddAsync(r);

            Loan l = new Loan() { borrower = person, DateLoaned = DateTime.Now, DateReturned = DateTime.Now, loanID = 0 };
            await _context.loans.AddAsync(l);

            Catalogue cat = new Catalogue();
            cat.book = newbook;
            cat.Owner = (Member)person;
            cat.ReserveList.Add(r);
            cat.LoanList.Add(l);
            await _context.catalogues.AddAsync(cat);

            //person.books.Add(
            //    new Book() {Title="Treasure Island", Author="R.L. Stevenson", ISBN = "1234567891234" });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> addTest()
        {
            Test mytest = new Test() { TestID = 0, FName = "Work!" };
            //await _context.tests.AddAsync(mytest);
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
