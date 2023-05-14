using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDbContext>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("CorsPolicy");

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});
app.MapGet("/", () => "Hello World!");

app.MapGet("/items", (ToDoDbContext context) =>
{
    return context.Items.ToList();
});

app.MapPost("/items", async(ToDoDbContext context, Item item)=>{
    context.Add(item);
    await context.SaveChangesAsync();
    return item;
});

app.MapPut("/items/{id}", async(ToDoDbContext context, Boolean IsComplete, int id)=>{
    var Item = await context.Items.FindAsync(id);
    if(Item is null) 
    return Results.NotFound();   
    Item.IsComplete = IsComplete;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/items/{id}", async(ToDoDbContext context, int id)=>{
    var existItem = await context.Items.FindAsync(id);
    if(existItem is null) return Results.NotFound();
    context.Items.Remove(existItem);
    await context.SaveChangesAsync();
    return Results.NoContent();
});
app.Run();
