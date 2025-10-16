using System.Net.Http.Json;
using MessyHouseAPIProject.Models;

namespace MessyHouse.Tests.IntegerationTesting;

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

    [Fact]
    public async Task EditStorageBox_UpdateSuccessfully()
    {
        // First, create a new storage box to edit
        var formData = new MultipartFormDataContent
        {
            { new StringContent("BoxToEdit"), "Name" },
            { new StringContent("123456789018"), "Barcode" },
            { new StringContent("EditLocation"), "Location" }
        };

        var createResponse = await _client.PostAsync("/storageboxes", formData);
        createResponse.EnsureSuccessStatusCode();
        var createdBox = await createResponse.Content.ReadFromJsonAsync<StorageBox>();
        Assert.NotNull(createdBox);

        // Now, edit the created storage box
        var editFormData = new MultipartFormDataContent();

        editFormData.Add(new StringContent("EditedBox"), "Name");
        editFormData.Add(new StringContent("123456789018"), "Barcode");
        editFormData.Add(new StringContent("NewEditLocation"), "Location");

        var editResponse = await _client.PutAsync($"/storageboxes/{createdBox.Id}", editFormData);
        editResponse.EnsureSuccessStatusCode();
        var editedBox = await editResponse.Content.ReadFromJsonAsync<StorageBox>();
        Assert.NotNull(editedBox);
        Assert.Equal("EditedBox", editedBox.Name);
        Assert.Equal("123456789018", editedBox.Barcode);
        Assert.Equal("NewEditLocation", editedBox.Location);
    }

    [Fact]
    public async Task EditStorageBox_MissingFields_ReturnsBadRequest()
    {
        // First, create a new storage box to edit
        var formData = new MultipartFormDataContent
        {
            { new StringContent("BoxToEdit"), "Name" },
            { new StringContent("123456789019"), "Barcode" },
            { new StringContent("EditLocation"), "Location" }
        };

        var createResponse = await _client.PostAsync("/storageboxes", formData);
        createResponse.EnsureSuccessStatusCode();
        var createdBox = await createResponse.Content.ReadFromJsonAsync<StorageBox>();
        Assert.NotNull(createdBox);

        // Now, attempt to edit the created storage box with missing fields
        var editFormData = new MultipartFormDataContent
        {
            { new StringContent(""), "Name" } // Missing Barcode and Location
        };

        var editResponse = await _client.PutAsync($"/storageboxes/{createdBox.Id}", editFormData);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, editResponse.StatusCode);
        var body = await editResponse.Content.ReadAsStringAsync();
        Assert.Contains("Name, Barcode, and Location are required.", body);
    }

    [Fact]
    public async Task EditStorageBox_EmptyFields_ReturnsBadRequest()
    {
        // First, create a new storage box to edit
        var formData = new MultipartFormDataContent
        {
            { new StringContent("BoxToEdit"), "Name" },
            { new StringContent("123456789020"), "Barcode" },
            { new StringContent("EditLocation"), "Location" }
        };

        var createResponse = await _client.PostAsync("/storageboxes", formData);
        createResponse.EnsureSuccessStatusCode();
        var createdBox = await createResponse.Content.ReadFromJsonAsync<StorageBox>();
        Assert.NotNull(createdBox);

        // Now, attempt to edit the created storage box with empty fields
        var editFormData = new MultipartFormDataContent
        {
            { new StringContent(""), "Name" },
            { new StringContent(""), "Barcode" },
            { new StringContent(""), "Location" }
        };

        var editResponse = await _client.PutAsync($"/storageboxes/{createdBox.Id}", editFormData);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, editResponse.StatusCode);
        var body = await editResponse.Content.ReadAsStringAsync();
        Assert.Contains("Name, Barcode, and Location are required.", body);
    }

    [Fact]
    public async Task EditStorageBox_WhitespaceFields_ReturnsBadRequest()
    {
        // First, create a new storage box to edit
        var formData = new MultipartFormDataContent
        {
            { new StringContent("BoxToEdit"), "Name" },
            { new StringContent("123456789021"), "Barcode" },
            { new StringContent("EditLocation"), "Location" }
        };

        var createResponse = await _client.PostAsync("/storageboxes", formData);
        createResponse.EnsureSuccessStatusCode();
        var createdBox = await createResponse.Content.ReadFromJsonAsync<StorageBox>();
        Assert.NotNull(createdBox);

        // Now, attempt to edit the created storage box with whitespace fields
        var editFormData = new MultipartFormDataContent
        {
            { new StringContent("   "), "Name" },
            { new StringContent("   "), "Barcode" },
            { new StringContent("   "), "Location" }
        };

        var editResponse = await _client.PutAsync($"/storageboxes/{createdBox.Id}", editFormData);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, editResponse.StatusCode);
        var body = await editResponse.Content.ReadAsStringAsync();
        Assert.Contains("Name, Barcode, and Location are required.", body);
    }

    [Fact]
    public async Task EditStorageBox_NonExistentId_ReturnsNotFound()
    {
        // Attempt to edit a storage box with a non-existent ID
        var editFormData = new MultipartFormDataContent
        {
            { new StringContent("EditedBox"), "Name" },
            { new StringContent("123456789022"), "Barcode" },
            { new StringContent("NewEditLocation"), "Location" }
        };

        var editResponse = await _client.PutAsync($"/storageboxes/99999", editFormData); // Assuming 99999 does not exist

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, editResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteStorageBox_SuccessfullyDeletesEmptyBox()
    {
        // First, create a new storage box to delete
        var formData = new MultipartFormDataContent
        {
            { new StringContent("BoxToDelete"), "Name" },
            { new StringContent("123456789023"), "Barcode" },
            { new StringContent("DeleteLocation"), "Location" }
        };

        var createResponse = await _client.PostAsync("/storageboxes", formData);
        createResponse.EnsureSuccessStatusCode();
        var createdBox = await createResponse.Content.ReadFromJsonAsync<StorageBox>();
        Assert.NotNull(createdBox);

        // Now, delete the created storage box
        var deleteResponse = await _client.DeleteAsync($"/storageboxes/{createdBox.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteStorageBox_BoxWithItems_ReturnsBadRequest()
    {
        // Attempt to delete a storage box that contains items
        // Assuming "Box1" contains items based on seeded data
        var response = await _client.GetAsync("/storageboxes/search?search=Box1");
        response.EnsureSuccessStatusCode();
        var storageBoxes = await response.Content.ReadFromJsonAsync<List<StorageBox>>();
        Assert.NotNull(storageBoxes);
        var boxWithItems = storageBoxes.FirstOrDefault();
        Assert.NotNull(boxWithItems);

        var deleteResponse = await _client.DeleteAsync($"/storageboxes/{boxWithItems.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, deleteResponse.StatusCode);
        var body = await deleteResponse.Content.ReadAsStringAsync();
        Assert.Contains("Cannot delete storage box that contains items.", body);
    }


    [Fact]
    public async Task DeleteStorageBox_NonExistentId_ReturnsNotFound()
    {
        // Attempt to delete a storage box with a non-existent ID
        var deleteResponse = await _client.DeleteAsync($"/storageboxes/99999"); // Assuming 99999 does not exist

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

}