using ISL.Modelos;
using LiteDB;

namespace ISL.Servicios;

public interface IExpedienteLocalServicio
{
    bool ExisteBd { get; }
    bool ExisteDatos { get; }

    bool AgregarLabores(Labor entity);
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

    public ExpedienteLocal GetSemana(int noSemana) => collection.FindById(noSemana);

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
                {fechaServ.PrimerDia.AddDays(5), null },
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
}
