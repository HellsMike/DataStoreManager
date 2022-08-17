using InventoryService.Model;
using InventoryService.Service;
using InventoryService.Repository;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.File("C:/Users/User/Logs/MDSLatest.txt").ReadFrom.Configuration(ctx.Configuration));
builder.Services.AddControllers();
builder.Services.Configure<KafkaConnectionSettings>(builder.Configuration.GetSection("KafkaConnection"));
builder.Services.AddSqlServer<InventoryDbContext>(builder.Configuration.GetConnectionString("LocalDatabase"));
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IPalletRepository, PalletRepository>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IPalletService, PalletService>();
builder.Services.AddHostedService<KafkaConsumerOrder>();
builder.Services.AddHostedService<KafkaConsumerItem>();

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
