namespace ISL.Modelos;

//public record ExpedienteLocal(string Usuario, int? NoSemana, List<ActividadDiaria> Labores, string Observaciones)
//{
//}

public class ExpedienteLocal : Expediente
{
	//public string Usuario { get; set; }
	//public int? NoSemana { get; set; }
	//public List<ActividadDiaria> Labores { get; set; }
	//public string Observaciones { get; set; }

	public ExpedienteLocal(){}

	//public ExpedienteLocal(string usuario, int? noSemana, List<ActividadDiaria> labores, string observaciones)
	//{
	//	Usuario = usuario;
	//	NoSemana = noSemana;
	//	Labores = labores;
	//	Observaciones = observaciones;
	//}
	public ExpedienteLocal(int noSemana, List<ActividadDiaria> labores)
	{
		NoSemana = noSemana;
		Labores = labores;
	}


	public override string Usuario { set { value = string.Empty; } get => string.Empty; }
	public override string Observaciones { set { value = string.Empty; } get => string.Empty; }
}
