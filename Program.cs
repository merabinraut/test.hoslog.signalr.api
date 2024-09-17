using System.Reflection;
using hoslog.signalr.api.Hubs;
using hoslog.signalr.api.Models.Cache;
using hoslog.signalr.api.Repository.CustomerNotification;
using hoslog.signalr.api.Services;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials();
    });
});

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "hoslog.signalr.api",
        Version = "v1"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    x.IncludeXmlComments(xmlPath);
});

builder.Services.Configure<RedisCacheSetting>(configuration.GetSection(nameof(RedisCacheSetting)));
builder.Services.AddSingleton(sp =>
{
    return sp.GetRequiredService<IOptions<RedisCacheSetting>>().Value;
});
// builder.Services.AddSingleton<IRedisCacheSetting>(sp =>
// {
//     var redisCacheSetting = sp.GetRequiredService<IOptions<RedisCacheSetting>>().Value;
//     return redisCacheSetting;
// });

var redisCacheSetting = configuration.GetSection(nameof(RedisCacheSetting)).Get<RedisCacheSetting>();

if (redisCacheSetting.isEnabled)
{
    var redisConfiguration = $"{redisCacheSetting.connectionString},password={redisCacheSetting.password}";

    var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConfiguration);
    builder.Services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConfiguration;
        options.InstanceName = redisCacheSetting.instanceName;
    });

    builder.Services.AddSignalR()
        .AddStackExchangeRedis(redisConfiguration, options =>
        {
            options.Configuration.ChannelPrefix = redisCacheSetting.channelPrefix;
        }
    );

    builder.Services.AddSingleton<RedisConnectionService>();
}

builder.Services.AddSignalR();
builder.Services.AddScoped<ICustomerNotificationRepository, CustomerNotificationRepository>();
builder.Services.AddScoped<CustomerNotificationServices>();

var app = builder.Build();

var redisService = app.Services.GetService<RedisConnectionService>();
redisService?.ClearAllData();

app.Lifetime.ApplicationStopped.Register(() =>
{
    redisService?.ClearAllData();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "hoslog.signalr.api v1");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<CustomerNotificationHub>("/CustomerNotificationHub");
});

app.Run();

