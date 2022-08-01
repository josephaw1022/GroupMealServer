using GroupMealApi.Models;
using MongoDB.Driver;



namespace GroupMealApi.Services;

public class AccountService
{

    private readonly string TableName = "Account";
    private readonly IMongoCollection<AccountDBO> _context;
    private readonly ILogger<AccountService> _logger;


    /// <summary>
    /// The service constructor that takes in the database settings
    /// </summary>
    ///  <param name="logger">
    ///  The logger to use
    /// </param>
    public AccountService
    (
        ILogger<AccountService> logger
    )
    {

        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";
        var client = new MongoClient(connectionString);

        var databaseName = Environment.GetEnvironmentVariable("DB_NAME") ?? "";
        var database = client.GetDatabase(databaseName);

        _context = database.GetCollection<AccountDBO>(TableName);
        _logger = logger;
    }


    /// <summary>
    /// Gets the total number of accounts in the database 
    /// </summary>
    /// <returns>
    ///  The total number of accounts in the database
    /// </returns>
    public int CountAccounts()
    {
        var count =
        (
            from AccountDBO _account in _context.AsQueryable<AccountDBO>()
            where _account.Deleted == false
            select _account
        ).Count();

        return count;
    }


    /// <summary>
    /// Converts a AccountDBO to a AccountDTO
    /// </summary>
    /// <param name="dbo">
    /// The AccountDBO to convert
    /// </param>
    /// <returns>
    /// The AccountDTO
    /// </returns>
    public AccountDTO DboToDto(AccountDBO dbo) => new AccountDTO
    {
        Id = dbo.Id,
        FirstName = dbo.FirstName,
        LastName = dbo.LastName,
        Email = dbo.Email,
        PhoneNumber = dbo.PhoneNumber,
        GroupId = dbo.GroupId,
    };


    /// <summary>
    /// Converts a AccountDTO to a AccountDBO 
    /// </summary>
    /// <param name="dto">
    /// The AccountDTO to convert
    /// </param>
    /// <returns>
    /// The AccountDBO
    /// </returns>
    public AccountDBO DtoToDbo(AccountDTO dto)
    {
        return new AccountDBO
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            GroupId = dto.GroupId,
        };
    }




    public bool CreateAccount(Account account)
    {

        AccountDBO newAccount = new AccountDBO()
        {
            FirstName = account.FirstName,
            LastName = account.LastName,
            Email = account.Email,
            PhoneNumber = account.PhoneNumber,
            GroupId = account.GroupId ?? null,
        };

        _context.InsertOne(newAccount);

        return true;
    }


    /// <summary>
    /// Delete a account from the database
    /// </summary>
    /// <param name="id">
    /// The id of the account to delete
    /// </param>
    /// <returns>
    /// True if the account was deleted, false if not
    /// </returns>
    public bool DeleteAccount(string id)
    {
        // check to see if account exists
        bool exist = AccountExist(id);

        if (!exist)
        {
            return false;
        }

        // delete account

        var account = _context.Find(x => x.Id == id).FirstOrDefault();

        account.Deleted = true;

        _context.ReplaceOne(x => x.Id == id, account);

        return true;
    }



    /// <summary>
    ///  The default offset for pagination
    /// </summary>
    private readonly int DefaultOffset = 0;


    /// <summary>
    /// The default number of accounts to return in a request 
    /// </summary>
    private readonly int DefaultLimit = 1000;


    /// <summary>
    /// Get all accounts from the database
    /// </summary>
    /// <returns>
    /// A list of accounts
    /// </returns>
    public AccountDTO[] GetAllAccounts()
    {

        this._logger.LogInformation($"Getting all accounts in the service level");

        var accountQuery =
        (
            from _account in _context.AsQueryable<AccountDBO>()
            where _account.Deleted == false
            orderby _account.CreatedAt descending
            select _account
        )
        .Take(DefaultLimit)
        .Skip(DefaultOffset);


        var dbResponse = accountQuery.ToArray();

        this._logger.LogInformation($"Got {dbResponse.Length} accounts");

        return dbResponse
        .Select(x => DboToDto(x))
        .ToArray();
    }


    /// <summary>
    /// Get all accounts in the database
    /// </summary>
    /// <param name="offset">
    ///  The number of accounts to skip
    /// </param>
    /// <param name="limit">
    ///  The number of accounts to return
    /// </param>
    /// <returns>
    ///  The accounts in the database
    /// </returns>
    public AccountDTO[] GetAllAccounts(int offset, int limit)
    {

        // If the limit is less than 0 or greater than default, set it to the default limit
        if (limit < 0 || limit > DefaultLimit)
        {
            limit = DefaultLimit;
        }


        // If the offset is less than 0, set it to 0
        if (offset < 0)
        {
            offset = DefaultOffset;
        }


        // If the offset is greater than the total number of accounts, then set the offset to 0
        if (offset > CountAccounts())
        {
            offset = DefaultOffset;
        }



        // get the accounts
        var accountQuery =
        (
            from AccountDBO _account in _context.AsQueryable<AccountDBO>()
            orderby _account.CreatedAt descending
            where _account.Deleted == false
            select _account
        )
        .Take(limit)
        .Skip(offset);


        return accountQuery.Select(DboToDto).ToArray();

    }


    /// <summary>
    /// Get a account from the database by id
    /// </summary>
    /// <param name="id">
    /// The id of the account to get
    /// </param>
    /// <returns>
    /// The account or null if not found
    /// </returns>
    public AccountDTO? GetOneAccount(string id)
    {
        var account = _context.Find(x => x.Id == id).FirstOrDefault();

        return DboToDto(account);
    }



    /// <summary>
    /// Filter out accounts from the database based on the query
    /// </summary>
    /// <param name="accountQuery">
    ///  The query to use
    /// </param>
    /// <returns>
    ///  A list of accounts
    /// </returns>
    public AccountDTO[] ScanAccounts(AccountScan accountQuery)
    {

        var dbQuery = (
            from _account in _context.AsQueryable<AccountDBO>()
            orderby _account.CreatedAt descending
            where _account.Deleted == false
            select new AccountDTO()
            {
                Id = _account.Id,
                FirstName = _account.FirstName,
                LastName = _account.LastName,
                Email = _account.Email,
                PhoneNumber = _account.PhoneNumber,
                GroupId = _account.GroupId
            }
        );

        dbQuery = ModifyGroupQuery(dbQuery, accountQuery);

        Console.WriteLine(dbQuery.ToString());

        return dbQuery.ToArray();
    }


    /// <summary>
    /// Private helper method to modify the group query based on the account query
    /// </summary>
    /// <param name="dbQuery">
    /// The query to modify
    /// </param>
    /// <param name="query">
    /// The account query to use to modify the query
    /// </param>
    /// <returns>
    /// The modified query
    /// </returns>
    private IQueryable<AccountDTO> ModifyGroupQuery(IQueryable<AccountDTO> dbQuery, AccountScan query)
    {

        if (query.FirstName is not null)
        {
            dbQuery = dbQuery.Where(_account => _account.FirstName.Contains(query.FirstName));
        }

        if (query.LastName is not null)
        {
            dbQuery = dbQuery.Where(_account => _account.LastName.Contains(query.LastName));
        }

        if (query.Email is not null)
        {
            dbQuery = dbQuery.Where(_account => _account.Email.Contains(query.Email));
        }

        if (query.PhoneNumber is not null)
        {
            dbQuery = dbQuery.Where(_account => _account.PhoneNumber.Contains(query.PhoneNumber));
        }


        if (query.GroupId is not null)
        {
            dbQuery = dbQuery
            .Where(
                _account => (_account.GroupId ?? "")
                .ToString()
                .Contains(query.GroupId.ToString() ?? "")
            );
        }


        return dbQuery;
    }



    /// <summary>
    ///  Update a account in the database
    /// </summary>
    /// <param name="account">
    ///  The account to update
    /// </param>
    /// <returns>
    ///  Returns true if the account was updated, false otherwise
    /// </returns>
    public bool UpdateOneAccount(AccountDTO account)
    {

        var updatedAccount =
        (
            from _account in _context.AsQueryable()

            where _account.Id == account.Id && _account.Deleted == false

            select _account
        )
        .FirstOrDefault();


        // if the account doesn't exist, return false
        if (updatedAccount is null)
        {
            return false;
        }

        _logger.LogInformation("updating account is not null");

        // update the account
        updatedAccount.FirstName = account.FirstName;
        updatedAccount.LastName = account.LastName;
        updatedAccount.Email = account.Email;
        updatedAccount.PhoneNumber = account.PhoneNumber;
        updatedAccount.UpdatedAt = DateTime.Now;
        updatedAccount.GroupId = account.GroupId;

        _logger.LogInformation("Set updated account attributes");


        // save the account
        _context.ReplaceOne(account => account.Id == updatedAccount.Id, updatedAccount);

        _logger.LogInformation("account updated");

        return true;
    }



    /// <summary>
    /// Check to see if a account exists in the database
    /// </summary>
    /// <param name="id">
    ///  The id of the account to check for
    /// </param>
    /// <returns>
    ///  True if the account exists, false otherwise
    /// </returns>
    public bool AccountExist(string id)
    {
        // check to see if an account with the id exists
        var accountCount =
        (
            from _account in _context.AsQueryable<AccountDBO>()
            where _account.Deleted == false
            where _account.Id == id
            orderby _account.CreatedAt descending
            select _account
        )
        .Count();


        return accountCount > 0;
    }


}