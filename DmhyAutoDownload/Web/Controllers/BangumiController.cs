using DmhyAutoDownload.Core;
using DmhyAutoDownload.Core.Data.Models;
using DmhyAutoDownload.Core.Interfaces;
using DmhyAutoDownload.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DmhyAutoDownload.Web.Controllers;

[ApiController]
[Route("bangumi")]
public class BangumiController : ControllerBase
{
    private readonly ILogger<BangumiController> _logger;
    private readonly IBangumiRepository _bangumiRepository;

    public BangumiController(ILogger<BangumiController> logger, IBangumiRepository bangumiRepository)
    {
        _logger = logger;
        _bangumiRepository = bangumiRepository;
    }

    [HttpGet("all/")]
    public async Task<ICollection<Bangumi>> GetAllBangumisAsync([FromQuery(Name = "finished")] bool? finished = null)
    {
        if (finished.HasValue)
        {
            return await _bangumiRepository.GetBangumisAsync(finished.Value);
        }
        
        return await _bangumiRepository.GetAllBangumisAsync();
    }

    [HttpGet("get/{name}/")]
    public async Task<IActionResult> GetBangumiAsync(string name)
    {
        var bangumi = await _bangumiRepository.GetBangumiAsync(name);
        if (bangumi != null)
        {
            return new JsonResult(bangumi);
        }
        return NotFound();
    }

    [HttpPut("add/")]
    public async Task<IActionResult> AddBangumiAsync([FromServices] RefresherService refresherService, Bangumi bangumi)
    {
        if (!await _bangumiRepository.TryAddBangumiAsync(bangumi))
        {
            return BadRequest();
        }
        
        refresherService.Refresh(null);
        return Created();
    }

    [HttpPost("refresh/")]
    public void Refresh([FromServices] RefresherService refresherService)
    {
        refresherService.Refresh(null);
    }
    
    [HttpPost("markFinished/{name}/")]
    public async Task<IActionResult> MarkFinishedAsync(string name, [FromQuery(Name = "finished")] bool finished = true)
    {
        if (await _bangumiRepository.TryMarkAsFinishedAsync(name, finished))
        {
            var bangumi = await _bangumiRepository.GetBangumiAsync(name);
            return new JsonResult(bangumi!);
        }
        
        return NotFound();
    }
    
    [HttpDelete("delete/{name}/")]
    public async Task<IActionResult> DeleteBangumiAsync(string name)
    {
        if (await _bangumiRepository.TryDeleteBangumiAsync(name))
        {
            return NoContent();
        }
        
        return NotFound();
    }
}