
# Restaurant Management System 🍽️

A full-stack Restaurant Management System built with **ASP.NET Core MVC**, structured using **Onion Architecture**, and secured with **ASP.NET Core Identity**. This system simulates real-world restaurant operations and includes both **Admin** and **Client** interfaces for a complete management and ordering experience.

---

## 📌 Project Overview

This application was developed as part of the ASP.NET Core MVC track at the **Information Technology Institute (ITI)**. While the original scope included only an admin-side interface, the system was extended to support a complete **client-side experience**, offering real-world restaurant features like menu browsing, order placement, and user authentication.

---

## ✅ Key Features

### 🔐 Admin Side
- **Menu Management**: Create, update, and delete food items.
- **Order Management**: View and update order statuses (e.g., Pending, Preparing, Delivered).
- **Inventory Simulation**: Track ingredients used and simulate item availability.
- **Analytics Dashboard**: View total orders, revenue, top-selling items, and more.
- **Role-Based Access Control**: Use of ASP.NET Identity to secure admin functions.

### 👥 Client Side
- **User Registration and Login**: Secure account creation with validation.
- **Menu Browsing**: View menu categories and individual item details.
- **Shopping Cart**: Add items, adjust quantities, and place orders.
- **Order History**: Track all previous orders with status updates.
- **Discount System**: Apply predefined discounts during checkout.

---

## 🧱 Onion Architecture Layers

```

Presentation Layer (ASP.NET Core MVC - Views, Controllers)
│
├── Application Layer (Services, DTOs, Contracts)
│
├── Domain Layer (Core business models and interfaces)
│
└── Infrastructure Layer (EF Core, Identity, Data Repositories)

````

---

## 🛠️ Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server
- C#
- Bootstrap (for UI responsiveness)
- Onion Architecture

---

## 🚀 Getting Started

### Prerequisites
- [.NET SDK 7.0+](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) (with ASP.NET & EF workloads)

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/MOSTAFA17RASHEEDY/ResturantFinalProject.git
   cd ResturantFinalProject
````

2. **Create your local SQL Server database**
   Update the connection string in `appsettings.json` if necessary.

3. **Apply migrations**

   ```bash
   dotnet ef database update
   ```

4. **Run the application**

   ```bash
   dotnet run
   ```

---

## 📂 Folder Structure (Onion Style)

* **ECommerce.Models** – Domain entities
* **ECommerce.DTOs** – Data Transfer Objects
* **ECommerce.Application** – Services, contracts, and mapping
* **ECommerce.Infrastructure** – Repositories, database context
* **ECommerce.Presentation** – MVC Controllers, Views, Identity UI
* **ECommerce.sln** – Solution file

---

## 📄 License

This project is for educational purposes and part of the ITI .NET track. Feel free to explore and build on it.

---

## 👨‍💻 Author

**Mostafa Rasheedy**
[LinkedIn](https://www.linkedin.com/in/mostafa-rasheedy/)
[GitHub](https://github.com/MOSTAFA17RASHEEDY)

---

```

---

If you'd like it with:
- Markdown badges
- A `GIF` demo
- A deployed live version link

Just tell me, and I’ll enhance it for you.

