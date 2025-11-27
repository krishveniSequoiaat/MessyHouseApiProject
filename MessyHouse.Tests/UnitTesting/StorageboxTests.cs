namespace MessyHouse.Tests.UnitTesting;

public class StorageBoxTests : BaseUITest
{
    private readonly string _baseUrl = "https://localhost:7161/login";

    internal async Task Login()
    {
        await Page.GotoAsync(_baseUrl);

        //fill username and pwd
        await Page.FillAsync("#loginUsername", "testuser");
        await Page.FillAsync("#loginPassword", "Test@1234");

        await Page.ClickAsync("#loginButton");

        await Page.WaitForURLAsync("**/Index");
    }

    [Fact]
    public async Task StorageBox_Should_Load()
    {
        //open login page
        await Login();

        await Page.ClickAsync("#addStorageBoxBtn");
        await Page.WaitForSelectorAsync("#addStorageBoxModal");

        string boxName = string.Concat("Test Box ", System.Guid.NewGuid().ToString().AsSpan(0, 5));

        await Page.FillAsync("#boxName", boxName);
        await Page.FillAsync("#boxLocation", "TestLocation");

        await Page.ClickAsync("#saveStorageBoxBtn");

        await Page.WaitForSelectorAsync("#storageBoxTable");

        string tableContent = await Page.InnerHTMLAsync("#storageBoxTableBody");

        Console.WriteLine(tableContent);

        Assert.Contains(boxName, tableContent);

    }

    [Fact]
    public async Task ViewITems_should_loadItems()
    {
        //open login page
        await Login();

        await Page.ClickAsync("#viewItemsBtn");

        await Page.WaitForSelectorAsync("#viewItemsModal");

        string tableContent = await Page.InnerHTMLAsync("#itemsTableBody");

        Console.WriteLine(tableContent);

        Assert.Contains("Item", tableContent);
    }


}