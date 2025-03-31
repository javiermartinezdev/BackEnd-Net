using Dapper;
public class SentenciaUsuarios
{
    int safePage;
    int safeLimit;
    string? sort;
    string safeOrder;
    bool safeActive;
    bool safeisDeleted;
    bool safeIsSuperuser;
    bool safeEmailVerified; 

    public string sentencia = "SELECT * FROM \"Users\" WHERE 1=1 ";

    // Primary Constructor
    public SentenciaUsuarios(int safePage, int safeLimit, string? sort, string safeOrder, bool safeActive, bool safeisDeleted, bool safeIsSuperuser, bool safeEmailVerified)
    {
        this.safePage = safePage;
        this.safeLimit = safeLimit;
        this.sort = sort;
        this.safeOrder = safeOrder;
        this.safeActive = safeActive;
        this.safeisDeleted = safeisDeleted;
        this.safeIsSuperuser = safeIsSuperuser;
        this.safeEmailVerified = safeEmailVerified;
    }

    public (string Sentencia, DynamicParameters Parametros) CrearSenentiaSQLUser()
    {
        var parametros = new DynamicParameters();
        sentencia += " AND is_active = @active ";
        parametros.Add("@active", safeActive);

        sentencia += " AND is_deleted = @is_deleted ";
        parametros.Add("@is_deleted", safeisDeleted);

        sentencia += " AND is_superuser = @is_superuser ";
        parametros.Add("@is_superuser", safeIsSuperuser);

        sentencia += " AND email_verified = @email_verified ";
        parametros.Add("@email_verified", safeEmailVerified);

        //Verifiquemos que el orden no sea null ya que safeOrder tiene valor asc por defecto.
        if (!string.IsNullOrEmpty(sort))
        {
            sentencia += $" ORDER BY {sort} {safeOrder} ";
        }

        // Paginación
        sentencia += " LIMIT @limit OFFSET @offset ";
        parametros.Add("@limit", safeLimit);
        parametros.Add("@offset", (safePage - 1) * safeLimit);

        return (sentencia, parametros);
    }
}
