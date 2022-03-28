using Serilog;
using SimpleRabbitMQCore;
using Schedule.Worker;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        var appSettings = configuration.Get<AppSettings>();


        var logger = (Serilog.ILogger)new LoggerConfiguration()
           .MinimumLevel.Debug()
           .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
           .CreateLogger();

        var rabbitMQLib = new SimpleRabbitMQ(appSettings.SimpleRabbitMQSettings, logger);

        // Cria o Exchange, caso n�o exista
        rabbitMQLib.CreateExchange(appSettings.SimpleRabbitMQSettings.ScheduleExchange);

        // Cria a Queue, caso n�o exista
        rabbitMQLib.CreateQueue(appSettings.SimpleRabbitMQSettings.MessageQueue);

        // Inje��o de depend�ncia
        // Cada DTO tem sua inje��o de depend�ncia atrav�s do IPublisher e IConsumer

        services.AddSingleton<Serilog.ILogger>( sp => logger);

        services.AddSingleton<ISimpleRabbitMQ, SimpleRabbitMQ>(sp => rabbitMQLib);
        services.AddSingleton<IPublisher<MessageDTO>, Publisher<MessageDTO>>(sp => new Publisher<MessageDTO>(logger, rabbitMQLib, appSettings.SimpleRabbitMQSettings.MessageQueue));
        services.AddSingleton<IConsumer<MessageDTO>, Consumer<MessageDTO>>(sp => new Consumer<MessageDTO>(logger, rabbitMQLib, appSettings.SimpleRabbitMQSettings.MessageQueue));

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
