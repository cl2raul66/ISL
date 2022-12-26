 namespace ISL.Modelos;

public class Labor
{
    public DateTime? HorarioEntrada { set; get; }
    public DateTime? HorarioSalida { set; get; }
    public List<string> Actividades { set; get; }
}

public class LaborView
{
    public DateOnly Dia { set; get; }
    public TimeOnly? HorarioEntrada { set; get; }
    public TimeOnly? HorarioSalida { set; get; }
    public string NoActividades { set; get; }
}