using ClickBox.Web.Models;
using ClickBox.Web.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ClickBox.Web.Controllers
{
    [RequireHttps(Order = 1)]
    public class PurchaseController : Controller
    {
        private CloudTableClient client;

        public PurchaseController(CloudTableClient client)
        {
            this.client = client;
        }

        // GET: Purchase
        public async Task<ActionResult> Index(string productId)
        {
            var toRet = await client.GetEntityByPropertyFilterAsync<Product>("Id", productId);
            var model = new { Product = toRet };
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> Charge(ChargeData charge)
        {
            try
            {
                return await Task.Run<JsonResult>(() =>
                {
                    var myCharge = new StripeChargeCreateOptions();

                    myCharge.Amount = ((charge.Quantity * (int)charge.Price) * 100);
                    myCharge.Currency = "aud";

                    // set this if you want to
                    //myCharge.Description = charge.;

                    myCharge.Source = new StripeSourceOptions()
                    {
                        TokenId = charge.TokenId
                    };

                    var chargeService = new StripeChargeService("sk_test_IHVDwrGtPOznVB0BsyEmqCjC");
                    //var chargeService = new StripeChargeService("bob");

                    StripeCharge stripeCharge = chargeService.Create(myCharge);

                    dynamic data = new
                    {
                        State = "Success",
                        Email = charge.Email,
                        ChargedAmount = (stripeCharge.Amount / 100),
                        SupportId = Guid.NewGuid()
                    };

                    return new JsonResult() { Data = data };
                });
            }
            catch(StripeException stex)
            {
                dynamic data = new
                {
                    State = "Failed",
                    Email = charge.Email,
                    Error = stex.StripeError.Code != null ? stex.StripeError.Code : "Please contact QCAT Support",
                    SupportId = Guid.NewGuid()
                };
                return new JsonResult() { Data = data };
            }
            catch (Exception ex)
            {
                dynamic data = new
                {
                    State = "Failed",
                    Email = charge.Email,
                    Error = "Please contact QCAT support",
                    SupportId = Guid.NewGuid()
                };
                return new JsonResult() { Data = data };
            }
        }
    }
}