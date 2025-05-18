using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using ApiCatalogo.Context;
using ApiCatalogo.Extentions;
using ApiCatalogo.Filters;
using ApiCatalogo.Logging;
using ApiCatalogo.Models;
using ApiCatalogo.Repositories;
using ApiCatalogo.Services;
using APICatalogo.RateLimitOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

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

var OrigensComAcessoPermitido = "_origensComAcessoPermitido";
builder.Services.AddCors(options =>
    options.AddPolicy(name: OrigensComAcessoPermitido,
                      policy =>
                      {
                          policy.WithOrigins(
                                "https://apirequest.io",
                                "https://localhost:7072",
                                "https://localhost:5038",
                                "http://localhost:7072",
                                "http://localhost:5038"
                          )
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                      })
);


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Configura OpenAPI com suporte à autenticação JWT
builder.Services.AddOpenApi();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

var mySqlConnection =
builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

//// JWT
///////

var secretKey = builder.Configuration["JWT:SecretKey"]
    ?? throw new ArgumentException("Invalid secret key!!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // TODO: produção true
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))

    };
});
//////
////


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
                        options => options.RequireRole("Admin"));
    options.AddPolicy("SuperAdminOnly",
                        options => options.RequireRole("Admin")
                        .RequireClaim("id", "fabiovalinhos"));
    options.AddPolicy("UserOnly",
                        options => options.RequireRole("User"));
    options.AddPolicy("ExclusiveOnly",
                        options => options.RequireAssertion(context => context.User.HasClaim(
                            c => c.Type == "id" &&
                            c.Value == "fabiovalinhos")
                            || context.User.IsInRole("SuperAdminOnly")));
});


//Política de RateLimiter chamada de FixedWindow 
var myOptions = new MyRateLimitOptions();
builder.Configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter(policyName: "fixedwindow", options =>
        {
            options.PermitLimit = myOptions.PermitLimit;
            options.Window = TimeSpan.FromSeconds(myOptions.Window);
            options.QueueLimit = myOptions.QueueLimit;
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Aqui é um RateLimiter global
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpcontext.User.Identity?.Name ?? httpcontext.Request.Headers.Host.ToString(),
        factory: partition =>
        new FixedWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            PermitLimit = 2,
            QueueLimit = 0,
            Window = TimeSpan.FromSeconds(10),
        }
    ));
});



builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenServices, TokenService>();


builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggingProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseRateLimiter();

app.UseCors(OrigensComAcessoPermitido);

app.UseAuthentication();
app.UseAuthorization();


app.Use(async (context, next) =>
{
    // adiciona o código antes do request
    //await next(context);
    // adiciona o código depois do request

    //teste para CORS
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro no servidor: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        throw;
    }
}
);

app.MapControllers();
app.Run();

