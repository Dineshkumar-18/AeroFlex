using AeroFlex.Data;
using AeroFlex.Helpers;
using AeroFlex.Repository.Contracts;
using AeroFlex.Repository.Implementations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DBconnection")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserAccount,UserAccountRepository>();
builder.Services.AddScoped<IUserAccount, FlightOwnerAccountRepository>();
builder.Services.AddScoped<IUserAccount, AdminAcccountRepository>();
builder.Services.Configure<JwtSection>(builder.Configuration.GetSection("JwtSection"));

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
