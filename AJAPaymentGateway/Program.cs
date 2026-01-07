using AJAPaymentGateway.BackgroundServices;
using AJAPaymentGateway.Core.Payments;
using AJAPaymentGateway.Data;
using AJAPaymentGateway.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<PaymentService>();
builder.Services.AddHttpClient<IWebhookService, WebhookService>();
builder.Services.AddScoped<SandboxPaymentProcessor>();
builder.Services.AddScoped<ProductionPaymentProcessor>();
builder.Services.AddScoped<IPaymentProcessorFactory, PaymentProcessorFactory>();
builder.Services.AddHostedService<WebhookRetryService>();
builder.Services.AddScoped<IdempotencyService>();
builder.Services.AddHostedService<WebhookDispatcherService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AJA Payment Gateway API",
        Version = "v1",
        Description = """
        Base URL:
        - Sandbox: https://sandbox.api.ajapay.test
        - Production: https://api.ajapay.test

        Use Sandbox for testing with dummy payments.
        """,
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Me",
            Email = "albertusja01@gmail.com"
        }
    });

    // Idempotency Key Header
    c.AddSecurityDefinition("Idempotency-Key", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Idempotency-Key",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Unique key to safely retry payment creation"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Idempotency-Key"
                }
            },
            new string[] {}
        }
    });
});

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(xmlPath);
});



builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", context =>
{
    context.Response.Redirect("/admin/dashboard");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}");


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
