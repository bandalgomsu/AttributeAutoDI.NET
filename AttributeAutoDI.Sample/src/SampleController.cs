using AttributeAutoDI.Attribute;
using Microsoft.AspNetCore.Mvc;

namespace AttributeAutoDI.Sample;

public class SampleController(
    ISingletonService primarySingletonService,
    [Named("NamedDI")] ISingletonService namedSingletonService,
    IScopedService scopedService,
    TransientService transientService)
    : ControllerBase
{
    [HttpGet("/singleton/primary")]
    public ActionResult<string> Primary()
    {
        var response = primarySingletonService.GetMessage();
        return Ok(response);
    }

    [HttpGet("/singleton/named")]
    public ActionResult<string> Named()
    {
        var response = namedSingletonService.GetMessage();
        return Ok(response);
    }

    [HttpGet("/scoped")]
    public ActionResult<string> Scoped()
    {
        var response = scopedService.GetMessage();
        return Ok(response);
    }

    [HttpGet("/transient")]
    public ActionResult<string> Transient()
    {
        var response = transientService.GetMessage();
        return Ok(response);
    }
}