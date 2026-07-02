# 🚀 Eshtri Menii Backend

<p align="center">

<img src="https://img.shields.io/badge/.NET-8-512BD4?style=for-the-badge&logo=dotnet" />
<img src="https://img.shields.io/badge/ASP.NET_Core-8-5C2D91?style=for-the-badge&logo=dotnet" />
<img src="https://img.shields.io/badge/C%23-12-239120?style=for-the-badge&logo=csharp" />
<img src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
<img src="https://img.shields.io/badge/Entity_Framework_Core-8-512BD4?style=for-the-badge" />
<img src="https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" />
<img src="https://img.shields.io/badge/Stripe-635BFF?style=for-the-badge&logo=stripe&logoColor=white" />
<img src="https://img.shields.io/badge/SignalR-512BD4?style=for-the-badge" />
<img src="https://img.shields.io/badge/Hangfire-000000?style=for-the-badge" />
<img src="https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens" />

</p>

<p align="center">

A production-ready <b>Multi-Vendor E-Commerce REST API</b> built with <b>ASP.NET Core 8</b>, following <b>Clean Architecture</b> principles and modern backend best practices.

Designed to power a complete marketplace with secure authentication, Stripe payments, Redis basket persistence, SignalR real-time notifications, background jobs, reporting, and scalable business logic.

</p>

---

## 🌍 About The Project

**Eshtri Menii** is a full-featured backend for a modern multi-vendor e-commerce platform.

The project was built to simulate a real production environment rather than a simple CRUD application. It focuses on scalability, maintainability, clean code, and enterprise-level architecture.

The API serves three different user roles:

- 👤 Customer
- 🏪 Vendor
- 👑 Administrator

Every feature was designed to reflect how modern e-commerce platforms operate, including secure authentication, payment processing, inventory management, order lifecycle, background processing, and real-time communication.

---

## ✨ Highlights

- 🔐 JWT Authentication + Refresh Tokens
- 🏪 Multi-Vendor Marketplace
- 💳 Stripe PaymentIntent Integration
- 🛒 Redis Persistent Shopping Basket
- 🔔 SignalR Real-Time Notifications
- ⚙ Hangfire Background Jobs
- 📧 Email Confirmation & Password Recovery
- 📊 Admin Analytics & Reports
- 📄 PDF & Excel Export
- ⭐ Product Reviews & Ratings
- 🎟 Coupon & Discount System
- 📦 Complete Order Management
- ❤️ Wishlist
- 🛡 Role-Based Authorization
- 🏗 Clean Architecture
- 🚀 Production-Ready REST API

---

# 🏗 Architecture

The backend follows **Clean Architecture**, separating business rules from infrastructure and presentation concerns.

This architecture ensures the application remains scalable, testable, maintainable, and independent of external frameworks.

```
                ┌────────────────────┐
                │      Clients       │
                │ Angular • Swagger  │
                └─────────┬──────────┘
                          │
                          ▼
                ┌────────────────────┐
                │      ECom.API      │
                │ Controllers        │
                │ Middlewares        │
                │ Authentication     │
                └─────────┬──────────┘
                          │
                          ▼
                ┌────────────────────┐
                │ ECom.Application   │
                │ Business Rules     │
                │ Services           │
                │ DTOs               │
                │ Interfaces         │
                │ Validators         │
                └─────────┬──────────┘
                          │
                          ▼
                ┌────────────────────┐
                │     ECom.Core      │
                │ Entities           │
                │ Enums              │
                │ Specifications     │
                │ Contracts          │
                └─────────┬──────────┘
                          │
                          ▼
                ┌────────────────────┐
                │ ECom.Infrastructure│
                │ EF Core            │
                │ Identity           │
                │ Redis              │
                │ Stripe             │
                │ SignalR            │
                │ Email              │
                └────────────────────┘
```

---

## 🎯 Design Principles

The project was built around modern backend engineering practices.

- Clean Architecture
- SOLID Principles
- Repository & Unit of Work
- Dependency Injection
- Separation of Concerns
- RESTful API Design

---

# 📂 Project Structure

```
ECom

├── 📦 ECom.API
│   ├── Controllers
│   ├── Middlewares
│   ├── Extensions
│   ├── Helpers
│   ├── Filters
│   ├── Program.cs
│   └── appsettings.json
│
├── 📦 ECom.Application
│   ├── DTOs
│   ├── Interfaces
│   ├── Services
│   ├── Validators
│   ├── Mappings
│   ├── Specifications
│   ├── Exceptions
│   └── DependencyInjection
│
├── 📦 ECom.Core
│   ├── Entities
│   ├── Enums
│   ├── Interfaces
│   ├── Specifications
│   ├── Constants
│   └── Common
│
└── 📦 ECom.Infrastructure
    ├── Data
    ├── Persistence
    ├── Identity
    ├── Redis
    ├── SignalR
    ├── Services
    ├── Repositories
    ├── Migrations
    └── DependencyInjection
```

---

# 🧩 Architectural Patterns

The backend combines several architectural and design patterns.
Built using Clean Architecture, Repository Pattern, Unit of Work, Specification Pattern, Dependency Injection, DTO Pattern.

---

# 🛠 Tech Stack

The backend leverages modern Microsoft technologies and production-ready tools to deliver a scalable, secure, and maintainable e-commerce platform.

---

## 💻 Backend Framework

| Technology | Purpose |
|------------|---------|
| ASP.NET Core 8 | RESTful Web API |
| C# 12 | Primary Programming Language |
| Entity Framework Core 8 | ORM |
| SQL Server | Relational Database |
| ASP.NET Core Identity | Authentication & User Management |
| JWT | Secure Authentication |
| AutoMapper | Object Mapping |
| FluentValidation | Request Validation |

---

## 🗄 Data Storage

| Technology | Purpose |
|------------|---------|
| SQL Server | Persistent Data |
| Redis | Shopping Basket Storage |

---

## ⚡ Real-Time Communication

| Technology | Purpose |
|------------|---------|
| SignalR | Live Notifications |
| WebSockets | Instant Client Updates |

---

## 💳 Payments

| Technology | Purpose |
|------------|---------|
| Stripe PaymentIntent API | Online Payments |
| Stripe Webhooks | Payment Verification |
| Stripe Refund API | Automatic Refunds |

---

## ⚙ Background Processing

| Technology | Purpose |
|------------|---------|
| Hangfire | Background Jobs |
| Hosted Services | Scheduled Tasks |

---

## 📧 Email Services

| Technology | Purpose |
|------------|---------|
| MailKit | SMTP Email Sending |
| Gmail SMTP | Email Provider |

---

## 📊 Reporting

| Technology | Purpose |
|------------|---------|
| QuestPDF | PDF Reports |
| EPPlus | Excel Reports |

---

## 🔐 Security

| Technology | Purpose |
|------------|---------|
| JWT Tokens | Authentication |
| Refresh Tokens | Session Renewal |
| HttpOnly Cookies | Secure Token Storage |
| ASP.NET Identity | User & Role Management |
| Role-Based Authorization | Endpoint Protection |

---

# 🗄 Database Design

The application is built on a relational SQL Server database with normalized tables and properly defined relationships.

The database supports three primary business domains:

- Identity & Authentication
- E-Commerce
- Administration

---

## 📦 Core Entities

AppUser
Product
Category
Order
OrderItem
Review
Coupon
Notification
Wishlist

---

# 🔗 Entity Relationships

```
Category
    │
    │ 1
    │
    ▼
Products
    │
    ├──────────────┐
    │              │
    ▼              ▼
Photos         Reviews
                   │
                   ▼
                AppUser

AppUser
   │
   ├──────────► Orders
   │
   ├──────────► Wishlist
   │
   ├──────────► Notifications
   │
   └──────────► Address

Order
   │
   ├──────────► OrderItems
   │
   ├──────────► DeliveryMethod
   │
   └──────────► Coupon
```

---

## 📈 Database Highlights

- Identity integrated with ASP.NET Core Identity
- Optimized foreign key relationships
- One Review per Product per User
- Soft business validation
- Coupon usage tracking
- Order history preservation
- Inventory tracking
- Product image collections
- Payment status management
- Delivery methods
- Vendor ownership support

---

## 💡 Data Integrity

- Business rules are enforced through EF Core constraints and application-level validation, including review uniqueness, stock validation, coupon usage, order transitions, and payment verification.

---
# ✨ Features

## 👤 Customer Features

### 🛍 Shopping Experience
- Browse products with pagination, search, and advanced filtering.
- View detailed product pages with multiple images, ratings, and reviews.
- Add products to Wishlist or Shopping Basket.
- Persistent Redis-powered shopping basket.
- Product recommendations and related products.

### 💳 Checkout & Orders
- Multi-step checkout experience.
- Multiple delivery methods.
- Secure Stripe PaymentIntent integration.
- Order history and detailed order tracking.
- Live order status updates.
- Cancel pending orders with automatic stock restoration and Stripe refund.

### ⭐ Reviews & Ratings
- Leave verified product reviews.
- 5-star rating system.
- Average rating calculation.
- One review per customer per product.

### 🔐 Account & Security
- User registration with email confirmation.
- JWT Authentication & Refresh Tokens.
- Forgot / Reset Password.
- HttpOnly Cookie authentication.
- Role-based authorization.

### 🔔 Notifications
- Real-time SignalR notifications.
- Payment updates.
- Order status updates.

---

## 🏪 Vendor Portal

- Vendor dashboard with business overview.
- Complete product management (CRUD).
- Inventory & stock quantity management.
- Order management.
- Revenue overview.
- Low-stock alerts.

---

## 👑 Admin Portal

### 📊 Dashboard & Analytics
- Marketplace analytics and sales overview.
- Revenue insights.
- Order status statistics.
- Best-selling products.
- User & vendor statistics.

### ⚙ Management
- User management (Block / Unblock).
- Product management.
- Category management.
- Delivery methods management.
- Coupon management.
- Order management.
- Inventory management.

### 📄 Reports
Generate professional reports in:

- PDF
- Excel

Including:

- Sales
- Revenue
- Products
- Inventory
- Users

---

## 🛠 Platform Features

### 🛒 Shopping Basket
- Redis-backed persistent basket.
- Quantity management.
- Coupon support.
- Basket summary.

### 💳 Payment System
- Stripe PaymentIntent API.
- Stripe Webhooks.
- Automatic payment verification.
- Automatic refunds.
- Secure checkout flow.

### 🎟 Coupons
- Fixed amount & percentage discounts.
- Expiration dates.
- Usage limits.
- Minimum order amount.
- Maximum discount cap.
- Active / Inactive status.

### ⚙ Background Processing
- Hangfire background jobs.
- Scheduled tasks.
- Email processing.
- Cleanup jobs.

### 🌍 Localization
- English & Arabic support.
- RTL layout.
- Dynamic language switching.

### 📡 REST API
- RESTful architecture.
- Swagger documentation.
- Global exception handling.
- Fluent validation.
- Consistent API responses.
- Pagination support.