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
        [Display(Name = "Romance")] Romance = 9,
        [Display(Name = "SciFi")] SciFi = 7,
        [Display(Name = "Thriller")] Thriller= 8,
    }
    public enum Type
    {
        [Display(Name = "Fiction")] Fiction = 0,
        [Display(Name = "Non Fiction")] Non_Fiction = 1,
    }
       
    public class Catalogue : IEquatable<Catalogue>
    {
        [Key]
        public int bID { get; set; }
        //bID is the Catalogue ID

        //not the book/owner details, a reference to them
        public Book book { get; set; }
        public ApplicationUser Owner { get; set; }

        //list of members who want particular book
        public List<Loan> LoanList { get; set; }
        //book is inuse or not
        [Display(Name = "In Use")]
        public bool inUse { get; set; } = true;

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public Catalogue()
        {
            LoanList = new List<Loan>();
        }

        public bool Equals(Catalogue other)
        {
            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal.
            return book.BookID.Equals(other.book.BookID);
        }

        public override int GetHashCode()
        {

            //Get hash code for the Code field.
            int hashBookID = book.BookID.GetHashCode();

            //Calculate the hash code for the product.
            return hashBookID;
        }
    }

    //books that members want to read
    public class Reservation
    {
        [Key]
        public int? reservationID { get; set; }
        public virtual ApplicationUser borrower { get; set; }
        public DateTime DateReserved { get; set; }
        public int ReadingOrder { get; set; }      
        public Loan loan { get; set; }

    }

    //recording when loan takes place
    public class Loan
    {
        [Key]
        public int? loanID { get; set; }

        [Display(Name = "Borrower")]
        public virtual ApplicationUser borrower { get; set; }

        [Display(Name = "Date Loaned")]
        public DateTime DateLoaned { get; set; }

        [Display(Name = "Date Returned")]
        public DateTime DateReturned { get; set; }

        public Catalogue catalogue { get; set; }
    }

    public class CatalogueVM
    {
        [Key]
        public int ID { get; set; }
        public List<Catalogue> Catalogue { get; set; }
        public List<Reservation> ReserveList { get; set; }
    }

    public class LoanVM
    {
        [Key]
        public int ID { get; set; }
        public Loan loan { get; set; }

        public Reservation reservation { get; set; }

        [Display(Name = "Next Borrower")]
        public ApplicationUser nextBorrower { get; set; }

    }

}
