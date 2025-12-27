using apitienda.Data;
using apitienda.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using productos.Data;
using System.Text;
using Microsoft.OpenApi.Models; // Import necesario para OpenApi

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Carga las variables de entorno.
/// </summary>
Env.Load();

/// <summary>
/// Obtiene las cadenas de conexión.
/// </summary>
var connectionStringUser = builder.Configuration.GetConnectionString("CONNECTIONSTRINGS__DEFAULTCONNECTIONUSERS");
var connectionStringProducts = builder.Configuration.GetConnectionString("CONNECTIONSTRINGS__DEFAULTCONNECTIONPRODUCTS");


/// <summary>
/// Configura los contextos de datos.
/// </summary>
builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(connectionStringUser));
builder.Services.AddDbContext<DataContextProduct>(options => options.UseNpgsql(connectionStringProducts));

/// <summary>
/// Registra servicios personalizados.
/// </summary>
builder.Services.AddScoped<IUsuarioDAO, UsuarioDAO>();
builder.Services.AddScoped<UsuarioMapper>();
builder.Services.AddScoped<CreateUserMapper>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<PasswordResetEmail>();

builder.Services.AddScoped<IProductDAO, ProductDAO>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ProductMapper>();
builder.Services.AddScoped<CreateProductMapper>();

// Agrega el servicio JwtService
builder.Services.AddScoped<JwtService>();

/// <summary>
/// Agrega controladores y swagger.
/// </summary>
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });

    // Configuración para JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' seguido de un espacio y luego el token JWT."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference 
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

/// <summary>
/// Configura la autenticación JWT.
/// </summary>
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

//Se elimino la condición.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hola mundo! Nuestra primera API usando C#");

app.Run();
