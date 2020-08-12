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
        [HttpPost("PurchaseItem")]
        public ActionResult PurchaseItem(PaymentIntentCreateRequest request)
        {
            var customers = new CustomerService();
            var customer = customers.Create(new CustomerCreateOptions());
            var paymentIntents = new PaymentIntentService();
            var paymentIntent = paymentIntents.Create(new PaymentIntentCreateOptions
            {
                Customer = customer.Id,
                SetupFutureUsage = "off_session",
                Amount = CalculateOrderAmount(request.Items),
                Currency = "usd",
            });

            return Json(new { clientSecret = paymentIntent.ClientSecret });
        }

        [HttpPost("PurchaseSubscription")]
        public ActionResult PurchaseSubscription(PaymentIntentCreateRequest request)
        {
            var customers = new CustomerService();
            var customer = customers.Create(new CustomerCreateOptions());
            var options = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                       Price = "price_1HDWISHb4Zd1savwuVLDE8zN",
                    },
                },
            };
            var service = new SubscriptionService();
            Subscription subscription = service.Create(options);

            return Json(new { clientSecret = options.ClientSecret });
        }

        private int CalculateOrderAmount(Item[] items)
        {
            // Replace this constant with a calculation of the order's amount
            // Calculate the order total on the server to prevent
            // people from directly manipulating the amount on the client
            return 1400;
        }

        public void ChargeCustomer(string customerId)
        {
            // Lookup the payment methods available for the customer
            var paymentMethods = new PaymentMethodService();
            var availableMethods = paymentMethods.List(new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = "card",
            });
            // Charge the customer and payment method immediately
            var paymentIntents = new PaymentIntentService();
            var paymentIntent = paymentIntents.Create(new PaymentIntentCreateOptions
            {
                Amount = 1099,
                Currency = "usd",
                Customer = customerId,
                PaymentMethod = availableMethods.Data[0].Id,
                OffSession = true,
                Confirm = true
            });
            if (paymentIntent.Status == "succeeded")
                Console.WriteLine("✅ Successfully charged card off session");
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

        /*
        [HttpPost]
        public IActionResult ChargeChange()
        {
            var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], WebhookSecret, throwOnApiVersionMismatch: true);
                Charge charge = (Charge)stripeEvent.Data.Object;
                switch (charge.Status)
                {
                    case "succeeded":
                        //This is an example of what to do after a charge is successful
                        charge.Metadata.TryGetValue("Product", out string Product);
                        charge.Metadata.TryGetValue("Quantity", out string Quantity);
                        Database.ReduceStock(Product, Quantity);
                        break;
                    case "failed":
                        //Code to execute on a failed charge
                        break;
                }
            }
            catch (Exception e)
            {
                e.Ship(HttpContext);
                return BadRequest();
            }
            return Ok();
        }
        */
        /*
        public class StripeCharge
        {
            public string cardNumber { get; set; }

            public int cardExpYear { get; set; }

            public int cardExpMonth { get; set; }

            public string cardCvc { get; set; }

            public string stripeEmail { get; set; }

            public int amount { get; set; }

            public string product { get; set; }

            public string quantity { get; set; }

            public string currency { get; set; }

            public string source { get; set; }

            public string description { get; set; }
        }
    } */
}