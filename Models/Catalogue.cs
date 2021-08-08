using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModel.Models
{
    public enum Genre
    {
        [Display(Name = "Action")] Action = 0,
        [Display(Name = "Adventure")] Adventure = 1,
        [Display(Name = "Comedy")] Comedy = 2,
        [Display(Name = "Crime")] Crime = 3,
        [Display(Name = "Drama")] Drama = 4,
        [Display(Name = "Fantasy")] Fantasy = 5,
        [Display(Name = "Horror")] Horror = 6,
        [Display(Name = "SciFi")] SciFi = 7,
        [Display(Name = "Thriller")] Thriller= 8,
    }
    public enum Type
    {
        [Display(Name = "Fiction")] Fiction = 0,
        [Display(Name = "Non Fiction")] Non_Fiction = 1,
    }
       
    public class Catalogue
    {
        [Key]
        public int bID { get; set; }
        //public int BookID { get; set; }

        //not the book/owner details, a reference to them
        public virtual Book book { get; set; }
        public virtual ApplicationUser Owner { get; set; }

        //list of members who want particular book
        public virtual List<Reservation> ReserveList { get; set; }
        public virtual List<Loan> LoanList { get; set; }
        //book is inuse or not
        public bool inUse { get; set; } = true;

        public Catalogue()
        {
            ReserveList = new List<Reservation>();
            LoanList = new List<Loan>();
        }


    }

    //books that members want to read
    public class Reservation
    {
        [Key]
        public int reservationID { get; set; }
        //public Book book { get; set; }
        public virtual ApplicationUser borrower { get; set; }
        public DateTime DateReserved { get; set; }

        public int ReadingOrder { get; set; }

    }

    ////recording when loan takes place
    public class Loan
    {
        [Key]
        public int loanID { get; set; }
        //public Book book { get; set; }
        public virtual ApplicationUser borrower { get; set; }

        public DateTime DateLoaned { get; set; }

        public DateTime DateReturned { get; set; }
    }
}
