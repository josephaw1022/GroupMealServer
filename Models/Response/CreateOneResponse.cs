

namespace GroupMealApi.Models
{
    /// <summary>
    /// The Response format for a create request 
    /// </summary>
    public class CreateOneResponse<T>
    {
        public T? data { get; set; }

        public bool success { get; set; }

        public string? message { get; set; }

    }

}