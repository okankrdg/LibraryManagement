using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementData.Models
{
    public class Patron
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime DateofBirth { get; set; }
        public string TelephoneNumber { get; set; }
        public virtual LibraryCard LibraryCard { get; set; }
        public virtual LibraryBranch HomeLibraryBranch { get; set; }
    }
}
