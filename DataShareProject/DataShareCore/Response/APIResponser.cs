namespace DataShareCore.Response;

public class APIResponser<TData>
{
    public bool success { get; set; }
    public string message { get; set; }
    public TData content { get; set; }
    
    
}