using GestorPedidosRestaurante.Sockets;
using GestorPedidosRestaurante_API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;



var builder = WebApplication.CreateBuilder(args);



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "tu_issuer",
            ValidAudience = "tu_audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("tu_clave_secreta"))
        };
    });    

    
// Agregar servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Registrar el servicio IPedidoService
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddSingleton<JsonFileService>();
builder.Services.AddSingleton<SocketServer>(); // ⬅ REGISTRAR SOCKETSERVER
builder.Services.AddScoped<ISocketService, SocketService>(); // ⬅ REGISTRAR SOCKETSERVICE


  


var app = builder.Build();
// Inicia el servidor de sockets
var socketServer = app.Services.GetRequiredService<SocketServer>();
_ = socketServer.Iniciar();

// Configurar middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "¡La aplicación está fun  cionando!");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
