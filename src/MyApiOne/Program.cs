using Core.Models;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyApiOne;
using MyApiOne.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer",options =>
    {
        options.Authority = "https://localhost:5001";
        options.TokenValidationParameters.ValidateAudience = false;
        //options.TokenValidationParameters = new TokenValidationParameters
        //{
        //    ValidateAudience = false
        //};
        //options.Audience = "scope1";
        //options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
    });

builder.Services.AddAuthorization(options =>
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api1");
    })
);



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
    //x.SetKebabCaseEndpointNameFormatter();
    //x.AddConsumer<MessageConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        //cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3)));
        //cfg.UseMessageRetry(r => r.Immediate(3));
        cfg.Host("dev-rmq.nitro.local","/",h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        //cfg.ConfigureEndpoints(context);
        //cfg.ReceiveEndpoint("sendmessage_queue", e =>
        //{
        //    e.Durable = true;
        //    e.AutoDelete = false;
        //    e.PrefetchCount = 1;
        //    //e.ConfigureConsumeTopology = false;
        //    e.DiscardSkippedMessages();
        //    //e.UseMessageRetry(r => r.Immediate(5));
        //    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        //    e.UseInMemoryOutbox();
        //    //e.BindDeadLetterQueue("dead_letter_exchange", "sendmessage_queue");
        //    e.UseRateLimit(5, new TimeSpan(0, 0, 0, 1));
        //    e.ConfigureConsumer<MessageConsumer>(context);
        //});
        //cfg.UsePublishConfirmation();
    });
});

//builder.Services.AddHostedService<PublisherService>();

builder.Services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
builder.Services.AddHostedService<MassTransitHostedService>();
//builder.Services.AddMassTransitHostedService();
//builder.Services.AddMassTransitHostedService();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); //.RequireAuthorization("ApiScope");

app.MapGet("api/identity", (ClaimsPrincipal user) => user.Claims.Select(c => new { c.Type, c.Value }))
    .RequireAuthorization("ApiScope");
app.Run();
