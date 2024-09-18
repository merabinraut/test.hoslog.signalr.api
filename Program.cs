using System.Reflection;
using hoslog.signalr.api.Filters;
using hoslog.signalr.api.Hubs;
using hoslog.signalr.api.Models.Cache;
using hoslog.signalr.api.Repository.CustomerNotification;
using hoslog.signalr.api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
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

var configuration = builder.Configuration;
builder.Configuration.AddJsonFile("appsettings.json");

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

    x.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authentication header using the Bearer scheme. Example: \"Authorization: Basic {credentials}\""
    });

    x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.Configure<RedisCacheSetting>(configuration.GetSection(nameof(RedisCacheSetting)));
var redisCacheSetting = configuration.GetSection(nameof(RedisCacheSetting)).Get<RedisCacheSetting>();

builder.Services.AddSingleton(sp =>
{
    return sp.GetRequiredService<IOptions<RedisCacheSetting>>().Value;
});

if (redisCacheSetting?.isEnabled == true)
{
    var redisConfiguration = $"{redisCacheSetting.connectionString},password={redisCacheSetting.password}";

    builder.Services.TryAddSingleton<IConnectionMultiplexer>(sp =>
    {
        return ConnectionMultiplexer.Connect(redisConfiguration);
    });

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConfiguration;
        options.InstanceName = redisCacheSetting.instanceName;
    });

    builder.Services.AddSignalR()
            .AddStackExchangeRedis(redisConfiguration, options =>
            {
                options.Configuration.ChannelPrefix = redisCacheSetting.channelPrefix;
            });

    builder.Services.AddSingleton<RedisConnectionService>();
}
else
{
    builder.Services.AddSignalR();
}

builder.Services.AddScoped<ICustomerNotificationRepository, CustomerNotificationRepository>();
builder.Services.AddScoped<CustomerNotificationServices>();

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

var app = builder.Build();

if (redisCacheSetting?.isEnabled == true)
{
    var redisService = app.Services.GetService<RedisConnectionService>();
    redisService?.ClearAllData();
    app.Lifetime.ApplicationStopped.Register(() =>
    {
        redisService?.ClearAllData();
    });
}


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
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<CustomerNotificationHub>("/CustomerNotificationHub");
});

app.Run();