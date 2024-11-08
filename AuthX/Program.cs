using System.Text;
using AuthX.Domain.Models;
using AuthX.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var allowAllCorsPolicy = "";

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<AuthXDbContext>(option => option.UseLazyLoadingProxies().UseSqlServer(configuration.GetConnectionString("SqlServerConnection")));

// Add Identity
builder.Services.AddIdentity<User, AuthX.Domain.Models.Role>().AddEntityFrameworkStores<AuthXDbContext>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowAllCorsPolicy, builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});

// Add Redis configuration
builder.Services.AddScoped<IConnectionMultiplexer>(sp =>
{
    var redisConnectionString = configuration.GetConnectionString("RedisConnection");
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

//for JWT token [authorize]
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = configuration["JWT:ValidIssuer"],
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudiance"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]))
    };
}
);

// Constants for reusability
const string API_TITLE = "AuthX API";
const string API_VERSION = "v1";
const string BEARER_SCHEME = "Bearer";

builder.Services.AddSwaggerGen(c =>
{
    // Add API Info with additional metadata
    c.SwaggerDoc(API_VERSION, new OpenApiInfo
    {
        Title = API_TITLE,
        Version = API_VERSION,
        Description = "API for AuthX - Centralized Authentication Microservice",
        Contact = new OpenApiContact
        {
            Name = "AuthX Support",
            Email = "support@authx.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Define BearerAuth security scheme
    c.AddSecurityDefinition(BEARER_SCHEME, new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Authorization: Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Apply security globally to all endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = BEARER_SCHEME
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
