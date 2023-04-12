﻿using EShop.Domain;
using EShop.Domain.DTO;
using EShop.Repository.Interface;
using EShop.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EShop.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ProductInOrder> _productInOrderRepository;
        private readonly IUserRepository _userRepository;
        
        
        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IUserRepository userRepository, IRepository<Order> orderRepository, IRepository<ProductInOrder> productInOrderRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _productInOrderRepository = productInOrderRepository;
        }
        public bool deleteProductFromSoppingCart(string userId, Guid productId)
        {
            if (!string.IsNullOrEmpty(userId) && productId != null)
            {
                var loggedInUser = this._userRepository.Get(userId);
                    //ja zema kartichkata
                var userShoppingCart = loggedInUser.UserCart;

                var itemToDelete = userShoppingCart.ProductInShoppingCarts.Where(z => z.ProductId.Equals(productId)).FirstOrDefault();
                userShoppingCart.ProductInShoppingCarts.Remove(itemToDelete);

                this._shoppingCartRepository.Update(userShoppingCart);
                return true;

            }
            return false;
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            var loggedInUser = this._userRepository.Get(userId);           //ja zema kartichkata
            var userShoppingCart = loggedInUser.UserCart;
            //gi zema produktite
            var AllProducts = userShoppingCart.ProductInShoppingCarts.ToList();

            var allProductPrice = AllProducts.Select(z => new
            {
                ProductPrice = z.Product.ProductPrice,
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

            return scDto;
        }

        public bool order(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);
                //ja zema kartichkata
                var userShoppingCart = loggedInUser.UserCart;

                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    User = loggedInUser,
                    UserId = userId
                };

                this._orderRepository.Insert(order);


                List<ProductInOrder> productInOrders = new List<ProductInOrder>();

                var result = userShoppingCart.ProductInShoppingCarts.Select(z => new ProductInOrder
                {
                    ProductId = z.ProductId,
                    OrderedProduct = z.Product,
                    OrderId = order.Id,
                    UserOrder = order
                }).ToList();

                productInOrders.AddRange(result);

                foreach (var item in productInOrders)
                {
                    this._productInOrderRepository.Insert(item);
                }

                loggedInUser.UserCart.ProductInShoppingCarts.Clear();
                this._userRepository.Update(loggedInUser);

                return true;
            }
            return false;

        }
    }
}
