
namespace ISL.Modelos;

//public class Expediente
//{
//    public string Usuario { get; set; }
//    public int NoSemana { get; set; }
//    public List<ActividadDiaria> Labores { get; set; }
//}
public record Expediente(Contacto Usuario, int NoSemana, List<ActividadDiaria> Labores, string Observaciones)
{
}
