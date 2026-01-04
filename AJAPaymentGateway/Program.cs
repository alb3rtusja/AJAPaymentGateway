using AJAPaymentGateway.Core.Payments;
using AJAPaymentGateway.Data;
using AJAPaymentGateway.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<PaymentService>();
builder.Services.AddHttpClient<WebhookService>();
builder.Services.AddScoped<SandboxPaymentProcessor>();
builder.Services.AddScoped<ProductionPaymentProcessor>();
builder.Services.AddScoped<PaymentProcessorFactory>();

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();
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

app.MapRazorPages();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
