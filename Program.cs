using Microsoft.AspNetCore.Authentication.Certificate;
using TreinoSportAPI.MapperNoSQL;
using TreinoSportAPI.MapperNoSQL.Connection;
using TreinoSportAPI.Mappers;
using TreinoSportAPI.Services;
using TreinoSportAPI.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate();

builder.Services.AddSingleton<MongoDBConnection>();

builder.Services.AddScoped<ContaService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<TreinoService>();

builder.Services.AddScoped<ContaMapper>();
builder.Services.AddScoped<LoginMapper>();
builder.Services.AddScoped<TreinoMapper>();
builder.Services.AddScoped<TreinoMapperNoSQL>();

UtilEnvironment.Load(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
