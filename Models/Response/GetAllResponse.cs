namespace GroupMealApi.Models;



/// <summary>
/// The Response format for a get all request
/// </summary>
/// <typeparam name="T">
///  The type of the data in the response
/// </typeparam>
public class GetAllResponse<T>
{
    public T[] dataset { get; set; } = null!;

    public int total { get; set; }
}



