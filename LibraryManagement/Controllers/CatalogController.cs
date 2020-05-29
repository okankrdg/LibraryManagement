using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Models.Catalog;
using LibraryManagementData;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAsset _assets;
        public CatalogController(ILibraryAsset assets)
        {
            _assets = assets;
        }
        public IActionResult Index()
        {
            var assetModels = _assets.GetAll();

            var listingResults = assetModels
                .Select(result => new AssetIndexListingModel
                {
                    Id = result.Id,
                    ImageUrl = result.Imageurl,
                    NumberOfCopies = result.NumberOfCopies,
                    AuthorDirector = _assets.GetAuthorOrDirector(result.Id),
                    DeweyCallNumber = _assets.GetDeweyIndex(result.Id),
                    Title = result.Title,
                    Type = _assets.GetType(result.Id)
                }) ;

            var model = new AssetsIndexModel
            {
                Assets = listingResults
            };
            return View(model);
        }
        public IActionResult Detail(int id)
        {
            var asset = _assets.GetById(id);
            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.Imageurl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(id),
                CurrentLocation = _assets.GetCurrentLocation(id).Name,
                DeweyCallNumber = _assets.GetDeweyIndex(id),
                ISBN = _assets.GetIsbn(id)

            };
            return View(model);
        }
    }
}