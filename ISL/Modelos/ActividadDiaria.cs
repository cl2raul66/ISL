using LiteDB;

namespace ISL.Modelos;

//public class ActividadDiaria
//{
//    public DateTime Inicio { get; set; }
//    public DateTime Fin { get; set; }
//    public List<string> Adtividades { get; set; }
//}

//public record ActividadDiaria(DateTime? Fecha, DateTime? HorarioEntrada, DateTime? HorarioSalida, List<string> Actividades)
//{
//}

public class ActividadDiaria
{
    public ObjectId Id { get; set; }
    public long Fecha { set; get; }
    public long HorarioEntrada { set; get; }
    public long HorarioSalida { set; get; }
    public List<string> Actividades { set; get; }
}

public class ActDiaView
{
    public string DiaSemanaIniciales { set; get; }
    public string HorarioEntrada { set; get; }
    public string HorarioSalida { set; get; }
    public string Actividades { set; get; }
}