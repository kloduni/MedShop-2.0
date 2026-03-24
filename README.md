# MedShop

MedShop is an ASP.NET Core web application built to manage and browse medical consumables. Originally started as a student project a few years ago, it recently received a major facelift to practice modern UI integration (Tailwind CSS) and improve the backend architecture. 

It serves as a personal learning sandbox for full-stack .NET development, demonstrating core e-commerce workflows, database management, and asynchronous frontend interactions.

## Core Features
* **Storefront & Seller Dashboard:** Users can browse the main catalog or act as sellers to manage their own inventory. The app ensures sellers cannot buy or wishlist their own items.
* **Asynchronous Cart & Wishlist:** Uses vanilla JavaScript (Fetch API) to let users toggle wishlist items and add products to their cart without full page reloads.
* **Role-Based Access:** Basic ASP.NET Core Identity implementation to separate standard users from Admins.
* **Admin Controls:** Admins can ban/unban users and use a soft-delete "Recycle Bin" system to archive products without destroying order history.
* **Modern UI:** A dark "Slate" theme built from scratch using Tailwind CSS and Bootstrap Icons.

## Technical Stack
* **Backend:** ASP.NET Core (MVC), C#
* **Database:** Entity Framework Core, SQL Server
* **Frontend:** HTML5, vanilla JavaScript, Tailwind CSS
* **Authentication:** ASP.NET Core Identity

---

## Local Setup Guide

Follow these steps to get the project running on your local machine.

### Prerequisites
Before you start, make sure you have the following installed:
1. [Visual Studio](https://visualstudio.microsoft.com/) (or the .NET SDK if using VS Code).
2. [Node.js](https://nodejs.org/) (Required to compile the Tailwind CSS).
3. SQL Server (LocalDB or SQL Server Express).

### Step-by-Step Installation

**1. Clone the repository**
```bash
git clone https://github.com/YourUsername/MedShop.git
cd MedShop
```

**2. Configure the Database**
* Open `appsettings.json`.
* Find the `DefaultConnection` string and update it to match your local SQL Server instance name.

**3. Install and Build Frontend Dependencies (Tailwind CSS)**
Because this project uses Tailwind CSS, you need to install the Node packages and compile the stylesheet before running the app.
* Open a terminal in the root project folder (where `package.json` is located).
* Run the following commands:
```bash
npm install
npm run build:css
```
*(Note: If you plan on editing the CSS or HTML classes, you can run `npm run watch:css` to recompile the stylesheet automatically as you save files).*

**4. Initialize the Database**
You need to create the SQL tables and apply the initial seed data.
* **If using Visual Studio:** Open the **Package Manager Console** (`Tools > NuGet Package Manager`) and run:
  ```powershell
  Update-Database
  ```
* **If using the .NET CLI:** Open your terminal and run:
  ```bash
  dotnet ef database update
  ```

**5. Run the Application**
Launch the project via Visual Studio (F5) or the CLI (`dotnet run`). 
* *Note on Admin Access:* On the very first launch, the `SeedAdminAsync` method will automatically create the Administrator role and a default admin account so you can test the administrative dashboards.
