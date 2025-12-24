using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;
using System.Threading.Tasks;

namespace Banking.E2ETests
{
    public class UserFlowTests : PageTest
    {
        private const string BaseUrl = "http://localhost:5087";

        [Fact]
        public async Task FullBankingFlow_CreateAccounts_Deposit_Transfer_Verify()
        {
            // 1. Create Alice (Savings)
            await Page.GotoAsync(BaseUrl);
            await Expect(Page).ToHaveTitleAsync("NeoBank AI");
            
            await Page.FillAsync("#ownerName", "Alice");
            await Page.SelectOptionAsync("#accountType", "Savings");
            await Page.ClickAsync("text=Create Account");

            // Verify logged in
            await Expect(Page.Locator("#dashboard-view")).ToBeVisibleAsync();
            await Expect(Page.Locator("#displayAccountInfo")).ToContainTextAsync("Alice");
            await Expect(Page.Locator("#displayBalance")).ToContainTextAsync("$0.00");

            // Capture Account Number directly from JS state
            var aliceAccNum = await Page.EvaluateAsync<string>("() => currentAccount.accountNumber");
            Assert.False(string.IsNullOrEmpty(aliceAccNum), "Alice Acc Num via JS is empty");

            // 2. Deposit 1000
            await Page.ClickAsync("text=Deposit");
            await Page.FillAsync("#modalAmount", "1000");
            await Page.ClickAsync("text=Confirm");

            // Verify Balance Update
            await Expect(Page.Locator("#displayBalance")).ToContainTextAsync("$1000.00");
            await Expect(Page.Locator("#transactionList")).ToContainTextAsync("Cash Deposit");

            // Logout
            await Page.ClickAsync("text=Exit");
            await Expect(Page.Locator("#auth-view")).ToBeVisibleAsync();

            // 3. Create Bob (Checking)
            await Page.FillAsync("#ownerName", "Bob");
            await Page.SelectOptionAsync("#accountType", "Checking");
            await Page.ClickAsync("text=Create Account");
            
            // Verify logged in
            await Expect(Page.Locator("#dashboard-view")).ToBeVisibleAsync();

            // Capture Bob Account Number
            var bobAccNum = await Page.EvaluateAsync<string>("() => currentAccount.accountNumber");
            Assert.False(string.IsNullOrEmpty(bobAccNum), "Bob Acc Num via JS is empty");

            // Logout Bob
            await Page.ClickAsync("text=Exit");
            
            // Reload to ensure clean state
            await Page.ReloadAsync();

            // 4. Login Alice
            await Page.FillAsync("#loginAccountNumber", aliceAccNum);
            
            await Page.ClickAsync("text=Login");
            await Expect(Page.Locator("#displayAccountInfo")).ToContainTextAsync("Alice");

            // 5. Transfer 300 to Bob
            await Page.ClickAsync("text=Transfer Money");
            await Page.FillAsync("#modalAmount", "300");
            await Page.FillAsync("#modalDestAccount", bobAccNum);
            await Page.ClickAsync("text=Confirm");

            // Verify Alice Balance (1000 - 300 = 700)
            await Expect(Page.Locator("#displayBalance")).ToContainTextAsync("$700.00");
            await Expect(Page.Locator("#transactionList")).ToContainTextAsync($"Transfer to {bobAccNum}");

            // Logout Alice
            await Page.ClickAsync("text=Exit");

            // 6. Login Bob & Verify Receipt
            await Page.FillAsync("#loginAccountNumber", bobAccNum);
            await Page.ClickAsync("text=Login");
            
            // Verify Bob Balance (0 + 300 = 300)
            await Expect(Page.Locator("#displayBalance")).ToContainTextAsync("$300.00");
            await Expect(Page.Locator("#transactionList")).ToContainTextAsync($"Transfer from {aliceAccNum}");
        }
    }
}
