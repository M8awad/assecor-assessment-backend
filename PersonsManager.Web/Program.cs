using Microsoft.EntityFrameworkCore;
using PersonsManager.Repository.DBContext;
using PersonsManager.Repository.Implementation;
using PersonsManager.Repository.Interface;
using PersonsManager.Service.Implementation;
using PersonsManager.Service.Interface;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<DbContext, ApplicationDbContext>();
builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

// Register services
builder.Services.AddScoped<IPersonService, PersonService>();


// Configure CSV data source if needed
var dataSource = builder.Configuration.GetValue<string>("DataSource", "Database");
if (string.Equals(dataSource, "CSV", StringComparison.OrdinalIgnoreCase))
{
    var csvFilePath = builder.Configuration.GetValue<string>("CsvFilePath", "sample-input.csv");
    builder.Services.AddScoped<IPersonRepository>(provider => new CsvPersonRepository(csvFilePath));
}
else
{
    builder.Services.AddScoped<IPersonRepository, PersonRepository>();
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();