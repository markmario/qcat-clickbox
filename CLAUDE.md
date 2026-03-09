# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ClickBox is a software licensing and account management system for QCAT Pty Ltd. It handles license generation, account creation, usage tracking (click counts, document coding, batch isolation), email notifications, and Stripe payment processing for QCAT's software products (PageMaker, PageMerger, PageStreamer, CaseHub.io Razor Review).

## Build Commands

```bash
# Build entire solution
msbuild Licencing.sln

# Build specific project
msbuild ClickBox.Web/ClickBox.Web.csproj

# Restore NuGet packages (uses packages.config, not PackageReference)
nuget restore Licencing.sln
```

## Tech Stack

- **.NET Framework 4.8.1** (not .NET Core) — all projects target this
- **ASP.NET MVC 5 + Web API 2** for the web application
- **Azure Table Storage** as the primary data store (not SQL)
- **Azure WebJobs** for background processing (account creation, email sending)
- **Autofac** for dependency injection
- **AutoMapper** for model mapping
- **Rhino.Licensing** for license key generation
- **Mandrill** (Mailchimp) for transactional emails
- **Stripe** for payment processing
- **NuGet packages.config** (not PackageReference) for dependency management

## Architecture

### Solution Structure (Licencing.sln)

- **ClickBox.Web** — ASP.NET MVC/Web API app. Main entry point. Contains controllers, models, infrastructure (auth, HTTPS enforcement, token validation), and Azure Table Storage utilities.
- **ClickBox.CreateAccountsWebJob** — Azure WebJob that processes account creation messages from the `create-account` queue and sends confirmation emails via the `account-created` queue.
- **ClickBox.Email** — Email sending library using Mandrill API. Handles trial success emails, monthly ODES reports, and generic emails. Embeds HTML templates from Resources.resx.
- **ClickBox.Messages** — Shared message contracts (queue message DTOs) used between the web app and WebJob.
- **ClickBox.Util** — Shared utilities (Base32 encoding, GUID extensions).
- **Odes.Licence.Model** — Domain model library for licensing (LicenseRequest, BatchIsolated, DocumentCoded, Product, CredentialAgent). Referenced by both the web app and WebJob.
- **Odes.License.Updater** — License update utility.
- **ClickBoxOdesClickCountReports** — Reporting tool for click count statistics.
- **ClickBox.Linqpad.Reports** — LINQPad scripts for ad-hoc reporting and data queries.

### Key Data Flow

1. User creates account via web UI → `AccountController` places `AccountCreationMessage` on `create-account` Azure queue
2. `ClickBox.CreateAccountsWebJob.Functions.ProcessAccountCreationMessage` picks up the message, creates the account in Azure Table Storage, and places a `SendAccountAndDownloadInstructionsMessage` on the `account-created` queue
3. `Functions.ProcessSendAccountAndDownloadInstructionsMessage` picks up that message and sends the welcome email via Mandrill

### License API Flow

The `LicenseController` (Web API at `api/License`) handles license requests from desktop clients:
- Validates credentials against `UserAccounts` table
- Checks seat allocation and handles renewals
- Generates license keys using `Rhino.Licensing.LicenseGenerator`
- Stores `ClientIssuedLicense` records and tracks issued license count

### Web API Security

- `TokenValidationAttribute` and `CustomHttpsAttribute` are registered as global Web API filters
- `RequireHttps` attribute enforced on controllers
- API routes: `api/{controller}/{id}`
- MVC default route: `{controller}/{action}/{id}` (defaults to `Account/Login`)

### Azure Table Storage

All data persistence uses Azure Table Storage (not SQL). The `TableStorageUtil.cs` contains extension methods for CRUD operations on `CloudTableClient`. Tables are primed on app startup in `Global.asax.cs`.

### Configuration

- Debug mode reads connection strings from a local file referenced via DropBox path (base64-encoded in a local file)
- Production mode uses standard `ConnectionStrings` from web.config
- Runtime mode controlled by `AppSettings["Runtime"]` (`"debug"` vs production)

### Shared Assembly Info

Projects share version info via a linked `CommonAssemblyInfo.cs` file at the solution root.
