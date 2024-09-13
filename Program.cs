using AeroFlex.Data;
using AeroFlex.Helpers;
using AeroFlex.Repository.Contracts;
using AeroFlex.Repository.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DBconnection")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserAccount,UserAccountRepository>();
builder.Services.AddScoped<IFlightOwnerAccount, FlightOwnerAccountRepository>();
builder.Services.AddScoped<IAdminAccount, AdminAcccountRepository>();
builder.Services.AddScoped<IFlight, FlightRepository>();
builder.Services.AddScoped<IBookingService,BookingService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IFlightPricingService,FlightPricingService>();


builder.Services.AddScoped<IFlightTaxRepository, FlightTaxRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IAirlineRepository, AirlineRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IPaymentRepository,PaymentRepository>();
builder.Services.AddScoped<ICountryTaxService, CountryTaxService>();
builder.Services.AddScoped<IFlightScheduleRepository, FlightScheduleRepository>();

builder.Services.Configure<JwtSection>(builder.Configuration.GetSection("JwtSection"));

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme=JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options=>
{
	options.SaveToken=false;
	options.RequireHttpsMetadata=false;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["JwtSection:Issuer"] ,
		ValidAudience = builder.Configuration["JwtSection:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSection:Key"]!))
	};

	options.Events = new JwtBearerEvents
	{
		OnMessageReceived = ctx =>
		{
			ctx.Request.Cookies.TryGetValue("AuthToken", out var accessToken);
			if (!string.IsNullOrEmpty(accessToken)) ctx.Token = accessToken;

			return Task.CompletedTask;
		}
	};
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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
