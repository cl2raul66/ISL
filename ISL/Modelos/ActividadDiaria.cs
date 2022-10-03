namespace ISL.Modelos;

//public class ActividadDiaria
//{
//    public DateTime Inicio { get; set; }
//    public DateTime Fin { get; set; }
//    public List<string> Adtividades { get; set; }
//}
public record ActividadDiaria(DateTime Inicio, DateTime Fin, List<string> Adtividades)
{
}