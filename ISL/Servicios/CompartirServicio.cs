using ISL.Modelos;

namespace ISL.Servicios;

public interface ICompartirServicio
{
    Task CompartirPlantilla(Expediente exp);
    Task CompartirPlantillaManual();
}

public class CompartirServicio : ICompartirServicio
{
    private readonly IGenerarDocServicio generarDocServ;
    private readonly string ModeloCache = Path.Combine(FileSystem.Current.CacheDirectory, "Modelo_ISL.docx");
    private readonly string PlantillaCache = Path.Combine(FileSystem.Current.CacheDirectory, "Plantilla_ISL.docx");

    public CompartirServicio(IGenerarDocServicio generarDocServicio)
    {
        generarDocServ = generarDocServicio;
    }

    public async Task CompartirPlantillaManual()
    {
        var doc = await FileSystem.Current.OpenAppPackageFileAsync("Plantilla_ISL_Manual.docx");
        FileStream fs = File.OpenWrite(ModeloCache);
        await doc.CopyToAsync(fs);
        fs.Close();

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Modelo ISL",
            File = new ShareFile(ModeloCache)
        });
    }

    public async Task CompartirPlantilla(Expediente exp)
    {
        await generarDocServ.Crear(exp);
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = $"ISL_{exp.NoSemana} - {exp.Usuario}",
            File = new ShareFile(PlantillaCache)
        });
    }
}
