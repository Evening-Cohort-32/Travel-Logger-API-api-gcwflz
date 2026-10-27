using Microsoft.EntityFrameworkCore;
using TravelLogger.Models;

namespace TravelLogger;

public class TravelLoggerDbContext : DbContext
{

    // Define tables here
    public DbSet<User> Users { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<Recommendation> Recommendations { get; set; }
    public DbSet<Upvote> Upvotes { get; set; }


    public TravelLoggerDbContext(DbContextOptions<TravelLoggerDbContext> context) : base(context)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed data here

        modelBuilder.Entity<City>().HasData(new City[]
        {
        new City {Id = 1, Name = "Nashville"},
        new City {Id = 2, Name = "Chicago"},
        new City {Id = 3, Name = "New York"},
        });

        modelBuilder.Entity<User>().HasData(new User[]
        {
            new User
            {
                Id = 1,
                Name = "Rick Sanchez",
                Email = "rick@mail.com",
                Description = "Genius scientist, interdimensional traveler, and reluctant tourist. I've seen infinite universes, and most of them suck.",
                ImageUrl = "https://static.wikia.nocookie.net/ricksanchez/images/7/71/Rick.jpg/revision/latest?cb=20160605174338"
            },
              new User
            {
                Id = 2,
                Name = "Morty Smith",
                Email = "morty@mail.com",
                Description = "Just a regular teenager trying to survive adventures with my grandpa. I've been to places you wouldn't believe.",
                ImageUrl = "https://static.wikia.nocookie.net/rickandmorty/images/e/ee/Morty501.png/revision/latest/thumbnail/width/360/height/450?cb=20210827150137"
            },
              new User
            {
                Id = 3,
                Name = "Summer Smith",
                Email = "summer@mail.com",
                Description = "Exploring the multiverse, making questionable decisions, and looking fabulous doing it.",
                ImageUrl = "https://static.wikia.nocookie.net/rickandmorty/images/a/a7/Sum-Sum.jpg/revision/latest?cb=20261024190751"
            },
              new User
            {
                Id = 4,
                Name = "Beth Smith",
                Email = "beth@mail.com",
                Description = "Horse surgeon, mother, and occasional space adventurer. Looking for something beyond suburban life.",
                ImageUrl = "https://static.wikia.nocookie.net/rickandmorty/images/5/58/Beth_Smith.png/revision/latest?cb=20231221010211"
            },
              new User
            {
                Id = 5,
                Name = "Jerry Smith",
                Email = "jerry@mail.com",
                Description = "Family man who enjoys simple vacations, predictable weather, and destinations where nothing tries to kill me.",
                ImageUrl = "https://static.wikia.nocookie.net/rickandmorty/images/f/f1/Jerry_Smith.png/revision/latest?cb=20260914135038"
            }


        });

        modelBuilder.Entity<Log>().HasData(new Log[]
        {
            new Log
            {
                Id = 1,
                UserId = 1,
                CityId = 1,
                Comments = "This dimension has surprisingly good food.",
                LoggedAt = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Log
            {
                Id = 2,
                UserId = 2,
                CityId = 2,
                Comments = "I just want a normal vactaion for once.",
                LoggedAt = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        });

        modelBuilder.Entity<Recommendation>().HasData(new Recommendation[]
        {
            new Recommendation
            {
                Id = 1,
                UserId = 1,
                CityId = 1,
                PlaceName = "Grand Ole Opry",
                Description = "Not the worst entertainment in this dimension."
            },
             new Recommendation
            {
                Id = 2,
                UserId = 2,
                CityId = 2,
                PlaceName = "Field Museum",
                Description = "Finally, a place where nothing is trying to kill me."
            },


        });

        modelBuilder.Entity<Upvote>().HasData(new Upvote[]
        {
            new Upvote
            {
                Id = 1,
                UserId = 1,
                RecommendationId = 2
            },
             new Upvote
            {
                Id = 2,
                UserId = 2,
                RecommendationId = 1
            }
        });

    }
}