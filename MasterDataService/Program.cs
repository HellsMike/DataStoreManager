using Microsoft.EntityFrameworkCore;
using MasterDataService.Repository;
using MasterDataService.Service;
using MasterDataService.Model;
using KafkaProducer.Service;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.File("C:/Users/User/Logs/MDSLatest.txt").ReadFrom.Configuration(ctx.Configuration));
builder.Services.AddControllers();
builder.Services.AddSqlServer<AppDbContext>(builder.Configuration.GetConnectionString("LocalDb"));
builder.Services.AddScoped<IWriteRepository, WriteRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IItemService, ItemService>();
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
