using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModel.Models
{
    public class Newsletter
    {
        [Key]
        public int newsletterID { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime dateTime { get; set; }
        public string NewBooks { get; set; }
        [Display(Name = "New Members")]
        public string NewMembers { get; set; }
        public string Salutation { get; set; }
        [Display(Name = "Next Meeting Location")]
        public string NextMeetingLocation { get; set; }
        [Display(Name = "Next Meeting Date")]
        public DateTime NextMeetingDate { get; set; }
        [Display(Name = "Book of the Month Title")]
        public string BookOfTheMonthTitle { get; set; }
        [Display(Name = "Book of the Month Author")]
        public string BookOfTheMonthAuthor { get; set; }
        [Display(Name = "Book of the Month Blurb")]
        public string BookOfTheMonthBlurb { get; set; }
        [Display(Name = "Book of the Month Image")]
        public string BookOfTheMonthImage { get; set; }

    }
}
