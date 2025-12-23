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
    public static class SaleMemberPermissions
    {
        public const string Module = "SaleMember";
        public const string ClaimType = "Permission";
        public const string GroupName = "Sale Member Permissions";
        public const string Description = "Permissions related to Sale Member management.";
        public const string Category = "Sales Management";
        public const string Role = "Sale Member";

        public static class Customer
        {
            public const string Create = "SaleMember.Customer.Create";
            public const string Update = "SaleMember.Customer.Update";
            public const string Delete = "SaleMember.Customer.Delete";
            public const string View = "SaleMember.Customer.View";

            public const string Import = "SaleMember.Customer.Import";
            public const string Export = "SaleMember.Customer.Export";
        }

        public static class Order
        {
            public const string Create = "SaleMember.Order.Create";
            public const string Update = "SaleMember.Order.Update";
            public const string Delete = "SaleMember.Order.Delete";
            public const string View = "SaleMember.Order.View";

            public const string Import = "SaleMember.Order.Import";
            public const string Export = "SaleMember.Order.Export";
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
            };
        }
    }
}
