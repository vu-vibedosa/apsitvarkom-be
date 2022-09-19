using Microsoft.AspNetCore.Mvc;

namespace Apsitvarkom.Api.Controllers;

[ApiController]
// Specifies that this controller's endpoint (the uri to reach it) is '/api/Example'
[Route("/api/[controller]")]
public class ExampleController : ControllerBase
{
    private readonly ILogger<ExampleController> _logger;

    // Logger magically appears here in constructor
    // This is due to it being registered in Dependency Injection container (default behaviour of web api)
    // This method of retrieving a dependency is called construction injection
    public ExampleController(ILogger<ExampleController> logger)
    {
        _logger = logger;
    }

    // This action does not specify a route, so it will be reachable at controller endpoint + '/' = '/api/Example'
    [HttpGet]
    // Additional documentation for swagger
    [ProducesResponseType(typeof(ExampleModel), StatusCodes.Status200OK)]
    public ExampleModel GetExample()
    {
        _logger.LogInformation("Example action called");

        var result = new ExampleModel
        {
            Text = "Hello from back end!"
        };

        // result automatically gets transformed to JSON and returned back to the requester
        return result;
    }
}

/// <summary>
/// Should be located in a separate file
/// Left here for the simplicity of example
/// </summary>
public class ExampleModel
{
    public string Text { get; set; } = null!;
}