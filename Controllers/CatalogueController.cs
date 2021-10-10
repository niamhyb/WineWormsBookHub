using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DomainModel.Data;
using DomainModel.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace DomainModel.Controllers
{
    public class CatalogueController : Controller
    {
        private readonly ILogger<CatalogueController> _logger;
        private readonly DomainModelContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CatalogueController(DomainModelContext context, UserManager<ApplicationUser> userManager, ILogger<CatalogueController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Catalogue
        public async Task<IActionResult> MyIndex()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            List<Catalogue> myBooks = new List<Catalogue>();

            
                myBooks = await _context.catalogues.Where(b => b.Owner == person)
                .Include(p => p.book)
                .ToListAsync();
            
            //List<Book> booklist = new List<Book>();
            //foreach (Catalogue c in myBooks)
            //{
            //    booklist.Add(c.book);

            //}
            return View(myBooks);
        }

        // GET: Reserve
        public async Task<IActionResult> Reserve(int id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            Catalogue c = await _context.catalogues.FirstOrDefaultAsync(p => p.bID == id);
            
            Reservation reservation = new Reservation();
            reservation.borrower = person;
            reservation.DateReserved = DateTime.Now;
            reservation.ReadingOrder = 1;
            c.ReserveList.Add(reservation);
            

            await _context.SaveChangesAsync();

            ViewBag.thisUser = userId;

            //return View();
            return RedirectToAction("Index", c);

        }
        // GET: Unreserve
        public async Task<IActionResult> Unreserve(int? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.reservations
                //.FirstOrDefaultAsync(m => m.reservationID == id);
                .Where(m => m.borrower.Id == userId)
                .Where(p => p.catalogue.bID == id)
                .FirstAsync();

            if (reservation == null)
            {
                return NotFound();
            }

            _context.reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            ViewBag.thisUser = userId;

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            var myBooks = await _context.catalogues
            .Where(b => b.inUse == true)
            .Include(p => p.book)
            .Include(f => f.Owner)
            .ToListAsync();

            ViewBag.thisUser = userId;


            return View(myBooks);
        }
        // GET: Catalogue/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogue = await _context.catalogues
                .FirstOrDefaultAsync(m => m.bID == id);
            if (catalogue == null)
            {
                return NotFound();
            }

            return View(catalogue);
        }


        // GET: Catalogue/Create (to return the view)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Catalogue/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);
            Catalogue catalogue = new Catalogue();
            catalogue.Owner = person;
            catalogue.book = book;
            

            if (ModelState.IsValid)
            {
                _context.Add(book);
                _context.Add(catalogue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(catalogue);
        }

        // GET: Catalogue/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
              return NotFound();
            }

            var catalogue = await _context.catalogues.FindAsync(id);
            if (catalogue == null)
            {
                return NotFound();
            }

            //Catalogue c = await _context.catalogues.FirstOrDefaultAsync(p => p.bID == id);
            if (catalogue.inUse == true)
            {
                catalogue.inUse = false;
            }
            else
            {
                catalogue.inUse = true;
            }

            _context.Update(catalogue);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyIndex");
            //return View(catalogue);
        }

        // POST: Catalogue/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("bID,inUse")] Catalogue catalogue)
        {
            if (id != catalogue.bID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catalogue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatalogueExists(catalogue.bID))
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
            return View(catalogue);
        }

        // GET: CatalogueController/Edit/7
        public ActionResult EditBook(int? id)
        {
            return View();
        }

        // POST: CatalogueController/EditBook
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBook(int id, [Bind("bID,inUse")] Catalogue catalogue)
        {
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Catalogue/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogue = await _context.catalogues
                .FirstOrDefaultAsync(m => m.bID == id);
            if (catalogue == null)
            {
                return NotFound();
            }

            return View(catalogue);
        }

        // POST: Catalogue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var catalogue = await _context.catalogues.FindAsync(id);
            _context.catalogues.Remove(catalogue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CatalogueExists(int id)
        {
            return _context.catalogues.Any(e => e.bID == id);
        }
    }
}
