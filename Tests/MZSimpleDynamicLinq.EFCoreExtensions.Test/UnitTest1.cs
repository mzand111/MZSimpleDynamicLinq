namespace MZSimpleDynamicLinq.EFCoreExtensions.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using MZSimpleDynamicLinq.Core;
    using MZSimpleDynamicLinq.EFCoreExtensions.Test.TestHelperClasses;

    public class UnitTest1
    {
        private class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions options) : base(options) { }

            public DbSet<OwnerView> Owners { get; set; }
        }

        //[Fact]
        //public async Task ToLinqDataResultAsync_AppliesFilterLoadedFromJson()
        //{
        //    // Arrange - load filter from Assets/OwnersQuery.json
        //    var jsonPath = Path.Combine(AppContext.BaseDirectory, "Assets", "OwnersQuery.json");
        //    var json = File.ReadAllText(jsonPath);

        //    var filter = JsonSerializer.Deserialize<Filter>(json, new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    });

        //    // create in-memory EF Core context
        //    var options = new DbContextOptionsBuilder<TestDbContext>()
        //        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        //        .Options;

        //    // Seed data: one matching ID from the JSON and two non-matching
        //    var matchingId = "0010043896"; // present in OwnersQuery.json
        //    using (var ctx = new TestDbContext(options))
        //    {
        //        ctx.Owners.AddRange(new[] {
        //            new OwnerView { ID = matchingId, Title = "Match" },
        //            new OwnerView { ID = "NONMATCH1", Title = "Nope" },
        //            new OwnerView { ID = "NONMATCH2", Title = "Nope2" }
        //        });
        //        ctx.SaveChanges();
        //    }

        //    // Act
        //    LinqDataResult<OwnerView> result;
        //    using (var ctx = new TestDbContext(options))
        //    {
        //        var queryable = ctx.Owners.AsQueryable();

        //        result = await queryable.ToLinqDataResultAsync(10, 0, null, filter);
        //    }

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(3, result.RecordsTotal);
        //    // RecordsFiltered should reflect only matched entries
        //    Assert.Equal(1, result.RecordsFiltered);
        //    Assert.Single(result.Data);
        //    Assert.Contains(result.Data, d => d.ID == matchingId);
        //}

        [Fact]
        public async Task ToLinqDataResultAsync_IsNull()
        {


            var filter = new Filter()
            {
                Logic = "and"               
            };
            filter.Filters = new List<Filter>() {
                new Filter() { Field = "ProvinceId", Operator = "isnull" }
            };

            // create in-memory EF Core context
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var matchingId = "0010043896";
            // Seed data: one matching ID from the JSON and two non-matching
            using (var ctx = new TestDbContext(options))
            {
                ctx.Owners.AddRange(new[] {
                    new OwnerView { ID ="NONMATCH0", Title = "Match" , ProvinceId=1},
                    new OwnerView { ID = "NONMATCH1", Title = "Nope" ,ProvinceId=2},
                    new OwnerView { ID = matchingId, Title = "Nope2", ProvinceId=null}
                });
                ctx.SaveChanges();
            }

            // Act
            LinqDataResult<OwnerView> result;
            using (var ctx = new TestDbContext(options))
            {
                var queryable = ctx.Owners.AsQueryable();

                result = await queryable.ToLinqDataResultAsync(10, 0, null, filter);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.RecordsTotal);
            // RecordsFiltered should reflect only matched entries
            Assert.Equal(1, result.RecordsFiltered);
            Assert.Single(result.Data);
            Assert.Contains(result.Data, d => d.ID == matchingId);
        }
        [Fact]
        public async Task ToLinqDataResultAsync_IsNotNull()
        {


            var filter = new Filter()
            {
                Logic = "and"
            };
            filter.Filters = new List<Filter>() {
                new Filter() { Field = "ProvinceId", Operator = "isnotnull" }
            };

            // create in-memory EF Core context
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var matchingId = "0010043896";
            // Seed data: one matching ID from the JSON and two non-matching
            using (var ctx = new TestDbContext(options))
            {
                ctx.Owners.AddRange(new[] {
                    new OwnerView { ID ="NONMATCH0", Title = "Match" , ProvinceId=null},
                    new OwnerView { ID = "NONMATCH1", Title = "Nope" ,ProvinceId=null},
                    new OwnerView { ID = matchingId, Title = "Nope2", ProvinceId=3}
                });
                ctx.SaveChanges();
            }

            // Act
            LinqDataResult<OwnerView> result;
            using (var ctx = new TestDbContext(options))
            {
                var queryable = ctx.Owners.AsQueryable();

                result = await queryable.ToLinqDataResultAsync(10, 0, null, filter);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.RecordsTotal);
            // RecordsFiltered should reflect only matched entries
            Assert.Equal(1, result.RecordsFiltered);
            Assert.Single(result.Data);
            Assert.Contains(result.Data, d => d.ID == matchingId);
        }
    }
}
