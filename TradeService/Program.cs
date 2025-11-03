using TradeService.Config;
using TradeService.Services.HttpService;
using TradeService.Services.RabbitMQ;
using TradeService.Services.Redis;

var builder = WebApplication.CreateBuilder(args);

//Env variables
builder.Configuration.AddEnvironmentVariables();

// Configs
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));

// Add services to the container.
builder.Services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddHostedService<RabbitMQConsumer>();
builder.Services.AddTransient<TradeRequestHttpService>();

// Register HttpClient with dependency injection using HttpClientFactory
builder.Services.AddHttpClient();

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//BRUG SWAGGER I PROD
app.UseSwagger();
app.UseSwaggerUI();

//STOP REDIRECTING TO HTTPS
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
