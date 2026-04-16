using Microsoft.EntityFrameworkCore;
using RPCMAS.Core.Data;
using RPCMAS.Core.Interfaces;
using RPCMAS.Infrastructure.Repositories;
using RPCMAS.Infrastructure.Seeder;
using RPCMAS.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Services
builder.Services.AddScoped<IItemCatalogService, ItemCatalogService>();

//Repositories
builder.Services.AddScoped<IItemCatalogRepository, ItemCatalogRepository>();


// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

await ItemCatalogSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapControllers();

if ( app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();

app.Run();
