using ISL.Modelos;
using LiteDB;

namespace ISL.Servicios;

public interface ITransitoriaBdServicio
{
    bool ExisteBd { get; }
    bool ExisteDatos { get; }
    Expediente GetExpediente { get; }

    Dictionary<string, int> GetTotalLaboresPorDia(Dictionary<string, ActividadDiaria[]> labores);
    bool UpsertActividadDiaria(int noSemana, string dia, ActividadDiaria actividad, string observaciones);
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

    public Dictionary<string, int> GetTotalLaboresPorDia(Dictionary<string, ActividadDiaria[]> labores)
    {
        Dictionary<string, int> resul = new();
        foreach (var item in labores)
        {
            resul.Add(item.Key, item.Value.Count());
        }

        return resul;
    }

    public bool UpsertActividadDiaria(int noSemana, string dia, ActividadDiaria actividad, string observaciones)
    {
        Expediente expediente = collection.FindOne(x => x.NoSemana == noSemana);
        List<ActividadDiaria> resul = new();

        if (!expediente.Labores?.Any() ?? false || !expediente.Labores.ContainsKey(dia))
        {
            resul.Add(actividad);
            expediente.Labores.Add(dia, resul.ToArray());
        }
        else
        {
            resul = expediente.Labores[dia].ToList();
            resul.Add(actividad);
            expediente.Labores.Add(dia, resul.ToArray());
        }

        Expediente expedienteNew = new(expediente.Usuario, expediente.NoSemana, expediente.Labores, observaciones);

        return collection.Update(expedienteNew);
    }
}
