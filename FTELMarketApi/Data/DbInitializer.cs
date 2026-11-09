using FTELMarketApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FTELMarketApi.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.Migrate();

        if (context.Categories.Any())
        {
            return;
        }

        var categories = new[]
        {
            new Category { Name = "Électronique" },
            new Category { Name = "Maison" },
            new Category { Name = "Sport" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        context.Products.AddRange(
            new Product
            {
                Name = "Ordinateur portable",
                Description = "Ultrabook léger pour travail quotidien.",
                Price = 899.99m,
                QuantityInStock = 12,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categories[0].Id
            },
            new Product
            {
                Name = "Lampe LED",
                Description = "Lampe intelligente avec télécommande.",
                Price = 49.90m,
                QuantityInStock = 25,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categories[1].Id
            },
            new Product
            {
                Name = "Ballon de football",
                Description = "Ballon officiel pour entraînement et match.",
                Price = 24.50m,
                QuantityInStock = 40,
                CreatedAt = DateTime.UtcNow,
                CategoryId = categories[2].Id
            });

        context.SaveChanges();
    }
}
