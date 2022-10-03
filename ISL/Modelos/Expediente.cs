using LiteDB;

namespace ISL.Modelos;

//public class Expediente
//{
//    public string Usuario { get; set; }
//    public int NoSemana { get; set; }
//    public List<ActividadDiaria> Labores { get; set; }
//}
public record Expediente(string Usuario, int NoSemana, Dictionary<string, ActividadDiaria> Labores)
{
}
