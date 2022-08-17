using KafkaProducer.Service;
using OrderService.Model;
using OrderService.Repository;
using OrderService.Service;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.File("C:/Users/User/Logs/MDSLatest.txt").ReadFrom.Configuration(ctx.Configuration));
builder.Services.AddControllers();
builder.Services.Configure<OrderServiceDatabaseSettings>(builder.Configuration.GetSection("OrderServiceDatabase"));
builder.Services.AddScoped<IIncomingRepository, IncomingRepository>();
builder.Services.AddScoped<IOutgoingRepository, OutgoingRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IIncomingService, IncomingService>();
builder.Services.AddScoped<IOutgoingService, OutgoingService>();
builder.Services.AddSingleton<IKafkaProducerService>(sp => new KafkaProducerService
    (builder.Configuration.GetSection("KafkaConnection").GetValue<string>("BootstrapServers")));
builder.Services.AddHostedService<EventPublisherService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
