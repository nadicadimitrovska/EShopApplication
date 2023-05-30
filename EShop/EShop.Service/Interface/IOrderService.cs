using EShop.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace EShop.Service.Interface
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
    }
}
