using AttributeAutoDI.Attribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AttributeAutoDI.Sample;

public class SampleController(
    ISingletonService primarySingletonService,
    [Named("NamedDI")] ISingletonService namedSingletonService,
    IScopedService scopedService,
    TransientService transientService,
    IOptions<SampleOption> sampleOption)
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

    [HttpGet("/option-binding")]
    public ActionResult<string> OptionBinding()
    {
        var response = sampleOption.Value.Sample;
        return Ok(response);
    }
}