namespace MessyHouse.Tests.UnitTesting;

public class LoginTests : BaseUITest
{
    private readonly string _baseUrl = "https://localhost:7161/login";

    [Fact]
    public async Task Register_user_Should_Succeed()
    {
        //open login page
        await Page.GotoAsync(_baseUrl);

        //click register link
        await Page.ClickAsync("text=Create New Account");

        //fill registration form
        await Page.FillAsync("#regUsername", "testuser");
        await Page.FillAsync("#regEmail", "testuser@example.com");
        await Page.FillAsync("#regPassword", "Test@1234");

        await Page.ClickAsync("#registerButton");

        await Page.WaitForURLAsync("**/Login");

        Assert.Contains("Login", Page.Url);
    }

    [Fact]
    public async Task Login_Should_Redirect_To_Index()
    {
        //open login page
        await Page.GotoAsync(_baseUrl);

        //fill username and pwd
        await Page.FillAsync("#loginUsername", "testuser");
        await Page.FillAsync("#loginPassword", "Test@1234");

        await Page.ClickAsync("#loginButton");

        await Page.WaitForURLAsync("**/Index");

        Assert.Contains("Index", Page.Url);

    }

    [Fact]
    public async Task Login_With_Invalid_Credentials_Should_Show_Error()
    {
        //open login page
        await Page.GotoAsync(_baseUrl);

        //fill invalid username and pwd
        await Page.FillAsync("#loginUsername", "invaliduser");
        await Page.FillAsync("#loginPassword", "WrongPassword");

        await Page.ClickAsync("#loginButton");

        //wait for error message
        var errorMessage = await Page.TextContentAsync("#loginError");

        Assert.Equal("Invalid username or password", errorMessage);
    }

    [Fact]
    public async Task Register_With_Existing_Username_Should_Show_Error()
    {
        //open login page
        await Page.GotoAsync(_baseUrl);

        //click register link
        await Page.ClickAsync("text=Create New Account");

        //fill registration form with existing username
        await Page.FillAsync("#regUsername", "testuser");
        await Page.FillAsync("#regEmail", "testuser@example.com");
        await Page.FillAsync("#regPassword", "Test@1234");

        await Page.ClickAsync("#registerButton");

        //wait for error message
        var errorMessage = await Page.TextContentAsync("#registerError");

        Assert.Equal("Username already exists", errorMessage);
    }


}