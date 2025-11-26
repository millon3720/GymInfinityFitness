using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using Tesina.Models;

namespace Tesina.Controllers
{
    public class GenerarFacturaPDF
    {
        private readonly EmailSettings _email;
        public GenerarFacturaPDF(IOptions<EmailSettings> emailSettings)
        {
            _email = emailSettings.Value;
        }
        public byte[] GenerarFactura(FacturaViewModel model)
        {
            var factura = model.Factura;
            var detalles = model.Detalles;
            var cliente = factura.Usuario;
            var culture = new CultureInfo("es-CR");

            // Calcular totales
            decimal subtotal = detalles.Sum(d => d.ProductoServicio.Precio * d.Cantidad);
            decimal total = subtotal;

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("INFINITY FITNESS").Bold().FontSize(18).FontColor(Colors.Blue.Medium);
                        col.Item().Text("Teléfono: 8411-7404 / 71613145");
                        col.Item().Text("Correo: Kpaizv.0895@gmail.com");
                        col.Item().Text("Ubicación: 📍 Frente a Pali, Aguas Zarcas, San Carlos, Alajuela, Costa Rica");
                        col.Spacing(10); 
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().Text("Factura Electrónica").FontSize(12).Italic();
                        col.Item().Text($"Número de factura: {factura.IdFactura}");
                        col.Item().Text($"Fecha: {factura.Fecha:dd-MM-yyyy}  Hora: {factura.Fecha:HH:mm:ss}");
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });


                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text("👤 Cliente").Bold();
                        col.Item().Text($"Nombre: {cliente.NombreCompleto}");
                        col.Item().Text($"Correo: {cliente.Correo}");
                        col.Item().Text($"Identificación: {cliente.Cedula}");

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().Text("📦 Detalle de productos/servicios").Bold();
                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Nombre
                                columns.RelativeColumn(3); // Descripción
                                columns.RelativeColumn(1); // Cantidad
                                columns.RelativeColumn(1); // Precio
                                columns.RelativeColumn(1); // Total
                            });

                            tabla.Header(header =>
                            {
                                header.Cell().Text("Nombre").Bold();
                                header.Cell().Text("Descripción").Bold();
                                header.Cell().AlignRight().Text("Cantidad").Bold();
                                header.Cell().AlignRight().Text("Precio").Bold();
                                header.Cell().AlignRight().Text("Total").Bold();
                            });

                            foreach (var item in detalles)
                            {
                                var precio = item.ProductoServicio.Precio;
                                var cantidad = item.Cantidad;
                                var totalItem = precio * cantidad;
                                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                                tabla.Cell().Element(cell =>
                                {
                                    cell.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Text(item.ProductoServicio.Nombre ?? "");
                                });

                                tabla.Cell().Element(cell =>
                                {
                                    cell.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Text(item.ProductoServicio.Descripcion?.Length > 60
                                            ? item.ProductoServicio.Descripcion.Substring(0, 60) + "..."
                                            : item.ProductoServicio.Descripcion ?? "");
                                });

                                tabla.Cell().Element(cell =>
                                {
                                    cell.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .AlignRight().Text(cantidad.ToString("0.00"));
                                });

                                tabla.Cell().Element(cell =>
                                {
                                    cell.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .AlignRight().Text(precio.ToString("C", culture));
                                });

                                tabla.Cell().Element(cell =>
                                {
                                    cell.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .AlignRight().Text(totalItem.ToString("C", culture));
                                });
                            }

                        });

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().AlignRight().Column(totales =>
                        {
                            totales.Spacing(4);
                            totales.Item().Text($"Total: {total.ToString("C", culture)}").Bold();
                        });
                    });

                    page.Footer().AlignCenter().Text("Gracias por su compra en Infinity Fitness").FontSize(10).Italic();
                });
            });

            return documento.GeneratePdf();
        }

        public async Task EnviarFacturaPorCorreoAsync(string correoDestino, byte[] pdfBytes, string nombreArchivo)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Infinity Fitness", _email.User));
            mensaje.To.Add(new MailboxAddress("", correoDestino));
            mensaje.Subject = "Factura electrónica Infinity Fitness";

            var builder = new BodyBuilder
            {
                TextBody = "Adjunto encontrará su factura electrónica. ¡Gracias por preferirnos!"
            };

            builder.Attachments.Add(nombreArchivo, pdfBytes, new ContentType("application", "pdf"));
            mensaje.Body = builder.ToMessageBody();

            using var cliente = new SmtpClient();
            await cliente.ConnectAsync(_email.Host, _email.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await cliente.AuthenticateAsync(_email.User, _email.Password);
            await cliente.SendAsync(mensaje);
            await cliente.DisconnectAsync(true);
        }
        public async Task EnviarRecuperacionContrasenaAsync(string correoDestino, string nombreUsuario, string nuevaContrasena)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Infinity Fitness", _email.User));
            mensaje.To.Add(new MailboxAddress(nombreUsuario, correoDestino));
            mensaje.Subject = "Recuperación de contraseña - Infinity Fitness";

            var builder = new BodyBuilder
            {
                TextBody = $@"
                    Hola {nombreUsuario},

                    Hemos recibido una solicitud para recuperar tu contraseña.

                    Tu nueva contraseña temporal es: {nuevaContrasena}

                    Por favor, inicia sesión y cámbiala lo antes posible desde tu perfil.

                    Si no solicitaste este cambio, comunicate con nosotros de inmediato.

                    Gracias por confiar en Infinity Fitness."
            };

            mensaje.Body = builder.ToMessageBody();

            using var cliente = new SmtpClient();
            try
            {
               
                await cliente.ConnectAsync(_email.Host, _email.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await cliente.AuthenticateAsync(_email.User, _email.Password);
                await cliente.SendAsync(mensaje);
                await cliente.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al enviar correo: " + ex.Message);
                throw;
            }

            
        }
        public async Task EnviarNotificacionRegistroAsync(string correoDestino, string nombreUsuario, string contrasenaTemporal)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Infinity Fitness", _email.User));
            mensaje.To.Add(new MailboxAddress(nombreUsuario, correoDestino));
            mensaje.Subject = "Bienvenido a Infinity Fitness";

            var builder = new BodyBuilder
            {
                TextBody = $@"
                    Hola {nombreUsuario},

                    Tu cuenta en Infinity Fitness ha sido creada exitosamente.

                    Tu contraseña es: {contrasenaTemporal}

                    Podés iniciar sesión en el sistema y cambiarla desde tu perfil en cualquier momento.

                    Si no reconocés este registro, comunicate con nosotros de inmediato.

                    ¡Gracias por formar parte de Infinity Fitness!"
            };

            mensaje.Body = builder.ToMessageBody();

            using var cliente = new SmtpClient();
            await cliente.ConnectAsync(_email.Host, _email.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await cliente.AuthenticateAsync(_email.User, _email.Password);
            await cliente.SendAsync(mensaje);
            await cliente.DisconnectAsync(true);
        }

    }
}
