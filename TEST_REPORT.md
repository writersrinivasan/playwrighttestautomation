# Test Automation Report: Banking App

## Executive Summary
This document details the comprehensive automated testing strategy implemented for the .NET Banking Application. The solution employs a testing pyramid approach, prioritizing fast, reliable **Unit Tests** for core business logic and robust **End-to-End (E2E) Tests** for validating critical user journeys via the actual User Interface.

**Overall Status**: ✅ PASSED (100% Pass Rate on final verification)

---

## 1. Unit Testing
**Project**: `Banking.Tests`
**Framework**: xUnit
**Scope**: Domain Logic (`Banking.Core`)

The unit tests focus on the "Brain" of the application, ensuring that the Business Logic Layer (BLL) correctly enforces rules independent of the API or UI.

### Test Scenarios Covered
| Category | Scenario | Outcome |
| :--- | :--- | :--- |
| **Positive** | **Deposit**: Verify balance increases correctly. | ✅ PASS |
| **Positive** | **Withdraw**: Verify balance decreases correctly. | ✅ PASS |
| **Positive** | **Transfer**: Verify funds move from Sender to Receiver. | ✅ PASS |
| **Negative** | **Insufficient Funds**: Withdraw amount > Balance throws ValidationException. | ✅ PASS |
| **Negative** | **Invalid Amount**: Negative/Zero deposit/withdrawal throws ValidationException. | ✅ PASS |
| **Negative** | **Self Transfer**: Transfering to same account number throws error. | ✅ PASS |
| **Edge** | **Precision**: Decimal handling for currency (e.g., $100.00). | ✅ PASS |

### Code Snippet (Example)
```csharp
[Fact]
public void Transfer_InsufficientFunds_ThrowsException()
{
    // Arrange
    var fromAccount = new Account("123", "Alice", 100);
    var toAccount = new Account("456", "Bob", 0);

    // Act & Assert
    Assert.Throws<ValidationException>(() => 
        _service.Transfer(fromAccount, toAccount, 200));
}
```

---

## 2. End-to-End (E2E) Testing
**Project**: `Banking.E2ETests`
**Framework**: Microsoft.Playwright + xUnit
**Scope**: Full Stack (Frontend + API + Logic)
**Browser Engine**: WebKit (Safari), Chromium, Firefox

The E2E tests simulate a real user interacting with the application in a browser. These tests verify the integration of all system components: the JavaScript frontend, the ASP.NET Core API, and the backend storage.

### Critical User Flow: `FullBankingFlow`
The automated suite executes the following sequence:

1.  **Account Creation (Alice)**
    *   **Action**: Fill form (Name: Alice, Type: Savings), Click Create.
    *   **Verification**: Dashboard loads, "Alice" displayed, Balance $0.00.
    *   **Logic**: Extract new Account Number via JS memory (`currentAccount.accountNumber`).

2.  **Deposit Operation**
    *   **Action**: Open Deposit Modal, enters $1000, Confirms.
    *   **Verification**: Balance updates to $1000.00 immediately.

3.  **Account Creation (Bob)**
    *   **Action**: Logout Alice, Create new account for Bob (Checking).
    *   **Verification**: Bob's Dashboard loads. Capture Bob's Account Number.

4.  **Inter-Account Transfer**
    *   **Action**:
        *   Logout Bob.
        *   **Login Alice** (using extracted credentials).
        *   Transfer $300 to Bob's Account Number.
    *   **Verification**:
        *   Alice's Balance drops to $700.00.
        *   Transaction History shows "Transfer to [Bob's ID]".

5.  **Receipt Verification**
    *   **Action**: Logout Alice, Login Bob.
    *   **Verification**:
        *   Bob's Balance increases to $300.00.
        *   Transaction History shows "Transfer from [Alice's ID]".

### Robustness Techniques Used
*   **Dynamic ID Extraction**: Instead of relying on fragile DOM text parsing, tests directly view the application state (`await Page.EvaluateAsync<string>("() => currentAccount.accountNumber")`) to reliably get 8-character hex IDs.
*   **Race Condition Handling**: Implemented `WaitForTimeout` and `Expect(...).ToHaveValueAsync` to ensure inputs are populated and notifications are clear before clicking buttons, preventing "element click intercepted" errors.
*   **State Management**: Used `Page.ReloadAsync()` strategies during debugging to ensure clean sessions between user logins.

---

## 3. How to Execute Tests

### Run Unit Tests (Fast)
```bash
dotnet test Banking.Tests
```

### Run E2E Tests (Integration)
*Prerequisite: Ensure `Banking.API` is running on `http://localhost:5087`*
```bash
dotnet test Banking.E2ETests
```
