using GroupMealModels.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace GroupMealApi.Controllers;



[ApiController]
[Route("api/[controller]")]
public class TestingController : ControllerBase 
{

    private readonly ILogger<TestingController> _logger;

    public TestingController(ILogger<TestingController> logger)
    {
        _logger = logger;
    }


    [HttpGet]
    public IActionResult Get()
    {

        var testingAccount = new TestingAccount
        {
            Id = "1",
            Username = "testing",
            Password = "testing",
            Email = "",
        };


        return Ok(testingAccount);


    }


}