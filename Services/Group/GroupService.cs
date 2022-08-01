using GroupMealApi.Models;
using MongoDB.Driver;

namespace GroupMealApi.Services;


public class GroupService
{

    private readonly IMongoCollection<GroupDBO> _groups;
    private readonly IMongoCollection<AccountDBO> _accounts;
    private readonly IMongoCollection<MealChoiceDBO> _mealChoices;

    private readonly string TableName = "Group";
    private readonly ILogger<GroupService> _logger;

    private readonly int DefaultOffset = 0;
    private readonly int DefaultLimit = 1000;



    /// <summary>
    /// The service constructor that takes in the database settings
    /// </summary>
    /// <param name="logger">
    /// The logger to use
    /// </param>
    public GroupService
    (
        ILogger<GroupService> logger
    )
    {

        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";
        MongoClient client = new MongoClient(connectionString);
        string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "";
        IMongoDatabase database = client.GetDatabase(dbName);

        // define the collections
        _groups = database.GetCollection<GroupDBO>(TableName);
        _mealChoices = database.GetCollection<MealChoiceDBO>("MealChoice");
        _accounts = database.GetCollection<AccountDBO>("Account");

        // set the logger
        this._logger = logger;
    }




    /// <summary>
    /// Get all groups
    /// </summary>
    /// <param name="limit">
    ///   The number of groups to return
    /// </param>
    /// <param name="offset">
    ///  The number of groups to skip
    /// </param>
    /// <returns>
    ///  A list of groups
    /// </returns>
    public IEnumerable<GroupDTO> GetAll(int limit, int offset)
    {

        if (limit < 0 || limit > DefaultLimit)
        {
            limit = DefaultLimit;
        }

        if (offset < Count())
        {
            offset = DefaultOffset;
        }

        IQueryable<GroupDTO>? allGroups =
        (
            from Group in _groups.AsQueryable<GroupDBO>()
            where Group.Deleted == false
            select new GroupDTO
            {
                Id = Group.Id,
                Name = Group.Name,
                Description = Group.Description,
                CreatorId = Group.CreatorId,
            }
         )
         .Skip(offset)
         .Take(limit);


        return allGroups.ToArray();
    }





    /// <summary>
    /// Get a group by id
    /// </summary>
    /// <param name="id">
    /// The id of the group to get.
    /// </param>
    /// <returns>
    /// The group with the specified id.
    /// </returns>
    public GroupAndMoreDTO? Get(string id)
    {
        return (
            _groups.Aggregate()
            .Lookup("Account", "GroupId", "Id", @as: "GroupAccounts")
            .Lookup("MealChoice", "GroupId", "Id", @as: "GroupMealChoices")
            .As<GroupAndMoreDBO>()
            .ToEnumerable()
            .Select
            (
                _group => new GroupAndMoreDTO
                {
                    Id = _group.Id,
                    Name = _group.Name,
                    Description = _group.Description,
                    CreatorId = _group.CreatorId,
                    GroupAccounts = _group.GroupAccounts.Select(ga =>
                        new AccountJoinGroup
                        {
                            FirstName = ga.FirstName,
                            LastName = ga.LastName,
                            Email = ga.Email,
                            PhoneNumber = ga.PhoneNumber,
                            GroupId = ga.GroupId ?? "",
                        })
                        .Where(_ga => _ga.GroupId == _group.Id),

                    GroupMealChoices = _group.GroupMealChoices.Select(mc =>
                        new MealChoiceJoinGroup
                        {
                            Name = mc.Name,
                            Description = mc.Description,
                            ImageUrl = mc.ImageUrl,
                            MenuUrl = mc.MenuUrl,
                            GroupId = mc.GroupId,
                        })
                        .Where(_mc => _mc.GroupId == _group.Id),
                }
            )
            .FirstOrDefault(_group => _group.Id == id)
        );
    }



    /// <summary>
    /// Gets all of the groups and the users that are in them.
    /// </summary>
    /// <returns>
    ///  A list of groups with the users that are in them.
    /// </returns>
    public GroupAndMoreDTO[] GetAll()
    {


        var groupWithAccounts = _groups
            .Aggregate()
            .Lookup("Account", "GroupId", "Id", @as: "GroupAccounts")
            .Lookup("MealChoice", "GroupId", "Id", @as: "GroupMealChoices")
            .As<GroupAndMoreDBO>()
            .ToEnumerable()
            .Select
            (
            _group => new GroupAndMoreDTO
            {
                Id = _group.Id,
                Name = _group.Name,
                Description = _group.Description,
                CreatorId = _group.CreatorId,
                GroupAccounts = _group.GroupAccounts.Select(ga =>
                    new AccountJoinGroup
                    {
                        Id = ga.Id,
                        FirstName = ga.FirstName,
                        LastName = ga.LastName,
                        Email = ga.Email,
                        PhoneNumber = ga.PhoneNumber,
                        GroupId = ga.GroupId ?? "",
                    })
                    .Where(_ga => _ga.GroupId == _group.Id),

                GroupMealChoices = _group.GroupMealChoices.Select(mc =>
                    new MealChoiceJoinGroup
                    {
                        Id = mc.Id,
                        Name = mc.Name,
                        Description = mc.Description,
                        ImageUrl = mc.ImageUrl,
                        MenuUrl = mc.MenuUrl,
                        GroupId = mc.GroupId,
                    })
                    .Where(_mc => _mc.GroupId == _group.Id),
            }
        )
            .Take(DefaultLimit)
            .Skip(DefaultOffset);

        _logger.LogInformation("Get Groups With Account Query {query}", groupWithAccounts);

        return groupWithAccounts.ToArray();

    }



    /// <summary>
    /// Create a new  group
    /// </summary>
    /// <param name="group">
    /// The group to create.
    /// </param>
    /// <returns>
    /// true if the group was created, false otherwise.
    /// </returns>
    public bool Create(Group group)
    {
        GroupDBO newGroup = new GroupDBO
        {
            Name = group.Name,
            Description = group.Description,
            CreatorId = group.CreatorId,
        };

        this._groups.InsertOne(newGroup);

        return true;
    }


    /// <summary>
    /// Update a group by id
    /// </summary>
    /// <param name="id">
    /// The id of the group to update
    /// </param>
    /// <param name="updatedGroup">
    /// The group object that contains the updated values
    /// </param>
    /// <returns>
    /// True if the group was updated, false if the group was not found
    /// </returns>
    public bool Update(string id, GroupDTO updatedGroup)
    {

        // Get the group by id
        var group =
        (
            from _group in _groups.AsQueryable<GroupDBO>()
            where _group.Id == id && _group.Deleted == false
            select _group
        )
        .FirstOrDefault();



        // If the group does not exist, return NotFound
        if (group is null)
        {
            return false;
        }

        // Update the group
        group.Name = updatedGroup.Name ?? group.Name;
        group.Description = updatedGroup.Description ?? group.Description;
        group.UpdatedAt = DateTime.UtcNow;

        // Update the group in the database
        this._groups.ReplaceOne(g => g.Id == id, group);

        // Return the updated group
        return true;
    }


    /// <summary>
    /// Delete a group by id
    /// </summary>
    /// <param name="Id">
    /// The id of the group to delete
    /// </param>
    /// <returns>
    ///  return true if the group was deleted, false if the group does not exist
    /// </returns>
    public bool Remove(string Id)
    {
        // Get the group by id
        GroupDBO? group =
        (
            from _group in _groups.AsQueryable<GroupDBO>()
            where _group.Id == Id && _group.Deleted == false
            select _group

        )
        .FirstOrDefault();

        // If the group does not exist, return NotFound
        if (group is null)
        {
            return false;
        }

        // Delete the group
        group.Deleted = true;
        group.UpdatedAt = DateTime.UtcNow;

        // Update the group in the database
        _groups.ReplaceOne(g => g.Id == Id, group);

        // Return the status of the delete
        return true;
    }



    /// <summary>
    /// Scans the database for all groups that have the given account as a member
    /// </summary>
    /// <param name="query">
    ///  the query to search for
    /// </param>
    /// <returns>
    /// an array of groups that match the query 
    /// </returns>
    public GroupDTO[] Scan(GroupScan query)
    {

        // destructure the query
        string? name = query.Name ?? "";
        string? description = query.Description ?? "";

        // create the query
        IQueryable<Group>? groups =
        (
            from Group in _groups.AsQueryable<GroupDBO>()
            where Group.Deleted == false
            select Group
        );

        // If name is in query parameters, add filter to db query
        if (!string.IsNullOrEmpty(name) || !string.IsNullOrWhiteSpace(name))
        {
            groups = groups.Where(g => g.Name.Contains(name) || false);
        }

        // if description is in query parameters, add filter to db query 
        if (!string.IsNullOrEmpty(description) || !string.IsNullOrWhiteSpace(description))
        {
            groups = groups.Where(g => g.Description.Contains(description));
        }

        // query the database for all groups that match the query
        GroupDTO[] response = groups.Select(g => new GroupDTO
        {
            Name = g.Name,
            Description = g.Description,
            CreatorId = g.CreatorId,
        }).ToArray();

        return response;
    }


    /// <summary>
    /// Truncates the group collection
    /// </summary>
    public void Truncate()
    {
        DeleteResult? deleteResponse = this._groups.DeleteMany(_ => true);
        _logger.LogInformation("Truncated group collection {deleteResponse}", deleteResponse);
    }



    /// <summary>
    /// Checks if a group exists
    /// </summary>
    /// <param name="id">
    /// The id of the group to check
    /// </param>
    /// <returns>
    ///  true if the group exists, false otherwise
    /// </returns>
    public bool Exists(string id)
    {
        return this._groups.AsQueryable<GroupDBO>().Any(g => g.Id == id);
    }



    /// <summary>
    /// Get the number of groups in the database
    /// </summary>
    /// <returns></returns>
    public int Count()
    {
        return
        (
            from Group in _groups.AsQueryable<GroupDBO>()
            where Group.Deleted == false
            select Group
        )
        .Count();
    }


}

