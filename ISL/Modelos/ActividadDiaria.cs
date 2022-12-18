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
    public DateTime? Fecha { set; get; }
    public DateTime? HorarioEntrada { set; get; }
    public DateTime? HorarioSalida { set; get; }
    public List<string> Actividades { set; get; }

    public ActividadDiaria() { }

    public ActividadDiaria(DateTime? fecha, DateTime? horarioEntrada, DateTime? horarioSalida, List<string> actividades)
    {
        Fecha = fecha;
        HorarioEntrada = horarioEntrada;
        HorarioSalida = horarioSalida;
        Actividades = actividades;
    }
}