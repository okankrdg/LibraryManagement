using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Models.Catalog
{
    public class AssetsIndexModel
    {
        public IEnumerable<AssetIndexListingModel> Assets { get; set; }
    }
}
