using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using EShop.Service.Interface;

namespace EShop.Web.Controllers
{
    public class ShoppingCartController : Controller
    {

        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            return View(this._shoppingCartService.getShoppingCartInfo(userId));
        }

        public IActionResult DeleteFromShoppingCart(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.deleteProductFromSoppingCart(userId, id);

            if (result)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }
            else
            {
                return RedirectToAction("Index", "ShoppingCart");

            }
        }

        public IActionResult Order()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.order(userId);

            if (result)
            {
                return RedirectToAction("Index", "ShoppingCart");

            }
            else
            {
                return RedirectToAction("Index", "ShoppingCart");
            }
        }
    }
}
