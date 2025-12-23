using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Domain.Constants
{
    public static class InventoryPermission
    {
        public const string ClaimType = "Permission";
        public const string GroupName = "Inventory";
        public const string Role = "Inventory";
        public const string Description = "Inventory management permissions";
        public const string Category = "Inventory";

        public static class Order
        {
            public const string Create = "Iventory.Order.Create";
            public const string Update = "Iventory.Order.Update";
            public const string Delete = "Iventory.Order.Delete";
            public const string View = "Iventory.Order.View";

            public const string Import = "Iventory.Order.Import";
            public const string Export = "Iventory.Order.Export";
        }

        public static class Product
        {
            public const string Create = "Iventory.Product.Create";
            public const string Update = "Iventory.Product.Update";
            public const string Delete = "Iventory.Product.Delete";
            public const string View = "Iventory.Product.View";

            public const string Import = "Iventory.Product.Import";
            public const string Export = "Iventory.Product.Export";
        }

        public static class Inventory
        {
            public const string Create = "Iventory.Inventory.Create";
            public const string Update = "Iventory.Inventory.Update";
            public const string Delete = "Iventory.Inventory.Delete";
            public const string View = "Iventory.Inventory.View";

            public const string Import = "Iventory.Inventory.Import";
            public const string Export = "Iventory.Inventory.Export";
        }

        public static class ProductCollection
        {
            public const string Create = "Iventory.ProductCollection.Create";
            public const string Update = "Iventory.ProductCollection.Update";
            public const string Delete = "Iventory.ProductCollection.Delete";
            public const string View = "Iventory.ProductCollection.View";

            public const string Import = "Iventory.ProductCollection.Import";
            public const string Export = "Iventory.ProductCollection.Export";
        }

        public static class WareHouseCollection
        {
            public const string Create = "Iventory.WareHouseCollection.Create";
            public const string Update = "Iventory.WareHouseCollection.Update";
            public const string Delete = "Iventory.WareHouseCollection.Delete";
            public const string View = "Iventory.WareHouseCollection.View";

            public const string Import = "Iventory.WareHouseCollection.Import";
            public const string Export = "Iventory.WareHouseCollection.Export";
        }

        /// <summary>
        /// Returns all permission values as a list.
        /// </summary>
        public static List<string> GetAllPermissions()
        {
            return new List<string>
            {
                // Product
                Product.Create, Product.Update, Product.Delete, Product.View,
                // Inventory
                Inventory.Create, Inventory.Update, Inventory.Delete, Inventory.View,
                // Product Collection
                ProductCollection.Create, ProductCollection.Update,ProductCollection.Delete, ProductCollection.View,
                // WareHouse Collection
                WareHouseCollection.Create, WareHouseCollection.Update, WareHouseCollection.Delete, WareHouseCollection.View,
            };
        }
    }
}
