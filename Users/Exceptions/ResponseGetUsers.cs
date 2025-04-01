using System.Text.Json.Serialization;

public class ResponseGetUsers<T>
{
    public int Total { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] //Para evitar imprimir valores null al serializar la respuesta.
    public object? Page { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public object? Limit { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public object? Sort { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public object? Order { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public object? is_active { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public object? is_deleted { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public object? is_superuser { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public object? email_verified { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public T? Users { get; set; }

    // Constructor
    public ResponseGetUsers(int total,object? page, object? limit, object? sort,object? order, object? is_active, object? is_deleted , object? is_superuser , object? email_verified , T? users)
    {
        Total = total;
        Page = page;
        Limit = limit;
        Sort = sort;
        Order = order;
        this.is_active = is_active;
        this.is_deleted = is_deleted;
        this.is_superuser = is_superuser;
        this.email_verified = email_verified;
        Users = users;
    }
}