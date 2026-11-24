namespace Prestamos.Api.Services;

// Reglas de negocio en un solo sitio, para poder cambiarlas fácilmente
public static class ReglasPrestamo
{
    public const decimal TasaAnual = 0.18m;            // 18 % anual fijo
    public const decimal MontoMinimo = 100m;
    public const decimal MontoMaximo = 50_000m;
    public const int PlazoMinimoMeses = 3;
    public const int PlazoMaximoMeses = 60;
    public const int MaxPrestamosActivos = 2;          // por cliente
    public const decimal MaxEndeudamiento = 0.40m;     // las cuotas no pueden pasar del 40 % del ingreso

    // Devuelve un mensaje de error, o null si el monto y el plazo son válidos
    public static string? ValidarMontoYPlazo(decimal monto, int meses)
    {
        if (monto < MontoMinimo || monto > MontoMaximo)
        {
            return $"El monto debe estar entre {MontoMinimo} y {MontoMaximo}.";
        }

        if (meses < PlazoMinimoMeses || meses > PlazoMaximoMeses)
        {
            return $"El plazo debe estar entre {PlazoMinimoMeses} y {PlazoMaximoMeses} meses.";
        }

        return null;
    }
}
