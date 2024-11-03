using System.Text;
using AuthX.Domain.Models;
using AuthX.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var allowAllCorsPolicy = "";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuthXDbContext>(option => option.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("AuthXDbConnection")));
builder.Services.AddIdentity<UserModel, RoleModel>().AddEntityFrameworkStores<AuthXDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(allowAllCorsPolicy, builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});

//for jwt token [authorize]
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

// swagger configuration
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthX_API", Version = "v1" });
//     // Define the BearerAuth security scheme
//     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
//         Name = "Authorization",
//         In = ParameterLocation.Header,
//         Type = SecuritySchemeType.Http,
//         Scheme = "bearer",
//         BearerFormat = "JWT"
//     });
//     // Apply the security globally
//     c.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//                 {
//                     Reference = new OpenApiReference
//                     {
//                         Type = ReferenceType.SecurityScheme,
//                         Id = "Bearer"
//                     }
//                 },
//             Array.Empty<string>()
//         }
//     });
// });

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

    // Apply the security globally to all endpoints
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
