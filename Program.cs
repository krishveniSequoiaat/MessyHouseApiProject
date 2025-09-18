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
//app.UseHttpsRedirection();

//---Create a StorageBox-----

app.MapPost("/storagebox", async (StorageBox storageBox, AppDbContext dbContext) =>
{
    //var newBox = storageBox with { Id = storageBoxes.Count + 1 };
    dbContext.Add(storageBox);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/storagebox/{storageBox.Id}", storageBox);
})
.WithName("CreateStorageBox");



//----Create an item----

app.MapPost("/item",  async (Item item, AppDbContext dbContext) =>
{
    //var newItem = item with { Id = newItemid++ };
    dbContext.Add(item);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/item/{item.Id}", item);
});



//Get all items

app.MapGet("/items", async (AppDbContext dbContext) =>
{
    var items = await dbContext.Items.ToListAsync();
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


//get items by storage box area --keywords   

// app.MapGet("/items/{location}", async (string location, AppDbContext dbContext) =>
// {
//     var box = await dbContext.StorageBoxes.FirstOrDefaultAsync(b => b.Location.ToLower() == location.ToLower());
//     if (box == null)
//     {
//         return Results.NotFound();
//     }
//     var boxItems = await dbContext.Items.Where(i => i.Barcode == box.Barcode).ToListAsync();
//     return Results.Ok(boxItems);
// })
// .WithName("GetItemsByStorageBoxArea");



// //get item by barcode
// app.MapGet("/items/barcode/{barcode}", async (string barcode, AppDbContext dbContext) =>
// {
//     var items = await dbContext.Items.Where(i => i.Barcode == barcode).ToListAsync();
//     if (items == null || items.Count == 0)
//     {
//         return Results.NotFound();
//     }
//     return Results.Ok(items);
// })
// .WithName("GetItemsByBarcode");

// //get item by keyword
// app.MapGet("/item/keyword/{keyword}", async (string keyword, AppDbContext dbContext) =>
// {
//     var items = await dbContext.Items.Where(i => i.Label.ToLower().Contains(keyword.ToLower()) || i.Name.ToLower().Contains(keyword.ToLower())).ToListAsync();
//     if (items == null || items.Count == 0)
//     {
//         return Results.NotFound();
//     }
//     return Results.Ok(items);
// })
// .WithName("SearchItemsByKeyword");

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


app.Run();

