using LibraryManagementData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementData
{
    public interface IPatron
    {
        Patron Get(int Id);
        IEnumerable<Patron> GetAll();
        void Add(Patron newPatron);
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId);
        IEnumerable<Hold> GetHolds(int patronId);
        IEnumerable<Checkout> GetCheckouts(int patronId);
    }
}
