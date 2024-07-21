using MassTransit;
using MassTransit.Internals;
using MyApiOne.Models;

namespace MyApiOne
{
    public class MyMessageConsumer : IConsumer<Message>
    {
        public Task Consume(ConsumeContext<Message> context)
        {
            Console.WriteLine($"Received message: {context.Message.Title}, {context.MessageId}");
            return Task.CompletedTask;
        }
    }
}
