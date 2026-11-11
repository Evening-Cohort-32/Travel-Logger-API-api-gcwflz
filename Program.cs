using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using TravelLogger.Models.DTOs;
using TravelLogger;
using TravelLogger.Models;
using Microsoft.AspNetCore.Mvc;

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

app.MapGet("/api/cities/{cityId}/recommendations", (TravelLoggerDbContext db, int cityId) =>
{
    return db.Recommendations
        .Where(r => r.CityId == cityId)
        .Select(r => new RecommendationDTO
        {
            Id = r.Id,
            UserId = r.UserId,
            CityId = r.CityId,
            PlaceName = r.PlaceName,
            Description = r.Description
        }).ToList();
});

app.MapGet("/api/recommendations/{id}", (TravelLoggerDbContext db, int id) =>
{
    RecommendationDTO? recommendation = db.Recommendations
        .Where(r => r.Id == id)
        .Select(r => new RecommendationDTO
        {
            Id = r.Id,
            UserId = r.UserId,
            CityId = r.CityId,
            PlaceName = r.PlaceName,
            Description = r.Description
        }).SingleOrDefault();

    if (recommendation == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(recommendation);
});

app.MapPost("/api/recommendations", (TravelLoggerDbContext db, Recommendation newRecommendation) =>
{
    db.Recommendations.Add(newRecommendation);
    db.SaveChanges();
    return Results.Created($"/api/recommendations/{newRecommendation.Id}", newRecommendation);
});

app.MapPut("/api/recommendations/{id}", (TravelLoggerDbContext db, int id, int userId, Recommendation updatedRecommendation) =>
{
    Recommendation? recommendation = db.Recommendations
    .SingleOrDefault(r => r.Id == id);

    if (recommendation == null)
    {
        return Results.NotFound();
    }
    if (recommendation.UserId != userId)
    {
        return Results.StatusCode(403);
    }
    recommendation.PlaceName = updatedRecommendation.PlaceName;
    recommendation.Description = updatedRecommendation.Description;

    db.SaveChanges();

    return Results.Ok(recommendation);

});

app.MapDelete("/api/recommendations/{id}", (TravelLoggerDbContext db, int id, int userId) =>
{
    Recommendation? recommendation = db.Recommendations
    .SingleOrDefault(r => r.Id == id);

    if (recommendation == null)
    {
        return Results.NotFound();
    }
    if (recommendation.UserId != userId)
    {
        return Results.StatusCode(403);
    }
    db.Recommendations.Remove(recommendation);
    db.SaveChanges();

    return Results.NoContent();
});

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


app.MapGet("/api/users/{userId}/logs", (TravelLoggerDbContext db, int userId) =>
{

    if (db.Users.SingleOrDefault(u => u.Id == userId) == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(

     db.Logs
    .Include(l => l.User)
    .Where(l => l.UserId == userId)
    .Select(l => new LogDTO
    {
        Id = l.Id,
        CityId = l.CityId,
        UserId = l.UserId,
        User = new User
        {
            Id = l.User.Id,
            Name = l.User.Name,
            Email = l.User.Email,
            Description = l.User.Description,
            ImageUrl = l.User.ImageUrl
        },
        Comments = l.Comments,
        LoggedAt = l.LoggedAt
    })
    );
});

app.MapGet("/api/cities/{cityId}/logs", (TravelLoggerDbContext db, int cityId) =>
{
    if (db.Cities.SingleOrDefault(c => c.Id == cityId) == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(
        db.Logs
        .Where(l => l.CityId == cityId)
        .Select(l => new LogDTO
        {
            Id = l.Id,
            CityId = l.CityId,
            UserId = l.UserId,
            Comments = l.Comments,
            LoggedAt = l.LoggedAt
        })
    );
});

app.MapPost("/api/users", (TravelLoggerDbContext db, User newUser) =>
{
    if (db.Users.Any(u => u.Email == newUser.Email))
    {
        return Results.BadRequest("Email is already being used");
    }
    db.Users.Add(newUser);
    db.SaveChanges();
    return Results.Created($"/api/users/{newUser.Id}", newUser);
});

app.MapPut("/api/users/{id}", (TravelLoggerDbContext db, int id, int userId, User user) =>
{
    if (id != userId)
    {
        return Results.StatusCode(403);
    }
    User? userToUpdate = db.Users.SingleOrDefault(u => u.Id == id);
    if (userToUpdate == null)
    {
        return Results.NotFound();
    }
    userToUpdate.Name = user.Name;
    userToUpdate.Description = user.Description;
    userToUpdate.ImageUrl = user.ImageUrl;
    db.SaveChanges
    return Results.NoContent();
});

app.MapGet("/api/users/signin/{email}", (TravelLoggerDbContext db, string email) =>
{
    User? user = db.Users.SingleOrDefault(u => u.Email == email);
    if (user == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new UserDTO
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Description = user.Description,
        ImageUrl = user.ImageUrl
    });
});

app.MapPost("/api/upVotes", (TravelLoggerDbContext db, Upvote upvote) =>
{
    db.Upvotes.Add(upvote);
    db.SaveChanges();
    return Results.Created($"/api/upvotes/{upvote.Id}", upvote);
});

app.MapDelete("/api/upVotes/{id}", (int id, int userId, TravelLoggerDbContext db) =>
{
    Upvote? upvoteToDelete = db.Upvotes.SingleOrDefault(uv => uv.Id == id);
    if (upvoteToDelete == null)
    {
        return Results.BadRequest();
    }
    if (upvoteToDelete.UserId != userId)
    {
        return Results.StatusCode(403);
    }
    db.Upvotes.Remove(upvoteToDelete);
    db.SaveChanges();
    return Results.NoContent();
});

app.MapPost("/api/logs", (TravelLoggerDbContext db, Log newLog) =>
{
    db.Logs.Add(newLog);
    db.SaveChanges();
    return Results.Created($"/api/logs/{newLog.Id}", newLog);
});

app.MapPut("/api/logs/{id}", (TravelLoggerDbContext db, int id, int userId, Log updatedLog) =>
{
    Log? log = db.Logs
    .SingleOrDefault(l => l.Id == id);

    if (log == null)
    {
        return Results.NotFound();
    }
    if (log.UserId != userId)
    {
        return Results.StatusCode(403);
    }

    log.Comments = updatedLog.Comments;
    log.LoggedAt = updatedLog.LoggedAt;

    db.SaveChanges();

    return Results.Ok(log);
});

app.MapDelete("/api/logs/{id}", (TravelLoggerDbContext db, int id, int userId) =>
{
    Log? log = db.Logs
    .SingleOrDefault(l => l.Id == id);

    if (log == null)
    {
        return Results.NotFound();
    }
    if (log.UserId != userId)
    {
        return Results.StatusCode(403);
    }
    db.Logs.Remove(log);
    db.SaveChanges();

    return Results.NoContent();
});

app.Run();
