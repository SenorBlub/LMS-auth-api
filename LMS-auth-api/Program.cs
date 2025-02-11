using System.Text;
using DAL.Contexts;
using DAL.Repositories;
using Logic.Configuration;
using Logic.IRepositories;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using DotNetEnv;
using Logic.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Env.Load();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(corsPolicyBuilder =>
	{
		corsPolicyBuilder.WithOrigins()
			.AllowAnyOrigin()
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials()
			.SetIsOriginAllowed(_ => true);
	});
});

var connectionString =
	$"Server={Env.GetString("DB_HOST")};Port={Env.GetString("DB_PORT")};Database={Env.GetString("DB_NAME")};User={Env.GetString("DB_USER")};Password={Env.GetString("DB_PASSWORD")};";

builder.Services.AddDbContext<AuthDbContext>(options =>
	options.UseMySql(
		connectionString,
		new MySqlServerVersion(new Version(Env.GetInt("SQL_MAJOR"), Env.GetInt("SQL_MINOR"), Env.GetInt("SQL_BUILD")))
	)
);

//!TODO auth

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<HttpClient, HttpClient>();

var jwtConfig = new JwtConfig
{
	Secret = Env.GetString("JWT_SECRET_KEY"),
	Issuer = Env.GetString("JWT_ISSUER"),
	Audience = Env.GetString("JWT_AUDIENCE")
};
builder.Services.AddSingleton(jwtConfig);

var secretKey = Encoding.UTF8.GetBytes(jwtConfig.Secret);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(secretKey),
			ValidateIssuer = true,
			ValidIssuer = jwtConfig.Issuer,
			ValidateAudience = true,
			ValidAudience = jwtConfig.Audience,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
