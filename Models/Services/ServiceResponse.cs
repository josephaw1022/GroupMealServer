namespace GroupMealApi.Models
{
    public class ServiceResponse
    {
        public bool Success { get; set; } = default!;
        public string SuccessMessage { get; set; } = null!; 
        public string? ErrorMessage { get; set; }
    }

    public class GetAllServiceResponse<T> : ServiceResponse
    {
        public List<T>  Data { get; set; } = new List<T>(); 
    }

    public class GetOneServiceResponse<T> : ServiceResponse
    {
        public T? Data { get; set; }
    }

    public class CreateOneServiceResponse<T> : ServiceResponse
    {
        public T? Data { get; set; }
    }

    public class UpdateOneServiceResponse<T> : ServiceResponse
    {
        public T? Data { get; set; }
    }

    public class DeleteOneServiceResponse<T> : ServiceResponse
    {
        public T? Data { get; set; }
    }

}