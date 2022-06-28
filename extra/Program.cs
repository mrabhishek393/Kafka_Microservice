using Confluent.Kafka;
using Database;
using extra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Reflection;
using Database.Models;
Console.WriteLine("Hello, World!");
//var host = Host.CreateDefaultBuilder()
//            .ConfigureAppConfiguration(app =>
//            {
//                //app.AddJsonFile("appsettings.json");
//                app.AddUserSecrets("8f282f66-069f-4706-940f-723c2210f397");
//            })
//            .ConfigureServices(services =>
//            {
//                services.AddHostedService<ConsumerBackgroundService>();
//            });
//host.RunConsoleAsync();
//static void func([FromServices]IConfiguration configuration)
//{
//    Console.WriteLine(configuration["Topic"]);
//}
//func();

//var host = Host.CreateDefaultBuilder()
//            .ConfigureAppConfiguration(app =>
//            {
//                app.AddUserSecrets("8f282f66-069f-4706-940f-723c2210f397");
//            }).ConfigureServices(services =>
//{
//    services.AddSqlDatabaseConnector();
//    services.AddHostedService<ConsumerBackgroundService>();
//});
//host.RunConsoleAsync();

void kc()
{
    //var obj = new ModelProp
    //{
    //    Id = 1,
    //    Name = "ABC",
    //    Version = 10
    //};
    //string msg=JsonConvert.SerializeObject(obj);
    //String str = "{\"Id\":\"123\",\"DateOfRegistration\":\"2012-10-21T00:00:00+05:30\",\"Status\":0}";
    //var val=JsonConvert.DeserializeObject<ModelProp>(str, new JsonSerializerSettings
    //{
    //    MissingMemberHandling = MissingMemberHandling.Error
    //});
    //Console.WriteLine(val.Id+val.Name);
    ConsumerConfig consumerconfig = new ConsumerConfig
    {
        GroupId = "hue",
        BootstrapServers = "localhost:9092",
        AutoOffsetReset = AutoOffsetReset.Latest

    };
    IConsumer<Ignore, string> _consumer = new ConsumerBuilder<Ignore, string>(consumerconfig).Build();
    _consumer.Subscribe("orders");

    while (true)
    {
        try
        {
            var c = _consumer.Consume(1000);
            if (c == null)
            {
                _consumer.Dispose();
                throw new Exception("Broker Not available");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            break;
        }
    }
    _consumer.Dispose();
    _consumer.Dispose();
}
kc();

Console.WriteLine("Out of loop");
Console.ReadLine();