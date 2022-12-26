using DocumentFormat.OpenXml.Packaging;
using ISL.Modelos;
using System.Text;
using System.Text.RegularExpressions;

namespace ISL.Servicios;

public interface IGenerarDocServicio
{
    Task Crear(Expediente exp);
}

public class GenerarDocServicio : IGenerarDocServicio
{
    private readonly IFechaServicio fechaServ;

    private Dictionary<string, string> modeloDoc = new()
    {
        {"#nombrecompleto#",string.Empty},
        {"#fechaini#",string.Empty},
        {"#fechafin#",string.Empty},

        {"#luhi#",string.Empty},
        {"#luhf#",string.Empty},
        {"#lul#",string.Empty},

        {"#mahi#",string.Empty},
        {"#mahf#",string.Empty},
        {"#mal#",string.Empty},

        {"#mihi#",string.Empty},
        {"#mihf#",string.Empty},
        {"#mil#",string.Empty},

        {"#juhi#",string.Empty},
        {"#juhf#",string.Empty},
        {"#jul#",string.Empty},

        {"#vihi#",string.Empty},
        {"#vihf#",string.Empty},
        {"#vil#",string.Empty},

        {"#sahi#",string.Empty},
        {"#sahf#",string.Empty},
        {"#sal#",string.Empty},

        {"#obs#",string.Empty},
        {"#fir#",string.Empty}
    };

    string fileCache = Path.Combine(FileSystem.Current.CacheDirectory, "Plantilla_ISL.docx");

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

        ExpedienteToModeloDoc(exp);

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

        if (Preferences.Get("verDoc", false))
        {
            await Launcher.Default.OpenAsync(new OpenFileRequest(Path.GetFileNameWithoutExtension(fileCache), new ReadOnlyFile(fileCache)));
        }
    }

    #region Código extra
    private void ExpedienteToModeloDoc(Expediente exp)
    {
        modeloDoc["#nombrecompleto#"] = exp.Usuario;
        modeloDoc["#fechaini#"] = fechaServ.PrimerDia.ToShortDateString();
        modeloDoc["#fechafin#"] = fechaServ.UltimoDia.ToShortDateString();

        modeloDoc["#luhi#"] = exp.LaboresPorDia[0]?.HorarioEntrada.Value.ToString("t") ?? string.Empty;
        modeloDoc["#luhf#"] = exp.LaboresPorDia[0]?.HorarioSalida.Value.ToString("t") ?? string.Empty;
        modeloDoc["#lul#"] = ListStringToString(exp.LaboresPorDia[0]?.Actividades);

        modeloDoc["#mahi#"] = exp.LaboresPorDia[1]?.HorarioEntrada.Value.ToString("t") ?? string.Empty;
        modeloDoc["#mahf#"] = exp.LaboresPorDia[1]?.HorarioSalida.Value.ToString("t") ?? string.Empty;
        modeloDoc["#mal#"] = ListStringToString(exp.LaboresPorDia[1]?.Actividades);

        modeloDoc["#mihi#"] = exp.LaboresPorDia[2]?.HorarioEntrada.Value.ToString("t") ?? string.Empty;
        modeloDoc["#mihf#"] = exp.LaboresPorDia[2]?.HorarioSalida.Value.ToString("t") ?? string.Empty;
        modeloDoc["#mil#"] = ListStringToString(exp.LaboresPorDia[2]?.Actividades);

        modeloDoc["#juhi#"] = exp.LaboresPorDia[3]?.HorarioEntrada.Value.ToString("t") ?? string.Empty;
        modeloDoc["#juhf#"] = exp.LaboresPorDia[3]?.HorarioSalida.Value.ToString("t") ?? string.Empty;
        modeloDoc["#jul#"] = ListStringToString(exp.LaboresPorDia[3]?.Actividades);

        modeloDoc["#vihi#"] = exp.LaboresPorDia[4]?.HorarioEntrada.Value.ToString("t") ?? string.Empty;
        modeloDoc["#vihf#"] = exp.LaboresPorDia[4]?.HorarioSalida.Value.ToString("t") ?? string.Empty;
        modeloDoc["#vil#"] = ListStringToString(exp.LaboresPorDia[4]?.Actividades);

        modeloDoc["#sahi#"] = exp.LaboresPorDia[5]?.HorarioEntrada.Value.ToString("t") ?? string.Empty;
        modeloDoc["#sahf#"] = exp.LaboresPorDia[5]?.HorarioSalida.Value.ToString("t") ?? string.Empty;
        modeloDoc["#sal#"] = ListStringToString(exp.LaboresPorDia[5]?.Actividades);

        modeloDoc["#obs#"] = exp.Observaciones;
        modeloDoc["#fir#"] = string.Empty;
    }

    private string ListStringToString(List<string>? lista)
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
