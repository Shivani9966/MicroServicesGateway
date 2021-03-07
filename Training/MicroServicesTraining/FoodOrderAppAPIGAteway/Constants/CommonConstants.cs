using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderAppAPIGAteway.Constants
{
    public static class CommonConstants
    {
        public static string ActionNotAllowedForRole = "Your role is not allowed for this action!";

        #region DriverAPIs
        public static string GetOrderByDriverId = "Driver:GetOrderByDriverId";
        public static string GetDriverById = "Driver:GetDriverById";
        #endregion

        #region ResturantsAPIs
        public static string RestaurantGetAll = "Restaurant:GetAll";
        #endregion

        #region CustomerAPIs
        public static string GetCustomerById = "Customer:GetCustomerById";
        #endregion

        #region OrderAPIs
        public static string AddOrder = "Order:Add";
        public static string GetOrderByCustomerId = "Order:GetOrderByCustomerId";
        public static string GetOrderById = "Order:GetOrderById";
        public static string UpdateOrder = "Order:UpdateOrder";
        public static string GetOrderByRestaurantId = "Order:GetOrderByRestaurantId";
        #endregion
    }
}
