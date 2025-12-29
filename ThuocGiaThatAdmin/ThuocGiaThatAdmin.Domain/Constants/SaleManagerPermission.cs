using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Domain.Constants
{
    public class SaleManagerPermission
    {
        public const string Module = "SaleManager";
        public const string ClaimType = "Permission";
        public const string Role = "Sale Manager";
        public const string GroupName = "Sale Management";
        public const string Description = "Permissions related to Sale management.";
        public const string Category = "Sales Management";

        public static class Sale
        {
            public const string Create = "SaleManager.Sale.Create";
            public const string Update = "SaleManager.Sale.Update";
            public const string Delete = "SaleManager.Sale.Delete";
            public const string View = "SaleManager.Sale.View";

            public const string Import = "SaleManager.Sale.Import";
            public const string Export = "SaleManager.Sale.Export";
        }

        public static class Customer
        {
            public const string Create = "SaleManager.Customer.Create";
            public const string Update = "SaleManager.Customer.Update";
            public const string Delete = "SaleManager.Customer.Delete";
            public const string View = "SaleManager.Customer.View";

            public const string Import = "SaleManager.Customer.Import";
            public const string Export = "SaleManager.Customer.Export";
        }

        public static class Order
        {
            public const string Create = "SaleManager.Order.Create";
            public const string Update = "SaleManager.Order.Update";
            public const string Delete = "SaleManager.Order.Delete";
            public const string View = "SaleManager.Order.View";

            public const string Import = "SaleManager.Order.Import";
            public const string Export = "SaleManager.Order.Export";
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
                Sale.Create, Sale.Update, Sale.Delete, Sale.View
                // Brand
                // Category
                // Product
                // Inventory
                // Product Collection
                // WareHouse Collection
                // Customer Business
            };
        }
    }
}
