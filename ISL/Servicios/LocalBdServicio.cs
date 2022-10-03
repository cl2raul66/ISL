using ISL.Modelos;
using LiteDB;

namespace ISL.Servicios;

public interface ILocalBdServicio
{
    bool ExisteBd { get; }
    IEnumerable<Expediente> GetAll { get; }

    bool Insert(Expediente entity);
}

public class LocalBdServicio : ILocalBdServicio
{
    private readonly ILiteCollection<Expediente> collection;

    public LocalBdServicio()
    {
        var cnx = new ConnectionString()
        {
            Filename = Path.Combine(FileSystem.Current.AppDataDirectory, "ExpedientesISL.db"),
            Password = "12345678"
        };

        LiteDatabase db = new(cnx);

        ExisteBd = db.CollectionExists(nameof(Expediente));
        collection = db.GetCollection<Expediente>();
    }

    public bool ExisteBd { private set; get; }

    public IEnumerable<Expediente> GetAll => collection.FindAll().Reverse();

    public bool Insert(Expediente entity) => collection.Insert(entity) is not null;
}
