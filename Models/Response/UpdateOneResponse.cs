namespace GroupMealApi.Models;


/// <summary>
/// The Response format for an update request 
/// </summary>
public class UpdateOneResponse<T>
{
    public string id { get; set; } = null!;

    public T? data { get; set; }

    public bool success { get; set; }

    public string? message { get; set; }

}

