namespace GroupMealApi.Models;

/// <summary>
/// The Response format for a get one request
/// </summary>
/// <typeparam name="T">
/// The type of the data in the response
/// </typeparam>
public class GetOneResponse<T>
{
    public T? data { get; set; }

    public bool success { get; set; }

    public string? message { get; set; }

}