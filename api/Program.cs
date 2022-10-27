using api.Data;
using api.Logic;
using api.Middleware;
using api.Services;
using data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// deps lol
builder.Services.AddTransient<PolicyLogic>();

builder.Services.AddTransient<OpaService>();
builder.Services.AddTransient<PermissionService>();
builder.Services.AddTransient<PermissionData>();

builder.Services.AddHttpClient<Auth0Service>();

builder.Services.AddAutoMapper(x => x.AddProfile<DataProfile>());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<UserService>();

//builder.Services.AddHttpClient<UserService>();

builder.Services.AddDbContext<MainContext>();

builder.Services.AddCors(p => p.AddPolicy("main", b =>
{
    b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseExceptionHandler("/error");

app.UseMiddleware<AuthMiddleware>();

app.UseCors("main");

// Configure the HTTP request pipeline.
if (app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
