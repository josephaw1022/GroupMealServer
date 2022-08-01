using Microsoft.AspNetCore.Mvc;
using GroupMealApi.Models;
using GroupMealApi.Services;




namespace GroupMealApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AccountService _service;
        private readonly GroupService _groupService;

        public AccountController(AccountService accountService, GroupService groupService, ILogger<AccountController> logger)
        {
            this._service = accountService;
            this._logger = logger;
            this._groupService = groupService;
        }

        /// <summary>
        /// Get all accounts 
        /// </summary>
        /// <returns>
        ///  200: OK - Returns a list of accounts
        /// </returns>
        [HttpGet]
        public ActionResult<GetAllResponse<AccountDTO>?> GetAll()
        {
            var startTime = DateTime.UtcNow;
            this.ControllerLoggerStart(CrudRequest.GETALL);

            try
            {

                AccountDTO[] allAccounts = _service.GetAllAccounts();

                ControllerLoggerEnd(startTime);

                return Ok(new GetAllResponse<AccountDTO>
                {
                    dataset = allAccounts,
                    total = allAccounts.Length
                });

            }
            catch (Exception exception)
            {
                ControllerLoggerEnd(startTime);
                return this.HandleError(exception);
            }
        }

        /// <summary>
        /// Get an account by id 
        /// </summary>
        /// <param name="id">
        ///  The id of the account to get
        /// </param>
        /// <returns>
        ///  200: OK - Returns an account
        /// </returns>
        [HttpGet("{id:length(24)}")]
        public ActionResult<GetOneResponse<AccountDTO>> Get(string id)
        {
            var startTime = DateTime.UtcNow;
            ControllerLoggerStart(CrudRequest.GETONE);
            try
            {
                AccountDTO? getAccountResponse = _service.GetOneAccount(id);


                if (getAccountResponse is null)
                {
                    throw new Exception("Account not found");
                }

                ControllerLoggerEnd(startTime);

                return Ok(new GetOneResponse<AccountDTO>
                {
                    data = getAccountResponse,
                    success = true,
                    message = "Account found"
                });

            }
            catch (Exception exception)
            {
                ControllerLoggerEnd(startTime);
                return this.HandleError(exception);
            }
        }

        /// <summary>
        /// Create an account 
        /// </summary>
        /// <param name="account">
        ///  The account to create
        /// </param>
        /// <returns>
        ///  200: OK - Returns the created account
        /// </returns>
        [HttpPost]
        public ActionResult<CreateOneResponse<Account>> Create(Account account)
        {
            var StartTime = DateTime.UtcNow;
            this.ControllerLoggerStart(CrudRequest.CREATE);
            try
            {
                // Make sure the account email is unique
                var emailAlreadyUsed = _service.ScanAccounts(new AccountScan()
                {
                    Email = account.Email,
                });

                // If the email is already used, throw an error
                if (emailAlreadyUsed.Length > 0)
                {
                    throw new Exception("Email already in use");
                }

                // Validate whether the group id is valid
                if (account.GroupId is not null)
                {
                    // Make sure the group id exists 
                    var groupsWithKey = _groupService.Scan(new GroupScan
                    {
                        Key = account.GroupId
                    });

                    if (groupsWithKey.Length == 0)
                    {
                        throw new Exception("Group does not exist");
                    }
                }

                // Create the account
                bool createAccountResponse = _service.CreateAccount(account);

                // If the account was not created, throw an error
                if (!createAccountResponse)
                {
                    throw new Exception("Account creation failed");
                }

                this.ControllerLoggerEnd(StartTime);

                // Return the created account
                return Ok(new CreateOneResponse<Account>
                {
                    data = account,
                    success = true,
                    message = "Account created"
                });

            }
            catch (Exception exception)
            {
                ControllerLoggerEnd(StartTime);
                if (exception.Message == "Email already in use")
                {
                    return Conflict(exception.Message);
                }

                return this.HandleError(exception);
            }
        }

        /// <summary>
        /// Update an account 
        /// </summary>
        /// <param name="id">
        ///  The id of the account to update
        /// </param>
        /// <param name="account">
        ///  The account to update
        /// </param>
        /// <returns>
        ///  200: OK - Returns the updated account
        /// </returns>
        [HttpPut("{id:length(24)}")]
        public ActionResult<UpdateOneResponse<AccountDTO>> Update(string id, AccountDTO account)
        {
            var startTime = DateTime.UtcNow;
            ControllerLoggerStart(CrudRequest.UPDATE);
            try
            {

                if (account.Id != id)
                {
                    throw new Exception("Id in request body does not match id in url");
                }

                var getOneAccount = _service.GetOneAccount(id);

                if (getOneAccount is null)
                {
                    throw new Exception("Account not found");
                }

                if (getOneAccount.Email != account.Email)
                {
                    throw new Exception("Cannot Update Email");
                }

                if
                (
                    getOneAccount.GroupId is not null &&
                    account.GroupId is not null &&
                    getOneAccount.GroupId != account.GroupId
                )
                {
                    throw new Exception("Cannot Update Group");
                }

                bool updateAccountResponse = _service.UpdateOneAccount(account);

                if (!updateAccountResponse)
                {
                    throw new Exception("Account not updated");
                }

                ControllerLoggerEnd(startTime);

                return Ok(new UpdateOneResponse<AccountDTO>
                {
                    id = id,
                    data = account,
                    success = true,
                    message = "Account updated"
                });

            }
            catch (Exception exception)
            {
                return this.HandleError(exception);
            }
        }

        [HttpDelete("{id:length(24)}")]
        public ActionResult<DeleteOneResponse<AccountDTO>> Delete(string id)
        {
            var startTime = DateTime.UtcNow;
            ControllerLoggerStart(CrudRequest.DELETE);

            try
            {

                if (_service.GetOneAccount(id) is null)
                {
                    throw new Exception("Account not found");
                }

                bool deleteAccountResponse = _service.DeleteAccount(id);

                if (!deleteAccountResponse)
                {
                    throw new Exception("Account not deleted");
                }

                var deleteOneResponse = new DeleteOneResponse<AccountDTO>
                {
                    id = id,
                    success = true,
                    message = "Account deleted"
                };

                ControllerLoggerEnd(startTime);
                return Ok(deleteOneResponse);

            }
            catch (Exception exception)
            {
                ControllerLoggerEnd(startTime);
                return this.HandleError(exception);
            }
        }



        /// <summary>
        /// Handles the error.
        /// </summary>
        /// <param name="exception">The exception.</param>  
        /// <returns></returns>
        private ActionResult HandleError(Exception exception)
        {
            _logger.LogError(exception.ToString());
            return StatusCode(500, exception.Message);
        }



        private readonly string _controllerEntity = "account";


        /// <summary>
        /// Logs the request method and time to the console.
        /// </summary>
        /// <param name="request">
        ///  The request method
        /// </param>
        private void ControllerLoggerStart(CrudRequest request)
        {

            var logString = "";

            switch (request)
            {
                case CrudRequest.GETONE:
                    logString = $"{_controllerEntity} GETONE";
                    break;
                case CrudRequest.GETALL:
                    logString = $"{_controllerEntity} GETALL";
                    break;
                case CrudRequest.CREATE:
                    logString = $"{_controllerEntity} CREATE";
                    break;
                case CrudRequest.UPDATE:
                    logString = $"{_controllerEntity} UPDATE";
                    break;
                case CrudRequest.DELETE:
                    logString = $"{_controllerEntity} DELETE";
                    break;
            }

            _logger.LogInformation("{logString} {DateTime}", logString, DateTime.UtcNow);

        }


        /// <summary>
        /// Logs the time taken to the console. 
        /// </summary>
        /// <param name="startTime">
        ///  The start time.
        /// </param>
        private void ControllerLoggerEnd(DateTime startTime)
        {
            var timeTaken = DateTime.UtcNow.Millisecond - startTime.Millisecond;
            _logger.LogInformation("{timeTaken} milliseconds", timeTaken);
        }


    }
}