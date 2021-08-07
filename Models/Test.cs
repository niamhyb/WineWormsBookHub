using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModel.Models
{
    public class Test
    {
        [Key]
        public int TestID { get; set; }
        public string FName { get; set; }
    }
}
