using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Prestamos.Api.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddOpenApi();


var connectionString = builder.Configuration.GetConnectionString("Database");


builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseMySql(connectionString, ServerVersion.Parse("10.4.32-mariadb")));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();


DatosIniciales.Inicializar(app.Services);

app.Run();
