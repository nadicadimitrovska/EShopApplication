﻿using EShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Domain
{
    public class Order :BaseEntity
    {
        
        public string UserId { get; set; }
        public EShopApplicationUser User { get; set; }
        public IEnumerable<ProductInOrder> productInOrders { get; set; }
    }
}
