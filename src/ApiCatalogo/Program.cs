using System.Text.Json.Serialization;
using ApiCatalogo.Context;
using ApiCatalogo.Extentions;
using ApiCatalogo.Filters;
using ApiCatalogo.Logging;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(
    options =>
    {
        options.Filters.Add(typeof(ApiExceptionFilter));
    }).AddJsonOptions(
    options =>
                options.JsonSerializerOptions.ReferenceHandler
                = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

var mySqlConnection =
builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggingProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/openapi/v1.json", "Weather Api"));

    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();
// app.UseAuthentication(); // já defini lá em cima
app.UseAuthorization();

app.Use(async (context, next) =>
{
    // adiciona o código antes do request
    await next(context);
    // adiciona o código depois do request
}
);

app.MapControllers();
app.Run();

