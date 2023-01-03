using DocumentFormat.OpenXml.Packaging;
using ISL.Modelos;
using System.Text;
using System.Text.RegularExpressions;

namespace ISL.Servicios;

public interface IGenerarDocServicio
{
    Task Crear(Expediente exp);
    Task Visualizar(Expediente exp);
}

public class GenerarDocServicio : IGenerarDocServicio
{
    private readonly IFechaServicio fechaServ;

    private string fileCache = Path.Combine(FileSystem.Current.CacheDirectory, "Plantilla_ISL.docx");

    public GenerarDocServicio(IFechaServicio fechaServicio)
    {
        fechaServ = fechaServicio;
    }

    public async Task Crear(Expediente exp)
    {
        if (File.Exists(fileCache))
        {
            File.Delete(fileCache);
        }
        var doc = await FileSystem.Current.OpenAppPackageFileAsync("Plantilla_ISL.docx");
        FileStream fs = File.OpenWrite(fileCache);
        await doc.CopyToAsync(fs);
        fs.Close();

        var modeloDoc = ExpedienteToModeloDoc(exp);

        using (var wordDoc = WordprocessingDocument.Open(fileCache, true))
        {
            string docText = string.Empty;
            using (StreamReader sr = new(wordDoc.MainDocumentPart.GetStream()))
            {
                docText = sr.ReadToEnd();
            }

            foreach (var item in modeloDoc)
            {
                Regex regexText = new(item.Key);
                docText = regexText.Replace(docText, item.Value);
            }

            using (StreamWriter sw = new(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
            {
                sw.WriteLine(docText);
            }
        }
    }

    public async Task Visualizar(Expediente exp)
    {
        await Crear(exp);
        await Launcher.Default.OpenAsync(new OpenFileRequest(Path.GetFileNameWithoutExtension(fileCache), new ReadOnlyFile(fileCache)));
    }

    #region Código extra
    private Dictionary<string, string> ExpedienteToModeloDoc(Expediente exp)
    {
        Dictionary<string, string> resul = new(){
            {"#nombrecompleto#", exp.Usuario},
            {"#fechaini#", fechaServ.PrimerDia.ToShortDateString()},
            {"#fechafin#", fechaServ.UltimoDia.ToShortDateString()},

            {"#luhi#", exp.LaboresPorDia[0]?.HorarioEntrada?.ToString("t") ?? string.Empty},
            {"#luhf#", exp.LaboresPorDia[0]?.HorarioSalida?.ToString("t") ?? string.Empty},
            {"#lul#", ListStringToString(exp.LaboresPorDia[0]?.Actividades)},

            {"#mahi#", exp.LaboresPorDia[1]?.HorarioEntrada?.ToString("t") ?? string.Empty},
            {"#mahf#", exp.LaboresPorDia[1]?.HorarioSalida?.ToString("t") ?? string.Empty},
            {"#mal#", ListStringToString(exp.LaboresPorDia[1]?.Actividades)},

            {"#mihi#", exp.LaboresPorDia[2]?.HorarioEntrada?.ToString("t") ?? string.Empty},
            {"#mihf#", exp.LaboresPorDia[2]?.HorarioSalida?.ToString("t") ?? string.Empty},
            {"#mil#", ListStringToString(exp.LaboresPorDia[2]?.Actividades) },

            {"#juhi#", exp.LaboresPorDia[3]?.HorarioEntrada?.ToString("t") ?? string.Empty},
            { "#juhf#", exp.LaboresPorDia[3]?.HorarioSalida?.ToString("t") ?? string.Empty},
            {"#jul#", ListStringToString(exp.LaboresPorDia[3]?.Actividades) },

            {"#vihi#", exp.LaboresPorDia[4]?.HorarioEntrada?.ToString("t") ?? string.Empty},
            {"#vihf#", exp.LaboresPorDia[4]?.HorarioSalida?.ToString("t") ?? string.Empty},
            {"#vil#", ListStringToString(exp.LaboresPorDia[4]?.Actividades)},

            {"#sahi#", exp.LaboresPorDia[5]?.HorarioEntrada?.ToString("t") ?? string.Empty},
            {"#sahf#", exp.LaboresPorDia[5]?.HorarioSalida?.ToString("t") ?? string.Empty},
            {"#sal#", ListStringToString(exp.LaboresPorDia[5]?.Actividades)},

            {"#obs#", exp.Observaciones ?? string.Empty},
            {"#fir#", string.Empty}
        };
        return resul;
    }

    private string ListStringToString(List<string> lista)
    {
        if (lista is null)
        {
            return string.Empty;
        }

        StringBuilder sb = new();
        lista.Reverse();
        foreach (string item in lista)
        {
            if (item == lista.Last())
            {
                sb.Append($"{item}.");
            }
            else
            {
                sb.Append($"{item}; ");
            }
        }
        return sb.ToString();
    }
    #endregion
}
