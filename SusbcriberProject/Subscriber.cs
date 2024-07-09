using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

class Subscriber
{
    private static ConnectionMultiplexer? redis;
    private static ISubscriber? sub;

    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var redisHost = configuration["Redis:Host"];
        var redisPort = configuration["Redis:Port"];
        var redisPassword = configuration["Redis:Password"];

        var redisConnectionString = $"{redisHost}:{redisPort},password={redisPassword}";
        redis = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);
        sub = redis.GetSubscriber();

        await sub.SubscribeAsync(new RedisChannel("messages", RedisChannel.PatternMode.Literal), (channel, message) => {
            Console.WriteLine($"Mensaje Recivido: {message}");
        });

        Console.WriteLine("Suscrito al canal de 'mensajes'. Precione cualquier boton para cancelar la suscripción...");
        Console.ReadKey();
    }
}
