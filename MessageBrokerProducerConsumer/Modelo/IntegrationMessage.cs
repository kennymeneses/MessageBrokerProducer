namespace MessageBrokerProducerConsumer.Modelo
{
    public class IntegrationMessage : IMessage
    {
        public DateTime MessageDate { get; set; }
        public string? IpComputer { get; set; }
        public bool Critical { get; set; }
        public string? Message { get; set; }
    }
}
