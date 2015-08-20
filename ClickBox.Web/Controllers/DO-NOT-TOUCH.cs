using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickBox.Web.Controllers
{
    /// <summary>
    /// ELCAMINO ACCOUNT CONTROLLER CODE NOT CURRENTLY USED
    /// WILL BE LATER...............................
    /// FEAR OF DEATH
    /// FEAR OF DEATH
    /// FEAR OF DEATH
    /// </summary>
    public class DO_NOT_TOUCH
    {
        #region Not Using
        ////
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return this.View("Error");
        //    }

        //    IdentityResult result = await this.UserManager.ConfirmEmailAsync(userId, code);
        //    if (result.Succeeded)
        //    {
        //        return this.View("ConfirmEmail");
        //    }
        //    else
        //    {
        //        this.AddErrors(result);
        //        return this.View();
        //    }
        //}

        ////
        //// GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return this.View();
        //}

        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (this.ModelState.IsValid)
        //    {
        //        var user = await this.UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await this.UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            this.ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
        //            return this.View();
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return System.Web.UI.WebControls.View(model);
        //}

        ////
        //// GET: /Account/ForgotPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation()
        //{
        //    return this.View();
        //}

        ////
        //// GET: /Account/ResetPassword
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    if (code == null)
        //    {
        //        return this.View("Error");
        //    }
        //    return this.View();
        //}

        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (this.ModelState.IsValid)
        //    {
        //        var user = await this.UserManager.FindByNameAsync(model.Email);
        //        if (user == null)
        //        {
        //            this.ModelState.AddModelError("", "No user found.");
        //            return this.View();
        //        }
        //        IdentityResult result = await this.UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //        if (result.Succeeded)
        //        {
        //            return this.RedirectToAction("ResetPasswordConfirmation", "Account");
        //        }
        //        else
        //        {
        //            this.AddErrors(result);
        //            return this.View();
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return System.Web.UI.WebControls.View(model);
        //}

        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return this.View();
        //}

        ////
        //// POST: /Account/Disassociate
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        //{
        //    ManageMessageId? message = null;
        //    IdentityResult result = await this.UserManager.RemoveLoginAsync(this.User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        //    if (result.Succeeded)
        //    {
        //        var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());
        //        await this.SignInAsync(user, isPersistent: false);
        //        message = ManageMessageId.RemoveLoginSuccess;
        //    }
        //    else
        //    {
        //        message = ManageMessageId.Error;
        //    }
        //    return this.RedirectToAction("Manage", new { Message = message });
        //}

        ////
        //// GET: /Account/Manage
        //public ActionResult Manage(ManageMessageId? message)
        //{
        //    this.ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : message == ManageMessageId.Error ? "An error has occurred."
        //        : "";
        //    this.ViewBag.HasLocalPassword = this.HasPassword();
        //    this.ViewBag.ReturnUrl = this.Url.Action("Manage");
        //    return this.View();
        //}

        ////
        //// POST: /Account/Manage
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Manage(ManageUserViewModel model)
        //{
        //    bool hasPassword = this.HasPassword();
        //    this.ViewBag.HasLocalPassword = hasPassword;
        //    this.ViewBag.ReturnUrl = this.Url.Action("Manage");
        //    if (hasPassword)
        //    {
        //        if (this.ModelState.IsValid)
        //        {
        //            IdentityResult result = await this.UserManager.ChangePasswordAsync(this.User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
        //            if (result.Succeeded)
        //            {
        //                var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());
        //                await this.SignInAsync(user, isPersistent: false);
        //                return this.RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //            }
        //            else
        //            {
        //                this.AddErrors(result);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // User does not have a password so remove any validation errors caused by a missing OldPassword field
        //        ModelState state = this.ModelState["OldPassword"];
        //        if (state != null)
        //        {
        //            state.Errors.Clear();
        //        }

        //        if (this.ModelState.IsValid)
        //        {
        //            IdentityResult result = await this.UserManager.AddPasswordAsync(this.User.Identity.GetUserId(), model.NewPassword);
        //            if (result.Succeeded)
        //            {
        //                return this.RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //            }
        //            else
        //            {
        //                this.AddErrors(result);
        //            }
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return System.Web.UI.WebControls.View(model);
        //}

        ////
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, this.Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return this.RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var user = await this.UserManager.FindAsync(loginInfo.Login);
        //    if (user != null)
        //    {
        //        await this.SignInAsync(user, isPersistent: false);
        //        return this.RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        // If the user does not have an account, then prompt the user to create an account
        //        this.ViewBag.ReturnUrl = returnUrl;
        //        this.ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //        return System.Web.UI.WebControls.View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        ////
        //// POST: /Account/LinkLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LinkLogin(string provider)
        //{
        //    // Request a redirect to the external login provider to link a login for the current user
        //    return new ChallengeResult(provider, this.Url.Action("LinkLoginCallback", "Account"), this.User.Identity.GetUserId());
        //}

        ////
        //// GET: /Account/LinkLoginCallback
        //public async Task<ActionResult> LinkLoginCallback()
        //{
        //    var loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, this.User.Identity.GetUserId());
        //    if (loginInfo == null)
        //    {
        //        return this.RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //    }
        //    IdentityResult result = await this.UserManager.AddLoginAsync(this.User.Identity.GetUserId(), loginInfo.Login);
        //    if (result.Succeeded)
        //    {
        //        return this.RedirectToAction("Manage");
        //    }
        //    return this.RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (this.User.Identity.IsAuthenticated)
        //    {
        //        return this.RedirectToAction("Manage");
        //    }

        //    if (this.ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await this.AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return this.View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
        //        IdentityResult result = await this.UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await this.UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await this.SignInAsync(user, isPersistent: false);

        //                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //                // Send an email with this link
        //                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //                // SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");

        //                return this.RedirectToLocal(returnUrl);
        //            }
        //        }
        //        this.AddErrors(result);
        //    }

        //    this.ViewBag.ReturnUrl = returnUrl;
        //    return System.Web.UI.WebControls.View(model);
        //}

        ////
        //// POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LogOff()
        //{
        //    this.AuthenticationManager.SignOut();
        //    return this.RedirectToAction("Index", "Home");
        //}

        ////
        //// GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return this.View();
        //}

        //[ChildActionOnly]
        //public ActionResult RemoveAccountList()
        //{
        //    var linkedAccounts = this.UserManager.GetLogins(this.User.Identity.GetUserId());
        //    this.ViewBag.ShowRemoveButton = this.HasPassword() || linkedAccounts.Count > 1;
        //    return (ActionResult)this.PartialView("_RemoveAccountPartial", linkedAccounts);
        //} 
        #endregion

    }
}