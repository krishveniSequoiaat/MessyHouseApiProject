using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace MessyHouse.Tests.UnitTesting
{
    public class ViewItemsTests : BaseUITest
    {
        private const string Username = "testuser";
        private const string Password = "Test@1234";
        private readonly string _baseUrl = "https://localhost:7161/login";

        private async Task LoginAsync()
        {
            await Page.GotoAsync(_baseUrl);

            //fill username and pwd
            await Page.FillAsync("#loginUsername", "testuser");
            await Page.FillAsync("#loginPassword", "Test@1234");

            await Page.ClickAsync("#loginButton");

            await Page.WaitForURLAsync("**/Index");
        }

        [Fact]
        public async Task ViewItems_ShouldShowNoItems_WhenBoxIsEmpty()
        {
            await LoginAsync();

            // Wait for storage boxes to load
            await Page.WaitForSelectorAsync("table#storageBoxTable");

            // Click FIRST View button (we assume this is an empty box)
            await Page.ClickAsync("button.view-btn:nth-of-type(1)");

            // Wait for modal to open
            await Page.WaitForSelectorAsync("#viewItemsModal.show");

            // Check “no items” text (modify id)
            Assert.True(await Page.IsVisibleAsync("#emptyItemsMessage"));
        }

        [Fact]
        public async Task ViewItems_ShouldShowItems_WhenBoxHasItems()
        {
            await LoginAsync();

            // Wait for storage boxes to load
            await Page.WaitForSelectorAsync("table#storageBoxTable");

            // Click SECOND View button (we assume this box has items)
            await Page.WaitForSelectorAsync("button.view-btn");

            var viewButtons = await Page.QuerySelectorAllAsync("button.view-btn");
            await viewButtons[1].ClickAsync();

            // Wait for modal to open
            await Page.WaitForSelectorAsync("#viewItemsModal.show");

            await Page.WaitForSelectorAsync("#itemsTableBody_view");

            // Wait for table rows (modify selectors)
            var rows = await Page.QuerySelectorAllAsync("#itemsTableBody_view tr");

            Assert.NotEmpty(rows);
        }

        [Fact]
        public async Task ViewItems_ShouldCloseModal_WhenCloseButtonClicked()
        {
            await LoginAsync();

            // Wait for storage boxes to load
            await Page.WaitForSelectorAsync("table#storageBoxTable");

            // Click FIRST View button
            await Page.ClickAsync("button.view-btn:nth-of-type(1)");

            // Wait for modal to open
            await Page.WaitForSelectorAsync("#viewItemsModal.show");

            // Click close button
            await Page.ClickAsync("#closeViewItemsModalBtn");

            // Verify modal is closed
            Assert.False(await Page.IsVisibleAsync("#viewItemsModal.show"));
        }

        [Fact]
        public async Task ViewItems_ShouldOpenAddItemModal_WhenAddItemButtonClicked()
        {
            await LoginAsync();

            // Wait for storage boxes to load
            await Page.WaitForSelectorAsync("table#storageBoxTable");

            // Click FIRST View button
            await Page.ClickAsync("button.view-btn:nth-of-type(1)");

            // Wait for modal to open
            await Page.WaitForSelectorAsync("#viewItemsModal.show");

            // Click Add Item button
            await Page.ClickAsync("#addItemBtn_view");

            // Verify Add Item modal is displayed
            Assert.True(await Page.IsVisibleAsync("#addItemModal.show"));
        }

        [Fact]
        public async Task ViewItems_ShouldDisplayCorrectBoxDetails()
        {
            await LoginAsync();

            // Wait for storage boxes to load
            await Page.WaitForSelectorAsync("#storageBoxTable");

            // Get box name and location from the first ow
            var row = Page.Locator("#storageBoxTableBody tr").First;

            var id = await row.Locator("td:nth-of-type(1)").InnerTextAsync();
            var boxName = await row.Locator("td:nth-of-type(2)").InnerTextAsync();
            var boxLocation = await row.Locator("td:nth-of-type(3)").InnerTextAsync();
            var boxBarcode = await row.Locator("td:nth-of-type(4)").InnerTextAsync();

            // Click the View button (5th column)
            await row.Locator("td:nth-of-type(5) button.view-btn").ClickAsync();

            // Wait for modal to open
            await Page.WaitForSelectorAsync("#viewItemsModal.show");

            // Verify box details in modal
            var modalBoxName = await Page.InnerTextAsync("#boxName_view");
            var modalBoxLocation = await Page.InnerTextAsync("#boxLocation_view");

            Assert.Equal(boxName, modalBoxName);
            Assert.Equal(boxLocation, modalBoxLocation);
            Assert.Equal(boxBarcode, await Page.InnerTextAsync("#boxBarcode_view"));
        }

        [Fact]
        public async Task ViewItems_AddItem_ShouldPreselectCorrectBarcode()
        {
            await LoginAsync();

            // Locate first row
            var row = Page.Locator("#storageBoxTableBody tr").First;

            var barcode = await row.Locator("td:nth-of-type(4)").InnerTextAsync();

            // Click view
            await row.Locator("td:nth-of-type(5) button.view-btn").ClickAsync();
            await Page.WaitForSelectorAsync("#viewItemsModal.show");

            // Click Add Item
            await Page.ClickAsync("#addItemBtn_view");

            await Page.WaitForSelectorAsync("#addItemModal.show");

            // Check dropdown selected value
            var selected = await Page.InputValueAsync("#boxBarcodeSelect");
            Assert.Equal(barcode.Trim(), selected.Trim());
        }
    }
}
