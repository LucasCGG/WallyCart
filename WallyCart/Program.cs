using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using WallyCart.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<ListService>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddHttpClient<WhatsAppService>();

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

app.Run();
