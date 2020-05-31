using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Models.Branch;
using LibraryManagementData;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class BranchController : Controller
    {
        private ILibraryBranch _branch;
        public BranchController(ILibraryBranch branch)
        {
            _branch = branch;
        }
        public IActionResult Index()
        {
            var branches = _branch.GetAll().Select(branch => new BranchDetailModel
            {
                Id = branch.Id,
                Name = branch.Name,
                IsOpen = _branch.IsBranchOpen(branch.Id),
                NumberOfAssets = _branch.GetAssets(branch.Id).Count(),
                NumberOfPatrons = _branch.GetPatrons(branch.Id).Count()
            });
            var model = new BranchIndexModel()
            {
                Branches = branches
            };
            return View(model);
        }
        public IActionResult Detail(int id)
        {
            var branch = _branch.Get(id);
            var model = new BranchDetailModel
            {
                Id = branch.Id,
                Name = branch.Name,
                IsOpen = _branch.IsBranchOpen(branch.Id),
                NumberOfAssets = _branch.GetAssets(branch.Id).Count(),
                NumberOfPatrons = _branch.GetPatrons(branch.Id).Count(),
                Address = branch.Address,
                Description = branch.Description,
                HoursOpen = _branch.GetBranchHours(id),
                Telephone = branch.Telephone,
                ImageUrl = branch.ImageUrl,
                OpenDate = branch.OpenDate.ToString("dd-MM-yyyy"),
                TotalAssetValue = _branch.GetAssets(id).Sum(a => a.Cost)
            };
            return View(model);
        }
    }
}