namespace ISL.Modelos;

//public class Expediente
//{
//    public string Usuario { get; set; }
//    public int NoSemana { get; set; }
//    public List<ActividadDiaria> Labores { get; set; }
//}
//public record Expediente(Contacto Usuario, int NoSemana, List<ActividadDiaria> Labores, string Observaciones)
//{
//}

public class Expediente
{    
    public int NoSemana { get; set; }
    public string Usuario { get; set; }
    public List<Labor> LaboresPorDia { get; set; }
    public string Observaciones { get; set; }
}

public class ExpedienteLocal
{
    public int Id { get; set; }
    public Dictionary<DateTime, Labor> LaboresPorDia { get; set; }
}
