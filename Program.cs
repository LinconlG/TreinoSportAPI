using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TreinoSportAPI.BackgroundService;
using TreinoSportAPI.MapperNoSQL;
using TreinoSportAPI.MapperNoSQL.Connection;
using TreinoSportAPI.Mappers;
using TreinoSportAPI.Services;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate();

builder.Services.AddHostedService<RenovarAulasBackground>();

builder.Services.AddSingleton<MongoDBConnection>();

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddScoped<ContaService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<TreinoService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<ContaMapper>();
builder.Services.AddScoped<LoginMapper>();
builder.Services.AddScoped<TreinoMapper>();
builder.Services.AddScoped<TreinoMapperNoSQL>();

UtilEnvironment.Load(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
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
        ValidIssuer = "AuthApp",
        // N�o valida a audi�ncia
        ValidateAudience = true,
        ValidAudience = "Users",
        // Valida o tempo de expira��o do token
        ValidateLifetime = true,
        // Remove a toler�ncia padr�o de 5 minutos para expira��o
        ClockSkew = TimeSpan.Zero
    };
});


// Configura��o do CORS para permitir requisi��es de qualquer origem
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
