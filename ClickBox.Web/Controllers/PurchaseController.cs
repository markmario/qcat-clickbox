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
            var supportId = Guid.NewGuid();

            try
            {
                return await Task.Run<JsonResult>(async () =>
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

                    var chargeRequest = new ChargeRequest(charge.TokenId)
                    {
                        Amount = myCharge.Amount,
                        Currency = myCharge.Currency,
                        Quantity = charge.Quantity,
                        Price = charge.Price,
                        Organisation = charge.Organisation,
                        SupportId = supportId,
                        Email = charge.Email,
                        DateTimeOfRequest = DateTime.Now
                    };

                    StripeCharge stripeCharge = chargeService.Create(myCharge);

                    var stripeChargeResponse = new SuccessfulStripeCharge(charge.TokenId)
                    {
                        SupportId = supportId,
                        Id = stripeCharge.Id,
                        DateTimeCreated = stripeCharge.Created
                    };

                    await client.InsertStorageEntityAsync(chargeRequest);
                    await client.InsertStorageEntityAsync(stripeChargeResponse);

                    dynamic data = new
                    {
                        State = "Success",
                        Email = charge.Email,
                        ChargedAmount = (stripeCharge.Amount / 100),
                        SupportId = supportId
                    };

                    return new JsonResult() { Data = data };
                });
            }
            catch(StripeException stex)
            {
                var failedCharge = new FailedStripeCharge<StripeException>(charge.TokenId)
                {
                    Exception = stex,
                    ExceptionMessage = stex.Message,
                    HttpStatusCode = stex.HttpStatusCode.ToString(),
                    ErrorMessage = stex.StripeError.Message,
                    ErrorCode = stex.StripeError.Code,
                    Error = stex.StripeError.Error,
                    SupportId = supportId
                };

                await client.InsertStorageEntityAsync(failedCharge);

                dynamic data = new
                {
                    State = "Failed",
                    Email = charge.Email,
                    Error = stex.StripeError.Code != null ? stex.StripeError.Code : "Please contact QCAT Support",
                    SupportId = supportId
                };
                return new JsonResult() { Data = data };
            }
            catch (Exception ex)
            {
                var failedCharge = new FailedStripeCharge<Exception>(charge.TokenId)
                {
                    Exception = ex,
                    ExceptionMessage = ex.Message,
                    HttpStatusCode = FailedStripeCharge<Exception>.NotApplicableDescriptor,
                    ErrorMessage = FailedStripeCharge<Exception>.NotApplicableDescriptor,
                    ErrorCode = FailedStripeCharge<Exception>.NotApplicableDescriptor,
                    Error = FailedStripeCharge<Exception>.NotApplicableDescriptor,
                    SupportId = supportId
                };

                await client.InsertStorageEntityAsync(failedCharge);

                dynamic data = new
                {
                    State = "Failed",
                    Email = charge.Email,
                    Error = "Please contact QCAT support",
                    SupportId = supportId
                };
                return new JsonResult() { Data = data };
            }
        }
    }
}