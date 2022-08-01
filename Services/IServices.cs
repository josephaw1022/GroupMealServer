namespace GroupMealApi.Models;


/// <summary>
/// The base interface for all services 
/// </summary>
interface IService
{
    Type[] GetAll();

    Type GetById(string id);

    Type Create(Type type);

    bool Update(Type type);

    bool Delete(string id);

}



