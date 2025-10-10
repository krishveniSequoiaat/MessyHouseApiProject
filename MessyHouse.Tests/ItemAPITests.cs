using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MessyHouseAPIProject.Models;
using Microsoft.OpenApi.Expressions;
namespace MessyHouse.Tests;

[Collection("Sequential")]
public class ItemAPITests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    public ItemAPITests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private void SeedTestData()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MessyHouseAPIProject.Data.AppDbContext>();
            db.Items.RemoveRange(db.Items);
            db.StorageBoxes.RemoveRange(db.StorageBoxes);
            db.SaveChanges();
            db.StorageBoxes.AddRange(
                new StorageBox { Name = "Box1", Barcode = "987654321012", Location = "Location1" },
                new StorageBox { Name = "Box2", Barcode = "123456789012", Location = "Location2" }
            );
            db.Items.AddRange(
                new Item { Name = "TestItem1", ImageUrl = "ImageUrl1", Tag = "tag1", Barcode = "987654321012" },
                new Item { Name = "TestItem2", ImageUrl = "ImageUrl2", Tag = "tag2", Barcode = "123456789012" }
            );
            db.SaveChanges();
        }
    }

    [Fact]
    public async Task Get_ReturnsSuccessStatusCode()
    {
        SeedTestData();
        // Arrange
        var request = "/items";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var items = await response.Content.ReadFromJsonAsync<List<Item>>();
        Assert.NotNull(items);
        Assert.Contains(items, i => i.Name == "TestItem1"); // Adjust based on expected content
    }

    [Fact]
    public async Task AddItem_CreateNewItemSuccessfully()
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent("Desk Lamp"), "Name" },
            { new StringContent("Lighting"), "Tag" },
            { new StringContent("987654321012"), "Barcode" }
        };

        var dummyImage = new byte[] { 0x20, 0x21, 0x22 }; // Replace with actual image bytes
        formData.Add(new ByteArrayContent(dummyImage), "Image", "testimage.jpg");

        var response = await _client.PostAsync("/items", formData);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed with status code {response.StatusCode}: {errorMessage}");
        }
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AddItem_MissingName_ReturnsBadRequest()
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent(""), "Name" }, // Missing name
            { new StringContent("Lighting"), "Tag" },
            { new StringContent("987654321012"), "Barcode" }
        };

        var dummyImage = new byte[] { 0x20, 0x21, 0x22 }; // Replace with actual image bytes
        formData.Add(new ByteArrayContent(dummyImage), "Image", "testimage.jpg");

        var response = await _client.PostAsync("/items", formData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddItem_MissingImage_ReturnsBadRequest()
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent("Desk Lamp"), "Name" },
            { new StringContent("Lighting"), "Tag" },
            { new StringContent("987654321012"), "Barcode" }
            // Missing image
        };

        var response = await _client.PostAsync("/items", formData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddItem_NonExistentBarcode_ReturnsBadRequest()
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent("Desk Lamp"), "Name" },
            { new StringContent("Lighting"), "Tag" },
            { new StringContent("000000000000"), "Barcode" } // Non-existent barcode
        };

        var dummyImage = new byte[] { 0x20, 0x21, 0x22 }; // Replace with actual image bytes
        formData.Add(new ByteArrayContent(dummyImage), "Image", "testimage.jpg");

        var response = await _client.PostAsync("/items", formData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddItem_ValidData_ReturnsCreatedResponse()
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent("Desk Lamp"), "Name" },
            { new StringContent("Lighting"), "Tag" },
            { new StringContent("987654321012"), "Barcode" }
        };

        var dummyImage = new byte[] { 0x20, 0x21, 0x22 }; // Replace with actual image bytes
        formData.Add(new ByteArrayContent(dummyImage), "Image", "testimage.jpg");

        var response = await _client.PostAsync("/items", formData);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task SearchItems_ReturnsMatchingItems()
    {
        SeedTestData();
        var response = await _client.GetAsync("/items/search?search=TestItem1");
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<Item>>();
        Assert.NotNull(items);
        Assert.Single(items);
        Assert.Equal("TestItem1", items[0].Name);
    }

    [Fact]
    public async Task SearchItems_NoMatches_ReturnsNotFound()
    {
        SeedTestData();
        var response = await _client.GetAsync("/items/search?search=NonExistentItem");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
