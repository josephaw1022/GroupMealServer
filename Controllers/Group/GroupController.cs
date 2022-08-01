using GroupMealApi.Models;
using GroupMealApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace GroupMealApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly ILogger<GroupController> _logger;
        private readonly GroupService _groupService;

        public GroupController(GroupService groupService, ILogger<GroupController> logger)
        {
            this._groupService = groupService;
            this._logger = logger;
        }

        [HttpGet]
        public ActionResult<GetAllResponse<GroupAndMoreDTO>> GetAll()
        {
            try
            {
                var getAllResponse = this._groupService.GetAll();

                return Ok(new GetAllResponse<GroupAndMoreDTO>
                {
                    dataset = getAllResponse,
                    total = getAllResponse.Length
                });
            }

            catch (Exception exception)
            {
                this._logger.LogError(exception, "Error getting all groups");

                var errorResponse = new GetAllResponse<GroupDTO>
                {
                    dataset = new GroupDTO[] { },
                    total = 0
                };

                return BadRequest(errorResponse);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<GetOneResponse<GroupAndMoreDTO>?> Get(string id)
        {
            try
            {
                var response = this._groupService.Get(id);
                var formatResponse = new GetOneResponse<GroupAndMoreDTO>
                {
                    data = response,
                    success = true,
                    message = "Group found"
                };

                return Ok(formatResponse);
            }

            catch (Exception exception)
            {

                this._logger.LogError(exception, "Error getting group {0}", id);

                var errorResponse = new GetOneResponse<GroupAndMoreDTO>
                {
                    data = null,
                    success = false,
                    message = exception.Message
                };

                return BadRequest(errorResponse);
            }
        }

        [HttpPost]
        public ActionResult<CreateOneResponse<Group>> Create(Group group)
        {
            try
            {
                var response = this._groupService.Create(group);

                var formatResponse = new CreateOneResponse<Group>
                {
                    data = group,
                    success = true,
                    message = "Created"
                };

                return Ok(formatResponse);
            }

            catch (Exception exception)
            {
                this._logger.LogError(exception, "Error creating group {group}", group);

                var errorResponse = new CreateOneResponse<Group>
                {
                    data = null,
                    success = false,
                    message = exception.Message
                };
                return BadRequest(errorResponse);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<UpdateOneResponse<GroupDTO>> Update(GroupDTO group, [FromRoute] string id)
        {
            try
            {

                if (group.Id != id)
                {
                    return BadRequest("Id in path does not match id in body");
                }


                var getGroup = this._groupService.Get(id);


                if (getGroup == null)
                {
                    return NotFound("Group does not exist");
                }

                var serviceResponse = this._groupService.Update(id, group);

                if (!serviceResponse)
                {
                    throw new Exception("Group could not be updated");
                }

                var formatResponse = new UpdateOneResponse<GroupDTO>
                {
                    data = group,
                    success = true,
                    message = "Updated"
                };

                return Ok(formatResponse);
            }

            catch (Exception exception)
            {
                this._logger.LogError(exception, "Error updating group {group}", group);

                var errorResponse = new UpdateOneResponse<GroupDTO>
                {
                    data = null,
                    success = false,
                    message = exception.Message
                };
                return BadRequest(errorResponse);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<DeleteOneResponse<GroupDTO>> Delete(string id)
        {
            try
            {
                var response = this._groupService.Remove(id);

                if (!response)
                {
                    throw new Exception("Group could not be deleted");
                }

                var formatResponse = new DeleteOneResponse<GroupDTO>
                {
                    id = id,
                    success = true,
                    message = "Deleted"
                };

                return Ok(formatResponse);
            }

            catch (Exception exception)
            {
                this._logger.LogError(exception, "Error deleting group {id}", id);

                var errorResponse = new DeleteOneResponse<GroupDTO>
                {
                    id = id,
                    success = false,
                    message = exception.Message
                };
                return BadRequest(errorResponse);
            }
        }




    }
}