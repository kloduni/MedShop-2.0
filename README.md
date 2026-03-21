# MedShop

MedShop is a web application built with ASP.NET Core for managing and selling medical consumables. The project handles the core e-commerce workflow, from catalog management to user shopping carts.

## Project Overview
This was originally built as a student project and has been updated to improve the architecture and user interface. The current version features a dark "Slate" theme built with Tailwind CSS and a more robust administrative backend.

## Core Functionality
* **Role-Based Access:** Uses ASP.NET Core Identity to separate customer accounts from administrative roles.
* **Product Management:** Admins have full CRUD capabilities, including a soft-delete and restore system for archived products.
* **Storefront:** A searchable catalog with category filtering and pagination.
* **Readable URLs:** Implementation of custom slugs so product details use descriptive names in the URL instead of just IDs.
* **Data Integrity:** Custom model binders ensure accurate handling of decimal and numeric data (prices, quantities).

## Technical Stack
* **Backend:** ASP.NET Core (MVC)
* **Database:** Entity Framework Core / SQL Server
* **Security:** ASP.NET Core Identity
* **Frontend:** Tailwind CSS

## Local Setup
1. **Clone the repository.**
2. **Configure Connection String:** Update `appsettings.json` with your local SQL Server instance.
3. **Initialize Database:** Run `Update-Database` via the Package Manager Console.
4. **Admin Seeding:** On the first launch, the `SeedAdminAsync` method will automatically create the Administrator role and a default admin account.

---
*Created as a practical application for medical e-commerce management.*
