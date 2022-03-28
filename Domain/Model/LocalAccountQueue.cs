namespace Project.Schedule.Model
{
    public class LocalAccountQueue
    {
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
    }

}
