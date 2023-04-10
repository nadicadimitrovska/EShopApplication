using Microsoft.AspNetCore.Mvc;
using EShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using EShop.Web.Models.DTO;
using EShop.Web.Models.Domain;

namespace EShop.Web.Controllers
{
    public class ShoppingCartController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUser = await _context.Users.Where(z => z.Id == userId)
                .Include("UserCart")
                .Include("UserCart.ProductInShoppingCarts")
                .Include("UserCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();
            //ja zema kartichkata
            var userShoppingCart = loggedInUser.UserCart;
            //gi zema produktite
            var AllProducts = userShoppingCart.ProductInShoppingCarts.ToList();

            var allProductPrice = AllProducts.Select(z => new
            {
                ProductPrice=z.Product.ProductPrice,
                Quantity = z.Quantity
            }).ToList();

            var totalPrice = 0;
            foreach (var item in allProductPrice)
            {
                totalPrice += item.Quantity * item.ProductPrice;
            }

            ShoppingCartDto scDto = new ShoppingCartDto
            {
                Products = AllProducts,
                TotalPrice = totalPrice
            };


            return View(scDto);
        }

        public async Task<IActionResult> DeleteFromShoppingCart(Guid? id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(!string.IsNullOrEmpty(userId) && id!=null)
            {
                var loggedInUser = await _context.Users.Where(z => z.Id == userId)
                .Include("UserCart")
                .Include("UserCart.ProductInShoppingCarts")
                .Include("UserCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();
                //ja zema kartichkata
                var userShoppingCart = loggedInUser.UserCart;

                var itemToDelete = userShoppingCart.ProductInShoppingCarts.Where(z => z.ProductId.Equals(id)).FirstOrDefault();
                userShoppingCart.ProductInShoppingCarts.Remove(itemToDelete);

                _context.Update(userShoppingCart);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction("Index", "ShoppingCart");
        }

        public async Task<IActionResult> Order()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = await _context.Users.Where(z => z.Id == userId)
                .Include("UserCart")
                .Include("UserCart.ProductInShoppingCarts")
                .Include("UserCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();
                //ja zema kartichkata
                var userShoppingCart = loggedInUser.UserCart;

                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    User=loggedInUser,
                    UserId=userId
                };

                _context.Add(order);
                await _context.SaveChangesAsync();


                List<ProductInOrder> productInOrders = new List<ProductInOrder>();

                var result = userShoppingCart.ProductInShoppingCarts.Select(z => new ProductInOrder
                {
                    ProductId=z.ProductId,
                    OrderedProduct=z.Product,
                    OrderId=order.Id,
                    UserOrder=order
                }).ToList();

                productInOrders.AddRange(result);

                foreach (var item in productInOrders)
                {
                    _context.Add(item);
                }
                await _context.SaveChangesAsync();

                loggedInUser.UserCart.ProductInShoppingCarts.Clear();
                _context.Update(loggedInUser);
                await _context.SaveChangesAsync();
            }


             return RedirectToAction("Index", "ShoppingCart");
        }
    }
}
