﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MemberShipWebsite.Areas.Models
{
    public class EditButtonModel
    {

        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public int SubscriptionId { get; set; }


        public string Link
        {

            get
            {
                var s = new StringBuilder("?");
                if (ItemId > 0) s.Append(String.Format("{0}={1}&", "ItemId", ItemId));

                if (ProductId > 0) s.Append(String.Format("{0}={1}&", "ProductId", ProductId));
               
                if (SubscriptionId > 0) s.Append(String.Format("{0}={1}&", "SubscriptionId", SubscriptionId));

                return s.ToString().Substring(0,s.Length-1);
            }
        }
    }
}