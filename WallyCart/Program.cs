using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using WallyCart.Models;
using WallyCart.Session;
using WallyCart.Session.Handler;
using System.Windows.Input;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Resources/en.json", optional: false, reloadOnChange: true);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<ListService>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddScoped<ICommandHandler, HelpCommandHandler>();
builder.Services.AddScoped<ICommandHandler, CreateGroupCommandHandler>();
builder.Services.AddScoped<CommandRouter>();

builder.Services.AddHttpClient<WhatsAppService>();

builder.Services.AddSingleton<SessionManager>();
builder.Services.AddSingleton(_ => new CommandLanguageService("en"));

builder.Services.AddHostedService<SessionCleanupService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WallyCart API", Version = "v1" });
});

builder.Services.AddDbContext<WallyCartDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WallyCart API V1");
    });
}
app.UseHttpsRedirection();
app.MapControllers(); 

var commands = builder.Configuration.GetSection("commands").GetChildren();
Console.WriteLine("Commands loaded:");
foreach (var cmd in commands)
{
    Console.WriteLine($" - {cmd.Key}");
}

app.Run();
