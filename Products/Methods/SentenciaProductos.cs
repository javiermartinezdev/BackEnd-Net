using Dapper;

namespace productos.Methods{
    public class SentenciaProductos{
        int safePage;
        int safeLimit;
        string? sort;
        string? safeOrder;
        bool safeStatus;
        bool safeIsdelete;
        string? type;
        
        string sentencia = "SELECT * FROM \"Products\" WHERE 1=1 ";

        // Primary Constructor
        public SentenciaProductos(int safePage, int safeLimit, string? sort, string? safeOrder, bool safeStatus, bool safeIsdelete, string? type)
        {
            this.safePage = safePage;
            this.safeLimit = safeLimit;
            this.sort = sort;
            this.safeOrder = safeOrder;
            this.safeStatus = safeStatus;
            this.safeIsdelete = safeIsdelete;
            this.type = type;
        }

        public (string Sentencia, DynamicParameters Parametros) CrearSenentiaSQLProduct()
        {
            var parametros = new DynamicParameters();
            sentencia += " AND status = @status ";
            parametros.Add("@status", safeStatus);

            sentencia += " AND is_deleted = @is_deleted ";
            parametros.Add("@is_deleted", safeIsdelete);

            if (!string.IsNullOrEmpty(type))
            {
                sentencia += " AND type = @type ";
                parametros.Add("@type", type);
            }

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
}