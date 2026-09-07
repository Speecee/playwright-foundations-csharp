# Playwright Foundations in C# / .NET Test

## Where are the Playwright tests?

The root of this repo is the application to test (see [Getting Started](#getting-started) below).
Since Playwright tests are end-to-end, they are
generally not in the same solution as the system under test (SUT).

The Playwright tests for this application are in the [e2e](e2e/readme) folder and the
readme in that folder has lots of notes about the tests.

## Getting Started

This is an Aspire-based solution, so you should be able to run it
as long as you have [the Aspire prerequisites](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=windows&pivots=visual-studio#install-net-aspire)
installed.

If you're running the solution for the first time, there may be some
extra startup time pulling the container images being used:

- `postgres` (the database)
- `smtp4dev` (a fake / dev email server with a UI)

To make sure you can do everything with Playwright, check [its
setup requirements](https://playwright.dev/dotnet/docs/intro).

## Features

This is a simple e-commerce application that has a few features
that we want to explore automated end-to-end testing strategies for
with Playwright.

Here are the features:

- **API**
  - `GET` based on category (or "all") and by id allow anonymous requests
  - `POST` requires authentication and an `admin` role
  - Validation will be done with [FluentValidation](https://docs.fluentvalidation.net/en/latest/index.html) and errors returned as a `400 Bad Request` with `ProblemDetails`
  - A `GET` with a category of something other than "all", "boots", "equip", or "kayak" will return a `500 internal server error` with `ProblemDetails`
  - Data is refreshed to a known state as the app starts
  - Authentication provided by OIDC via the [demo Duende Identity Server](https://demo.duendesoftware.com)
  - A custom claims transformer will add the `admin` role to "Bob Smith" and any authentication via Google
- **WebApp**
  - The home page and listing pages will show a subset of products
  - There is a page at `/Admin` that will show a list of products that can be edited or added to
  - If you navigate to `/Admin` without the admin role, you should see an `AccessDenied` page
  - Any validation errors from the API should be displayed on the admin section edit pages
  - Can add items to cart and see a summary of the cart (shows when empty too)
  - Can submit an order or cancel the order and clear the cart
  - A submitted order will send a fake email

## VS Code Setup

The same instructions as above (Getting Started) apply here,
but the following extension should be installed
(it includes some other extensions):

- [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)

Then just hit `F5`.

## Deploying to Azure to Run Tests 

This application can be deployed easily to Azure Container Apps to
run Playwright tests against it.  The following steps are needed to
deploy the application to Azure - this assumes you have the
[Azure CLI (`azd`)](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) installed, and an active subscription in Azure.

Run these commands from the root directory of the repo / solution.

```bash
azd init
azd up
```

The above commands will ask you some questions that should be pretty
easy to answer (use aspire, which subscription, etc).  For more details
on deploying Aspire apps to Azure, see either [the documentation](https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azure/aca-deployment) or
my Pluralsight course 
[.NET Cloud-native Development: Aspire Build and Deployment Options](https://app.pluralsight.com/library/courses/cloud-native-development-dot-net-deployment-options/table-of-contents).

If you have updates you make to the application and then want to deploy
them after already having done `azd up`, you can use the following command to update the deployment:

```bash
azd deploy
```

For details on running the Playwright tests against the deployed
application, see the [readme in the e2e folder](e2e/readme.md).

### Removing the Azure Deployment

Once you are done with your experiments and testing, remove the Azure resources with
the following command:

```bash
azd down
```

## Data and EF Core Migrations

The `dotnet ef` tool is used to manage EF Core migrations.  The following command is used to create migrations (from the `CarvedRock.Data` folder).

```bash
dotnet ef migrations add Initial -s ../CarvedRock.Api
```

## Verifiying Emails

The very simple email functionality is done using a template
from [this GitHub repo](https://github.com/leemunroe/responsive-html-email-template)
and the [smtp4dev](https://github.com/rnwood/smtp4dev)
service that we're running in Aspire using its container image.

Click the link in the Aspire Dashboard for the smtpserver and you can
see any emails that have been sent by the application (they don't go
anywhere other than this UI).
