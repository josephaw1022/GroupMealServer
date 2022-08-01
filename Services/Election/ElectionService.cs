using GroupMealApi.Models;
using MongoDB.Driver;


namespace GroupMealApi.Services
{
    public class ElectionService
    {

        private readonly string TableName = "Election";
        private readonly IMongoCollection<ElectionDBO> _context;
        private readonly ILogger<ElectionRoundService> _logger;


        public ElectionService
        (
            ILogger<ElectionRoundService> logger
        )
        {

            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";
            MongoClient client = new MongoClient(connectionString);

            string databaseName = Environment.GetEnvironmentVariable("DB_NAME") ?? "";
            IMongoDatabase database = client.GetDatabase(databaseName);

            this._context = database.GetCollection<ElectionDBO>(TableName);
            this._logger = logger;
        }


        private readonly int DefaultOffset = 0;
        private readonly int DefaultLimit = 1000;



        public GetAllServiceResponse<ElectionDTO> GetAll()
        {
            _logger.LogTrace("Get All Election Service");
            try
            {

                var allElections =
                (
                    from _election in _context.AsQueryable<ElectionDBO>()
                    where _election.Deleted == false
                    select new ElectionDTO
                    {
                        Id = _election.Id,
                        GroupId = _election.GroupId,
                        Rounds = _election.Rounds,
                        CurrentRound = _election.CurrentRound,
                        IsActive = _election.IsActive,
                        StoppedHalfway = _election.StoppedHalfway
                    }
                )
                .Take(DefaultLimit)
                .Skip(DefaultOffset);

                return new GetAllServiceResponse<ElectionDTO>
                {
                    Data = allElections.ToList<ElectionDTO>(),
                    Success = true,
                    SuccessMessage = "Successfully retrieved all elections"
                };

            }
            catch (Exception serviceException)
            {
                _logger.LogError(serviceException, "Get All Election Service");

                return new GetAllServiceResponse<ElectionDTO>
                {
                    Success = false,
                    ErrorMessage = "Failed to retrieve all elections"
                };
            }
        }



        public GetAllServiceResponse<ElectionDTO> Scan(ElectionScan electionQuery)
        {
            _logger.LogTrace("Scan Election Service");

            try
            {

                var dbQuery = (
                    from _election in _context.AsQueryable<ElectionDBO>()
                    orderby _election.CreatedAt descending
                    where _election.Deleted == false
                    select new ElectionDTO
                    {
                        Id = _election.Id,
                        GroupId = _election.GroupId,
                        Rounds = _election.Rounds,
                        CurrentRound = _election.CurrentRound,
                        IsActive = _election.IsActive,
                        StoppedHalfway = _election.StoppedHalfway
                    }
                );


                dbQuery = ModifyQuery(dbQuery, electionQuery);

                Console.WriteLine(dbQuery.ToString());

                var dbQueryResponse = dbQuery.ToList<ElectionDTO>();

                return new GetAllServiceResponse<ElectionDTO>
                {
                    Data = dbQueryResponse,
                    Success = true,
                    SuccessMessage = "Successfully retrieved all elections"
                };

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Scan Election Service");

                return new GetAllServiceResponse<ElectionDTO>
                {
                    Success = false,
                    ErrorMessage = exception.Message,
                };
            }
        }



        private IQueryable<ElectionDTO> ModifyQuery(IQueryable<ElectionDTO> query, ElectionScan electionQuery)
        {
            if (electionQuery.GroupId is not null && !string.IsNullOrWhiteSpace(electionQuery.GroupId))
            {
                query = query.Where(e => e.GroupId == electionQuery.GroupId);
            }


            if (electionQuery.Rounds is not null)
            {
                query = query.Where(e => e.Rounds == electionQuery.Rounds);
            }


            if (electionQuery.CurrentRound is not null)
            {
                query = query.Where(e => e.CurrentRound == electionQuery.CurrentRound);
            }


            if (electionQuery.IsActive is not null)
            {
                query = query.Where(e => e.IsActive == electionQuery.IsActive);
            }

            if (electionQuery.StoppedHalfway is not null)
            {
                query = query.Where(e => e.StoppedHalfway == electionQuery.StoppedHalfway);
            }


            return query;
        }


        /// <summary>
        /// Get an election by id 
        /// </summary>
        /// <param name="id">
        /// The id of the election to get
        /// </param>
        /// <returns>
        /// The election with the given id
        /// </returns>
        public GetOneServiceResponse<ElectionDTO> Get(string id)
        {
            try
            {

                var getOne = _context
                    .Find<ElectionDBO>(e => e.Id == id)
                    .FirstOrDefault();

                return new GetOneServiceResponse<ElectionDTO>
                {
                    Data = new ElectionDTO
                    {
                        Id = getOne.Id,
                        GroupId = getOne.GroupId,
                        Rounds = getOne.Rounds,
                        CurrentRound = getOne.CurrentRound,
                        IsActive = getOne.IsActive,
                        StoppedHalfway = getOne.StoppedHalfway
                    },
                    Success = true,
                    SuccessMessage = "Successfully retrieved election"
                };

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Get Election Service");

                return new GetOneServiceResponse<ElectionDTO>
                {
                    Success = false,
                    ErrorMessage = exception.Message,

                };
            }
        }


        /// <summary>
        /// Create a new election
        /// </summary>
        /// <param name="election">
        ///  The election to create
        /// </param>
        /// <returns>
        ///  The created election
        /// </returns>
        public CreateOneServiceResponse<Election> Create(Election election)
        {
            try
            {

                // Create a new election
                var newOne = new ElectionDBO
                {
                    GroupId = election.GroupId,
                    Rounds = election.Rounds,
                    CurrentRound = election.CurrentRound,
                    IsActive = election.IsActive,
                    StoppedHalfway = election.StoppedHalfway
                };

                // Insert the new election
                _context.InsertOne(newOne);

                // Return the created election
                return new CreateOneServiceResponse<Election>
                {
                    Data = election,
                    Success = true,
                    SuccessMessage = "Successfully created election"
                };

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Create Election Service");

                return new CreateOneServiceResponse<Election>
                {
                    Success = false,
                    ErrorMessage = exception.Message,
                };
            }
        }



        public UpdateOneServiceResponse<ElectionDTO> Update(ElectionDTO election)
        {
            try
            {
                // get the election to update 
                var updateOne = _context
                    .Find<ElectionDBO>(e => e.Id == election.Id)
                    .FirstOrDefault();

                // update the election values
                updateOne.GroupId = election.GroupId;
                updateOne.Rounds = election.Rounds;
                updateOne.CurrentRound = election.CurrentRound;
                updateOne.IsActive = election.IsActive;
                updateOne.StoppedHalfway = election.StoppedHalfway;
                updateOne.UpdatedAt = DateTime.UtcNow;

                // update the election in the database
                _context.ReplaceOne(e => e.Id == election.Id, updateOne);

                // return the updated election
                return new UpdateOneServiceResponse<ElectionDTO>
                {
                    Data = election,
                    Success = true,
                    SuccessMessage = "Successfully updated election"
                };

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Update Election Service");

                return new UpdateOneServiceResponse<ElectionDTO>
                {
                    Success = false,
                    ErrorMessage = exception.Message,
                };
            }
        }


        public DeleteOneServiceResponse<ElectionDTO> Delete(string id)
        {
            try
            {
                // get the election to delete 
                var deleteOne = _context
                    .Find<ElectionDBO>(e => e.Id == id)
                    .FirstOrDefault();

                // delete the election from the database
                _context.DeleteOne(e => e.Id == id);

                // return the deleted election
                return new DeleteOneServiceResponse<ElectionDTO>
                {
                    Data = new ElectionDTO
                    {
                        Id = deleteOne.Id,
                        GroupId = deleteOne.GroupId,
                        Rounds = deleteOne.Rounds,
                        CurrentRound = deleteOne.CurrentRound,
                        IsActive = deleteOne.IsActive,
                        StoppedHalfway = deleteOne.StoppedHalfway
                    },
                    Success = true,
                    SuccessMessage = "Successfully deleted election"
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Delete Election Service");

                return new DeleteOneServiceResponse<ElectionDTO>
                {
                    Success = false,
                    ErrorMessage = exception.Message,
                };
            }
        }









    }
}