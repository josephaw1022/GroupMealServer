using Microsoft.AspNetCore.Mvc;
using GroupMealApi.Services;
using GroupMealApi.Models;

namespace GroupMealApi.Controllers
{

    [ApiController]
    [Route("api/electionround")]
    public class ElectionRoundController : Controller
    {

        private readonly ILogger<ElectionRoundController> _logger;
        private readonly ElectionRoundService _electionRoundService;
        private readonly GroupService _groupService;


        public ElectionRoundController
        (
            ILogger<ElectionRoundController> logger,
            ElectionRoundService electionRoundService,
            GroupService groupService
        )
        {
            _logger = logger;
            _electionRoundService = electionRoundService;
            _groupService = groupService;
        }

        /// <summary>
        /// Get all election rounds 
        /// </summary>
        /// <returns>
        ///   200: OK - Returns a list of election rounds
        /// </returns>
        [HttpGet]
        public ActionResult<GetAllResponse<ElectionRoundDTO>> GetAll()
        {
            var StartTime = DateTime.UtcNow;
            ControllerLoggerStart(CrudRequest.GETALL);
            try
            {
                var getAllResponse = _electionRoundService.GetAll();

                return Ok(new GetAllResponse<ElectionRoundDTO>
                {
                    dataset = getAllResponse,
                    total = getAllResponse.Length
                });
                
            }
            catch (Exception exception)
            {
                ControllerLoggerEnd(StartTime);
                return this.HandleError(exception);
            }
        }

        /// <summary>
        /// Get an election round by id 
        /// </summary>
        /// <param name="id">
        ///  The id of the election round to get
        /// </param>
        /// <returns>
        ///  200: OK - Returns the election round with the given id
        /// </returns>
        [HttpGet("{id:length(24)}")]
        public ActionResult<GetOneResponse<ElectionRoundDTO>> Get(string id)
        {
            var StartTime = DateTime.UtcNow;
            ControllerLoggerStart(CrudRequest.GETONE);
            try
            {
                var getOneResponse = _electionRoundService.Get(id);

                if (getOneResponse is null)
                {
                    throw new Exception("ElectionRound not found");
                }

                return Ok(new GetOneResponse<ElectionRoundDTO>
                {
                    data = getOneResponse,
                    success = true,
                    message = "ElectionRound found"
                });
            }
            catch (Exception exception)
            {
                ControllerLoggerEnd(StartTime);
                return this.HandleError(exception);
            }
        }


        /// <summary>
        /// Create an election round 
        /// </summary>
        /// <param name="electionRound">
        ///  The election round to create
        /// </param>
        /// <returns>
        ///  200: OK - Returns the created election round
        /// </returns>
        [HttpPost]
        public ActionResult<CreateOneResponse<ElectionRound>> CreateOne(ElectionRound electionRound)
        {
            var startTime = DateTime.UtcNow;
            ControllerLoggerStart(CrudRequest.CREATE);

            try
            {

                bool groupIdFormattedCorrectly = electionRound.GroupId.Length == 24;

                if (!groupIdFormattedCorrectly)
                {
                    throw new Exception("GroupId is not formatted correctly");
                }

                bool groupExists = _groupService.Exists(electionRound.GroupId);

                if (!groupExists)
                {
                    throw new Exception("Group does not exist");
                }


                var createOneResponse = _electionRoundService.Create(electionRound);

                if (createOneResponse)
                {
                    throw new Exception("ElectionRound not created");
                }

                return Ok(new CreateOneResponse<ElectionRound>
                {
                    data = electionRound,
                    success = true,
                    message = "ElectionRound created"
                });
            }
            catch (Exception exception)
            {
                ControllerLoggerEnd(startTime);
                return this.HandleError(exception);
            }
        }


        /// <summary>
        ///  Update an election round by id
        /// </summary>
        /// <param name="electionRound">
        ///  The election round to update
        /// </param>
        /// <returns>
        ///  200: OK - Returns the updated election round
        /// </returns>
        [HttpPut("{id:length(24)}")]
        public ActionResult<UpdateOneResponse<ElectionRoundDTO>> UpdateOne(ElectionRoundDTO electionRound)
        {
            var startTime = DateTime.UtcNow;
            ControllerLoggerStart(CrudRequest.UPDATE);
            try
            {

                var getOneResponse = _electionRoundService.Get(electionRound.Id);

                if (getOneResponse is null)
                {
                    throw new Exception("ElectionRound not found");
                }

                var updateOneResponse = _electionRoundService.Update(electionRound);

                if (!updateOneResponse)
                {
                    throw new Exception("ElectionRound not updated");
                }


                return Ok(new UpdateOneResponse<ElectionRoundDTO>
                {
                    data = electionRound,
                    success = true,
                    message = "ElectionRound updated"
                });

            }
            catch (Exception exception)
            {
                ControllerLoggerEnd(startTime);
                return this.HandleError(exception);
            }
        }


        /// <summary>
        /// Delete an election round by id 
        /// </summary>
        /// <param name="id">
        ///  The id of the election round to delete
        /// </param>
        /// <returns>
        ///  200: OK - Returns the deleted election round
        /// </returns>
        [HttpDelete("{id:length(24)}")]
        public ActionResult<DeleteOneResponse<ElectionRoundDTO>> DeleteOne(string id)
        {

            var startTime = DateTime.UtcNow;
            ControllerLoggerStart(CrudRequest.DELETE);
            try
            {
                var deleteOneResponse = _electionRoundService.Delete(id);

                if (deleteOneResponse)
                {
                    throw new Exception("ElectionRound not deleted");
                }

                return Ok(new DeleteOneResponse<ElectionRoundDTO>
                {
                    success = true,
                    message = "ElectionRound deleted"
                });

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
        /// <returns>
        ///  500: Internal Server Error - Returns the error message
        /// </returns>
        private ActionResult HandleError(Exception exception)
        {
            _logger.LogError(exception.ToString());
            return StatusCode(500, exception.Message);
        }



        private readonly string _controllerEntity = "mealchoice";


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