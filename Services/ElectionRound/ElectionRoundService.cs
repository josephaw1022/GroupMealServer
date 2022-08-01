using MongoDB.Driver;
using GroupMealApi.AppHubs;
using GroupMealApi.Models;
using Microsoft.Extensions.Logging;


namespace GroupMealApi.Services;
public class ElectionRoundService
{
    private readonly string TableName = "ElectionRound";
    private readonly IMongoCollection<ElectionRoundDBO> _context;
    private readonly ILogger<ElectionRoundService> _logger;


    public ElectionRoundService
    (
        ILogger<ElectionRoundService> logger
    )
    {

        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";
        MongoClient client = new MongoClient(connectionString);

        string databaseName = Environment.GetEnvironmentVariable("DB_NAME") ?? "";
        IMongoDatabase database = client.GetDatabase(databaseName);

        this._context = database.GetCollection<ElectionRoundDBO>(TableName);
        this._logger = logger;
    }


    public ElectionRoundDTO[] GetAll()
    {
        _logger.LogTrace("Get All Election Round Service");
        var electionRounds =
        (
            from electionRound in _context.AsQueryable<ElectionRoundDBO>()
            where electionRound.Deleted == false
            select new ElectionRoundDTO
            {
                ElectionId = electionRound.ElectionId,
                Id = electionRound.Id,
                GroupId = electionRound.GroupId,
                Winner = electionRound.Winner,
                Rounds = electionRound.Rounds
            }
        );
        return electionRounds.ToArray<ElectionRoundDTO>();
    }


    public ElectionRoundDTO? Get(string id)
    {
        var electionRound =
        (
            from _electionRound in _context.AsQueryable<ElectionRoundDBO>()
            where _electionRound.Deleted == false && _electionRound.Id == id
            select new ElectionRoundDTO
            {
                ElectionId = _electionRound.ElectionId,
                Id = _electionRound.Id,
                GroupId = _electionRound.GroupId,
                Winner = _electionRound.Winner,
                Rounds = _electionRound.Rounds
            }
        ).FirstOrDefault();

        return electionRound;
    }



    public bool Create(ElectionRound electionRound)
    {
        var electionRoundDBO = new ElectionRoundDBO
        {
            ElectionId = electionRound.ElectionId,
            GroupId = electionRound.GroupId,
            Winner = electionRound.Winner,
            Rounds = electionRound.Rounds
        };

        _context.InsertOne(electionRoundDBO);
        return true;
    }



    public bool Update(ElectionRoundDTO electionRound)
    {
        var electionRoundDBO = new ElectionRoundDBO
        {
            ElectionId = electionRound.ElectionId,
            GroupId = electionRound.GroupId,
            Winner = electionRound.Winner,
            Rounds = electionRound.Rounds,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ReplaceOne(e => e.Id == electionRound.Id, electionRoundDBO);
        return true;
    }


    public bool Delete(string id)
    {
        var electionRound = Get(id);

        if (electionRound is null)
        {
            return false;
        }

        return true;
    }



}



