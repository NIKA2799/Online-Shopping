Online Shopping Platform – ASP.NET Core

Clean, modular e-commerce backend built with ASP.NET Core 8, Entity Framework Core (SQL Server), Identity + JWT, and CQRS + Repository + UnitOfWork. Includes FluentValidation, AutoMapper, Serilog, and xUnit/Moq unit tests.

📄 Full project summary also lives in my CV.

✨ Features

Authentication & Authorization

ASP.NET Identity with JWT tokens

Role-based access control (Admin, Customer)

Email confirmation & password reset flow (links returned via API)

Catalog

Products, Categories, Product–Category (many-to-many), Reviews, Wishlist

Search, filter, price range, featured & related products

Cart & Checkout

Persistent cart per customer

Checkout creates Order + OrderDetails, updates inventory, clears cart

Orders & Shipping

Order lifecycle (Pending → Processing → Shipped → Delivered → Cancelled)

Shipping info & tracking model

Discounts & Inventory

Coupon/percentage discounts

Stock adjustments & low-stock detector

Architecture

CQRS (Command / Query services)

Repository + UnitOfWork

FluentValidation for request models

AutoMapper for DTO ↔ Entity mapping

Serilog structured logging

Optional Audit hooks

Testing

Unit tests with xUnit + Moq

🧱 Solution Layout
/Dto                 # DTO / ViewModel / Validators
/Interface           # Abstractions: Repositories, Services, CQRS interfaces
/Repositories        # EF Core Repositories + UnitOfWork + DbContext
/Service             # Command/Query services, Auth/Token, Audit, etc.
/Unit Test           # xUnit tests and Moq setups
/Webdemo             # ASP.NET Core Web API (Controllers, DI, Middleware, Startup)

🧰 Tech Stack

Runtime: .NET 8

Web: ASP.NET Core Web API

Data: EF Core (SQL Server)

Auth: ASP.NET Identity + JWT Bearer

Patterns: DI, SOLID, CQRS, Repository, UnitOfWork

Validation & Mapping: FluentValidation, AutoMapper

Logging: Serilog

Testing: xUnit, Moq

Localization: en-US, ka-GE (RequestLocalization middleware)

✅ Prerequisites

.NET SDK 8.x

SQL Server (LocalDB / Developer / Azure SQL)

(Recommended) dotnet user-secrets for dev secrets

⚙️ Setup
# 1) Clone
git clone https://github.com/<your-account>/<your-repo>.git
cd <your-repo>

# 2) Initialize user secrets (development)
dotnet user-secrets init --project Webdemo

# 3) Configure secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-sql-connection>" --project Webdemo
dotnet user-secrets set "Jwt:Key" "<a-very-strong-secret>" --project Webdemo
dotnet user-secrets set "Jwt:Issuer" "Webdemo" --project Webdemo
dotnet user-secrets set "Jwt:Audience" "Webdemo" --project Webdemo
dotnet user-secrets set "AdminSettings:Email" "admin@shop.com" --project Webdemo
dotnet user-secrets set "AdminSettings:Password" "StrongPassword1!" --project Webdemo

# 4) Apply migrations (ensure Webdemo is startup project)
dotnet ef database update -s Webdemo -p Repositories  # adjust if DbContext lives in Repositories

# 5) Run
dotnet run --project Webdemo


appsettings.Development.json (excerpt)

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ShopDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "use-user-secrets-for-dev",
    "Issuer": "Webdemo",
    "Audience": "Webdemo"
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [ { "Name": "Console" } ]
  },
  "Localization": {
    "DefaultCulture": "ka-GE",
    "SupportedCultures": [ "ka-GE", "en-US" ]
  }
}

🔐 Identity & JWT
Register

POST /api/account/register → returns { message, confirmationLink }.

Sample

{
  "email": "user1@shop.com",
  "password": "P@ssw0rd!",
  "name": "User One",
  "phoneNumber": "+995555000111",
  "shippingAddress": "Tbilisi, Georgia",
  "billingAddress": "Tbilisi, Georgia"
}

Confirm Email

GET /api/account/confirm-email?userId=...&token=...

Login

POST /api/account/login → { "token": "<JWT>" }
Use in headers:

Authorization: Bearer <JWT>

Forgot / Reset Password

POST /api/account/forgot-password?email=user1@shop.com → { resetLink }

POST /api/account/reset-password with { userId, token, newPassword }

Roles

Roles Admin and Customer are ensured on startup (SeedAdminHelper).

Protect admin endpoints with:

[Authorize(Roles = "Admin")]

📦 Key Endpoints (selection)
Products

GET /api/products (paging/filter)

GET /api/products/{id}

GET /api/products/category/{categoryId}

GET /api/products/featured

GET /api/products/search?keyword=

POST /api/products [Admin] (supports image upload)

PUT /api/products/{id} [Admin]

DELETE /api/products/{id} [Admin]

Cart

GET /api/cart/{customerId}

POST /api/cart/{customerId}/items

PUT /api/cart/{customerId}/items/{itemId}

DELETE /api/cart/{customerId}/items/{itemId}

DELETE /api/cart/{customerId} (clear)

Orders

POST /api/orders/checkout (creates Order + Details, decrements stock, clears cart)

GET /api/orders/{orderId}

GET /api/orders/user/{customerId}

PUT /api/orders/{orderId}/status?status=Shipped [Admin]

POST /api/orders/cancel/{orderId} (restores stock if needed)

Reviews & Wishlist

GET /api/reviews/product/{productId}

GET /api/reviews/product/{productId}/by-user/{customerId}

POST /api/reviews (authenticated)

GET /api/wishlist/{customerId}

POST /api/wishlist/{customerId}/items

See controllers for the full list and payload shapes.

🧪 Testing
dotnet test


xUnit + Moq

Coverage includes: Auth, Products, Categories, Cart, Orders (checkout & status), Reviews, Discounts, Inventory helpers.

Note: EF InMemory provider doesn’t support transactions—prefer SQLite in-memory for transaction tests.

🧭 Development Tips

Keep controllers thin: use Command/Query services, AutoMapper profiles, and FluentValidation.

Centralize logging with Serilog; add sinks as needed (file, Seq, etc.).

Add audit hooks for sensitive changes (orders, products).

Consider refresh tokens, caching, and background jobs (low-stock emails) for next steps.

🩺 Troubleshooting

Migrations/Design-time DbContext: make sure the correct startup/project pair is used with -s / -p flags.

401 Unauthorized: check Authorization: Bearer <token> header, token expiry, issuer/audience match.

Admin seeding: ensure AdminSettings:Email/Password exist in user secrets; check startup logs.

Localization: switch via query string ?culture=ka-GE&ui-culture=ka-GE.

🤝 Contributing

PRs welcome. Please include unit tests and follow the existing architecture (CQRS, Repository/UoW, FluentValidation, AutoMapper).

📄 License

MIT © Nika Mestvirishvili

📬 Contact

Email: nikamesota27@gmail.com

Location: Tbilisi, Georgia
