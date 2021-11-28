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
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore;
using System.IO;
using MimeKit;
using Microsoft.AspNetCore.Hosting;

namespace DomainModel.Controllers
{
    public class CatalogueController : Controller
    {
        private readonly ILogger<CatalogueController> _logger;
        private readonly DomainModelContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CatalogueController(DomainModelContext context, UserManager<ApplicationUser> userManager, ILogger<CatalogueController> logger, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Catalogue
        public async Task<IActionResult> MyIndex()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            List<Catalogue> myBooks = new List<Catalogue>();

            
                myBooks = await _context.catalogues.Where(b => b.Owner == person)
                .Include(p => p.book)
                .Include(p => p.LoanList)
                    .ThenInclude(p => p.borrower)
                .ToListAsync();
       
            return View(myBooks);
        }

        // GET: Reserve
        [Authorize(Roles= "Member")]
        public async Task<IActionResult> Reserve(int id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            var ReserveList = await _context.reservations.ToListAsync();
            Book b = await _context.BookTable.FirstOrDefaultAsync(p => p.BookID == id);

            Reservation reservation = new Reservation();
            reservation.borrower = person;
            reservation.DateReserved = DateTime.Now;
            reservation.ReadingOrder = 1;
            b.ReserveList.Add(reservation);
            

            await _context.SaveChangesAsync();

            ViewBag.thisUser = userId;

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
            .Where(b => b.inUse)
            .Include(p => p.book)
                .ThenInclude(r => r.ReserveList)
                    .ThenInclude(g => g.borrower)
            .OrderBy(g => g.book.Title)
            .ToListAsync();

            var reservations = await _context.reservations.ToListAsync();

            IEnumerable<Catalogue> filteredBooks = myBooks.Distinct().ToList();

            ViewBag.thisUser = userId;

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

            Loan loan = new Loan();

            loan.borrower = person;
            loan.DateLoaned = DateTime.Now;
            catalogue.LoanList.Add(loan);

            catalogue.Owner = person;
            catalogue.book = book;
            

            if (ModelState.IsValid)
            {
                _context.Add(book);
                _context.Add(catalogue);
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
        }

        // POST: Catalogue/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Catalogue catalogue)
        {
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

        // GET: Catalogue/Lend/5
        //catalogue/lend/1020?memberid=92767851-ba53-49fc-8be9-06f7e3281469&loanid=1002
        public async Task<IActionResult> Lend(int? id, string memberID, int? loanID)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (memberID == null )
            {
                return NotFound();
            }

            if (loanID == null)
            {
                return NotFound();
            }

            var catalogue = await _context.catalogues.Include(b => b.book).FirstOrDefaultAsync(b => b.bID == id);
            if (catalogue == null)
            {
                return NotFound();
            }

            var member = await _context.ApplicationUsers.FirstOrDefaultAsync(m => m.Id == memberID);

            Loan oldloan = await _context.loans.FirstOrDefaultAsync(b => b.loanID == loanID);

            oldloan.DateReturned = DateTime.Now;

            _context.Update(oldloan);
            await _context.SaveChangesAsync();

            Loan newLoan = new Loan()
            {
                borrower = member,
                DateLoaned = DateTime.Now,
                loanID = 0
            };

            catalogue.LoanList.Add(newLoan);

            await _context.SaveChangesAsync();

            return RedirectToAction("MyIndex");
        }

        public async Task<IActionResult> currentlyReading()
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            var loans = await _context.loans
                .Include(x => x.catalogue)
                    .ThenInclude(b => b.book)
                        .ThenInclude(c => c.ReserveList)
                            .ThenInclude(f => f.borrower)
                .Include(g => g.catalogue)
                    .ThenInclude(b => b.Owner)
                .Where(b => b.borrower.Id == userId && b.DateReturned == DateTime.Parse("01/01/0001 00:00:00")).ToListAsync();

            if (loans == null)
            {
                return NotFound();
            }

            List<LoanVM> newLoans = new List<LoanVM>();

            foreach(Loan loan in loans)
            {
                ApplicationUser theBorrower = null;
                Reservation theReservation = null;

                if (loan.catalogue.book.ReserveList.Count > 0)
                {
                    if (loan.catalogue.book.ReserveList.FirstOrDefault(b => b.loan == null)!= null)
                    {
                        theReservation = loan.catalogue.book.ReserveList.FirstOrDefault(b => b.loan == null);
                        theBorrower = loan.catalogue.book.ReserveList.FirstOrDefault(b => b.loan == null).borrower;
                    }          
                }


                LoanVM lvm = new LoanVM()
                {
                    ID = 0,
                    loan = loan,
                    nextBorrower = theBorrower,
                    reservation = theReservation
                };

                newLoans.Add(lvm);
            }

            return View(newLoans);
        }

        //GET
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendNewsletter()
        {
            return View();
        }

        // POST: Catalogue/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendNewsletter(Newsletter newsletter)
        {
            string subject = "Wine Worms Newsletter " + DateTime.Now.ToLongDateString();
            List<ApplicationUser> members = await _context.ApplicationUsers.ToListAsync();

            string emails = string.Join(";", members.Select(m => m.Email).ToArray());

            //Get email template located in folder wwwroot/Templates/
            var webRoot = _webHostEnvironment.WebRootPath;

            var pathToFile = webRoot
                    + Path.DirectorySeparatorChar.ToString()
                    + "Templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "newsletter.html";

            var builder = new BodyBuilder();

            using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }

            string newmembers = null;

            if (!string.IsNullOrEmpty(newsletter.NewMembers))
            {
                newmembers = "<h4> Welcome to our new members: " + newsletter.NewMembers + "</h4>";
                   
            }

            string messageBody = string.Format(builder.HtmlBody,
                                      newsletter.NextMeetingLocation,
                                      DateTime.Now.ToString("MMMM"),
                                      newmembers,
                                      newsletter.NextMeetingDate.ToLongDateString(),
                                      newsletter.BookOfTheMonthTitle,
                                      newsletter.BookOfTheMonthImage,
                                      newsletter.BookOfTheMonthAuthor,
                                      newsletter.BookOfTheMonthBlurb,
                                      newsletter.NextMeetingDate.ToShortTimeString()
                                      );

            await _emailSender.SendEmailAsync(emails, subject, messageBody.ToString());

            await _context.newsletters.AddAsync(newsletter);
            await _context.SaveChangesAsync();

            return RedirectToAction("MessageSent");

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MessageSent()
        {

            return View();
        }

        // POST: Catalogue/Lend/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lend(Catalogue catalogue, Member member)
        {
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

            Loan loan = new Loan();

            loan.borrower = person;
            loan.DateLoaned = DateTime.Now;

            List<Loan> loanList = new List<Loan>();
            loanList.Add(loan);

            await _context.catalogues.AddAsync(new Catalogue() { book = myBook, inUse = true, Owner = person, LoanList = loanList });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> catalogueEditor(string? srch)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ApplicationUser person = await _context.ApplicationUsers.FirstOrDefaultAsync(p => p.Id == userId);

            var allBooks = await _context.catalogues
            .Where(b => b.inUse)
            .Include(p => p.book)
            .Include(o => o.Owner)
            .OrderBy(g => g.book.Title)
            .ToListAsync();

            ViewBag.thisUser = userId;

            return View(allBooks);

        }

        public async Task<IActionResult> HandOver(int? id, int? currentLoan)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (currentLoan== null)
            {
                return NotFound();
            }

            var loan = await _context.loans
                .Include(b => b.catalogue)
                .FirstOrDefaultAsync(m => m.loanID == currentLoan);
            if (loan == null)
            {
                return NotFound();
            }

            Loan oldLoan = loan;

            loan.DateReturned = DateTime.Now;
            _context.Update(loan);

            var reservation = await _context.reservations
                .Include(g => g.borrower)
               .FirstOrDefaultAsync(m => m.reservationID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            Loan newLoan = new() {borrower = reservation.borrower, catalogue = oldLoan.catalogue, DateLoaned = DateTime.Now};

            _context.loans.Add(newLoan);

            _context.reservations.Remove(reservation);

            await _context.SaveChangesAsync();

            return RedirectToAction("currentlyReading");
        }

    }
}
