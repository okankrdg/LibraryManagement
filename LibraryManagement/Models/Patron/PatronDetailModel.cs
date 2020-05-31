using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementData.Models;

namespace LibraryManagement.Models.Patron
{
    public class PatronDetailModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName; 
            }
        }
        public int LibraryCardId { get; set; }
        public string  Address { get; set; }
        public DateTime MemberSince { get; set; }
        public string Telephone { get; set; }
        public string HomeLibraryBranch { get; set; }
        public decimal OverdueFess { get; set; }
        public IEnumerable<LibraryManagementData.Models.Checkout> AssetsCheckedOut { get; set; }
        public IEnumerable<CheckoutHistory> CheckoutHistory { get; set; }
        public IEnumerable<Hold> Holds { get; set; }
    }
}
