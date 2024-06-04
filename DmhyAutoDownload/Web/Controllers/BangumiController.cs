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
    private readonly Config _config;
    private readonly ConfigManager _configManager;

    public BangumiController(ILogger<BangumiController> logger, ConfigManager configManager)
    {
        _logger = logger;
        _configManager = configManager;
        _config = configManager.Config;
    }

    [HttpGet("all/")]
    public ICollection<Bangumi> GetAllBangumis()
    {
        return _config.BangumiList;
    }

    [HttpGet("get/{name}/")]
    public IActionResult GetBangumi(string name)
    {
        if (_config.Bangumis.TryGetValue(name, out var bangumi))
        {
            return new JsonResult(bangumi);
        }
        return NotFound();
    }

    [HttpPost("add/")]
    public IActionResult AddBangumi([FromServices] RefresherService refresherService, Bangumi bangumi)
    {
        if (_config.Bangumis.ContainsKey(bangumi.Name))
        {
            return BadRequest();
        }
        _config.Bangumis[bangumi.Name] = bangumi;
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
        if (_config.Bangumis.TryGetValue(name, out var bangumi))
        {
            bangumi.Finished = finished;
            _configManager.SaveConfig();
            return new JsonResult(bangumi);
        }
        return NotFound();
    }
    
    [HttpDelete("delete/{name}/")]
    public IActionResult DeleteBangumi(string name)
    {
        if (_config.Bangumis.ContainsKey(name))
        {
            _config.Bangumis.Remove(name);
            _configManager.SaveConfig();
            return NoContent();
        }
        return NotFound();
    }
}