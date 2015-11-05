using ClickBox.Web.Models;
using ClickBox.Web.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using Stripe;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ClickBox.Web.Controllers
{
    using System.Diagnostics.CodeAnalysis;

    using Messages;
    using QueueStorage;

    using Microsoft.WindowsAzure.Storage.Queue;

    [RequireHttps(Order = 1)]
    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    public class PurchaseController : Controller
    {
        private CloudTableClient _client;
        private CloudQueueClient _queueClient;

        public PurchaseController(CloudTableClient tableClient, CloudQueueClient queueClient)
        {
            _client = tableClient;
            _queueClient = queueClient;
        }

        // GET: Purchase
        public async Task<ActionResult> Index(string productId)
        {
            var toRet = await this._client.GetEntityByPropertyFilterAsync<Product>("Id", productId);
            if (toRet == null)
            {
                return View("InvalidProduct", new UnavailableProduct()
                {
                    HttpCode = 404,
                    ErrorMessage = "Product does not exist!",
                    
                });
            }

            if (!toRet.HasStripePaymentPage)
            {
                return View("InvalidProduct", new UnavailableProduct()
                {
                    HttpCode = 400,
                    ErrorMessage = "Invalid Product!",
                });
            }

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
                    //check if the clients exists with a paid up account and if so 
                    //skip the charge and only send the email reminding them.
                    //instead of charging and sending account create message we should
                    //send account-renewed message which will send a different email
                    var prod = await this._client.GetEntityByPropertyFilterAsync<Product>("Id", charge.ProductId);

                    var paidUpAccountExists = DoesPaidUpClientAccountExist(charge.Email, charge.ProductId);
                    if (paidUpAccountExists) {

                        dynamic noData = new
                        {
                            State = "Success",
                            Email = charge.Email,
                            ChargedAmount = 0,
                            SupportId = supportId
                        };

                        return new JsonResult() { Data = noData };
                    }

                    var myCharge = new StripeChargeCreateOptions();

                    myCharge.Amount = ((charge.Quantity * (int)charge.Price) * 100);
                    myCharge.Currency = "aud";
                    myCharge.Capture = true;
                    myCharge.Description = $"ProductID: {charge.ProductId} | SupportID: {supportId}";

                    myCharge.Source = new StripeSourceOptions()
                    {
                        TokenId = charge.TokenId
                    };

                    var chargeService = new StripeChargeService(MvcApplication.StripePurchaseString);

                    var chargeRequest = new ChargeRequest(charge.TokenId)
                    {
                        ProductId = charge.ProductId,
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

                    //TODO: SHOULD THIS BE A ACCOUNT CREATION OR RENEWAL
                    var account = new AccountCreationMessage()
                                      {
                                          AccountProductName = prod.Name,
                                          AccountEmail = charge.Email,
                                          AccountLicenseType = "Subscription",
                                          AccountName = charge.Contact,
                                          AccountOrganisation = charge.Organisation,
                                          AccountPassword = Guid.NewGuid().ToString(),
                                          PaymentReceived = true
                                      };

                    await this._client.InsertStorageEntityAsync(chargeRequest);
                    await this._client.InsertStorageEntityAsync(stripeChargeResponse);
                    await this._queueClient.SendMessageAync(account);

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

                await this._client.InsertStorageEntityAsync(failedCharge);

                dynamic data = new
                {
                    State = "Failed",
                    Email = charge.Email,
                    Error = stex.StripeError.Code ?? "Please contact QCAT Support",
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

                await this._client.InsertStorageEntityAsync(failedCharge);

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

        private bool DoesPaidUpClientAccountExist(string email, string productId)
        {
            return false;
        }
    }
}