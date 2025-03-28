using System.Text.Json.Serialization;

public class ResponseGetProducts<T>
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] //Para evitar imprimir valores null al serializar la respuesta.
    public string? Sort { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Order { get; set; }

    public bool Status { get; set; }
    public bool IsDeleted { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; set; }

    
    public T Products { get; set; }

    // Constructor
    public ResponseGetProducts(int total, int page, int limit, T products, string? sort = null, string? order = null, 
                               bool status = true, bool isDeleted = false, string? type = null)
    {
        Total = total;
        Page = page;
        Limit = limit;
        Sort = sort;
        Order = order;
        Status = status;
        IsDeleted = isDeleted;
        Type = type;
        Products = products;
    }
}
