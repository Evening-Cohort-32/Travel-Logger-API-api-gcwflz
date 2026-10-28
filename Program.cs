using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using TravelLogger.Models.DTOs;
using TravelLogger;
using TravelLogger.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<TravelLoggerDbContext>(builder.Configuration["TravelLoggerDbConnectionString"]);

var app = builder.Build();

// Comment out HTTPS redirection for now to simplify testing
// app.UseHttpsRedirection();

// Configure Swagger for all environments during development
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});

// Add all endpoints here

app.MapGet("/api/cities", (TravelLoggerDbContext db) =>
{
    return db.Cities
    .Select(c => new CityDTO
    {
        Id = c.Id,
        Name = c.Name
    })
    .ToList();
});

app.MapGet("/api/cities/{id}", (TravelLoggerDbContext db, int id) =>
{
    CityDTO? city = db.Cities
    .Where(c => c.Id == id)
    .Select(c => new CityDTO
    {
        Id = c.Id,
        Name = c.Name
    })
    .SingleOrDefault();

    if (city == null)
    {
        return Results.NotFound();
    }

    city.Logs = db.Logs
    .Where(l => l.CityId == id)
    .Select(l => new LogDTO
    {
        Id = l.Id,
        UserId = l.UserId,
        CityId = l.CityId,
        Comments = l.Comments,
        LoggedAt = l.LoggedAt
    })
    .ToList();

    city.Recommendations = db.Recommendations
    .Where(r => r.CityId == id)
    .Select(r => new RecommendationDTO
    {
        Id = r.Id,
        UserId = r.UserId,
        CityId = r.CityId,
        PlaceName = r.PlaceName,
        Description = r.Description
    })
    .ToList();

    city.Users = db.Users.Where(u => db.Logs
            .Where(l => l.UserId == u.Id)
            .OrderByDescending(l => l.LoggedAt)
            .ThenByDescending(l => l.Id)
            .Select(l => l.CityId)
            .FirstOrDefault() == id)
        .Select(u => new UserDTO
        {
            Id = u.Id,
            Name = u.Name,
            ImageUrl = u.ImageUrl
        })
        .ToList();

    return Results.Ok(city);
});


app.Run();