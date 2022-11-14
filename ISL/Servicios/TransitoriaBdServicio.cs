using ISL.Modelos;
using LiteDB;

namespace ISL.Servicios;

public interface ITransitoriaBdServicio
{
    bool ExisteBd { get; }
    bool ExisteDatos { get; }
    Expediente GetExpediente { get; }

    bool UpsertExpediente(Expediente entity);
}

public class TransitoriaBdServicio : ITransitoriaBdServicio
{
    private readonly ILiteCollection<Expediente> collection;

    public TransitoriaBdServicio()
    {
        var cnx = new ConnectionString()
        {
            Filename = Path.Combine(FileSystem.Current.CacheDirectory, "ExpedientesISL.db"),
            Password = "12345678"
        };

        LiteDatabase db = new(cnx);

        collection = db.GetCollection<Expediente>();
        ExisteBd = db.CollectionExists(nameof(Expediente));
        ExisteDatos = collection.LongCount() > 0;
        collection.EnsureIndex(x => x.NoSemana);
    }

    public bool ExisteBd { private set; get; }

    public bool ExisteDatos { private set; get; }

    public Expediente GetExpediente => collection.FindAll().FirstOrDefault();

    public bool UpsertExpediente(Expediente entity)
    {
        bool resul = ExisteDatos switch
        {
            true => collection.Update(entity),
            false => collection.Insert(entity) is not null
        };

        return resul;
    }
}
