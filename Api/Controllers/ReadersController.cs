using Api.Filters;
using Api.Models.Responses;
using Contracts;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ServiceFilter(typeof(CustomAuthorizeFilter))]
[ProducesResponseType(typeof(List<ErrorCodeModel>), StatusCodes.Status401Unauthorized)]
public class ReadersController : Controller
{
    private readonly IReadersInfoRepository _readersInfoRepository;

    public ReadersController(IReadersInfoRepository readersInfoRepository)
    {
        _readersInfoRepository = readersInfoRepository;
    }

    [Route("/api/readers/select")]
    [ProducesResponseType(typeof(List<ReadersInfo>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var readersInfo = await _readersInfoRepository.ListReadersInfo(cancellationToken);

        return Ok(readersInfo);
    }

    [Route("/api/readers/get/{readersCardId}")]
    [ProducesResponseType(typeof(ReadersInfo), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReadersInfo(int readersCardId, CancellationToken cancellationToken)
    {
        var readersInfo = await _readersInfoRepository.GetReadersInfo(readersCardId, cancellationToken);

        return Ok(readersInfo);
    }
}