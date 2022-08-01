
namespace GroupMealApi.Models;


/// <summary>
/// The delete one response format 
/// </summary>
/// <typeparam name="T">
///  The type of the data in the response
/// </typeparam>
public class DeleteOneResponse<T>
{

    public string id { get; set; } = null!;

    public bool success { get; set; }

    public string? message { get; set; }

}
