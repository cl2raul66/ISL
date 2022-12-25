using ISL.Modelos;
using LiteDB;
using System.Text;

namespace ISL.Servicios
{
    public interface ILaboresServicio
    {
        bool ExisteBd { get; }
        bool ExisteDatos { get; }
        IEnumerable<ActividadDiaria> GetTodos { get; }

        bool ExisteActividad(ActividadDiaria entity);
        ActividadDiaria GetByFecha(long fecha);
        bool Upsert(ActividadDiaria entity);

        ActDiaView ToActdiaview(ActividadDiaria actividadDiaria);
    }

    public class LaboresServicio : ILaboresServicio
    {
        private readonly ILiteCollection<ActividadDiaria> collection;

        public LaboresServicio()
        {
            var cnx = new ConnectionString()
            {
                Filename = Path.Combine(FileSystem.Current.CacheDirectory, "LaboresISL.db"),
                Password = "12345678"
            };

            LiteDatabase db = new(cnx);

            collection = db.GetCollection<ActividadDiaria>();
            ExisteBd = db.CollectionExists(nameof(ActividadDiaria));
            ExisteDatos = collection.LongCount() > 0;
            collection.EnsureIndex(x => x.Fecha);
        }

        public bool ExisteBd { private set; get; }

        public bool ExisteDatos { private set; get; }

        public bool ExisteActividad(ActividadDiaria entity) => collection.FindById(entity.Fecha) is not null;

        public IEnumerable<ActividadDiaria> GetTodos => collection.FindAll();

        public ActividadDiaria GetByFecha(long fecha) => collection.FindOne(x => x.Fecha == fecha);

        public bool Upsert(ActividadDiaria entity)
        {
            bool resul = false;
            if (ExisteDatos && ExisteActividad(entity))
            {
                resul = collection.Update(entity.Id, entity);
            }
            else
            {
                resul = collection.Insert(entity) is not null;
            };

            return resul;
        }

        public ActDiaView ToActdiaview(ActividadDiaria actividadDiaria) => new()
        {
            DiaSemanaIniciales = FechaLongToFechaString(actividadDiaria.Fecha),
            HorarioEntrada = HoraToString(actividadDiaria.HorarioEntrada),
            HorarioSalida = HoraToString(actividadDiaria.HorarioSalida),
            Actividades = ActividadesToString(actividadDiaria.Actividades)
        };

        #region Útiles
        private string ActividadesToString(List<string> actividades)
        {
            StringBuilder sb = new();
            foreach (string actividad in actividades)
            {
                sb.Append($"{actividad};");
            }

            return sb.ToString().Remove(sb.Length - 1);
        }

        public string HoraToString(long horario) => new DateTime(horario).ToString("t");

        public string FechaLongToFechaString(long fecha) => new DateTime(fecha).ToString("dd/MM/yyyy");
        //public long FechaStringToFechaLong(string fecha) => 
        #endregion
    }
}
