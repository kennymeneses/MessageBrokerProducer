using MessageBrokerProducerConsumer.Modelo;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace MessageBrokerProducerConsumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IConnection connection;
        private IModel _channel;

        public ProducerController(IConfiguration configuration)
        {
            _configuration = configuration;
            GetAMQConnection();
        }

        [NonAction]
        public void GetAMQConnection()
        {
            string urlAwsMQ = _configuration["AWS:AmazonMQ"];
            string user = _configuration["AWS:Username"];
            string password = _configuration["AWS:Password"];

            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri($"amqp://{user}:{password}@{urlAwsMQ}/"),
                DispatchConsumersAsync = true,
                ConsumerDispatchConcurrency = 1
            };
            connectionFactory.Ssl.Enabled = true;

            connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "hello",
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
        }
       
        [HttpGet]
        public async Task<IActionResult> GetSalute(int page = 0, int size = 3)
        {
            return Ok("Hola");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] IntegrationMessage message)
        {
            message.IpComputer = GetLocalIPAddress();
            message.Critical = false;
            message.MessageDate = DateTime.Now;

            string jsonMessage = JsonSerializer.Serialize(message);

            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(exchange: "ExchangeDemo",
                 routingKey: "hello",
                 basicProperties: null,
                 body: body);

            return Ok(message);
        }

        [NonAction]
        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
