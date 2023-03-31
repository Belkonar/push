using api.Logic;
using api.Middleware;
using api.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using shared;
using shared.Interfaces;
using shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

// deps lol
// Logics
builder.Services.AddTransient<PolicyLogic>();
builder.Services.AddTransient<OrganizationLogic>();
builder.Services.AddTransient<PipelineLogic>();
builder.Services.AddTransient<ThingLogic>();
builder.Services.AddTransient<JobLogic>();
builder.Services.AddTransient<IGithub, Github>();

// TODO: Properly handle distributed cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();

// Services
builder.Services.AddTransient<OpaService>();

builder.Services.AddHttpClient<Auth0Service>();

builder.Services.AddHttpClient("policy", client =>
{
    client.BaseAddress = new Uri("http://localhost:3000");
});

builder.Services.AddSingleton<ConfigService>();

#pragma warning disable 618
//BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
#pragma warning restore

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

builder.Services.AddSingleton<IMongoDatabase>(_ => new MongoClient(
        builder.Configuration.GetValue<string>("MongoUri")
        ).GetDatabase("push")
);

builder.Services.AddAutoMapper(x => x.AddProfile<DataProfile>());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<UserService>();

//builder.Services.AddHttpClient<UserService>();

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
