
using Microsoft.EntityFrameworkCore;
using Tesina.Models;
namespace Tesina.Data
{
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<UsuarioLogin> UsuariosLogin { get; set; }
        public DbSet<Expediente> Expedientes { get; set; }
        public DbSet<Lesion> Lesiones { get; set; }
        public DbSet<Rutina> Rutinas { get; set; }
        public DbSet<Asistencias> Asistencias { get; set; }
        public DbSet<Ejercicio> Ejercicios { get; set; }
        public DbSet<RutinaEjercicio> RutinaEjercicios { get; set; }
        public DbSet<ClienteRutina> ClienteRutina { get; set; }
        public DbSet<PlanesNutricionales> PlanesNutricionales { get; set; }
        public DbSet<AlimentosPlanNutricional> AlimentosPlanNutricional { get; set; }
        public DbSet<Notificaciones> Notificaciones { get; set; }
        public DbSet<ProductosServicios> ProductosServicios { get; set; }
        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<Facturas> Facturas { get; set; }
        public DbSet<DetalleFactura> DetalleFacturas { get; set; }
        public DbSet<Pagos> Pagos { get; set; }
        public DbSet<Eventos> Eventos { get; set; }
        public DbSet<ClienteEvento> ClienteEventos { get; set; }

    }

}

