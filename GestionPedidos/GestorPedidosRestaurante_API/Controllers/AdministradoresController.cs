using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GestorPedidosRestaurante_API.Models;
using GestorPedidosRestaurante_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestorPedidosRestaurante_API.Controllers
{
    [ApiController]
    [Route("api/administradores")]
    public class AdministradorController : ControllerBase
    {
        private readonly JsonFileService _jsonFileService;
        private List<Administrador> _administradores;

        public AdministradorController(JsonFileService jsonFileService)
        {
            _jsonFileService = jsonFileService;
            _administradores = _jsonFileService.CargarAdminsAsync().Result; // Cargar desde JSON al iniciar
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarAdministrador([FromBody] Administrador admin)
        {
            if (_administradores.Any(a => a.Correo == admin.Correo))
            {
                return BadRequest("❌ El correo ya está registrado.");
            }

            admin.Id = Guid.NewGuid();
            admin.EstablecerContraseña(admin.Contraseña); // Cifrar la contraseña antes de guardarla
            _administradores.Add(admin);

            await _jsonFileService.GuardarAdminsAsync(_administradores); // Guardar en JSON

            return Ok("✅ Administrador registrado con éxito.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] IniciarSesionRequest credenciales)
        {
            var admin = _administradores.FirstOrDefault(a => a.Correo == credenciales.Correo);
            if (admin == null || !admin.VerificarContraseña(credenciales.Contraseña))
            {
                return Unauthorized(new { message = "❌ Credenciales incorrectas." });
            }

            // Devuelve un objeto JSON con un mensaje de éxito
            return Ok(new { message = "✅ Inicio de sesión exitoso." });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerAdministradores()
        {
            return Ok(_administradores);
        }
    }
}
