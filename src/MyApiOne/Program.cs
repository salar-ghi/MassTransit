using MassTransit;
using MyApiOne;
using MyApiOne.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//try
//{
//    var sendEndpoint = await busControl.GetSendEndpoint(new Uri("queue:my-public-queue"));

//    var message = new Message { Title="This is the best",  Body= "Hello, this is a message for the public queue!" };
//    await sendEndpoint.Send(message);

//    Console.WriteLine("Message sent to the public queue.");
//}
//finally
//{
//    await busControl.StopAsync();
//}


builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    //x.AddConsumer<MyMessageConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        //cfg.Host("localhost", "/", "sendmessage-queue", hostConfigurator => { });
        cfg.Host("localhost", "/", h =>
        {
            h.Username("NitroMessageDispatcher");
            h.Password("NitroMessageDispatcher912*");
        });
        cfg.ConfigureEndpoints(context);
        //cfg.ReceiveEndpoint("sendmessage-queue", e =>
        //{
        //    e.PrefetchCount = 10;
        //    //e.ConfigureConsumer<MyMessageConsumer>(context);
        //    e.UseRateLimit(5, new TimeSpan(0, 0, 0, 1));
        //});
    });
});

//var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
//{
//    cfg.Host("localhost", "/", "sendmessage-queue", h =>
//    {
//        h.Username("guest");
//        h.Password("guest");
//    });
//});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();


app.Run();
