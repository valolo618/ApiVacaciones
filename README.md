# API de Gestión de Vacaciones

## Descripción

API desarrollada en C# con .NET 8 y Azure Functions para calcular los días hábiles de una solicitud de vacaciones. La solución cuenta los días de lunes a sábado, excluye los domingos y los festivos configurados.

## Tecnologías utilizadas

* C#
* .NET 8
* Azure Functions
* API REST
* Git y GitHub
* Power Apps
* Power Automate
* SharePoint

## Funcionalidad

La API recibe una fecha de inicio y una fecha de fin, calcula los días hábiles del periodo y devuelve el resultado en formato JSON.

### Endpoint

`POST https://apipruebavaleria-h7fcdkfpfkdhggby.westus3-01.azurewebsites.net/api/calculardiashabiles`

### Ejemplo de solicitud

```json
{
  "fechaInicio": "2026-10-10",
  "fechaFin": "2026-10-12"
}
```

### Ejemplo de respuesta

```json
{
  "diasHabiles": 1
}
```

El resultado corresponde a un periodo de ejemplo en el que se cuenta el sábado, se excluye el domingo y se cuenta el lunes, siempre que no sea festivo configurado.

## Integración con Power Platform

La API se consume desde una aplicación Canvas de Power Apps mediante el conector personalizado `API_CalcularDiasHabiles`. El resultado se guarda en la lista de SharePoint junto con la solicitud de vacaciones.

Power Automate complementa la solución mediante flujos de notificación, recordatorios de solicitudes pendientes y exportación de registros a CSV.

## Ejecución local

1. Instalar el SDK de .NET 8 y las herramientas necesarias de Azure Functions.
2. Clonar este repositorio.
3. Abrir una terminal en la carpeta del proyecto.
4. Ejecutar `dotnet restore`.
5. Ejecutar `dotnet build`.
6. Configurar y ejecutar el proyecto localmente con las herramientas de Azure Functions.

## Repositorio

Código fuente: https://github.com/valolo618/ApiVacaciones

## Autora

Desarrollado como parte de una prueba técnica de práctica en Power Platform y desarrollo de software.
