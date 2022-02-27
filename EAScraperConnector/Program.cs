global using EAScraperConnector.Data;
global using Microsoft.EntityFrameworkCore;
using EAScraperConnector;
using EAScraperConnector.Interfaces;
using EAScraperConnector.Scrapers;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IZooplaScraper, ZooplaScraper>();
builder.Services.AddSingleton<IAngleSharpWrapper, AngleSharpWrapper>();
builder.Services.AddSingleton<IExcelSaver, ExcelSaver>();
builder.Services.AddTransient<IRightMoveScraper, RightMoveScraper>();
builder.Services.AddTransient<IEFWrapper, EFWrapper>();
builder.Services.AddTransient<IAuditWrapper, AuditWrapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
