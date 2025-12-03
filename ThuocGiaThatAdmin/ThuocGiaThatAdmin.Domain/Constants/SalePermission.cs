using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Domain.Constants
{
    /// <summary>
    /// Defines all permission claims used in the application.
    /// Use these constants in authorization policies and attributes.
    /// </summary>
    public static class SalePermissions
    {
        public const string ClaimType = "Permission";

        public static class Customer
        {
            public const string Create = "Sale.Customer.Create";
            public const string Update = "Sale.Customer.Update";
            public const string Delete = "Sale.Customer.Delete";
            public const string View = "Sale.Customer.View";
        }

        public static class Order
        {
            public const string Create = "Sale.Order.Create";
            public const string Update = "Sale.Order.Update";
            public const string Delete = "Sale.Order.Delete";
            public const string View = "Sale.Order.View";
        }

        public static class Brand
        {
            public const string Create = "Sale.Brand.Create";
            public const string Update = "Sale.Brand.Update";
            public const string Delete = "Sale.Brand.Delete";
            public const string View = "Sale.Brand.View";
        }

        public static class Category
        {
            public const string Create = "Sale.Category.Create";
            public const string Update = "Sale.Category.Update";
            public const string Delete = "Sale.Category.Delete";
            public const string View = "Sale.Category.View";
        }

        public static class Product
        {
            public const string Create = "Sale.Product.Create";
            public const string Update = "Sale.Product.Update";
            public const string Delete = "Sale.Product.Delete";
            public const string View = "Sale.Product.View";
        }

        public static class Inventory
        {
            public const string Create = "Sale.Inventory.Create";
            public const string Update = "Sale.Inventory.Update";
            public const string Delete = "Sale.Inventory.Delete";
            public const string View = "Sale.Inventory.View";
        }

        public static class ProductCollection
        {
            public const string Create = "Sale.ProductCollection.Create";
            public const string Update = "Sale.ProductCollection.Update";
            public const string Delete = "Sale.ProductCollection.Delete";
            public const string View = "Sale.ProductCollection.View";
        }

        public static class WareHouseCollection
        {
            public const string Create = "Sale.WareHouseCollection.Create";
            public const string Update = "Sale.WareHouseCollection.Update";
            public const string Delete = "Sale.WareHouseCollection.Delete";
            public const string View = "Sale.WareHouseCollection.View";
        }

        public static class CustomerBusiness
        {
            public const string Create = "Sale.CustomerBusiness.Create";
            public const string Update = "Sale.CustomerBusiness.Update";
            public const string Delete = "Sale.CustomerBusiness.Delete";
            public const string View = "Sale.CustomerBusiness.View";
        }

        /// <summary>
        /// Returns all permission values as a list.
        /// </summary>
        public static List<string> GetAllPermissions()
        {
            return new List<string>
            {
                // Customer
                Customer.Create, Customer.Update, Customer.Delete, Customer.View,
                // Order
                Order.Create, Order.Update, Order.Delete, Order.View,
                // Brand
                Brand.Create, Brand.Update, Brand.Delete, Brand.View,
                // Category
                Category.Create, Category.Update, Category.Delete, Category.View,
                // Product
                Product.Create, Product.Update, Product.Delete, Product.View,
                // Inventory
                Inventory.Create, Inventory.Update, Inventory.Delete, Inventory.View,
                // Product Collection
                ProductCollection.Create, ProductCollection.Update,ProductCollection.Delete, ProductCollection.View,
                // WareHouse Collection
                WareHouseCollection.Create, WareHouseCollection.Update, WareHouseCollection.Delete, WareHouseCollection.View,
                // Customer Business
                CustomerBusiness.Create, CustomerBusiness.Update, CustomerBusiness.Delete, CustomerBusiness.View
            };
        }
    }
}
