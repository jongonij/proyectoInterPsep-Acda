using GestorPedidosRestaurante.Sockets;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<SocketServer>();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SocketServer socketServer)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Iniciar el servidor de sockets
        _ = socketServer.Iniciar();
    }
}