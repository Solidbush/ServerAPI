using ServerAPI.Entities;
using ServerAPI.Services;

namespace ServerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<StorageDb>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
            });
            builder.Services.AddScoped<IStorageRepository, StorageRepository>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<StorageDb>();
                db.Database.EnsureCreated();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            if (app.Environment.IsProduction())
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<StorageDb>();
                db.Database.EnsureCreated();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapGet("/keys/${key}", async (string key, IStorageRepository repository) => 
                {
                    var pair = await repository.GetPairAsync(key);
                    if (pair != null)
                    {
                        Results.Ok(pair);
                        return pair;
                    }
                    else
                    {
                        Results.NotFound();
                        return null;
                    }
                })
                .Produces<Models.KeyValuePair>(StatusCodes.Status200OK)
                .WithName("GetPair")
                .WithTags("Getters");

            app.MapGet("/keys", async (IStorageRepository repository) => 
                {
                    return await repository.GetPairsAsync();
                })
                .Produces<List<Models.KeyValuePair>>(StatusCodes.Status200OK)
                .WithName("GetPairs")
                .WithTags("Getters");

            app.MapPut("/keys/${key}&value=${value}", async (string key, string value, IStorageRepository repository) => 
                {
                    var pair = new Models.KeyValuePair();
                    pair.Key = key;
                    pair.Value = value;
                    await repository.InsertPairAsync(pair);
                    await repository.SaveAsync();
                    Results.Created($"/keys/${pair.Key}", pair);
                })
                .Produces<Models.KeyValuePair>(StatusCodes.Status201Created)
                .WithName("CreatePair")
                .WithTags("Creators");

            app.MapPost("/keys", async ([FromBody] Models.KeyValuePair pair, IStorageRepository repository) =>
                {
                    await repository.InsertPairAsync(pair);
                    await repository.SaveAsync();
                    Results.Created($"/keys/${pair.Key}", pair);
                    return pair;
                })
                .Accepts<Models.KeyValuePair>("application/json")
                .Produces<Models.KeyValuePair>(StatusCodes.Status201Created)
                .WithName("CreatePairJson")
                .WithTags("Creators");

            app.MapDelete("/keys/${key}", async (string key, IStorageRepository repository) => 
                { 
                    await repository.DeletePairAsync(key);
                    await repository.SaveAsync();
                    Results.NoContent();
                })
                .WithName("DeletePair")
                .WithTags("Deleters");

            app.MapControllers();

            app.Run();

        }
    }
}
