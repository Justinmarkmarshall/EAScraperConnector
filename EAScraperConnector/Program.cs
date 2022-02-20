using EAScraperConnector;
using EAScraperConnector.Interfaces;
using EAScraperConnector.Scrapers;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IZooplaScraper, ZooplaScraper>();
builder.Services.AddSingleton<IAngleSharpWrapper, AngleSharpWrapper>();
builder.Services.AddSingleton<IExcelSaver, ExcelSaver>();
builder.Services.AddSingleton<IRightMoveScraper, RightMoveScraper>();

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
