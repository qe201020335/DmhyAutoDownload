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
    private readonly ConfigManager _configManager;

    public BangumiController(ILogger<BangumiController> logger, ConfigManager configManager)
    {
        _logger = logger;
        _configManager = configManager;
        _config = configManager.Config;
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
    
    [HttpPost("markFinished/{name}/")]
    public IActionResult MarkFinished(string name, [FromQuery(Name = "finished")] bool finished = true)
    {
        var bangumi = _config.Bangumis.Find(bangumi => bangumi.Name == name);
        if (bangumi == null)
        {
            return NotFound();
        }
        bangumi.Finished = finished;
        _configManager.SaveConfig();
        return new JsonResult(bangumi);
    }
    
    [HttpDelete("delete/{name}/")]
    public IActionResult DeleteBangumi(string name)
    {
        var bangumi = _config.Bangumis.Find(bangumi => bangumi.Name == name);
        if (bangumi == null)
        {
            return NotFound();
        }
        _config.Bangumis.Remove(bangumi);
        _configManager.SaveConfig();
        return NoContent();
    }
}