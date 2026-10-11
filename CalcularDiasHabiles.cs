
using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiVacaciones;

public class CalcularDiasHabiles
{
    private readonly ILogger<CalcularDiasHabiles> _logger;

    public CalcularDiasHabiles(ILogger<CalcularDiasHabiles> logger)
    {
        _logger = logger;
    }

    [Function("CalcularDiasHabiles")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequest req)
    {
        _logger.LogInformation("Calculando dias habiles.");

        SolicitudDias? solicitud;

        try
        {
            solicitud = await JsonSerializer.DeserializeAsync<SolicitudDias>(
                req.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(
                new { error = "El cuerpo de la solicitud no contiene JSON valido." });
        }

        if (solicitud == null ||
            string.IsNullOrWhiteSpace(solicitud.FechaInicio) ||
            string.IsNullOrWhiteSpace(solicitud.FechaFin))
        {
            return new BadRequestObjectResult(
                new { error = "Debes enviar fechaInicio y fechaFin." });
        }

        if (!DateTimeOffset.TryParse(
                solicitud.FechaInicio,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var inicio) ||
            !DateTimeOffset.TryParse(
                solicitud.FechaFin,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var fin))
        {
            return new BadRequestObjectResult(
                new { error = "Las fechas deben tener un formato valido, por ejemplo 2026-10-01." });
        }

        DateOnly fechaInicio = DateOnly.FromDateTime(inicio.Date);
        DateOnly fechaFin = DateOnly.FromDateTime(fin.Date);

        if (fechaFin <= fechaInicio)
        {
            return new BadRequestObjectResult(
                new { error = "La fecha de fin debe ser posterior a la fecha de inicio." });
        }

        // Los festivos se configuran mediante la variable de entorno HOLIDAYS.
        // Formato: 2026-01-01,2026-03-23,2026-12-25
        string festivosConfigurados =
            Environment.GetEnvironmentVariable("HOLIDAYS") ?? "";

        var festivos = festivosConfigurados
            .Split(',', StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries)
            .Select(valor =>
                DateOnly.TryParseExact(
                    valor,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var fecha)
                    ? (DateOnly?)fecha
                    : null)
            .Where(fecha => fecha.HasValue)
            .Select(fecha => fecha!.Value)
            .ToHashSet();

        int diasHabiles = 0;

        for (DateOnly fecha = fechaInicio;
             fecha <= fechaFin;
             fecha = fecha.AddDays(1))
        {
            bool esDomingo = fecha.DayOfWeek == DayOfWeek.Sunday;
            bool esFestivo = festivos.Contains(fecha);

            if (!esDomingo && !esFestivo)
            {
                diasHabiles++;
            }
        }

        return new OkObjectResult(new
        {
            fechaInicio = fechaInicio.ToString("yyyy-MM-dd"),
            fechaFin = fechaFin.ToString("yyyy-MM-dd"),
            diasHabiles
        });
    }

    public class SolicitudDias
    {
        public string? FechaInicio { get; set; }
        public string? FechaFin { get; set; }
    }
}