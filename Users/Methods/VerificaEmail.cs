using System.Net;
using DnsClient;
using System.Linq;
public class VerificaEmail{

public bool DominioExiste(String email){
    try
    {
        string dominio = email.Split('@')[1]; // Obtener el dominio del correo electrónico
        var lookup = new LookupClient(); // Crear un cliente de búsqueda DNS

        //Buscamos registros MX en los servidores DNS
        var resultado = lookup.Query(dominio, QueryType.MX);
         return resultado.Answers.MxRecords().Any();
    }
    catch
    {
        return false;
    }
}
}