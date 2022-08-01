using Microsoft.AspNetCore.Mvc;



namespace GroupMealApi.Models
{
    public interface IAppController
    {
        ActionResult HandleError(Exception exception);

        void ControllerLoggerStart(CrudRequest crudRequest);

        void ControllerLoggerEnd(DateTime startTime);

        // ActionResult<GetAllResponse<T>> GetAll<T>() where T : class;

    }
}