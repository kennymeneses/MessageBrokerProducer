namespace MessageBrokerProducerConsumer.Modelo
{
    public class Message : IMessage
    {
        public DateTime fecha { get; set; }
        public string? texto { get; set; }
    }
}
