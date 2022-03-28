using SimpleRabbitMQCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AppSettings
{
    public RabbitMQLibSettingsCustom SimpleRabbitMQSettings { get; set; }
}

public class RabbitMQLibSettingsCustom : SimpleRabbitMQSettings
{
    public QueueSettings MessageQueue { get; set; }
    public ExchangeSettings ScheduleExchange { get; set; }
}