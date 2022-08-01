using Microsoft.AspNetCore.Mvc;
using GroupMealApi.Models;
using GroupMealApi.Services;



namespace GroupMealApi.Controllers
{
    [ApiController]
    [Route("api/mealchoice")]
    public class MealChoiceController : Controller
    {
        private readonly ILogger<MealChoiceController> _logger;
        private readonly MealChoiceService _service;
        private readonly GroupService _groupService;

        public MealChoiceController(MealChoiceService service, GroupService groupService, ILogger<MealChoiceController> logger)
        {
            this._service = service;
            this._logger = logger;
            this._groupService = groupService;
        }


        /// <summary>
        /// Get all meal choices for a group 
        /// </summary>
        /// <returns>
        ///  A list of meal choices for a group
        /// </returns>
        [HttpGet]
        public ActionResult<GetAllResponse<MealChoiceDTO>?> GetAll()
        {
            var startTime = DateTime.UtcNow;
            this.ControllerLoggerStart(CrudRequest.GETALL);
            try
            {
                // Get all meal choices
                List<MealChoiceDTO> allMealChoices = _service.GetAll();

                // Log the end of the request
                ControllerLoggerEnd(startTime);

                // Return the meal choices
                return Ok(new GetAllResponse<MealChoiceDTO>
                {
                    dataset = allMealChoices.ToArray(),
                    total = allMealChoices.Count
                });

            }
            catch (Exception exception)
            {
                // Log the end of the request
                ControllerLoggerEnd(startTime);

                // Handle the exception
                return this.HandleError(exception);
            }
        }
        

        /// <summary>
        /// Get a meal choice by id 
        /// </summary>
        /// <param name="id">
        ///  The id of the meal choice to get
        /// </param>
        /// <returns>
        ///  The meal choice with the given id
        /// </returns>
        [HttpGet("{id:length(24)}")]
        public ActionResult<GetOneResponse<MealChoiceDTO>> Get(string id)
        {
            var startTime = DateTime.UtcNow;
            ControllerLoggerStart(CrudRequest.GETONE);
            try
            {
                // Get the meal choice
                MealChoiceDTO? getMealChoiceResponse = _service.GetOne(id);

                // If the meal choice does not exist, return a 404 
                if (getMealChoiceResponse is null)
                {
                    return NotFound("MealChoice not found");
                }

                // Log the end of the request
                ControllerLoggerEnd(startTime);

                // Return the meal choice
                return Ok(new GetOneResponse<MealChoiceDTO>
                {
                    data = getMealChoiceResponse,
                    success = true,
                    message = "MealChoice found"
                });

            }
            catch (Exception exception)
            {
                // Log the end of the request
                ControllerLoggerEnd(startTime);

                // Handle the exception
                return this.HandleError(exception);
            }

        }

        /// <summary>
        /// Create a meal choice 
        /// </summary>
        /// <param name="mealChoice">
        ///  The meal choice to create
        /// </param>
        /// <returns>
        ///  The created meal choice
        /// </returns>
        [HttpPost]
        public ActionResult<CreateOneResponse<MealChoice>> CreateOne(MealChoice mealChoice)
        {
            var startTime = DateTime.UtcNow;
            ControllerLoggerStart(CrudRequest.CREATE);
            try
            {

                // Ensure that the link for the menu is a valid URL
                if (!string.IsNullOrEmpty(mealChoice.MenuUrl))
                {
                    if (!Uri.IsWellFormedUriString(mealChoice.MenuUrl, UriKind.Absolute))
                    {
                        return BadRequest("Menu url is not a valid URL");
                    }
                }

                // Ensure that the link for the image is a valid URL
                if (!string.IsNullOrEmpty(mealChoice.ImageUrl))
                {
                    if (!Uri.IsWellFormedUriString(mealChoice.ImageUrl, UriKind.Absolute))
                    {
                        return BadRequest("Image url is not a valid URL");
                    }
                }


                // Prevent duplicate name in the same group
                var mealChoicesWithSameName = _service.Scan(
                    new MealChoiceScan
                    {
                        GroupId = mealChoice.GroupId,
                        Name = mealChoice.Name,
                    }
                );
                // If there is already a meal choice with the same name, return a 400 
                if (mealChoicesWithSameName.Count > 0)
                {
                    return BadRequest("MealChoice with same name already exists");
                }


                // Prevent duplicate menu in the same group
                var mealChoicesWithSameMenuUrl = _service.Scan(
                    new MealChoiceScan
                    {
                        GroupId = mealChoice.GroupId,
                        MenuUrl = mealChoice.MenuUrl,
                    }
                );

                // If there is already a meal choice with the same menu url, return a 400
                if (mealChoicesWithSameMenuUrl.Count > 0)
                {
                    return BadRequest("MealChoice with same menu url already exists in another meal choice");
                }


                // Make sure group exists
                bool groupExists = _groupService.Exists(mealChoice.GroupId);


                // If the group does not exist, return a 404
                if (!groupExists)
                {
                    return BadRequest("Group does not exist");
                }

                // Create the meal choice
                var createResponse = _service.CreateOne(mealChoice);


                // If the meal choice was not created, return a 400
                if (!createResponse)
                {
                    throw new Exception("Create One MealChoice Service Failed");
                }

                // Log the end of the request
                ControllerLoggerEnd(startTime);

                // Return the meal choice
                return Ok(new CreateOneResponse<MealChoice>
                {
                    data = mealChoice,
                    success = true,
                    message = "MealChoice Created"
                });


            }
            catch (Exception exception)
            {
                // Log the end of the request
                ControllerLoggerEnd(startTime);

                // Handle the exception
                return this.HandleError(exception);
            }
        }


        /// <summary>
        /// Update a meal choice 
        /// </summary>
        /// <param name="id">
        ///  The id of the meal choice to update
        /// </param>
        /// <param name="mealChoice">
        /// The meal choice to update
        /// </param>
        /// <returns>
        /// The updated meal choice
        /// </returns>
        [HttpPut("{id:length(24)}")]
        public ActionResult<UpdateOneResponse<MealChoiceDTO>> UpdateOne(string id, MealChoiceDTO mealChoice)
        {
            var startTime = DateTime.UtcNow;
            ControllerLoggerStart(CrudRequest.UPDATE);
            try
            {

                // Prevent updating meal choice with an id that does not match the id in the url
                if (mealChoice.Id != id)
                {
                    return BadRequest("Id in the request body does not match the id in the url");
                }


                // Ensure that the link for the menu is a valid URL
                if (!string.IsNullOrEmpty(mealChoice.MenuUrl))
                {
                    if (!Uri.IsWellFormedUriString(mealChoice.MenuUrl, UriKind.Absolute))
                    {
                        return BadRequest("Menu url is not a valid URL");
                    }
                }

                // Ensure that the link for the image is a valid URL
                if (!string.IsNullOrEmpty(mealChoice.ImageUrl))
                {
                    if (!Uri.IsWellFormedUriString(mealChoice.ImageUrl, UriKind.Absolute))
                    {
                        return BadRequest("Image url is not a valid URL");
                    }
                }

                // Make sure group exists
                bool groupExists = _groupService.Exists(mealChoice.GroupId);
                if (!groupExists)
                {
                    return BadRequest("Group does not exist");
                }

                // Update the meal choice
                var updateResponse = _service.UpdateOne(mealChoice);


                // If the update failed, return a bad request
                if (!updateResponse)
                {
                    throw new Exception("Update One MealChoice Service Failed");
                }

                // Log the end of the request
                ControllerLoggerEnd(startTime);

                // Return the updated meal choice
                return Ok(new UpdateOneResponse<MealChoice>
                {
                    data = mealChoice,
                    success = true,
                    message = "MealChoice Updated"
                });

            }
            catch (Exception exception)
            {
                // Log the end of the request
                ControllerLoggerEnd(startTime);

                // Handle the exception
                return this.HandleError(exception);
            }
        }



        [HttpDelete("{id:length(24)}")]
        public ActionResult<DeleteOneResponse<MealChoiceDTO>> DeleteOne(string id)
        {
            var startTime = DateTime.UtcNow;
            ControllerLoggerStart(CrudRequest.DELETE);
            try
            {
                // Get the meal choice
                var getMealChoice = _service.GetOne(id);

                // If the meal choice does not exist, return a 404
                if (getMealChoice is null)
                {
                    return NotFound("MealChoice not found");
                }

                // Delete the meal choice
                var deleteResponse = _service.DeleteOne(id);

                // If the delete failed, return a bad request
                if (!deleteResponse)
                {
                    throw new Exception("Delete One MealChoice Service Failed");
                }

                // Log the end of the request
                ControllerLoggerEnd(startTime);

                // Return the deleted meal choice
                return Ok(new DeleteOneResponse<MealChoice>
                {
                    success = true,
                    message = "MealChoice Deleted"
                });
            }
            catch (Exception exception)
            {
                // Log the end of the request
                ControllerLoggerEnd(startTime);

                // Handle the exception
                return this.HandleError(exception);
            }
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
            _logger.LogError(exception.ToString());

            // Return the error response
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

            string ? logString; 

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