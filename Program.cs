using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using TreinoSportAPI.BackgroundService;
using TreinoSportAPI.Mappers.NoSQL;
using TreinoSportAPI.Mappers.NoSQL.Connection;
using TreinoSportAPI.Mappers;
using TreinoSportAPI.Mappers.Connection;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Services;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Utilities;
using TreinoSportAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => {
        options.InvalidModelStateResponseFactory = context => {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .SelectMany(e => e.Value.Errors.Select(err => err.ErrorMessage))
                .ToList();
            var apiError = new TreinoSportAPI.Models.ApiError(
                string.Join("; ", errors), true);
            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(apiError);
        };
    });

builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate();

builder.Services.AddHostedService<RenovarAulasBackground>();

builder.Services.AddSingleton<MongoDBConnection>();
builder.Services.AddSingleton<SqlConnectionFactory>();

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddScoped<IContaService, ContaService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ITreinoService, TreinoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddTransient<GlobalExceptionMiddleware>();
builder.Services.AddHttpClient<UsuarioService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IContaMapper, ContaMapper>();
builder.Services.AddScoped<ILoginMapper, LoginMapper>();
builder.Services.AddScoped<ITreinoMapper, TreinoMapper>();
builder.Services.AddScoped<ITreinoMapperNoSQL, TreinoMapperNoSQL>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") 
    ?? builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtSecret))
    throw new InvalidOperationException("JWT secret is not configured. Set the JWT_SECRET environment variable.");
var key = Encoding.ASCII.GetBytes(jwtSecret);
builder.Services.AddAuthentication(auth => {
    // Define o esquema padr�o de autentica��o como JWT Bearer
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // Define o esquema padr�o para desafios de autentica��o
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt => {
    // N�o requer HTTPS para desenvolvimento (remova em produ��o)
    jwt.RequireHttpsMetadata = false;
    // Indica que o token deve ser salvo ap�s a valida��o
    jwt.SaveToken = true;
    // Par�metros de valida��o do token
    jwt.TokenValidationParameters = new TokenValidationParameters {
        // Valida a assinatura do token
        ValidateIssuerSigningKey = true,
        // Define a chave de seguran�a usada para validar a assinatura
        IssuerSigningKey = new SymmetricSecurityKey(key),
        // N�o valida o emissor
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        // N�o valida a audi�ncia
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        // Valida o tempo de expira��o do token
        ValidateLifetime = true,
        // Remove a toler�ncia padr�o de 5 minutos para expira��o
        ClockSkew = TimeSpan.Zero
    };

    jwt.Events = new JwtBearerEvents {
        OnAuthenticationFailed = context => {
            Console.WriteLine($"Falha na autentica��o: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context => {
            Console.WriteLine("Token validado com sucesso!");
            return Task.CompletedTask;
        }
    };

});




var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngularDev",
        policy => policy
            .WithOrigins(corsOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader());
            //.AllowCredentials());
});
builder.WebHost.ConfigureKestrel(serverOptions => {
    serverOptions.ListenAnyIP(5050); // Porta desejada
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no campo abaixo."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

builder.Services.AddRateLimiter(options => {
    options.AddFixedWindowLimiter("PasswordReset", opt => {
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromMinutes(15);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// Use isso ANTES de UseAuthorization() e MapControllers()
app.UseCors("AllowAngularDev");

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
