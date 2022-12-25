using ISL.Modelos;
using LiteDB;

namespace ISL.Servicios;

public interface IExpedienteLocalServicio
{
    bool ExisteBd { get; }
    bool ExisteDatos { get; }

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
        ExisteDatos = collection.LongCount() > 0;
        collection.EnsureIndex(x => x.NoSemana);
    }

    public bool ExisteBd { private set; get; }

    public bool ExisteDatos { private set; get; }

    public ExpedienteLocal GetSemana(int noSemana) => collection.FindOne(x => x.NoSemana == noSemana);

    public bool NuevaSemana(int noSemana)
    {
        ExpedienteLocal entity = new()
        {
            NoSemana = noSemana,
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
}
