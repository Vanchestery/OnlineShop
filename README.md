```
Classic **layered (N-tier)** architecture: upper layers depend on lower ones; lower layers do not know about upper ones.
**Architecture guard:** `Web` does **not** reference `Db` directly — injecting `IProductsStorage` into a controller will not compile, so you go through `Core` services.
> **Note on Clean Architecture.** Alternative: Onion / Clean (`Web → Core ← Db`, where `Db` implements interfaces from `Core`). Clean decouples `Core` from EF Core so you can swap the ORM without touching business logic. Cost: dual models (domain in `Core` + EF entities in `Db`) and explicit mapping.
>
> For this pet project, Layered is simpler and closer to the common ASP.NET Core tutorial layout. On a real product with a larger team or likely infra swaps, Clean would be justified.
## Features
### Customer
- Product catalog with name/description search (PostgreSQL `ILIKE`)
- Category filter (Coffee / Tea / Accessories / Other)
- Product page with description, price, “Add to cart”, and reviews
- **Anonymous cart** via cookie — shop without logging in
- **Cart merge on login** — anonymous cart merges into the user cart on sign-in/register; duplicate lines sum quantities
- Favorites (authenticated users)
- Checkout with delivery address
- Account: profile, order history, password change
- Reviews with 1–5 ★ rating and average score
### Admin
- **Products**: CRUD with image upload to `wwwroot/images/products/`, format (jpg/png/webp) and size (≤5 MB) validation, files deleted from disk when a product is removed
- **Orders**: status filter, detail view, status updates
- **Users**: list, details with roles, admin password reset via token (no old password), self-lock guard against removing your own Admin role
- **Roles**: list and create
### Technical highlights
- **Frozen snapshot in `OrderItem`** — `ProductName` and `Price` are copied at checkout. Old orders stay correct after catalog price changes or product deletion.
- **Partial unique index on `Cart.UserId`** — at most one cart per user; many anonymous carts allowed
- **DB CHECK constraints** — `Quantity > 0` for CartItem/OrderItem, `Rating BETWEEN 1 AND 5` for Review
- **Owned-type Address** on Order — `DeliveryAddress_*` columns instead of a separate table
- **Identity security flow**: AntiForgeryToken on all POSTs, `IsLocalUrl` on `returnUrl` (open-redirect protection), `RefreshSignInAsync` after password change, POST logout (CSRF-safe), ownership check on Order/Details (IDOR protection)
## Local run
### Requirements
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for PostgreSQL)
- `dotnet ef` tool: `dotnet tool install --global dotnet-ef --version 9.0.0`
### 1. Clone
```bash
git clone https://github.com/Vanchestery/OnlineShop.git
cd OnlineShop
```
### 2. Start PostgreSQL
```bash
docker compose up -d
```
Postgres listens on `localhost:5433` (non-default port to avoid clashing with a local 5432 install). Dev credentials are in `appsettings.Development.json`. See comments in `docker-compose.yml`.
### 3. Apply migrations
```bash
dotnet ef database update --project OnlineShop.Db --startup-project OnlineShop.Web
```
> Alternatively, migrations run on first start via `ApplicationDbContext.Database.MigrateAsync()` in `Program.cs`. In production that is an **anti-pattern** (migrations belong in CI/CD); for a pet project it is convenient.
### 4. Run the app
```bash
dotnet run --project OnlineShop.Web
```
Or in Visual Studio: F5 (`https` profile).
Site: `https://localhost:5001`. On first start the app:
- creates `Admin` and `User` roles
- creates the admin account
- seeds 20 demo products with categories
### Default credentials
```
Email:    admin@onlineshop.local
Password: Admin#123
```
> For production, override these via User Secrets or environment variables and update `IdentityInitializer.cs`.
## Tests
```bash
dotnet test
```
30 unit tests cover **critical business scenarios**:
- **`CartServiceTests`** — anonymous cart merge (happy path, missing source, foreign-cart merge attempt, same-cart-id no-op), add-to-cart validation (not found, unavailable, non-positive quantity)
- **`OrderServiceTests`** — frozen product snapshot, reject empty cart / unavailable product, clear cart after success, multi-line snapshot
- **`ReviewServiceTests`** — rating in [1, 5], non-empty text, product exists, trim whitespace
Storage is mocked with Moq — no DB required; suite ~600 ms.
## Project layout
```
OnlineShop.Db/
├── Models/                  — User, Role, Product, Cart, CartItem, Order, OrderItem,
│                              ProductCategory, OrderStatus, Address, FavouritesItem, Review
├── Configurations/          — IEntityTypeConfiguration<T> per entity
├── Interfaces/              — IProductsStorage, IShoppingCartStorage, IOrdersStorage, ...
├── Storage/                 — storage implementations (async, EF Core)
├── Migrations/              — EF Core migrations
├── Extensions/              — AddDataLayer() for DI
├── ApplicationDbContext.cs  — single context : IdentityDbContext<User, Role, Guid>
└── IdentityInitializer.cs   — idempotent seeding of roles, admin, products
OnlineShop.Core/
├── Dtos/                    — records with init properties
│   └── Requests/            — CreateOrderRequest, AddReviewRequest
├── Interfaces/              — IProductService, ICartService, IOrderService, ...
├── Services/                — business service implementations
├── Mappings/                — AutoMapper profiles
└── Extensions/              — AddCoreLayer() for DI
OnlineShop.Web/
├── Controllers/             — Home, Product, Cart, Favourite, Order, Reviews, Account
├── Areas/Admin/             — Admin Area: Products / Orders / Users / Roles
│   ├── Controllers/
│   ├── ViewComponents/      — LeftMenu (brutal sidebar)
│   ├── ViewModels/
│   └── Views/
├── ViewModels/              — customer VMs (Auth, Cart, Order, ...)
├── Views/                   — Razor pages
├── ViewComponents/          — CartBadge, FavouriteButton, ProductReviews
├── Infrastructure/          — CartContext (anonymous cart via cookie)
├── Helpers/                 — OrderStatusExtensions, ProductCategoryExtensions,
│                              RussianPluralizer
├── wwwroot/css/brutal.css   — design styles (~1700 lines)
└── Program.cs               — DI, migrations, Identity, Serilog
OnlineShop.Tests/
└── Services/                — CartServiceTests, OrderServiceTests, ReviewServiceTests
```
## Design
UI is **Brutalist Modern**: bold black borders, uppercase monospace labels, accent colors (lime `#b5d333`, orange `#e85d04`, dark green `#1a472a`), large Bebas Neue headings, minimal gradients and shadows.
Intent: specialty coffee shop with a countercultural look, clearly not a stock Bootstrap template.
CSS lives in one file `wwwroot/css/brutal.css` with no preprocessor — CSS custom properties (`var(--black)`, `var(--lime)`, …). Bootstrap stays in the layout only for grid/flex utilities (`row`, `col-md-X`, `d-flex`); visual components are custom.
## Design decisions (interview notes)
- **One `ApplicationDbContext : IdentityDbContext<User, Role, Guid>`**, not two contexts. Identity and domain share one DB; FKs work without cross-context gymnastics.
- **Cart with nullable `UserId`** instead of UserCart + AnonymousCart: one table, one model, simple merge. Partial unique index enforces “one cart per user”; many anonymous carts allowed.
- **`Web → Core → Db` without Web→Db** — forces the service layer. A future API can reuse Core as-is.
- **Anonymous cart via scoped `ICartContext`** (not middleware) — hits the DB only when CartBadge or a controller needs the cart, not on every static request.
- **Frozen snapshot in `OrderItem`** — protects historical orders after catalog renames or `IsAvailable=false`.
- **Identity attack surface**: AntiForgeryToken on POST, `IsLocalUrl` on returnUrl, POST logout, ownership checks on Order/Details (IDOR).
- **30 unit tests target hard logic**, not CRUD wrappers — mock storage and assert the service called the right things with the right arguments.
## Possible improvements
- `Category` entity with FK instead of enum (multi-level hierarchy, SEO URLs)
- Full-text search via PostgreSQL `tsvector` (currently simple `ILIKE`)
- Pagination on `/Product/Index` (fine for 20 demo products; needed at 1000+)
- Email confirmation on register (disabled for easy local dev)
- Image processing (resize, WebP on upload)
- Global exception handler with custom error pages
- Integration tests with Testcontainers (real Postgres) alongside mocked units
- Health checks for production readiness
- OpenTelemetry for distributed tracing
## CI/CD
GitHub Actions in `.github/workflows/ci.yml`:
- Runs on push to any branch and PRs to `main`
- Steps: checkout → setup .NET 9 → restore → build (Release) → test
- All tests must pass before merge
## License
[MIT](LICENSE) © 2026. Product and hero images from Unsplash — free for commercial use without attribution.
---
**Contact:** [GitHub profile](https://github.com/Vanchestery)
