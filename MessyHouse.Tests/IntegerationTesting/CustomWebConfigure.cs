using MessyHouseAPIProject.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MessyHouse.Tests.IntegerationTesting
{
    // Custom Web Application Factory for integration tests
    // This allows us to configure the application for testing purposes
    // such as using an in-memory database or setting specific environment variables

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                builder.UseEnvironment("Testing");
            });
        }
    }

}