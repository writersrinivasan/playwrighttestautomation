# .NET Banking App with Playwright Test Automation

![Status](https://img.shields.io/badge/Status-Complete-success)
![Tests](https://img.shields.io/badge/Test%20Coverage-100%25-brightgreen)

A modern, full-stack Banking Application built with **.NET 9**, **ASP.NET Core Web API**, and a **Vanilla JS** frontend. This project demonstrates robust engineering practices, Domain-Driven Design (DDD), and a comprehensive automated testing strategy using **xUnit** and **Playwright**.

## 🚀 Features

### Core Banking Logic
- **Account Management**: Create Savings and Checking accounts.
- **Transactions**: Deposit, Withdraw, and Transfer funds between accounts.
- **Validation**: Strict business rules (no overdrafts, no negative amounts).

### Modern UI/UX
- **Dark Mode Design**: Sleek, glassmorphism-inspired interface.
- **SPA Architecture**: Fast, single-page interactions using Vanilla JS.
- **Real-time Updates**: Instant balance and transaction history reflection.

### Robust Test Automation
- **Unit Tests**: Full coverage of `Banking.Core` business logic.
- **E2E Tests**: **Playwright** scenarios verifying the complete `UserFlow` from creation to transfer.
- **See Full Report**: [TEST_REPORT.md](./TEST_REPORT.md)

---

## 🛠️ Project Structure

- **`Banking.Core`**: Class library containing Entities (`Account`, `Transaction`) and Domain Services (`AccountService`).
- **`Banking.API`**: ASP.NET Core Web API serving REST endpoints and the static frontend.
- **`Banking.Tests`**: xUnit project for Unit Testing the Core logic.
- **`Banking.E2ETests`**: Playwright project for End-to-End browser automation.

---

## 🏁 Getting Started

### Prerequisites
- .NET 9.0 SDK
- PowerShell / Terminal

### 1. Run the Application
Start the backend API (which serves the frontend):
```bash
dotnet run --project Banking.API --urls=http://localhost:5087
```
Open **[http://localhost:5087](http://localhost:5087)** in your browser.

### 2. Run Unit Tests
Execute the fast unit test suite:
```bash
dotnet test Banking.Tests
```

### 3. Run End-to-End Tests
Ensure the application is running, then execute the Playwright suite:
```bash
dotnet test Banking.E2ETests
```

---

## 📄 API Documentation
Swagger UI is available for exploring the API endpoints:
- **URL**: [http://localhost:5087/swagger](http://localhost:5087/swagger)

---

## 📝 License
This project is open-source and created for educational purposes to demonstrate AI Pair Programming capabilities.
