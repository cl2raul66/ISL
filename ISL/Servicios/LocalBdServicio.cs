using ISL.Modelos;
using LiteDB;

namespace ISL.Servicios;

public interface ILocalBdServicio
{
    bool ExisteBd { get; }
    bool ExisteDatos { get; }
    IEnumerable<ExpedienteLocal> GetAll { get; }

    void DeleteAll();
    ExpedienteLocal GetByNoSemana(int noSemana);
    bool Upsert(ExpedienteLocal entity);
}

public class LocalBdServicio : ILocalBdServicio
{
    private readonly ILiteCollection<ExpedienteLocal> collection;

    public LocalBdServicio()
    {
        var cnx = new ConnectionString()
        {
            Filename = Path.Combine(FileSystem.Current.AppDataDirectory, "ExpedientesISL.db"),
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

    public IEnumerable<ExpedienteLocal> GetAll => collection.FindAll().Reverse();

    public ExpedienteLocal GetByNoSemana(int noSemana) => collection.FindOne(e => e.NoSemana == noSemana);

    //public bool Insert(ExpedienteLocal entity) => collection.Insert(entity) is not null;

    //public bool Update(ExpedienteLocal entity) => collection.Update(entity.NoSemana, entity);

    public bool Upsert(ExpedienteLocal entity)
    {
        bool resul = ExisteEntity(entity.NoSemana) switch
        {
            true => collection.Update(entity.NoSemana, entity),
            false => collection.Insert(entity) is not null
        };

        return resul;
    }

    public void DeleteAll() => collection.DeleteAll();

    private bool ExisteEntity(int noSemana) => collection.FindOne(e => e.NoSemana == noSemana) is not null;

    #region Extras
    #endregion
}
