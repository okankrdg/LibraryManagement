using LibraryManagementData;
using LibraryManagementData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryManagement.Services
{
    public class PatronService : IPatron
    {
        private LibraryDbContext _context;
        public PatronService(LibraryDbContext context)
        {
            _context = context;
        }
        public void Add(Patron newPatron)
        {
            _context.Add(newPatron);
            _context.SaveChanges();
        }
        public IEnumerable<Patron> GetAll()
        {
            return _context.Patrons.Include(pt => pt.LibraryCard)
              .Include(pt => pt.HomeLibraryBranch);
        }

        public Patron Get(int id)
        {
            return GetAll()
                .FirstOrDefault(pt=>pt.Id == id);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId)
        {
            var cardId = GetLibraryCardId(patronId);
            return _context.CheckoutHistories
                .Include(ch=>ch.LibraryCard)
                .Include(ch=>ch.LibraryAsset)
                .Where(ch => ch.LibraryCard.Id == cardId)
                .OrderByDescending(ch=>ch.CheckedOut);
        }

        public IEnumerable<Checkout> GetCheckouts(int patronId)
        {
            var cardId = GetLibraryCardId(patronId);
            return _context.Checkouts
                .Include(ch => ch.LibraryCard)
                .Include(ch => ch.LibraryAsset)
                .Where(ch => ch.LibraryCard.Id == cardId);
        }

        public IEnumerable<Hold> GetHolds(int patronId)
        {

            var cardId = GetLibraryCardId(patronId);
            return _context.Holds
               .Include(h => h.LibraryCard)
               .Include(h => h.LibraryAsset)
               .Where(h => h.LibraryCard.Id == cardId)
               .OrderByDescending(h => h.HoldPlaced);

        }

        private int GetLibraryCardId(int patronId)
        {
            return _context.Patrons.Include(pt => pt.LibraryCard)
              .FirstOrDefault(pt => pt.Id == patronId)
              .LibraryCard.Id;
        }
    }
}
