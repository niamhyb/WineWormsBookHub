using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModel.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
        [PersonalData]
        public DateTime BirthDate { get; set; }
        //public List<Book> books { get; set; }
        //public ApplicationUser()
        //{
        //    //books = new List<Book>();
        //}
    }

    public class Administrator : ApplicationUser
    {
        //this is the administrator
    }

    public class Member : ApplicationUser
    {
        //all other members
    }
}
