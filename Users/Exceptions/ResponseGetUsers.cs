using System.Text.Json.Serialization;

public class ResponseGetUsers<T>
{
    public int Page { get; set; }
    public int Limit { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] //Para evitar imprimir valores null al serializar la respuesta.
    public string ? Sort { get; set; }
    public string Order { get; set; }
    public bool is_active { get; set; }
    public bool is_deleted { get; set; }
    public bool is_superuser { get; set; }
    public bool email_verified { get; set; }
    public T Users { get; set; }

    // Constructor
    public ResponseGetUsers(int page, int limit, string? sort,string order, bool is_active, bool is_deleted , bool is_superuser , bool email_verified , T users)
    {
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