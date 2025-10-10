using System.Net.Http.Json;
using MessyHouseAPIProject.Models;

namespace MessyHouse.Tests;

[Collection("Sequential")]
public class StorageboxAPITests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    public StorageboxAPITests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsSuccessStatusCode()
    {
        // Arrange
        var request = "/storageboxes";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var storageBoxes = await response.Content.ReadFromJsonAsync<List<StorageBox>>();
        Assert.NotNull(storageBoxes);
        Assert.Contains(storageBoxes, sb => sb.Name == "Box1"); // Adjust based on expected content
    }


    [Fact]
    public async Task AddStorageBox_CreateNewStorageBoxSuccessfully()
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent("NewBox"), "Name" },
            { new StringContent("123456789016"), "Barcode" },
            { new StringContent("NewLocation"), "Location" }
        };

        // Act
        var response = await _client.PostAsync("/storageboxes", formData);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Request failed with status code {response.StatusCode}: {body}");
        }
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AddStorageBox_MissingFields_ReturnsBadRequest()
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent("IncompleteBox"), "Name" }
            // Missing Barcode and Location
        };

        // Act
        var response = await _client.PostAsync("/storageboxes", formData);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Name, Barcode, and Location are required.", body);
    }

    [Fact]
    public async Task AddStorageBox_EmptyFields_ReturnsBadRequest()
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent(""), "Name" },
            { new StringContent(""), "Barcode" },
            { new StringContent(""), "Location" }
        };

        // Act
        var response = await _client.PostAsync("/storageboxes", formData);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Name, Barcode, and Location are required.", body);
    }

    [Fact]
    public async Task AddStorageBox_WhitespaceFields_ReturnsBadRequest()
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent("   "), "Name" },
            { new StringContent("   "), "Barcode" },
            { new StringContent("   "), "Location" }
        };

        // Act
        var response = await _client.PostAsync("/storageboxes", formData);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Name, Barcode, and Location are required.", body);
    }

    [Fact]
    public async Task GetStorageboxes_Search_ReturnsMatchingStorageBoxes()
    {
        // Arrange
        var request = "/storageboxes/search?search=Box1";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var storageBoxes = await response.Content.ReadFromJsonAsync<List<StorageBox>>();
        Assert.NotNull(storageBoxes);
        Assert.All(storageBoxes, sb => Assert.Contains("Box1", sb.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetStorageboxes_Search_NoMatches_ReturnsNotFound()
    {
        // Arrange
        var request = "/storageboxes/search?search=NonExistentBox";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

}