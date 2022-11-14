using LiteDB;

namespace ISL.Modelos;

public record Contacto(ObjectId Id, string Nombre1, string Nombre2, string Apellido1, string Apellido2, string Identificacion, string Sexo, Dictionary<string,string> Telefonos, byte[] FirmaDigital)
{
}