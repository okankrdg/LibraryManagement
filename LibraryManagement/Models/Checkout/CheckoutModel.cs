﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Models.Checkout
{
    public class CheckoutModel
    {
        public string LibrarycardId { get; set; }
        public string Title { get; set; }
        public int AssetId { get; set; }
        public string ImageUrl { get; set; }
        public int HoldCount { get; set; }
        public bool IsCheckedOut { get; set; }
    }
}
