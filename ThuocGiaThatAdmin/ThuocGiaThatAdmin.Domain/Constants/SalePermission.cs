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

            public const string Import = "Sale.Customer.Import";
            public const string Export = "Sale.Customer.Export";
        }

        public static class Order
        {
            public const string Create = "Sale.Order.Create";
            public const string Update = "Sale.Order.Update";
            public const string Delete = "Sale.Order.Delete";
            public const string View = "Sale.Order.View";

            public const string Import = "Sale.Order.Import";
            public const string Export = "Sale.Order.Export";
        }

        public static class Brand
        {
            public const string Create = "Sale.Brand.Create";
            public const string Update = "Sale.Brand.Update";
            public const string Delete = "Sale.Brand.Delete";
            public const string View = "Sale.Brand.View";

            public const string Import = "Sale.Brand.Import";
            public const string Export = "Sale.Brand.Export";
        }

        public static class Category
        {
            public const string Create = "Sale.Category.Create";
            public const string Update = "Sale.Category.Update";
            public const string Delete = "Sale.Category.Delete";
            public const string View = "Sale.Category.View";

            public const string Import = "Sale.Category.Import";
            public const string Export = "Sale.Category.Export";
        }

        public static class Product
        {
            public const string Create = "Sale.Product.Create";
            public const string Update = "Sale.Product.Update";
            public const string Delete = "Sale.Product.Delete";
            public const string View = "Sale.Product.View";

            public const string Import = "Sale.Product.Import";
            public const string Export = "Sale.Product.Export";
        }

        public static class Inventory
        {
            public const string Create = "Sale.Inventory.Create";
            public const string Update = "Sale.Inventory.Update";
            public const string Delete = "Sale.Inventory.Delete";
            public const string View = "Sale.Inventory.View";

            public const string Import = "Sale.Inventory.Import";
            public const string Export = "Sale.Inventory.Export";
        }

        public static class ProductCollection
        {
            public const string Create = "Sale.ProductCollection.Create";
            public const string Update = "Sale.ProductCollection.Update";
            public const string Delete = "Sale.ProductCollection.Delete";
            public const string View = "Sale.ProductCollection.View";

            public const string Import = "Sale.ProductCollection.Import";
            public const string Export = "Sale.ProductCollection.Export";
        }

        public static class WareHouseCollection
        {
            public const string Create = "Sale.WareHouseCollection.Create";
            public const string Update = "Sale.WareHouseCollection.Update";
            public const string Delete = "Sale.WareHouseCollection.Delete";
            public const string View = "Sale.WareHouseCollection.View";

            public const string Import = "Sale.WareHouseCollection.Import";
            public const string Export = "Sale.WareHouseCollection.Export";
        }

        public static class CustomerBusiness
        {
            public const string Create = "Sale.CustomerBusiness.Create";
            public const string Update = "Sale.CustomerBusiness.Update";
            public const string Delete = "Sale.CustomerBusiness.Delete";
            public const string View = "Sale.CustomerBusiness.View";

            public const string Import = "Sale.CustomerBusiness.Import";
            public const string Export = "Sale.CustomerBusiness.Export";
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
