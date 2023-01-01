using ISL.Modelos;
using LiteDB;

namespace ISL.Servicios;

public interface IExpedienteLocalServicio
{
    bool ExisteBd { get; }
    bool ExisteDatos { get; }
    bool ExistenLabores { get; }

    bool AgregarLabores(Labor entity);
    bool AgregarObservaciones(string observaciones, int? noSemana = null);
    bool ExisteSemana(int noSemana);
    string GetObservaciones(int noSemana);
    ExpedienteLocal GetSemana(int noSemana);
    bool NuevaSemana(int noSemana);
}

public class ExpedienteLocalServicio : IExpedienteLocalServicio
{
    private readonly ILiteCollection<ExpedienteLocal> collection;
    private readonly IFechaServicio fechaServ;

    public ExpedienteLocalServicio(IFechaServicio fechaServicio)
    {
        fechaServ = fechaServicio;

        var cnx = new ConnectionString()
        {
            Filename = Path.Combine(FileSystem.Current.CacheDirectory, "ExpedienteLocal.db")
        };

        LiteDatabase db = new(cnx);

        collection = db.GetCollection<ExpedienteLocal>();
        ExisteBd = db.CollectionExists(nameof(ExpedienteLocal));
    }

    public bool ExisteBd { private set; get; }

    public bool ExisteDatos => collection.LongCount() > 0;

    public bool ExisteSemana(int noSemana) => collection.Exists(x => x.Id == noSemana);

    public bool ExistenLabores
    {
        get
        {
            var r1 = collection.FindById(fechaServ.NoSemanaDelAnio())?.LaboresPorDia;
            if (r1 is not null)
            {
                foreach (var item in r1.Values)
                {
                    if (item?.Actividades?.Any() ?? false)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public ExpedienteLocal GetSemana(int noSemana) => collection.FindById(noSemana);

    public string GetObservaciones(int noSemana) => collection.FindById(noSemana)?.Observaciones ?? string.Empty;

    public bool NuevaSemana(int noSemana)
    {
        ExpedienteLocal entity = new()
        {
            Id = noSemana,
            LaboresPorDia = new()
            {
                {fechaServ.PrimerDia, null },
                {fechaServ.PrimerDia.AddDays(1), null },
                {fechaServ.PrimerDia.AddDays(2), null },
                {fechaServ.PrimerDia.AddDays(3), null },
                {fechaServ.PrimerDia.AddDays(4), null },
                {fechaServ.UltimoDia, null }
            }
        };
        return collection.Insert(entity) is not null;
    }

    public bool AgregarLabores(Labor entity)
    {
        var expediente = GetSemana(fechaServ.NoSemanaDelAnio(entity.HorarioEntrada));
        expediente.LaboresPorDia[entity.HorarioEntrada.Value.Date] = entity;
        return collection.Update(expediente.Id, expediente);
    }

    public bool AgregarObservaciones(string observaciones, int? noSemana = null)
    {
        ExpedienteLocal expediente = noSemana is not null ? GetSemana(noSemana.Value) : GetSemana(fechaServ.NoSemanaDelAnio());
        expediente.Observaciones = observaciones;
        return collection.Update(expediente.Id, expediente);
    }
}
