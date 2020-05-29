using LibraryManagementData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementData
{
    public interface ICheckout
    {
        void Add(Checkout newCheckout);

        IEnumerable<Checkout> GetAll();
        IEnumerable<Hold> GetCurrentHolds(int id);
        IEnumerable<CheckoutHistory> GetCheckoutHistories(int id);

        Checkout GetById(int checkoutId);
        Checkout GetLatestCheckout(int assetId);
        string GetCurrentHoldPatronName(int id);
        string GetCurrentCheckoutPatron(int assetId);

        void CheckOutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId, int LibraryCardId);
        void PlaceHold(int assetId, int LibraryCardId);
       
       

        DateTime GetCurrentHoldPlaced(int id);
       
       

        void MarkLost(int assetId);
        void MarkFound(int assetId);
    }
}
