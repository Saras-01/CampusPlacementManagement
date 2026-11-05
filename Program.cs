using CampusPlacementManagement.Data;
using Microsoft.EntityFrameworkCore;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Repositories;
using CampusPlacementManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

// SQL Server and Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
builder.Services.AddScoped<ICollegeRepository, CollegeRepository>();

builder.Services.AddScoped<ICollegeService, CollegeService>();

var app = builder.Build();

// HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();