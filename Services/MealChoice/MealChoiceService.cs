using GroupMealApi.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Logging;



namespace GroupMealApi.Services
{
    public class MealChoiceService
    {

        private readonly string TableName = "MealChoice";
        private readonly IMongoCollection<MealChoiceDBO> _mealChoices;
        private readonly ILogger<MealChoiceService> _logger;


        public MealChoiceService(ILogger<MealChoiceService> logger)
        {

            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";
            var client = new MongoClient(connectionString);
            var databaseName = Environment.GetEnvironmentVariable("DB_NAME") ?? "";
            var database = client.GetDatabase(databaseName);
            _mealChoices = database.GetCollection<MealChoiceDBO>(TableName);
            _logger = logger;
        }

        private readonly int DefaultLimit = 1000;

        private readonly int DefaultOffset = 0;


        public List<MealChoiceDTO> GetAll()
        {
            var mealChoices =
            (
                from _mealChoice in _mealChoices.AsQueryable<MealChoiceDBO>()
                where _mealChoice.Deleted == false
                select new MealChoiceDTO
                {
                    Id = _mealChoice.Id,
                    GroupId = _mealChoice.GroupId,
                    Name = _mealChoice.Name,
                    Description = _mealChoice.Description,
                    ImageUrl = _mealChoice.ImageUrl,
                    MenuUrl = _mealChoice.MenuUrl,
                }
            )
            .Take(DefaultLimit)
            .Skip(DefaultOffset)
            .ToList();

            return mealChoices;
        }



        public List<MealChoiceDTO> GetAll(int offset, int limit)
        {

            if (offset < 0)
            {
                offset = DefaultOffset;
            }

            if (limit < 0)
            {
                limit = DefaultLimit;
            }

            if (limit > DefaultLimit)
            {
                limit = DefaultLimit;
            }

            var mealChoices =
            (
                from _mealChoice in _mealChoices.AsQueryable<MealChoiceDBO>()
                where _mealChoice.Deleted == false
                select new MealChoiceDTO
                {
                    Id = _mealChoice.Id,
                    GroupId = _mealChoice.GroupId,
                    Name = _mealChoice.Name,
                    Description = _mealChoice.Description,
                    ImageUrl = _mealChoice.ImageUrl,
                    MenuUrl = _mealChoice.MenuUrl,
                }
            )
            .Take(limit)
            .Skip(offset)
            .ToList();
            return mealChoices;
        }



        public MealChoiceDTO? GetOne(string id)
        {
            var mealChoice =
            (
                from _mealChoice in _mealChoices.AsQueryable<MealChoiceDBO>()
                where _mealChoice.Id == id
                select new MealChoiceDTO
                {
                    Id = _mealChoice.Id,
                    GroupId = _mealChoice.GroupId,
                    Name = _mealChoice.Name,
                    Description = _mealChoice.Description,
                    ImageUrl = _mealChoice.ImageUrl,
                    MenuUrl = _mealChoice.MenuUrl,
                }
            ).FirstOrDefault();


            return mealChoice;
        }


        public bool CreateOne(MealChoice mealChoice)
        {
            var mealChoiceDBO = new MealChoiceDBO
            {
                GroupId = mealChoice.GroupId,
                Name = mealChoice.Name,
                Description = mealChoice.Description,
                ImageUrl = mealChoice.ImageUrl,
                MenuUrl = mealChoice.MenuUrl,
            };

            _mealChoices.InsertOne(mealChoiceDBO);
            return true;
        }



        public bool UpdateOne(MealChoiceDTO mealChoice)
        {
            var mealChoiceDBO = new MealChoiceDBO
            {
                Id = mealChoice.Id,
                GroupId = mealChoice.GroupId,
                Name = mealChoice.Name,
                Description = mealChoice.Description,
                ImageUrl = mealChoice.ImageUrl,
                MenuUrl = mealChoice.MenuUrl,
            };

            _mealChoices.ReplaceOne(
                Builders<MealChoiceDBO>.Filter.Eq("Id", mealChoice.Id),
                mealChoiceDBO);

            return true;
        }


        public bool DeleteOne(string id)
        {

            var mealChoice =
            (
                from _mealChoice in _mealChoices.AsQueryable<MealChoiceDBO>()
                where _mealChoice.Id == id
                select _mealChoice
            ).FirstOrDefault();


            if (mealChoice is null || mealChoice.Deleted == true)
            {
                return false;
            }

            mealChoice.Deleted = true;

            _mealChoices.ReplaceOne(
                Builders<MealChoiceDBO>.Filter.Eq("Id", mealChoice.Id),
                mealChoice);


            return true;
        }




        public List<MealChoiceDTO> Scan(MealChoiceScan mealChoiceQuery)
        {
            var mealChoicesQuery =
            (
                from _mealChoice in _mealChoices.AsQueryable<MealChoiceDBO>()
                where _mealChoice.Deleted == false
                select _mealChoice
            )
            .Take(DefaultLimit)
            .Skip(DefaultOffset);

            var modifiedMealQuery = ModifyQuery(mealChoicesQuery, mealChoiceQuery);

            var mealChoices = modifiedMealQuery
            .Select(DboToDto)
            .ToList();

            return mealChoices;
        }


        private IQueryable<MealChoiceDBO> ModifyQuery(IQueryable<MealChoiceDBO> dbQuery, MealChoiceScan mealChoiceQuery)
        {
            if (mealChoiceQuery.GroupId is not null)
            {
                dbQuery = dbQuery.Where(x => x.GroupId == mealChoiceQuery.GroupId);
            }

            if (mealChoiceQuery.Name is not null)
            {
                dbQuery = dbQuery.Where(x => x.Name == mealChoiceQuery.Name);
            }

            if (mealChoiceQuery.Description is not null)
            {
                dbQuery = dbQuery.Where(x => x.Description == mealChoiceQuery.Description);
            }

            if (mealChoiceQuery.ImageUrl is not null)
            {
                dbQuery = dbQuery.Where(x => x.ImageUrl == mealChoiceQuery.ImageUrl);
            }

            if (mealChoiceQuery.MenuUrl is not null)
            {
                dbQuery = dbQuery.Where(x => x.MenuUrl == mealChoiceQuery.MenuUrl);
            }


            return dbQuery;
        }





        public bool Exist(string id)
        {
            var mealChoice =
            (
                from _mealChoice in _mealChoices.AsQueryable<MealChoiceDBO>()
                where _mealChoice.Id == id
                select _mealChoice
            ).FirstOrDefault();

            return mealChoice is not null;
        }



        public int Count()
        {
            var count =
            (
                from _mealChoice in _mealChoices.AsQueryable<MealChoiceDBO>()
                where _mealChoice.Deleted == false
                select _mealChoice
            ).Count();

            return count;
        }



        private MealChoiceDTO DboToDto(MealChoiceDBO mealChoice)
        {
            var mealChoiceDTO = new MealChoiceDTO
            {
                Id = mealChoice.Id,
                GroupId = mealChoice.GroupId,
                Name = mealChoice.Name,
                Description = mealChoice.Description,
                ImageUrl = mealChoice.ImageUrl,
                MenuUrl = mealChoice.MenuUrl,
            };

            return mealChoiceDTO;
        }
















    }
}