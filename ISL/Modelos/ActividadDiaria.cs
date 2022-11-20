namespace ISL.Modelos;

//public class ActividadDiaria
//{
//    public DateTime Inicio { get; set; }
//    public DateTime Fin { get; set; }
//    public List<string> Adtividades { get; set; }
//}
public record ActividadDiaria(DateTime? Fecha, DateTime? HorarioEntrada, DateTime? HorarioSalida, List<string> Actividades)
{
}