using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RPCMAS.Core.Data;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Models;

namespace RPCMAS.Infrastructure.Seeder
{
    public static class ItemCatalogSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger("ItemCatalogSeeder");

            var seededItems = BuildSeedItems();
            var seededIds = seededItems.Select(item => item.Id).ToList();

            var existingIds = await dbContext.ItemCatalogs
                .Where(item => seededIds.Contains(item.Id))
                .Select(item => item.Id)
                .ToListAsync();

            var existingIdSet = existingIds.ToHashSet();
            var itemsToInsert = seededItems
                .Where(item => !existingIdSet.Contains(item.Id))
                .ToList();

            if (itemsToInsert.Count == 0)
            {
                logger.LogInformation("Item catalog seed skipped. All seeded items already exist.");
                return;
            }

            await dbContext.ItemCatalogs.AddRangeAsync(itemsToInsert);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Item catalog seed completed. Inserted {Count} items.", itemsToInsert.Count);
        }

        private static List<ItemCatalogModel> BuildSeedItems()
        {
            var items = new List<ItemCatalogModel>();

            items.AddRange(BuildApparelItems(
                department: "Men's Wear",
                category: "Shirts",
                baseItemName: "Men's Polo Shirt",
                skuPrefix: "MW-POLO",
                priceStart: 699m,
                costStart: 350m));

            items.AddRange(BuildApparelItems(
                department: "Ladies' Wear",
                category: "Blouses",
                baseItemName: "Ladies Blouse",
                skuPrefix: "LW-BLOUSE",
                priceStart: 749m,
                costStart: 375m));

            items.AddRange(BuildShoeItems());
            items.AddRange(BuildLipstickItems());
            items.AddRange(BuildBedsheetItems());
            items.AddRange(BuildHandbagItems());
            items.AddRange(BuildPanItems());

            return items;
        }

        private static List<ItemCatalogModel> BuildApparelItems(
            string department,
            string category,
            string baseItemName,
            string skuPrefix,
            decimal priceStart,
            decimal costStart)
        {
            var brands = new[] { "Urban Thread", "North Point", "Blue Harbor", "Prime Line", "Vista Wear", "Mono Label" };
            var colors = new[] { "Black", "White", "Navy", "Gray", "Red", "Green", "Khaki", "Maroon" };
            var sizes = new[] { "XS", "S", "M", "L", "XL", "2XL", "3XL" };

            var items = new List<ItemCatalogModel>();
            var index = 0;

            foreach (var brand in brands)
            {
                foreach (var color in colors)
                {
                    foreach (var size in sizes)
                    {
                        index++;
                        items.Add(CreateItem(
                            department,
                            category,
                            brand,
                            $"{baseItemName} - {color} - Size {size}",
                            $"{skuPrefix}-{brand[..2].ToUpperInvariant()}-{color[..2].ToUpperInvariant()}-{size}",
                            priceStart + (index % 6 * 45m),
                            costStart + (index % 6 * 20m)));
                    }
                }
            }

            return items;
        }

        private static List<ItemCatalogModel> BuildShoeItems()
        {
            var brands = new[] { "Stride Lab", "Urban Step", "Motion Peak", "Run Pro", "Aero Walk", "Lite Pace" };
            var colors = new[] { "Black", "White", "Gray", "Blue", "Red", "Beige", "Olive", "Pink" };
            var sizes = new[] { "5", "6", "7", "8", "9", "10", "11", "12" };

            var items = new List<ItemCatalogModel>();
            var index = 0;

            foreach (var brand in brands)
            {
                foreach (var color in colors)
                {
                    foreach (var size in sizes)
                    {
                        index++;
                        items.Add(CreateItem(
                            "Shoes",
                            "Footwear",
                            brand,
                            $"Rubber Shoes - {color} - Size {size}",
                            $"SH-RUB-{brand[..2].ToUpperInvariant()}-{color[..2].ToUpperInvariant()}-{size}",
                            1499m + (index % 7 * 90m),
                            800m + (index % 7 * 40m)));
                    }
                }
            }

            return items;
        }

        private static List<ItemCatalogModel> BuildLipstickItems()
        {
            var brands = new[] { "Glow Lab", "Velvet Hue", "Color Muse", "Bloom Beauty", "Luxe Tint", "Shine Edit" };
            var shades = new[] { "Rose", "Cherry", "Berry", "Nude", "Coral", "Plum", "Mocha", "Peach", "Ruby", "Wine", "Caramel", "Pink" };
            var finishes = new[] { "Matte", "Cream", "Gloss" };

            var items = new List<ItemCatalogModel>();
            var index = 0;

            foreach (var brand in brands)
            {
                foreach (var shade in shades)
                {
                    foreach (var finish in finishes)
                    {
                        index++;
                        items.Add(CreateItem(
                            "Cosmetics",
                            "Makeup",
                            brand,
                            $"Lipstick - {shade} - {finish}",
                            $"COS-LIP-{brand[..2].ToUpperInvariant()}-{shade[..2].ToUpperInvariant()}-{finish[..2].ToUpperInvariant()}",
                            299m + (index % 5 * 35m),
                            140m + (index % 5 * 15m)));
                    }
                }
            }

            return items;
        }

        private static List<ItemCatalogModel> BuildBedsheetItems()
        {
            var brands = new[] { "Home Nest", "Soft Haven", "Dream Weave", "Cozy Lane", "Casa Daily", "Rest Line" };
            var colors = new[] { "White", "Gray", "Blue", "Beige", "Pink", "Green", "Lavender", "Navy" };
            var sizes = new[] { "Single", "Twin", "Full", "Queen", "King", "California King" };

            var items = new List<ItemCatalogModel>();
            var index = 0;

            foreach (var brand in brands)
            {
                foreach (var color in colors)
                {
                    foreach (var size in sizes)
                    {
                        index++;
                        items.Add(CreateItem(
                            "Housewares",
                            "Bedroom",
                            brand,
                            $"Bedsheet Set - {color} - {size}",
                            $"HW-BED-{brand[..2].ToUpperInvariant()}-{color[..2].ToUpperInvariant()}-{size.Replace(" ", string.Empty).ToUpperInvariant()}",
                            1199m + (index % 6 * 120m),
                            650m + (index % 6 * 50m)));
                    }
                }
            }

            return items;
        }

        private static List<ItemCatalogModel> BuildHandbagItems()
        {
            var brands = new[] { "Belle Carry", "Modern Tote", "City Charm", "Mode Daily", "Avenue Bag", "Pure Style" };
            var colors = new[] { "Black", "Tan", "Brown", "Beige", "Red", "Pink", "White", "Olive" };
            var materials = new[] { "Leather", "Canvas", "Nylon", "Vegan Leather" };

            var items = new List<ItemCatalogModel>();
            var index = 0;

            foreach (var brand in brands)
            {
                foreach (var color in colors)
                {
                    foreach (var material in materials)
                    {
                        index++;
                        items.Add(CreateItem(
                            "Ladies' Wear",
                            "Bags",
                            brand,
                            $"Handbag - {color} - {material}",
                            $"LW-HAN-{brand[..2].ToUpperInvariant()}-{color[..2].ToUpperInvariant()}-{CreateCode(material)}",
                            1599m + (index % 6 * 110m),
                            850m + (index % 6 * 50m)));
                    }
                }
            }

            return items;
        }

        private static List<ItemCatalogModel> BuildPanItems()
        {
            var brands = new[] { "Chef Base", "Kitchen Craft", "Cook Ease", "Daily Flame", "Home Sizzle", "Iron Nest" };
            var colors = new[] { "Black", "Gray", "Red", "Blue" };
            var sizes = new[] { "20cm", "24cm", "26cm", "28cm", "30cm" };

            var items = new List<ItemCatalogModel>();
            var index = 0;

            foreach (var brand in brands)
            {
                foreach (var color in colors)
                {
                    foreach (var size in sizes)
                    {
                        index++;
                        items.Add(CreateItem(
                            "Housewares",
                            "Cookware",
                            brand,
                            $"Non-stick Frying Pan - {color} - {size}",
                            $"HW-PAN-{brand[..2].ToUpperInvariant()}-{color[..2].ToUpperInvariant()}-{size}",
                            899m + (index % 5 * 95m),
                            480m + (index % 5 * 35m)));
                    }
                }
            }

            return items;
        }

        private static ItemCatalogModel CreateItem(
            string department,
            string category,
            string brand,
            string itemName,
            string sku,
            decimal currentPrice,
            decimal cost)
        {
            var seedKey = $"{department}|{category}|{brand}|{itemName}|{sku}";

            return new ItemCatalogModel
            {
                Id = CreateDeterministicGuid(seedKey),
                SKU = sku,
                ItemName = itemName,
                Department = department,
                Category = category,
                Brand = brand,
                CurrentPrice = currentPrice,
                Cost = cost,
                Status = ItemStatus.Active
            };
        }

        private static Guid CreateDeterministicGuid(string value)
        {
            var hash = MD5.HashData(Encoding.UTF8.GetBytes(value));
            return new Guid(hash);
        }

        private static string CreateCode(string value)
        {
            return value
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .ToUpperInvariant();
        }
    }
}
