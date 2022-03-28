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

        // Cria o Exchange, caso não exista
        rabbitMQLib.CreateExchange(appSettings.SimpleRabbitMQSettings.ScheduleExchange);

        // Cria a Queue, caso não exista
        rabbitMQLib.CreateQueue(appSettings.SimpleRabbitMQSettings.MessageQueue);

        // Injeção de dependência
        // Cada DTO tem sua injeção de dependência através do IPublisher e IConsumer

        services.AddSingleton<Serilog.ILogger>( sp => logger);

        services.AddSingleton<ISimpleRabbitMQ, SimpleRabbitMQ>(sp => rabbitMQLib);
        services.AddSingleton<IPublisher<MessageDTO>, Publisher<MessageDTO>>(sp => new Publisher<MessageDTO>(logger, rabbitMQLib, appSettings.SimpleRabbitMQSettings.MessageQueue));
        services.AddSingleton<IConsumer<MessageDTO>, Consumer<MessageDTO>>(sp => new Consumer<MessageDTO>(logger, rabbitMQLib, appSettings.SimpleRabbitMQSettings.MessageQueue));

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
