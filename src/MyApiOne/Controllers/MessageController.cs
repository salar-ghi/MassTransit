using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApiOne.Models;

namespace MyApiOne.Controllers;

[Route("api/[controller]")]
[ApiController]
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



    private static readonly List<Message> messages = new()
    {
        new Message {Title= "ApiOne Sms", Body = "Test Event Architect Api one to get assure what is going on",
            Provider =1, SendDate=DateTime.Now, Type= MessageType.SMS,
            SenderId=0, Metadata="",Recipients = res1 },

        new Message {Title= "ApiOne Email", Body = "Test Event Architect Api one to get assure what is going on",
            Provider =1, SendDate=DateTime.Now, Type= MessageType.Email,
            SenderId=0, Metadata="",Recipients = res2 },
    };

    private static readonly List<Recipient> res1 = new()
    {
        new Recipient { UserId = 1, Destination = "09108592503"},
        new Recipient { UserId = 1, Destination = "09029182599"}
    };

    private static readonly List<Recipient> res2 = new()
    {
        new Recipient { UserId = 1, Destination = "salar.ghi1993@gmail.com"},
        new Recipient { UserId = 1, Destination = "salar.1993ghi@gmail.com"}
    };



    [HttpGet]
    public IEnumerable<Message> GetMessages()
    {
        return messages;
    }


    [HttpPost("SaveMessage")]
    public async Task<ActionResult> SaveMessage()
    {
        try
        {
           


            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri("queue:sendmessage-queue"));
            //await _busControl.StartAsync();

            foreach (var msg in messages)
            {
                await sendEndpoint.Send<Message>(msg);
            }
            //await _busControl.StopAsync();

            return Ok();
        }
        catch (Exception xe)
        {
            throw xe;
        }
    }
}
