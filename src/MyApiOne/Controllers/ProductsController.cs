using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApiOne.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace MyApiOne.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpointProvider _sendEndpoint;
    private readonly ILogger<ProductsController> _logger;
    private readonly IBusControl _busControl;
    private readonly IBus _bus;

    public ProductsController(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpoint, 
        ILogger<ProductsController> logger, IBusControl busControle, IBus bus)
    {
        _logger = logger;
        _busControl = busControle;
        _bus = bus;

        _publishEndpoint = publishEndpoint;
        _sendEndpoint = sendEndpoint;
    }


    private static readonly List<Product> products = new()
    {
        new Product { Id = 1, Name = "Product 1", Price = 9.99m },
        new Product { Id = 2, Name = "Product 2", Price = 19.99m },
        new Product { Id = 3, Name = "Product 3", Price = 29.99m },
        new Product { Id = 4, Name = "Product 4", Price = 39.99m },
        new Product { Id = 5, Name = "Product 5", Price = 49.99m }
    };


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
    public IEnumerable<Product> GetProducts()
    {
        return products;
    }

    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
        var product = products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return product;
    }

    [HttpGet("report/cheapest")]
    public ActionResult<Product> GetCheapestProduct()
    {
        var cheapestProduct = products.OrderBy(p => p.Price).FirstOrDefault();
        return cheapestProduct;
    }

    [HttpGet("report/most-expensive")]
    public ActionResult<Product> GetMostExpensiveProduct()
    {
        var mostExpensiveProduct = products.OrderByDescending(p => p.Price).FirstOrDefault();
        return mostExpensiveProduct;
    }

    [HttpGet("report/average-price")]
    public ActionResult<decimal> GetAverageProductPrice()
    {
        var averagePrice = products.Average(p => p.Price);
        return averagePrice;
    }


    [HttpPost]
    public ActionResult<Product> CreateProduct(Product product)
    {
        product.Id = products.Count + 1;
        products.Add(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public ActionResult<Product> UpdateProduct(int id, Product updatedProduct)
    {
        var product = products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        product.Name = updatedProduct.Name;
        product.Price = updatedProduct.Price;
        return product;
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        var product = products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        products.Remove(product);
        return NoContent();
    }




    [HttpPost("MessageDispatcher/SaveMessage")]
    public async Task<ActionResult> SaveMessage()
    {
        try
        {
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


            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri("queue:sendmessage-queue"));

            //await _busControl.StartAsync();
            //var sendEndpoint = await _busControl.GetSendEndpoint(new Uri("queue:sendmessage-queue-test"));
            // 11111111111111111111111111111111111111111

            foreach (var msg in messages)
            {
                await sendEndpoint.Send<Message>(msg);

                await _publishEndpoint.Publish(new Message
                {
                    Title = msg.Title,
                    Body= msg.Body,
                    Provider = 1,
                    SendDate = DateTime.Now,
                    SenderId = 0,
                    Metadata = msg.Metadata,
                    Recipients = msg.Recipients
                });
            }

            //await _busControl.StopAsync();

            //var endpoint = await _sendEndpoint.GetSendEndpoint(new Uri($"queue:{"sendmessage-queue-test2"}"));
            //await endpoint.Send(new Message()
            //{
            //    Title = "ApiOne Email",
            //    Body = "Test Event Architect Api one to get assure what is going on",
            //    Provider = 1,
            //    SendDate = DateTime.Now,
            //    Type = MessageType.Email,
            //    SenderId = 0,
            //    Metadata = "",
            //    Recipients = res2,
            //});

            //message = messages.FirstOrDefault();
            //await _bus.Publish(message);



            //await _publishEndpoint.Publish(msg1);

            return Ok();
        }
        catch (Exception xe)
        {
            throw xe;
        }
    }

}