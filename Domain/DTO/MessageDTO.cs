using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MessageDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string MicroserviceName { get; set; }
    public DateTime CreatedAt { get; set; }
}