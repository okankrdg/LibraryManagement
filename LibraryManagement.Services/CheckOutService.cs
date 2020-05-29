using LibraryManagementData;
using LibraryManagementData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryManagement.Services
{
    public class CheckOutService : ICheckout
    {
        private LibraryDbContext _context;
        public CheckOutService(LibraryDbContext context)
        {
            _context = context;
        }
        public void Add(Checkout newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }

      

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int checkoutId)
        {
            return GetAll().FirstOrDefault(checkout=>checkout.Id==checkoutId);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistories(int id)
        {
            return _context.CheckoutHistories
                .Include(h=>h.LibraryAsset)
                .Include(h=>h.LibraryCard)
                .Where(h=>h.LibraryAsset.Id==id);
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(h=>h.LibraryAsset)
                .Include(h=>h.LibraryCard)
                .Where(h=>h.LibraryAsset.Id==id);
        }
        public Checkout GetLatestCheckout(int assetId)
        {
            return _context.Checkouts
                .Where(c => c.LibraryAsset.Id == assetId)
                .OrderByDescending(c => c.Since)
                .FirstOrDefault();
        }
        public void MarkLost(int assetId)
        {
            UpdateAssetStatus(assetId, "Lost");
            _context.SaveChanges();
        }
        public void MarkFound(int assetId)
        {
            DateTime now = DateTime.Now;
            UpdateAssetStatus(assetId, "Available");
            RemoveExistingCheckouts(assetId);
            CloseExistingCheckoutHistory(assetId,now);
            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int assetId, string v)
        {
            var item = _context.LibraryAssets
              .FirstOrDefault(a => a.Id == assetId);
            _context.Update(item);
            item.Status = _context.Statuses
                .FirstOrDefault(status => status.Name == v);
        }

        private void CloseExistingCheckoutHistory(int assetId,DateTime now)
        {
            // close any existing checkout history
            var history = _context.CheckoutHistories
              .FirstOrDefault(h => h.LibraryAsset.Id == assetId
                  && h.CheckedIn == null);
            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
        }
        private void RemoveExistingCheckouts(int assetId)
        {
            //remove any existing checkouts on the item
            var checkout = _context.Checkouts
                .FirstOrDefault(co => co.LibraryAsset.Id == assetId);
            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }


        public void CheckInItem(int assetId, int LibraryCardId)
        {
            var now = DateTime.Now;
            var item = _context.LibraryAssets
                .FirstOrDefault(asset => asset.Id == assetId);
            _context.Update(item);
            //remove any existing checkouts on the item
            RemoveExistingCheckouts(assetId);
            // close any existing checkout history
            CloseExistingCheckoutHistory(assetId, now);
            //look for existing holds on item
            var currentHolds = _context.Holds
                .Include(h => h.LibraryCard)
                .Include(h => h.LibraryAsset)
                .Where(h => h.LibraryAsset.Id == assetId);

            //if there holds, checkout to item
            // librarycard with the earliest hold.
            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(assetId,currentHolds);
            }
            //otherwise, update the item status available
            UpdateAssetStatus(assetId, "Available");
            _context.SaveChanges();
        }

        private void CheckoutToEarliestHold(int assetId, IQueryable<Hold> currentHolds)
        {
            var earliestHold = currentHolds.OrderBy(holds => holds.HoldPlaced)
                .FirstOrDefault(asset => asset.LibraryAsset.Id == assetId);
            var card = earliestHold.LibraryCard;
            _context.Remove(earliestHold);
            _context.SaveChanges();
            CheckOutItem(assetId, card.Id);
        }

       

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            if (IsCheckedOut(assetId))
            {
                return;
                //add logic here to handle feeedback to the user
            }
            var item = _context.LibraryAssets
           .FirstOrDefault(a => a.Id == assetId);

            UpdateAssetStatus(assetId, "Checked Out");

            var libraryCard = _context.LibraryCards.Include(card => card.Checkouts)
                .FirstOrDefault(card => card.Id == libraryCardId);
            var now = DateTime.Now;
            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now)
            };
            _context.Add(checkout);
            var checkoutHistory = new CheckoutHistory
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                CheckedOut = now
            };
            _context.Add(checkoutHistory);

            _context.SaveChanges();
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        private bool IsCheckedOut(int assetId)
        {
            var isCheckedOut = _context.Checkouts.Where(co => co.LibraryAsset.Id == assetId).Any();
            return isCheckedOut;
        }

        public void PlaceHold(int assetId, int LibraryCardId)
        {
            var now = DateTime.Now;
            var asset = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);
            var card = _context.LibraryCards.FirstOrDefault(c => c.Id == assetId);
            if (asset.Status.Name=="Available")
            {
                UpdateAssetStatus(assetId, "On Hold");
            }
            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };
            _context.Add(hold);
            _context.SaveChanges();
        }
        public string GetCurrentHoldPatronName(int holdId)
        {
            var hold = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId);

            var cardId = hold?.LibraryCard.Id;

            var patron = _context.Patrons
                .Include(pt=>pt.LibraryCard)
                .FirstOrDefault(pt => pt.LibraryCard.Id == cardId);
            return patron?.Firstname + " " + patron?.LastName;
        }

        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            var hold = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId)
                .HoldPlaced;
            return hold;
                
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetCheckoutByAssetId(assetId);
            if (checkout==null)
            {
                return "Not checked out.";
            }
            var cardId = checkout.LibraryCard.Id;
            var patron = _context.Patrons
               .Include(pt => pt.LibraryCard)
               .FirstOrDefault(pt => pt.LibraryCard.Id == cardId);
            return patron.Firstname + " " + patron.LastName;

        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return _context.Checkouts
                .Include(co => co.LibraryAsset)
                .Include(co => co.LibraryCard)
                .FirstOrDefault(co => co.LibraryAsset.Id == assetId);
                
        }
    }
}
