namespace Catalog.Api.Data
{
    /// <summary>
    /// Class that contains static methods to populate <see cref="CatalogContext" /> database.
    /// </summary>
    public class SeedData
    {
        /// <summary>
        /// Ensures that the <see cref="CatalogContext" /> database is populated.
        /// </summary>
        /// <param name="app"><see cref="WebApplication" /> instance containing <see cref="CatalogContext" />.</param>
        /// <returns><see cref="Task" /> that represents asynchronous operation.</returns>
        public static async Task EnsurePopulated(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                await SeedAsync(
                    scope.ServiceProvider.GetRequiredService<CatalogContext>(),
                    scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>());
            }
        }

        private static async Task SeedAsync(CatalogContext context, ILogger<SeedData> logger)
        {
            var policy = CreatePolicy(logger, nameof(SeedData));

            await policy.ExecuteAsync(async () =>
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    logger.LogInformation($"---> Applying migrations...");
                    context.Database.Migrate();
                }

                if (!context.CatalogBrands.Any())
                {
                    logger.LogInformation("---> Seeding catalog brands...");
                    await context.CatalogBrands.AddRangeAsync(GetDefaultCatalogBrands());
                    await context.SaveChangesAsync();
                }

                if (!context.CatalogTypes.Any())
                {
                    logger.LogInformation("---> Seeding catalog types...");
                    await context.CatalogTypes.AddRangeAsync(GetDefaultCatalogTypes());
                    await context.SaveChangesAsync();
                }

                if (!context.CatalogItems.Any())
                {
                    logger.LogInformation("---> Seeding catalog items...");
                    await context.CatalogItems.AddRangeAsync(GetDefaultCatalogItems());
                    await context.SaveChangesAsync();
                }
            });
        }

        private static AsyncRetryPolicy CreatePolicy(ILogger<SeedData> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>()
                .WaitAndRetryAsync(
                    retries,
                    retry => TimeSpan.FromSeconds(5),
                    (exception, timeSpan, retry, context) =>
                    {
                        logger.LogWarning(exception,
                        "[{prefix}] ---> Exception {exceptionType} with message {message} on attempt {retry} of {retries}",
                            prefix, exception.GetType(), exception.Message, retry, retries);
                    });
        }

        private static IEnumerable<CatalogBrand> GetDefaultCatalogBrands()
        {
            return new List<CatalogBrand>()
            {
                new() { Brand = "Nike" },
                new() { Brand = "Adidas" },
                new() { Brand = "New Balance" },
                new() { Brand = "Under Armour" },
                new() { Brand = "Reebok" },
                new() { Brand = "FILA" },
                new() { Brand = "Columbia" },
                new() { Brand = "Puma" },
                new() { Brand = "Other" }
            };
        }

        private static IEnumerable<CatalogType> GetDefaultCatalogTypes()
        {
            return new List<CatalogType>()
            {
                new() { Type = "T-Shirt" },
                new() { Type = "Sneakers" },
                new() { Type = "Shorts" },
                new() { Type = "Sweatpants" },
                new() { Type = "Sweatshirts" },
                new() { Type = "Underwear" },
                new() { Type = "Sport Jacket" },
                new() { Type = "Other" }
            };
        }

        private static IEnumerable<CatalogItem> GetDefaultCatalogItems()
        {
            return new List<CatalogItem>()
            {
                new()
                {
                    Name = "Adidas Sneakers", Description = "Brand new black & white sneakers",
                    Price = 1200M, PictureFileName = "adidas-sneakers-1.jpg",
                    CatalogTypeId = 2, CatalogBrandId = 2,
                    AvailableInStock = 30, RestockThreshold = 10,
                    MaxStockThreshold = 100, OnReorder = false
                },
                new()
                {
                    Name = "Puma Shorts", Description = "Brand new black shorts",
                    Price = 400M, PictureFileName = "puma-shorts-1.jpg",
                    CatalogTypeId = 3, CatalogBrandId = 8,
                    AvailableInStock = 50, RestockThreshold = 30,
                    MaxStockThreshold = 200, OnReorder = false
                },
                new()
                {
                    Name = "Fila Bag", Description = "Perfect looking FILA sport bag",
                    Price = 800M, PictureFileName = "fila-bag-1.jpg",
                    CatalogTypeId = 8, CatalogBrandId = 6,
                    AvailableInStock = 50, RestockThreshold = 10,
                    MaxStockThreshold = 75, OnReorder = false
                },
                new()
                {
                    Name = "Nike sneakers", Description = "Brand new red sneakers",
                    Price = 1000M, PictureFileName = "nike-sneakers-1.png",
                    CatalogTypeId = 2, CatalogBrandId = 1,
                    AvailableInStock = 50, RestockThreshold = 20,
                    MaxStockThreshold = 100, OnReorder = false
                },
                new()
                {
                    Name = "Nike T-Shirt", Description = "Good-looking white T-Shirt",
                    Price = 900M, PictureFileName = "nike-tshirt-1.jpg",
                    CatalogTypeId = 1, CatalogBrandId = 1,
                    AvailableInStock = 100, RestockThreshold = 50,
                    MaxStockThreshold = 200, OnReorder = false
                },
                new()
                {
                    Name = "Adidas Sweatpants", Description = "Brand new black & white sweatpants",
                    Price = 2200M, PictureFileName = "adidas-sweatpants-1.jpg",
                    CatalogTypeId = 4, CatalogBrandId = 2,
                    AvailableInStock = 10, RestockThreshold = 5,
                    MaxStockThreshold = 50, OnReorder = false
                },
                new()
                {
                    Name = "Adidas Sweatshirt", Description = "Brand new black & white sweatshirt",
                    Price = 2000M, PictureFileName = "adidas-sweatshirt-1.jpg",
                    CatalogTypeId = 5, CatalogBrandId = 2,
                    AvailableInStock = 15, RestockThreshold = 5,
                    MaxStockThreshold = 50, OnReorder = false
                },
                new()
                {
                    Name = "New Balance Sneakers", Description = "Cool-looking red & blue sneakers",
                    Price = 3000M, PictureFileName = "newbalance-sneakers-1.jpg",
                    CatalogTypeId = 2, CatalogBrandId = 3,
                    AvailableInStock = 15, RestockThreshold = 10,
                    MaxStockThreshold = 40, OnReorder = false
                },
                new()
                {
                    Name = "Under Armour Shorts", Description = "Nice black shorts",
                    Price = 500M, PictureFileName = "underarmour-shorts-1.jpg",
                    CatalogTypeId = 3, CatalogBrandId = 4,
                    AvailableInStock = 50, RestockThreshold = 20,
                    MaxStockThreshold = 100, OnReorder = false
                },
                new()
                {
                    Name = "Nike Underwear", Description = "Black thermal underwear set",
                    Price = 2000M, PictureFileName = "nike-underwear-1.jpg",
                    CatalogTypeId = 6, CatalogBrandId = 1,
                    AvailableInStock = 25, RestockThreshold = 10,
                    MaxStockThreshold = 75, OnReorder = false
                },
                new()
                {
                    Name = "Reebok Sweatshirt", Description = "Blue sweatshirt",
                    Price = 1600M, PictureFileName = "reebok-sweatshirt-1.jpg",
                    CatalogTypeId = 5, CatalogBrandId = 5,
                    AvailableInStock = 15, RestockThreshold = 5,
                    MaxStockThreshold = 50, OnReorder = false
                },
                new()
                {
                    Name = "Columbia Jacket", Description = "Winter jacket",
                    Price = 4000M, PictureFileName = "columbia-jacket-1.jpg",
                    CatalogTypeId = 7, CatalogBrandId = 7,
                    AvailableInStock = 10, RestockThreshold = 3,
                    MaxStockThreshold = 30, OnReorder = false
                },
                new()
                {
                    Name = "Fila T-Shirt", Description = "Cool-looking T-Shirt",
                    Price = 600M, PictureFileName = "fila-tshirt-1.jpg",
                    CatalogTypeId = 1, CatalogBrandId = 6,
                    AvailableInStock = 50, RestockThreshold = 15,
                    MaxStockThreshold = 100, OnReorder = false
                },
                new()
                {
                    Name = "Reebok Sweatpants", Description = "Perfect fit pants",
                    Price = 1400M, PictureFileName = "reebok-sweatpants-1.jpg",
                    CatalogTypeId = 4, CatalogBrandId = 5,
                    AvailableInStock = 30, RestockThreshold = 5,
                    MaxStockThreshold = 50, OnReorder = false
                },
                new()
                {
                    Name = "Puma Jacket", Description = "Demi-season sports jacket",
                    Price = 2500M, PictureFileName = "puma-jacket-1.jpg",
                    CatalogTypeId = 7, CatalogBrandId = 8,
                    AvailableInStock = 20, RestockThreshold = 5,
                    MaxStockThreshold = 50, OnReorder = false
                }
            };
        }
    }
}