using FoodOrderAppAPIGAteway.Constants;
using FoodOrderAppAPIGAteway.Models;
using FoodOrderAppAPIGAteway.RoleHelper;
using FoodOrderAppAPIGAteway.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodOrderAppAPIGAteway.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        Httpclient httpclient = new Httpclient();
        private readonly IConfiguration configuration;

        public CustomerController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        /// <summary>
        /// Get All Restaurants List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllResturants")]
        public async Task<ActionResult<List<Restaurant>>> GetAllResturant()
        {
            if (GetUserRole() == UserRolesConstants.Customer)
                return Ok(await httpclient.RunAsyncGetAll<Restaurant>(configuration.GetValue<string>(
                CommonConstants.RestaurantGetAll)));
            return Unauthorized(CommonConstants.ActionNotAllowedForRole);
        }

        /// <summary>
        /// Order Food from Customer
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OrderFood")]
        public async Task<ActionResult<bool>> OrderFood(OrderRequestModel orderRequest)
        {
            if (GetUserRole() == UserRolesConstants.Customer)
            {
                Customer customer = await httpclient.RunAsyncGet<int, Customer>(configuration.GetValue<string>(CommonConstants.GetCustomerById), orderRequest.CustomerId);
                Orders order = new Orders
                {
                    CustomerName = customer.FirstName + ' ' + customer.LastName,
                    RestaurantName = orderRequest.RestaurantName,
                    Price = orderRequest.Price,
                    Quantity = orderRequest.Quantity,
                    RestaurantId = orderRequest.RestaurantId,
                    CustomerId = orderRequest.CustomerId
                };
                return Ok(await httpclient.RunAsyncPost<Orders, bool>(configuration.GetValue<string>(CommonConstants.AddOrder), order));
            }
            return Unauthorized(CommonConstants.ActionNotAllowedForRole);
        }

        /// <summary>
        /// Get current orders for customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllCurrentOrders")]
        public async Task<ActionResult<List<Orders>>> GetAllCurrentOrders(int customerId)
        {
            if (GetUserRole() == UserRolesConstants.Customer)
            {
                List<Orders> orders = await httpclient.RunAsyncGet<int, List<Orders>>(configuration.GetValue<string>(CommonConstants.GetOrderByCustomerId), customerId);
                if (orders != null && orders.Count > 0)
                    return Ok(orders.Where(x => x.IsOrderDelivered == false).ToList());
                return null;
            }
            return Unauthorized(CommonConstants.ActionNotAllowedForRole);
        }

        /// <summary>
        /// Get delivered orders for customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllDeliveredOrders")]
        public async Task<ActionResult<List<Orders>>> GetAllDeliveredOrders(int customerId)
        {
            if (GetUserRole() == UserRolesConstants.Customer)
            {
                List<Orders> orders = await httpclient.RunAsyncGet<int, List<Orders>>(configuration.GetValue<string>(CommonConstants.GetOrderByCustomerId), customerId);
                if (orders != null && orders.Count > 0)
                    return Ok(orders.Where(x => x.IsOrderDelivered == true).ToList());
                return null;
            }
            return Unauthorized(CommonConstants.ActionNotAllowedForRole);
        }

        private string GetUserRole()
        {
            if (HttpContext != null)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    return UserRoleHelper.GetUserRole(identity);
                }
            }
            return string.Empty;
        }
    }
}
