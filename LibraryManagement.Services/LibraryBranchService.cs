using LibraryManagementData;
using LibraryManagementData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryManagement.Services
{
    public class LibraryBranchService : ILibraryBranch
    {
        private LibraryDbContext _context;
        public LibraryBranchService(LibraryDbContext context)
        {
            _context = context;
        }
        public void add(LibraryBranch newBranch)
        {
            _context.Add(newBranch);
            _context.SaveChanges();
        }

        public LibraryBranch Get(int branchId)
        {
            return GetAll()
                 .FirstOrDefault(lb => lb.Id == branchId);
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return _context.LibraryBranches
                 .Include(lb => lb.LibraryAssets)
                 .Include(lb => lb.Patrons);
        }

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            return _context.LibraryBranches
                .Include(lb => lb.LibraryAssets)
                .FirstOrDefault(lb => lb.Id == branchId).LibraryAssets;
        }

        public IEnumerable<string> GetBranchHours(int branchId)
        {
            var hours = _context.BranchHours.Where(lb => lb.Branch.Id == branchId);
            return DataHelpers.HumanizeBusinessHours(hours);

        }

        public IEnumerable<Patron> GetPatrons(int branchId)
        {
            return _context.LibraryBranches
                .Include(b => b.Patrons)
                .FirstOrDefault(b => b.Id == branchId)
                .Patrons;
        }

        public bool IsBranchOpen(int branchId)
        {
            var currentTimeHour = DateTime.Now.Hour;
            var currentDayOfWeek = (int)DateTime.Now.DayOfWeek + 1;
            var hours = _context.BranchHours.Where(lb => lb.Branch.Id == branchId);
            var daysHours = hours.FirstOrDefault(h => h.DayOfWeek == currentDayOfWeek);
            var isOpen = currentTimeHour < daysHours.CloseTime && currentTimeHour > daysHours.OpenTime;
            return isOpen;
        }
    }
}
