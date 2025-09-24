using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using MessyHouseAPIProject.Data;
using MessyHouseAPIProject.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=MessyHouseDb.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

List<Item> items = new();
List<StorageBox> storageBoxes = new();
//var newItemid = 1;
app.UseCors("AllowAll");
app.UseStaticFiles();

//---Create a StorageBox-----

app.MapPost("/storageboxes", async (HttpRequest request, AppDbContext dbContext) =>
{
    var form = await request.ReadFormAsync();
    var name = form["Name"].ToString();
    var barcode = form["Barcode"].ToString();
    var location = form["Location"].ToString();
    var storageBox = new StorageBox
    {
        Barcode = barcode,
        Location = location
    };

    dbContext.Add(storageBox);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/storagebox/{storageBox.Id}", storageBox);
})
.WithName("CreateStorageBox");



// //----Create an item----

app.MapPost("/items", async (HttpRequest request, AppDbContext dbContext) =>
{
    var form = await request.ReadFormAsync();
    var imageFile = form.Files["Image"];
    var name = form["Name"].ToString();
    var tag = form["Tag"].ToString();
    var barcode = form["Barcode"].ToString();

    string imageUrl = string.Empty;
    if (imageFile != null && imageFile.Length > 0)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }
        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }
        var baseUrl = $"{request.Scheme}://{request.Host}";
        imageUrl = $"{baseUrl}/images/{uniqueFileName}";
    }
    var item = new Item
    {
        Name = name,
        Tag = tag,
        Barcode = barcode,
        ImageUrl = imageUrl
    };
    dbContext.Add(item);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/item/{item.Id}", item);
});



//Get all items

app.MapGet("/items", async (HttpRequest request, AppDbContext dbContext) =>
{
    var baseUrl = $"{request.Scheme}://{request.Host}";
    var items = await dbContext.Items.Select(i => new Item
    {
        Id = i.Id,
        Name = i.Name,
        Tag = i.Tag,
        Barcode = i.Barcode,
        ImageUrl = string.IsNullOrEmpty(i.ImageUrl) ? null : (i.ImageUrl.StartsWith("http") ? i.ImageUrl : $"{baseUrl}{i.ImageUrl}")
    }).ToListAsync();

    return Results.Ok(items);
})
.WithName("GetAllItems")
.WithOpenApi();



//get all storage boxes

app.MapGet("/storageboxes", async (AppDbContext dbContext) =>
{
    var boxes = await dbContext.StorageBoxes.ToListAsync();
    return Results.Ok(boxes);
})
.WithName("GetAllStorageBoxes")
.WithOpenApi();

//get items with tags or name or barcode   

app.MapGet("/items/search", async (string? tag, string? barcode, AppDbContext dbContext) =>
{
    if (string.IsNullOrEmpty(tag) && string.IsNullOrEmpty(barcode))
    {
        return Results.BadRequest("At least one query parameter (tag or barcode) must be provided.");
    }
    var query = dbContext.Items.AsQueryable();
    if (!string.IsNullOrEmpty(tag))
    {
        query = query.Where(i => i.Tag.ToLower().Contains(tag.ToLower()) || i.Name.ToLower().Contains(tag.ToLower()));
    }
    if (!string.IsNullOrEmpty(barcode))
    {
        query = query.Where(i => i.Barcode == barcode);
    }
    var items = await query.ToListAsync();
    if (items == null || items.Count == 0)
    {
        return Results.NotFound();
    }
    return Results.Ok(items);
})
.WithName("SearchItemsByTagOrBarcode");

//update items to another storage box
app.MapPut("/item/{id}/move/{newStorageBarcode}", async (int id, string newStorageBarcode, AppDbContext dbContext) =>
{
    var item = await dbContext.Items.FirstOrDefaultAsync(i => i.Id == id);
    if (item == null)
    {
        return Results.NotFound();
    }
    var newBox = await dbContext.StorageBoxes.FirstOrDefaultAsync(b => b.Barcode == newStorageBarcode);
    if (newBox == null)
    {
        return Results.NotFound();
    }

    item.Barcode = newStorageBarcode;
    await dbContext.SaveChangesAsync();

    return Results.Ok(item);
});

app.MapDelete("/item/{id}", async (int id, AppDbContext dbContext) =>
{
    var item = await dbContext.Items.FirstOrDefaultAsync(i => i.Id == id);
    if (item == null)
    {
        return Results.NotFound();
    }
    dbContext.Items.Remove(item);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/storagebox/{id}", async (int id, AppDbContext dbContext) =>
{
    var box = await dbContext.StorageBoxes.FirstOrDefaultAsync(b => b.Id == id);
    if (box == null)
    {
        return Results.NotFound();
    }
    var itemsInBox = await dbContext.Items.AnyAsync(i => i.Barcode == box.Barcode);
    if (itemsInBox)
    {
        return Results.BadRequest("Cannot delete storage box that contains items.");
    }
    dbContext.StorageBoxes.Remove(box);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});
app.Run();

public record ItemUploadDto
{
    public string Name { get; set; }
    public string Barcode { get; set; }
    public string Tag { get; set; }
    public IFormFile Image { get; set; }
}
