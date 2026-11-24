namespace Prestamos.Api.Services;

public static class CalculadoraPrestamo
{
    // Cuota mensual fija (sistema francés de amortización):
    //   cuota = P * r / (1 - (1 + r)^-n)
    // donde P = monto, r = tasa mensual, n = número de meses
    public static decimal CuotaMensual(decimal monto, decimal tasaAnual, int meses)
    {
        var tasaMensual = tasaAnual / 12m;

        if (tasaMensual == 0m)
        {
            return Math.Round(monto / meses, 2);
        }

        var factor = (decimal)Math.Pow((double)(1m + tasaMensual), -meses);
        return Math.Round(monto * tasaMensual / (1m - factor), 2);
    }
}
