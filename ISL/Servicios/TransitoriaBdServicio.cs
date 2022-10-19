using ISL.Modelos;
using LiteDB;

namespace ISL.Servicios;

public interface ITransitoriaBdServicio
{
    bool ExisteBd { get; }
    IEnumerable<Expediente> GetAllExpediente { get; }

    Dictionary<string, ActividadDiaria> GetLaboresFromExpediente(int noSemana);
    Dictionary<string, int> GetTotalLaboresFromExpediente(int noSemana);
    bool InsertExpediente(Expediente entity);
    bool UpsertActividadDiaria(int noSemana, string dia, ActividadDiaria actividad);
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
        ExisteBd = collection.LongCount() > 0;
        collection.EnsureIndex(x => x.NoSemana);
    }

    public bool ExisteBd { private set; get; }

    public IEnumerable<Expediente> GetAllExpediente => collection.FindAll().Reverse();

    public bool InsertExpediente(Expediente entity) => collection.Insert(entity) is not null;

    public Dictionary<string, int> GetTotalLaboresFromExpediente(int noSemana)
    {
        var labores = GetLaboresFromExpediente(noSemana);
        Dictionary<string, int> resul = new();
        foreach (var item in labores)
        {
            resul.Add(item.Key, item.Value.Adtividades.Count);
        }

        return resul;
    }

    //public bool UpdateExpediente(Expediente entity) => collection.Update(entity);

    public Dictionary<string, ActividadDiaria> GetLaboresFromExpediente(int noSemana) => collection.FindOne(x => x.NoSemana == noSemana)?.Labores ?? new();

    public bool UpsertActividadDiaria(int noSemana, string dia, ActividadDiaria actividad)
    {
        Dictionary<string, ActividadDiaria> resul = new();
        Expediente expediente = collection.FindOne(x => x.NoSemana == noSemana);

        if (!expediente.Labores?.Any() ?? false)
        {
            resul = expediente.Labores;
        }
        resul.Add(dia, actividad);

        Expediente expedienteNew = new(expediente.Usuario, expediente.NoSemana, resul);

        return collection.Update(expedienteNew);
    }
}
