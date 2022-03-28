namespace Schedule.Worker;

using Newtonsoft.Json;
using Serilog;
using SimpleRabbitMQCore;

public class Worker : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IConsumer<MessageDTO> _consumer;
    private readonly IPublisher<MessageDTO> _publisher;

    public Worker(
        ILogger logger,
        IConsumer<MessageDTO> consumer,
        IPublisher<MessageDTO> publisher)
    {
        _logger = logger;
        _consumer = consumer;
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Subscribe consumer to queue
        var result = _consumer.SubscribeConsumer(async (order) =>
        {
            Console.WriteLine(JsonConvert.SerializeObject(order));
        });

        if (result) _logger.Information("Consumer subscribed");

        while (!stoppingToken.IsCancellationRequested)
        {
            var messageDto = new MessageDTO()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Name = "Test Message",
                MicroserviceName = "ANTI-FRAUD",
                Type = "local"
            };

            await _publisher.PublishAsync(messageDto);

            _logger.Information("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(2000, stoppingToken);
        }
    }
}
