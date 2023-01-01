using ISL.Modelos;
using System.IO;
using System.Text;
using System.Text.Json;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace ISL.Servicios;

public interface ICunopaDatosBotServicio
{
    void EnviarDatosToBot(int noSemanaTerminada);
}

public class CunopaDatosBotServicio : ICunopaDatosBotServicio
{
    private readonly BotClient api;

    private readonly IExpedienteLocalServicio expedienteLocalServ;
    private readonly IConnectivity connectivityServ;

    public CunopaDatosBotServicio(IExpedienteLocalServicio expedienteLocalServicio)
    {
        expedienteLocalServ = expedienteLocalServicio;
        connectivityServ = Connectivity.Current;

        api = new BotClient("5960349452:AAGnKTbjF1oGKFaNet919wTlsrJmWXcGr1U");
    }

    public void EnviarDatosToBot(int noSemanaTerminada)
    {
        if (connectivityServ.NetworkAccess == NetworkAccess.None || connectivityServ.NetworkAccess == NetworkAccess.Unknown || connectivityServ.NetworkAccess == NetworkAccess.Local) { return; }

        ExpedienteLocal expLocal = expedienteLocalServ.GetSemana(noSemanaTerminada);

        if (expLocal is null) { return; }

        var exp = new Expediente()
        {
            Usuario = Preferences.Get("Nombreusuario", string.Empty),
            NoSemana = expLocal.Id,
            Observaciones = Preferences.Get("Obs", string.Empty),
            LaboresPorDia = expLocal.LaboresPorDia.Values.ToList()
        };

        var updates = api.GetUpdates();
        if (updates.Any())
        {
            foreach (var update in updates)
            {
                if (update.Type == UpdateType.Message)
                {
                    long chatId = update.Message.Chat.Id;
                    var file = new InputFile(Encoding.UTF8.GetBytes(JsonSerializer.Serialize<Expediente>(exp)), $"ISL_{exp.NoSemana}_{exp.Usuario}.json");
                    var resul = api.SendDocument(chatId, file);

                    Preferences.Set("telegramBot", resul is not null);
                    break;
                }
            }
        }
    }

}
