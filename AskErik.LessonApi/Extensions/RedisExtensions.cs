using System;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AskErik.LessonApi.Extensions
{
    public static class RedisExtensions
    {
        public static WebApplicationBuilder AddRedisClient(this WebApplicationBuilder builder, string name = "cache")
        {
            var cfgString =
                builder.Configuration.GetValue<string>($"{name}:Connection")
                ?? builder.Configuration.GetValue<string>("Redis:Connection")
                ?? builder.Configuration.GetValue<string>("AppHost:Redis:Connection")
                ?? Environment.GetEnvironmentVariable($"{name.ToUpperInvariant()}__Connection")
                ?? Environment.GetEnvironmentVariable("Redis__Connection")
                ?? "localhost:6379";

            var cfg = ConfigurationOptions.Parse(cfgString);
            cfg.AbortOnConnectFail = false;
            cfg.ConnectTimeout = 10000;
            cfg.ConnectRetry = 3;

            try
            {
                var mux = ConnectionMultiplexer.ConnectAsync(cfg).GetAwaiter().GetResult();
                builder.Services.AddSingleton<IConnectionMultiplexer>(mux);
            }
            catch (Exception ex)
            {
                try
                {
                    var sp = builder.Services.BuildServiceProvider();
                    sp.GetService<ILoggerFactory>()?.CreateLogger("RedisExtensions")?.LogWarning(ex, "Unable to connect to Redis using '{Endpoint}'. Falling back to non-blocking multiplexer.", cfgString);
                }
                catch { }

                var fallback = ConnectionMultiplexer.Connect(cfg);
                builder.Services.AddSingleton<IConnectionMultiplexer>(fallback);
            }

            return builder;
        }
    }
}
