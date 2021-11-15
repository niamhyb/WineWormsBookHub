using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModel.Models
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }

        [MinLength(13)]
        [MaxLength(13)]
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        [Display(Name = "Genre")]
        public Genre Genres { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be 1 to 5")]
        public int AvgRating { get; set; }

        public virtual List<Reservation> ReserveList { get; set; }

        public Book()
        {
            ReserveList = new List<Reservation>();
        }

    }
}
