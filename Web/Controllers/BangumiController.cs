using DmhyAutoDownload.Core;
using DmhyAutoDownload.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DmhyAutoDownload.Web.Controllers;

[ApiController]
[Route("bangumi")]
public class BangumiController : ControllerBase
{
    private readonly ILogger<BangumiController> _logger;
    private readonly Configuration _config;

    public BangumiController(ILogger<BangumiController> logger, Configuration configuration)
    {
        _logger = logger;
        _config = configuration;
    }

    [HttpGet("all/")]
    public IEnumerable<Bangumi> GetAllBangumis()
    {
        return _config.Bangumis;
    }

    [HttpGet("get/{Name}/")]
    public IActionResult GetBangumi(string Name)
    {
        var bangumi = _config.Bangumis.Find(bangumi => bangumi.Name == Name);
        if (bangumi == null)
        {
            return NotFound();
        }
        return new JsonResult(bangumi);
    }

    [HttpPost("add/")]
    public IActionResult AddBangumi([FromServices] RefresherService refresherService, Bangumi bangumi)
    {
        if (_config.Bangumis.Exists(bangumi1 => bangumi1.Name == bangumi.Name))
        {
            return BadRequest();
        }
        _config.Bangumis.Add(bangumi);
        refresherService.Refresh(null);
        return Accepted();
    }

    [HttpPost("refresh/")]
    public void Refresh([FromServices] RefresherService refresherService)
    {
        refresherService.Refresh(null);
    }
}