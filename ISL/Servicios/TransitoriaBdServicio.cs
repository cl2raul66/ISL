using ISL.Modelos;
using LiteDB;

namespace ISL.Servicios;

public interface ITransitoriaBdServicio
{
    bool ExisteBd { get; }
    bool ExisteDatos { get; }
    ExpedienteLocal GetExpediente { get; }

    bool UpsertExpediente(ExpedienteLocal entity);
}

public class TransitoriaBdServicio : ITransitoriaBdServicio
{
    private readonly ILiteCollection<ExpedienteLocal> collection;
    private List<ActividadDiaria> Labores = new();

    public TransitoriaBdServicio()
    {
        var cnx = new ConnectionString()
        {
            Filename = Path.Combine(FileSystem.Current.CacheDirectory, "ExpedientesISL.db"),
            Password = "12345678"
        };

        LiteDatabase db = new(cnx);

        collection = db.GetCollection<ExpedienteLocal>();
        ExisteBd = db.CollectionExists(nameof(ExpedienteLocal));
        ExisteDatos = collection.LongCount() > 0;
        collection.EnsureIndex(x => x.NoSemana);
    }

    public bool ExisteBd { private set; get; }

    public bool ExisteDatos { private set; get; }

    public ExpedienteLocal GetExpediente
    {
        get
        {
            var resul = collection.FindAll().FirstOrDefault();
            return resul;
            //return new(resul.Usuario, resul.NoSemana, resul.Labores, resul.Observaciones);
        }
    }

    public bool UpsertExpediente(ExpedienteLocal entity)
    {
        bool resul = ExisteDatos switch
        {
            true => collection.Update(entity.NoSemana,entity),
            false => collection.Insert(entity) is not null
        };

        return resul;
    }
}
