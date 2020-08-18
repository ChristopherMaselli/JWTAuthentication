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
using Stripe.Checkout;

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
        public async Task<JsonResult> PurchaseItem(JsonMessage json)
        {
            PurchaseOrder purchaseOrder = JsonConvert.DeserializeObject<PurchaseOrder>(json.json);

            var tokenVar = purchaseOrder.tokenVar;
            Item[] items = purchaseOrder.Items;

            //Get the customer id
            var customerId = await GetCustomer(tokenVar);

            var pmlOptions = new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = "card",
            };

            //IF THERE ARENT ANY THAN THROW AN ERROR!!!

            var pmService = new PaymentMethodService();
            var paymentMethods = pmService.List(pmlOptions);

            var paymentIntents = new PaymentIntentService();
            var paymentIntent = paymentIntents.Create(new PaymentIntentCreateOptions
            {
                Customer = customerId,
                SetupFutureUsage = "off_session",
                Amount = await GetTotalAmount(items),
                Currency = "usd",
                PaymentMethod = paymentMethods.Data[0].Id,
                Description = "Name of items here"
            });

            return Json(new { client_secret = paymentIntent.ClientSecret });
        }


        [HttpPost("PurchaseSubscription")]
        public async Task<IActionResult> PurchaseSubscription(JsonMessage json)
        {
            PurchaseOrder purchaseOrder = JsonConvert.DeserializeObject<PurchaseOrder>(json.json);

            var tokenVar = purchaseOrder.tokenVar;
            Item[] items = purchaseOrder.Items;

            //Get the customer id
            var customerId = await GetCustomer(tokenVar);

            var pmlOptions = new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = "card",
            };

            //IF THERE ARENT ANY THAN THROW AN ERROR!!!

            var pmService = new PaymentMethodService();
            var paymentMethods = pmService.List(pmlOptions);

            foreach (Item i in items)
            {
                var purchaseItem = await _context.PurchaseItems.Where<PurchaseItem>(PurchaseItem => PurchaseItem.ItemCode == i.Id).FirstOrDefaultAsync<PurchaseItem>();
                var options = new SubscriptionCreateOptions
                {
                    Customer = customerId,
                    DefaultPaymentMethod = paymentMethods.Data[0].Id,
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                           Price = purchaseItem.PriceId,
                        },
                    },
                };
                var service = new SubscriptionService();
                Subscription subscription = service.Create(options);
            }

            IActionResult response = Ok(new { message = "Order Proccessed!" });
            return response;
        }

        [HttpPost("RegisterCard")]
        public async Task<string> RegisterCard(Model.Token tokenVar)
        {
            var customerId = await GetCustomer(tokenVar);

            //Check if they already have the card in perhaps so it doesn't make repeats?

            var options = new SetupIntentCreateOptions
            {
                Customer = customerId,
            };
            var service = new SetupIntentService();
            var intent = service.Create(options);
            var clientSecret = intent.ClientSecret;

            return clientSecret;
        }

        public async Task<string> GetCustomer(Model.Token tokenVar)
        {
            var decodeToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenVar.token);
            IList<Claim> claim = decodeToken.Claims.ToList();
            var userName = claim[0].Value;
            var password = claim[1].Value;

            UserAccount userAccount = await _context.UserAccounts.Where<UserAccount>(UserAccount => UserAccount.UserName == userName).FirstOrDefaultAsync<UserAccount>();
            StripeCustomer customerObj = await _context.StripeCustomers.Where<StripeCustomer>(StripeCustomer => StripeCustomer.UserId == userAccount.Id).FirstOrDefaultAsync<StripeCustomer>();

            string customerId;

            if (customerObj == null)
            {
                var customers = new CustomerService();
                var customer = customers.Create(new CustomerCreateOptions());
                customerId = customer.Id;

                //Add the new customer id to the database!
                StripeCustomer regiCust = new StripeCustomer();
                regiCust.UserId = userAccount.Id;
                regiCust.CustomerCode = customerId;

                //Add the data to the Database and Save
                _context.StripeCustomers.Add(regiCust);
                await _context.SaveChangesAsync();
            }
            else
            {
                customerId = customerObj.CustomerCode;
            }

            return customerId;
        }

        public async Task<long> GetTotalAmount(Item[] items)
        {
            var amount = 0;

            foreach (Item i in items)
            {
                var purchaseItem = await _context.PurchaseItems.Where<PurchaseItem>(PurchaseItem => PurchaseItem.ItemCode == i.Id).FirstOrDefaultAsync<PurchaseItem>();

                amount += (purchaseItem.ItemCost) * (i.Qty);
            }

            return amount;
        }


        public class Item
        {
            public string Id { get; set; }
            public int Qty { get; set; }
        }
        public class PaymentIntentCreateRequest
        {
            [JsonProperty("items")]
            public Item[] Items { get; set; }
        }

        public class PurchaseOrder
        {
            public Item[] Items { get; set; }

            public Model.Token tokenVar { get; set; }
        }

        public class JsonMessage
        {
            public string json { get; set; }
        }
    }
}