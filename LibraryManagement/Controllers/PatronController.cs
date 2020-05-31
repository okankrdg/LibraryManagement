using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Models.Patron;
using LibraryManagementData;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class PatronController : Controller
    {
        private IPatron _patron;
        public PatronController(IPatron patron)
        {
            _patron = patron;
        }
        public IActionResult Index()
        {
            var allPatrons = _patron.GetAll();
            var patronModels = allPatrons.Select(p => new PatronDetailModel
            {
                Id = p.Id,
                FirstName = p.Firstname,
                LastName = p.LastName,
                LibraryCardId = p.LibraryCard.Id,
                OverdueFess=p.LibraryCard.Fees,
                HomeLibraryBranch = p.HomeLibraryBranch.Name
                //Telephone = p.TelephoneNumber,
                //Holds = _patron.GetHolds(p.Id),
                //CheckoutHistory = _patron.GetCheckoutHistory(p.Id),
                //AssetsCheckedOut = _patron.GetCheckouts(p.Id),
                //Address = p.Address
            });
            var model = new PatronIndexModel
            {
                Patrons = patronModels
            };
            return View(model);
        }
        public IActionResult Detail(int id)
        {
            var patron = _patron.Get(id);
            var patronModels = new PatronDetailModel
            {
                FirstName = patron.Firstname,
                LastName = patron.LastName,
                LibraryCardId = patron.LibraryCard.Id,
                OverdueFess = patron.LibraryCard.Fees,
                HomeLibraryBranch = patron.HomeLibraryBranch.Name,
                Telephone = patron.TelephoneNumber,
                Holds = _patron.GetHolds(id),
                CheckoutHistory = _patron.GetCheckoutHistory(id),
                AssetsCheckedOut = _patron.GetCheckouts(id).ToList() ?? new List<LibraryManagementData.Models.Checkout>(),
                Address = patron.Address
            };
            return View(patronModels);
        }
    }
}