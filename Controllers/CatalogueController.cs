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
using Microsoft.AspNetCore.Authorization;

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
       
            return View(myBooks);
        }

        // GET: Reserve
        [Authorize(Roles= "Member")]
        public async Task<IActionResult> Reserve(int id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            //Catalogue c = await _context.catalogues.FirstOrDefaultAsync(p => p.bID == id);
            //List<Reservation> ReserveList = new List<Reservation>();
            var ReserveList = await _context.reservations.ToListAsync();
            Book b = await _context.BookTable.FirstOrDefaultAsync(p => p.BookID == id);

            Reservation reservation = new Reservation();
            reservation.borrower = person;
            reservation.DateReserved = DateTime.Now;
            reservation.ReadingOrder = 1;
            //reservation.book = b;
            //ReserveList.Add(reservation);
            b.ReserveList.Add(reservation);
            

            await _context.SaveChangesAsync();

            ViewBag.thisUser = userId;

            //return View();
            return RedirectToAction("Index");

        }
        // GET: Unreserve
        //public async Task<IActionResult> Unreserve(int? id)
        //{
        //    var userId = _userManager.GetUserId(HttpContext.User);
        //    ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var reservation = await _context.reservations
        //        //.FirstOrDefaultAsync(m => m.reservationID == id);
        //        .Where(m => m.borrower.Id == userId)
        //        .Where(p => p.book.BookID == id)
        //        .FirstAsync();

        //    if (reservation == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.reservations.Remove(reservation);
        //    await _context.SaveChangesAsync();

        //    ViewBag.thisUser = userId;

        //    return RedirectToAction("Index");

        //}

        public async Task<IActionResult> Index(string? srch)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            var myBooks = await _context.catalogues
            //.Where((b => b.inUse && (b.book.Title.Contains(srch) || b.book.Author.Contains(srch))))
            .Where(b => b.inUse)
            .Include(p => p.book)
                .ThenInclude(r => r.ReserveList)
            //.Include(f => f.Owner)
            //.GroupBy(b => b.book.BookID)
            //.Select(p => p.First())
            .OrderBy(g => g.book.Title)
            .ToListAsync();

            var reservations = await _context.reservations.ToListAsync();

            IEnumerable<Catalogue> filteredBooks = myBooks.Distinct();

            //if (!string.IsNullOrEmpty(srch))
            //{
            //    filteredBooks = myBooks.Where(b => b.book.Title.ToLower().Contains(srch.ToLower()) || b.book.Author.ToLower().Contains(srch.ToLower())).ToList();
            //}

            //var fb = filteredBooks.Select(b => b.book.BookID).Distinct().ToList();
            //var fb = filteredBooks.Select(b => b.book.BookID + " " + b.book.Title + " " + b.book.Author + " " + b.book.ReserveList.Count).Distinct().ToList();
            //var fb = filteredBooks.Select(b => new Catalogue{book = b.book, inUse = b.inUse, Owner = b.Owner, bID = b.bID,LoanList = b.LoanList}).Select(b => b.book.BookID).Distinct().ToList();
            
          

            //List<Catalogue> filteredBooks = new List<Catalogue>();
            //int i;
            //foreach (Catalogue c in myBooks)
            //{
            //    if (filteredBooks[i].book.BookID == c.book.BookID) 
            //    {

            //    }
            //}

            //14/11/21
            //CatalogueVM catalogueVM = new CatalogueVM() { Catalogue = filteredBooks, ReserveList = reservations};

            ViewBag.thisUser = userId;

            //return View(catalogueVM);
            return View(filteredBooks);

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

            var catalogue = await _context.catalogues.Include(b=> b.book).FirstOrDefaultAsync(b=> b.bID == id);
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
        public async Task<IActionResult> Edit(/*int id,*/ /*[Bind("bID,ISBN,Title,Author,Genres,AvgRating")]*/ Catalogue catalogue)
        {
            //if (id != catalogue.bID)
            //{
            //    return NotFound();
            //}

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
        //public ActionResult Editbook(int? id)
        //{
        //    return View();
        //}

        //// POST: CatalogueController/EditBook
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditBook(int id, [Bind("bID,inUse")] Catalogue catalogue)
        //{
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

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

        //Get
        public async Task<IActionResult> GetBookByISBN()
        {
            //if (srch == null)
            //{
            //    return RedirectToAction(nameof(Index));
            //}

            //var userId = _userManager.GetUserId(HttpContext.User);
            //ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            //Book myBook = await _context.BookTable
            //.FirstOrDefaultAsync(b=> b.ISBN.ToUpper() == srch.ToUpper());

            //if (myBook == null)
            //{
            //    return RedirectToAction("Create");
            //}
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetBookByISBN(string? srch)
        {
            if (srch == null)
            {
                return View();
            }

            ViewBag.searchtext = srch;

            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            Book myBook = await _context.BookTable
            .FirstOrDefaultAsync(b => b.ISBN.ToUpper() == srch.ToUpper());

            if (myBook == null)
            {
                return RedirectToAction("Create");
            }
            return View(myBook);
        }

        public async Task<IActionResult> AddCopy(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            Book myBook = await _context.BookTable
            .FirstOrDefaultAsync(b => b.BookID == id);

            if (myBook == null)
            {
                return RedirectToAction("Create");
            }
            await _context.catalogues.AddAsync(new Catalogue() { book = myBook, inUse = true, Owner = person });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
