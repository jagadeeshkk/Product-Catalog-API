using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using ProductCatalog.API.Business.IRepository;
using ProductCatalog.API.Business.Repository;
using ProductCatalog.API.Data;
using ProductCatalog.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ProductCatalogDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "ProductDatabase.db");
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }); 
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    // Default version to execute if the client does not specify one
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Advertise supported/deprecated versions back to client via headers
    options.ReportApiVersions = true;

    // CHOOSE YOUR READING STRATEGY:
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    // Formats the version group tag for Swagger layout matching (e.g., 'v1')
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Product Catalog API",
        Version = "v1",
        Description = "Product summary, detail, and catalog metrics endpoints."
    });
});

var app = builder.Build();
// --- Migrate and seed the SQLite database on startup ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();
    db.Database.Migrate();
    var seedLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger("DbSeeder");
    await DbSeeder.SeedAsync(db, seedLogger);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
