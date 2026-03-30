using Microsoft.AspNetCore.Mvc;
using Torisho.Application.Interfaces.Dictionary;

namespace Torisho.API.Controllers;

[ApiController]
[Route("api/dictionary")]
public sealed class DictionaryDetailController : ControllerBase
{
    private readonly IDictionaryDetailService _detailService;

    public DictionaryDetailController(IDictionaryDetailService detailService)
    {
        _detailService = detailService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _detailService.GetWordDetailAsync(id, ct);
        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
