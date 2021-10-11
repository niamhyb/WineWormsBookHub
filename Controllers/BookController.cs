using DomainModel.Data;
using DomainModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DomainModel.Controllers
{
   
    public class BookController : Controller
    {
        private readonly ILogger<BookController> _logger;
        private readonly DomainModelContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public BookController(DomainModelContext context, UserManager<ApplicationUser> userManager, ILogger<BookController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        // GET: BookController
        public async Task<ActionResult> Index()
        {
            //var userId = _userManager.GetUserId(HttpContext.User);
            //replace line above with line below
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            
            var myBooks = await _context.catalogues.Where(b => b.Owner == person)
                .Include(p => p.book)
                .ToListAsync();
            List<Book> booklist = new List<Book>();
            foreach (Catalogue c in myBooks)
            {
                booklist.Add(c.book);

            }
            return View(myBooks);
        }

        // GET: BookController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BookController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BookController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BookController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BookController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
