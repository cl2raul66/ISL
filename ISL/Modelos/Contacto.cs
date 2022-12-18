using LiteDB;

namespace ISL.Modelos;

//public record Contacto(ObjectId Id, string Nombre1, string Nombre2, string Apellido1, string Apellido2, string Identificacion, string Sexo, Dictionary<string,string> Telefonos, byte[] FirmaDigital)
//{
//    public override string ToString()
//    {
//        //return base.ToString();
//        string nombreCompleto = $"{Nombre1} ";
//        if (!string.IsNullOrEmpty(Nombre2))
//        {
//            nombreCompleto += $"{Nombre2} ";
//        }
//        else
//        {
//            nombreCompleto += $"{Apellido1} ";
//        }

//        if (!string.IsNullOrEmpty(Apellido2))
//        {
//            nombreCompleto += Apellido2;
//        }
//        else
//        {
//            nombreCompleto = nombreCompleto.Trim();
//        }

//        return nombreCompleto;
//    }
//}

public class Contacto
{
    public ObjectId Id { set; get; }
    public string Nombre1 { set; get; }
    public string Nombre2 { set; get; }
    public string Apellido1 { set; get; }
    public string Apellido2 { set; get; }
    public string Identificacion { set; get; }
    public string Sexo { set; get; }
    public Dictionary<string, string> Telefonos { set; get; }
    public byte[] FirmaDigital { set; get; }

    public Contacto() { }
    public Contacto(string nombre1, string nombre2, string apellido1, string apellido2, string identificacion, string sexo, Dictionary<string, string> telefonos, byte[] firmaDigital)
    {
        Nombre1 = nombre1;
        Nombre2 = nombre2;
        Apellido1 = apellido1;
        Apellido2 = apellido2;
        Identificacion = identificacion;
        Sexo = sexo;
        Telefonos = telefonos;
        FirmaDigital = firmaDigital;
    }

    public override string ToString()
    {
        //return base.ToString();
        string nombreCompleto = $"{Nombre1} ";
        if (!string.IsNullOrEmpty(Nombre2))
        {
            nombreCompleto += $"{Nombre2} ";
        }
        else
        {
            nombreCompleto += $"{Apellido1} ";
        }

        if (!string.IsNullOrEmpty(Apellido2))
        {
            nombreCompleto += Apellido2;
        }
        else
        {
            nombreCompleto = nombreCompleto.Trim();
        }

        return nombreCompleto;
    }
}