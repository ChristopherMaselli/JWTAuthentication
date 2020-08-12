using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using JWTAuthentication.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stripe;

namespace JWTAuthentication.Controllers
{
    [Route("api/Stripe")]
    [ApiController]
    public class StripeController : Controller
    {
        private IConfiguration _config;

        private readonly JWTAuthenticationContext _context;

        public StripeController(IConfiguration config, JWTAuthenticationContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("PurchaseItem")]
        public async Task<ActionResult> PurchaseItem(string token, Item[] items)
        {
            //Get the customer id
            var customerId = await GetCustomer(token);

            //Get the total cost of all of the items
            var amount = await CalculateOrderAmount(items);

            //Get list of customer's payment methods (cards)
            var pmlOptions = new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = "card",
            };

            var pmService = new PaymentMethodService();
            var paymentMethods = pmService.List(pmlOptions);

            //Charge the payment method with the amount of the product
            try
            {
                var service = new PaymentIntentService();
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount, //1099 = 10.99
                    Currency = "usd",
                    Customer = customerId,
                    PaymentMethod = paymentMethods.Data[0].Id,
                    Confirm = true,
                    OffSession = true,
                };
                service.Create(options);
                ActionResult response = Ok(new { message = "Order Proccessed!" });
                return response;
            }
            catch (StripeException e) //Send these back to user in message later
            {
                switch (e.StripeError.Type)
                {
                    case "card_error":
                        // Error code will be authentication_required if authentication is needed
                        Console.WriteLine("Error code: " + e.StripeError.Code);
                        var paymentIntentId = e.StripeError.PaymentIntent.Id;
                        var service = new PaymentIntentService();
                        var paymentIntent = service.Get(paymentIntentId);

                        Console.WriteLine(paymentIntent.Id);
                        break;
                    default:
                        break;
                }
                ActionResult response = Ok(new { message = "Error code: " + e.StripeError.Code});
                return response;
            }      
        }

        [HttpPost("PurchaseSubscription")]
        public async Task<IActionResult> PurchaseSubscription(string token, string productCode)
        {
            var customerId = await GetCustomer(token);
            var options = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                       Price = productCode,
                    },
                },
            };
            var service = new SubscriptionService();
            Subscription subscription = service.Create(options);

            IActionResult response = Ok(new { message = "Order Proccessed!" });
            return response;
        }

        [HttpPost("RegisterCard")]
        public async Task<ActionResult> RegisterCard(string token, string productCode)
        {
            var customerId = await GetCustomer(token);

            var options = new SetupIntentCreateOptions
            {
                Customer = customerId,
            };
            var service = new SetupIntentService();
            var intent = service.Create(options);
            ViewData["ClientSecret"] = intent.ClientSecret;
            return View();

            ActionResult response = Ok(new { message = "Card Saved!" });
            return response;
        }

        public async Task<string> GetCustomer(string token)
        {
            var decodeToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            IList<Claim> claim = decodeToken.Claims.ToList();
            var userName = claim[0].Value;
            var password = claim[1].Value;

            UserAccount userAccount = await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.UserName == userName).FirstOrDefaultAsync<UserAccount>();
            StripeCustomer customerObj = await _context.StripeCustomers.Where<StripeCustomer>(StripeCustomer => StripeCustomer.UserId == userAccount.Id).FirstOrDefaultAsync<StripeCustomer>();

            var customerId = customerObj.CustomerCode;

            if (customerId == null)
            {
                var customers = new CustomerService();
                var customer = customers.Create(new CustomerCreateOptions());
                customerId = customer.Id;
            };
            return customerId;
        }

        private async Task<int> CalculateOrderAmount(Item[] items)
        {
            //This function will be used for ordering store items rather than subscriptions
            var priceTotal = 0;
            PurchaseItem purchaseItem = new PurchaseItem();
            foreach (Item i in items)
            {
                purchaseItem = await _context.PurchaseItems.Where<PurchaseItem>(PurchaseItem => PurchaseItem.ItemId == i.Id).FirstOrDefaultAsync<PurchaseItem>();
                priceTotal += purchaseItem.ItemAmount;
            }
            return priceTotal;
        }

        public class Item
        {
            [JsonProperty("id")]
            public string Id { get; set; }
        }
        public class PaymentIntentCreateRequest
        {
            [JsonProperty("items")]
            public Item[] Items { get; set; }
        }
    }
}