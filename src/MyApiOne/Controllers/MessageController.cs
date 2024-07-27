using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Core.Enums.Messaging.MessageDispatcher;
using Core.Models.Messaging.MessageDispatcher;
using Microsoft.AspNetCore.Authorization;

namespace MyApiOne.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpointProvider _sendEndpoint;
    private readonly ILogger<ProductsController> _logger;
    private readonly IBusControl _busControl;
    private readonly IBus _bus;

    public MessageController(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpoint,
       ILogger<ProductsController> logger, IBusControl busControle, IBus bus)
    {
        _logger = logger;
        _busControl = busControle;
        _bus = bus;

        _publishEndpoint = publishEndpoint;
        _sendEndpoint = sendEndpoint;
    }

    private static readonly List<SendMessageRq> messages = new()
    {
        new SendMessageRq {Title= "ApiOne Sms", Body = "Test Event Architect Api one to get assure what is going on",
            ProviderId =1, SendDate=DateTime.Now.AddMinutes(5), Type= MessageType.SMS,
            SenderId=1, Metadata="", Recipients = new List<RecipientRq>
            {
                new RecipientRq { UserId = 1, Destination = "09108592503"},
                new RecipientRq { UserId = 1, Destination = "09029182599"}
            } },

        new SendMessageRq {Title= "ApiOne Email", Body = "Test Event Architect Api one to get assure what is going on",
            ProviderId =1, SendDate=DateTime.Now.AddMinutes(5), Type= MessageType.Email,
            SenderId=1, Metadata="", Recipients = new List<RecipientRq>
            {
                new RecipientRq { UserId = 1, Destination = "salar.ghi1993@gmail.com"},
                new RecipientRq { UserId = 1, Destination = "salar.1993ghi@gmail.com"}
            } },
    };

    private static readonly List<RecipientRq> Recipients = new()
    {
        new RecipientRq { UserId = 1, Destination = "09108592503"},
        new RecipientRq { UserId = 1, Destination = "09029182599"}
    };

    private static readonly List<RecipientRq> res2 = new()
    {
        new RecipientRq { UserId = 1, Destination = "salar.ghi1993@gmail.com"},
        new RecipientRq { UserId = 1, Destination = "salar.1993ghi@gmail.com"}
    };


    [HttpGet]
    public IEnumerable<SendMessageRq> GetMessages()
    {
        return messages;
    }


    [HttpPost("SaveMessage")]
    public async Task<ActionResult> SaveMessage()
    {
        try
        {
            //await _busControl.StartAsync();
            //var endpoint = await _sendEndpoint.GetSendEndpoint(new Uri("queue:sendmessage_queue"));
            //await _publishEndpoint.PublishBatch(messages);
            //await endpoint.SendBatch(messages);


            // 22222222222222222222222222222222222222222
            //var factory = new ConnectionFactory { HostName = "localhost" };
            //using var connection = factory.CreateConnection();
            //using var channel = connection.CreateModel();

            //channel.ExchangeDeclare(exchange: "MessageDispatcher", type: ExchangeType.Fanout);

            //var queueName = channel.QueueDeclare().QueueName;
            //channel.QueueBind(queue: queueName, exchange:"MessageDispatcher", routingKey: string.Empty);

            //var consumer = new EventingBasicConsumer(channel);
            //consumer.Received += (model, ea) =>
            //{
            //    byte[] body = ea.Body.ToArray();
            //    var message =Encoding.UTF8.GetString(body);
            //};
            //channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            // 22222222222222222222222222222222222222222

            // 11111111111111111111111111111111111111111
            //channel.QueueDeclare(queue: "hello",
            //         durable: false,
            //         exclusive: false,
            //         autoDelete: false,
            //         arguments: null);

            //const string message = "Hello World!";
            //var body = Encoding.UTF8.GetBytes(message);

            //channel.BasicPublish(exchange: string.Empty,
            //                     routingKey: "hello",
            //                     basicProperties: null,
            //                     body: body);

            //await _busControl.StartAsync();
            //var sendEndpoint = await _busControl.GetSendEndpoint(new Uri("queue:sendmessage-queue-test"));
            // 11111111111111111111111111111111111111111


            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri("queue:sendmessage_queue"));
            await sendEndpoint.SendBatch(messages);

            foreach (var msg in messages)
            {
                //await sendEndpoint.Send<Message>(msg);
                await sendEndpoint.Send(msg);
                //await endpoint.Send(msg);
            }

            //await _bus.PublishBatch(messages);
            //await _bus.Publish(messages); 
            //await _busControl.StopAsync();

            return Ok();
        }
        catch (Exception xe)
        {
            throw xe;
        }
    }
}
