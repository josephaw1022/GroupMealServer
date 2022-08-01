using GroupMealApi.Models;
using GroupMealApi.Services;
using Microsoft.AspNetCore.Mvc;


namespace GroupMealApi.Controllers
{
    [ApiController]
    [Route("api/election")]
    public class ElectionController : ControllerBase
    {
        private readonly ILogger<ElectionController> _logger;

        private readonly ElectionService _electionService;
        private readonly GroupService _groupService;

        public ElectionController(ILogger<ElectionController> logger, ElectionService electionService, GroupService groupService)
        {
            this._electionService = electionService;
            this._groupService = groupService;
            this._logger = logger;
        }


        [HttpGet]
        public ActionResult<GetAllResponse<ElectionDTO>> GetAll([FromQuery] ElectionScan? electionQuery)
        {

            // Start the timer for the controller
            var startTime = DateTime.UtcNow;

            // Log the start of the controller
            ControllerLoggerStart(CrudRequest.GETALL);

            try
            {

                bool shouldScan = false;
                if (electionQuery is not null)
                {
                    shouldScan = this.ShouldScan(electionQuery);
                }

                if (shouldScan && electionQuery is not null)
                {
                    // Get all of the elections that match the query
                    var scanResponse = _electionService.Scan(electionQuery);

                    // Log the end of the controller
                    ControllerLoggerEnd(startTime);

                    // Return the scan response
                    return Ok(new GetAllResponse<ElectionDTO>
                    {
                        dataset = scanResponse.Data.ToArray(),
                        total = scanResponse.Data.Count(),
                    });

                }

                // Get all of the election rounds
                var getAllResponse = _electionService.GetAll();

                // If an error occurred, then throw an exception
                if (!getAllResponse.Success)
                {
                    throw new Exception(getAllResponse.ErrorMessage);
                }

                // Log the end of the controller
                ControllerLoggerEnd(startTime);

                // Return all of the election rounds
                return Ok(new GetAllResponse<ElectionDTO>
                {
                    dataset = getAllResponse.Data.ToArray(),
                    total = getAllResponse.Data.Count()
                });

            }
            catch (Exception exception)
            {
                // Log the end of the controller
                ControllerLoggerEnd(startTime);

                // Return the error
                return this.HandleError(exception);
            }
        }






        [HttpGet("{id:length(24)}")]
        public IActionResult GetOne(string id)
        {
            // Start the timer for the controller
            var startTime = DateTime.UtcNow;

            // Log the start of the controller
            ControllerLoggerStart(CrudRequest.GETONE);

            try
            {

                // Get the election round
                var getOneResponse = _electionService.Get(id);

                // If an error occurred, then throw an exception
                if (!getOneResponse.Success)
                {
                    throw new Exception(getOneResponse.ErrorMessage);
                }

                // Log the end of the controller
                ControllerLoggerEnd(startTime);

                // Return the election round
                return Ok(getOneResponse.Data);
            }
            catch (Exception exception)
            {
                // Log the end of the controller
                ControllerLoggerEnd(DateTime.UtcNow);

                // Return the error
                return this.HandleError(exception);
            }
        }




        [HttpPost]
        public IActionResult Create(Election electionDTO)
        {
            // Start the timer for the controller
            var startTime = DateTime.UtcNow;

            // Log the start of the controller
            ControllerLoggerStart(CrudRequest.CREATE);

            try
            {
                // Ensure that the group exists
                bool groupResponse = _groupService.Exists(electionDTO.GroupId);

                if (!groupResponse)
                {
                    return BadRequest("Group does not exist");
                }

                // Create the election round
                var createResponse = _electionService.Create(electionDTO);

                // If an error occurred, then throw an exception
                if (!createResponse.Success)
                {
                    throw new Exception(createResponse.ErrorMessage);
                }

                // Log the end of the controller
                ControllerLoggerEnd(startTime);

                // Return the election round
                return Ok(createResponse.Data);
            }
            catch (Exception exception)
            {
                // Log the end of the controller
                ControllerLoggerEnd(startTime);

                // Return the error
                return this.HandleError(exception);
            }
        }



        [HttpPut]
        public IActionResult Update(ElectionDTO election)
        {
            // Start the timer for the controller
            var startTime = DateTime.UtcNow;

            // Log the start of the controller
            ControllerLoggerStart(CrudRequest.UPDATE);

            try
            {
                // Ensure that the group exists
                bool groupResponse = _groupService.Exists(election.GroupId);

                if (!groupResponse)
                {
                    return BadRequest("Group does not exist");
                }

                // Update the election round
                var updateResponse = _electionService.Update(election);

                // If an error occurred, then throw an exception
                if (!updateResponse.Success)
                {
                    throw new Exception(updateResponse.ErrorMessage);
                }

                // Log the end of the controller
                ControllerLoggerEnd(startTime);

                // Return the election round
                return Ok(updateResponse.Data);
            }
            catch (Exception exception)
            {
                // Log the end of the controller
                ControllerLoggerEnd(startTime);

                // Return the error
                return this.HandleError(exception);
            }
        }


        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id )
        {
            // Start the timer for the controller
            var startTime = DateTime.UtcNow;

            // Log the start of the controller
            ControllerLoggerStart(CrudRequest.DELETE);

            try
            {
                // Get the election 
                var getOneResponse = _electionService.Get(id);

                if(!getOneResponse.Success)
                {
                    // Log the end of the controller
                    ControllerLoggerEnd(startTime);

                    // Return the error
                    return BadRequest(getOneResponse.ErrorMessage);
                }

                if(getOneResponse.Data is null)
                {
                    // Log the end of the controller
                    ControllerLoggerEnd(startTime);

                    // Return the error
                    return NotFound("Election does not exist");
                }


                // Delete the election round
                var deleteResponse = _electionService.Delete(id);

                // If an error occurred, then throw an exception
                if (!deleteResponse.Success)
                {
                    throw new Exception(deleteResponse.ErrorMessage);
                }

                // Log the end of the controller
                ControllerLoggerEnd(startTime);

                // Return the election round
                return Ok(deleteResponse.Data);

            }
            catch(Exception exception)
            {
                // Log the end of the controller
                ControllerLoggerEnd(startTime);

                // Return the error
                return this.HandleError(exception);
            }
        }



        private bool ShouldScan(ElectionScan electionQuery)
        {
            if (electionQuery is null)
            {
                return false;
            }

            if (electionQuery.Rounds is not null)
            {
                return true;
            }

            if (electionQuery.CurrentRound is not null)
            {
                return true;
            }

            if (electionQuery.IsActive is not null)
            {
                return true;
            }

            if (electionQuery.StoppedHalfway is not null)
            {
                return true;
            }

            if (electionQuery.GroupId is not null)
            {
                return true;
            }

            return false;
        }





        /// <summary>
        /// Handles the error.
        /// </summary>
        /// <param name="exception">
        /// The exception from the controller action.
        /// </param>  
        /// <returns>
        ///  The error response
        /// </returns>
        private ActionResult HandleError(Exception exception)
        {
            // Log the error
            _logger.LogError(exception, exception.Message);

            // Return the error response
            return StatusCode(500, exception.Message);
        }







        private readonly string _controllerEntity = "election";


        /// <summary>
        /// Logs the request method and time to the console.
        /// </summary>
        /// <param name="request">
        ///  The request method
        /// </param>
        private void ControllerLoggerStart(CrudRequest request)
        {

            string? logString;

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

                default:
                    logString = $"{_controllerEntity} UNKNOWN";
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

            // Calculate the time taken to complete the request in milliseconds
            var timeTaken = DateTime.UtcNow.Millisecond - startTime.Millisecond;

            // Log the time taken
            _logger.LogInformation("{timeTaken} milliseconds", timeTaken);
        }

    }
}