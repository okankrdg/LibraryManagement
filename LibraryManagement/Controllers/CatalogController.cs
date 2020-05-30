using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Models.Catalog;
using LibraryManagement.Models.Checkout;
using LibraryManagementData;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAsset _assets;
        private ICheckout _checkout;
        public CatalogController(ILibraryAsset assets, ICheckout checkouts)
        {
            _assets = assets;
            _checkout = checkouts;
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
            var currentHolds = _checkout.GetCurrentHolds(id).Select(x=>new AssetHoldModel { 
                PatronName =_checkout.GetCurrentHoldPatronName(x.Id),
                HoldPlaced = _checkout.GetCurrentHoldPlaced(x.Id).ToString("d")}) ;
            
            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Type=_assets.GetType(id),
                Year=asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.Imageurl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(id),
                CurrentLocation = _assets.GetCurrentLocation(id).Name,
                DeweyCallNumber = _assets.GetDeweyIndex(id),
                ISBN = _assets.GetIsbn(id),
                CheckoutHistories=_checkout.GetCheckoutHistories(id),
                LatestCheckout=_checkout.GetLatestCheckout(id),
                PatronName=_checkout.GetCurrentCheckoutPatron(id),
                CurrentHolds=currentHolds,

            };
            return View(model);
        }
        public IActionResult Checkout(int id)
        {
            var asset = _assets.GetById(id);
            var model = new CheckoutModel
            {
                AssetId = id,
                HoldCount = _checkout.GetCurrentHolds(id).Count(),
                Title=asset.Title,
                ImageUrl = asset.Imageurl,
                LibrarycardId="",
                IsCheckedOut = _checkout.IsCheckedOut(id)
            };
            return View(model);
        }
        public IActionResult CheckIn(int id)
        {
            _checkout.CheckInItem(id);
            return RedirectToAction("Detail", new { id = id });
        }
        public IActionResult Hold(int id)
        {
            var asset = _assets.GetById(id);
            var model = new CheckoutModel
            {
                AssetId = id,
                HoldCount = _checkout.GetCurrentHolds(id).Count(),
                Title = asset.Title,
                ImageUrl = asset.Imageurl,
                LibrarycardId = "",
                IsCheckedOut = _checkout.IsCheckedOut(id)
            };
            return View(model);
        }
        public IActionResult MarkLost(int assetId)
        {
            _checkout.MarkLost(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }
        public IActionResult MarkFound(int assetId)
        {
            _checkout.MarkFound(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }
        [HttpPost]
        public IActionResult PlaceCheckout(int assetId, int LibrarycardId)
        {
            _checkout.CheckOutItem(assetId, LibrarycardId);
            return RedirectToAction("Detail", new { id = assetId });
        }
        [HttpPost]
        public IActionResult PlaceHold(int assetId, int LibrarycardId)
        {
            _checkout.PlaceHold(assetId, LibrarycardId);
            return RedirectToAction("Detail", new { id = assetId });
        }
    }
}