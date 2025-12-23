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
    public static class AdminPermission
    {
        public const string ClaimType = "Permission";
        public const string Role = "Admin";

        public static class Customer
        {
            public const string Create = "Admin.Customer.Create";
            public const string Update = "Admin.Customer.Update";
            public const string Delete = "Admin.Customer.Delete";
            public const string View = "Admin.Customer.View";
        }

        public static class Order
        {
            public const string Create = "Admin.Order.Create";
            public const string Update = "Admin.Order.Update";
            public const string Delete = "Admin.Order.Delete";
            public const string View = "Admin.Order.View";
        }

        public static class Brand
        {
            public const string Create = "Admin.Brand.Create";
            public const string Update = "Admin.Brand.Update";
            public const string Delete = "Admin.Brand.Delete";
            public const string View = "Admin.Brand.View";
        }

        public static class Category
        {
            public const string Create = "Admin.Category.Create";
            public const string Update = "Admin.Category.Update";
            public const string Delete = "Admin.Category.Delete";
            public const string View = "Admin.Category.View";
        }

        public static class Product
        {
            public const string Create = "Admin.Product.Create";
            public const string Update = "Admin.Product.Update";
            public const string Delete = "Admin.Product.Delete";
            public const string View = "Admin.Product.View";
        }

        public static class Inventory
        {
            public const string Create = "Admin.Inventory.Create";
            public const string Update = "Admin.Inventory.Update";
            public const string Delete = "Admin.Inventory.Delete";
            public const string View = "Admin.Inventory.View";
        }

        public static class ProductCollection
        {
            public const string Create = "Admin.ProductCollection.Create";
            public const string Update = "Admin.ProductCollection.Update";
            public const string Delete = "Admin.ProductCollection.Delete";
            public const string View = "Admin.ProductCollection.View";
        }

        public static class WareHouseCollection
        {
            public const string Create = "Admin.WareHouseCollection.Create";
            public const string Update = "Admin.WareHouseCollection.Update";
            public const string Delete = "Admin.WareHouseCollection.Delete";
            public const string View = "Admin.WareHouseCollection.View";
        }

        public static class CustomerBusiness
        {
            public const string Create = "Admin.CustomerBusiness.Create";
            public const string Update = "Admin.CustomerBusiness.Update";
            public const string Delete = "Admin.CustomerBusiness.Delete";
            public const string View = "Admin.CustomerBusiness.View";
        }

        public static class SystemUser
        {
            public const string Create = "Admin.SystemUser.Create";
            public const string Update = "Admin.SystemUser.Update";
            public const string Delete = "Admin.SystemUser.Delete";
            public const string View = "Admin.SystemUser.View";
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
                ProductCollection.Create, ProductCollection.Update,ProductCollection.Delete, ProductCollection.View,
                WareHouseCollection.Create, WareHouseCollection.Update, WareHouseCollection.Delete, WareHouseCollection.View,
                CustomerBusiness.Create, CustomerBusiness.Update, CustomerBusiness.Delete, CustomerBusiness.View
            };
        }
    }
}
