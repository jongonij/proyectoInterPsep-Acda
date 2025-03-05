using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using GestorPedidosRestaurante_API.Models;

namespace GestorPedidosRestaurante_API.Services
{
    public class JsonFileService
    {
        private readonly string _pedidosPath = "data/data_pedidos.json";
        private readonly string _adminsPath = "data/data_admins.json";

        // Guardar pedidos en JSON
        public async Task GuardarPedidosAsync(List<Pedido> pedidos)
        {
            var json = JsonSerializer.Serialize(pedidos, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_pedidosPath, json);
        }

        // Cargar pedidos desde JSON
        public async Task<List<Pedido>> CargarPedidosAsync()
        {
            if (!File.Exists(_pedidosPath))
            {
                return new List<Pedido>();
            }

            var json = await File.ReadAllTextAsync(_pedidosPath);
            return JsonSerializer.Deserialize<List<Pedido>>(json);
        }

        // Guardar administradores en JSON
        public async Task GuardarAdminsAsync(List<Administrador> admins)
        {
            var json = JsonSerializer.Serialize(admins, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_adminsPath, json);
        }

        // Cargar administradores desde JSON
        public async Task<List<Administrador>> CargarAdminsAsync()
        {
            if (!File.Exists(_adminsPath))
            {
                return new List<Administrador>();
            }

            var json = await File.ReadAllTextAsync(_adminsPath);
            return JsonSerializer.Deserialize<List<Administrador>>(json);
        }
    }
}
