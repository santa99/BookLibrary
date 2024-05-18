using Contracts;
using Contracts.Models;
using Contracts.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <inheritdoc />
[Authorize]
[ProducesResponseType(typeof(List<ErrorCodeModel>), StatusCodes.Status401Unauthorized)]
public class ReadersController : Controller
{
    private readonly IReadersInfoRepository _readersInfoRepository;

    /// <inheritdoc />
    public ReadersController(IReadersInfoRepository readersInfoRepository)
    {
        _readersInfoRepository = readersInfoRepository;
    }

    /// <summary>
    /// Provides a list of readers.
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>List of readers info.</returns>
    [HttpGet("/api/readers/select")]
    [ProducesResponseType(typeof(List<ReadersInfo>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var result = await _readersInfoRepository.ListReadersInfo(cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Provides specific readers info or null.
    /// </summary>
    /// <param name="readersCardId">Readers card id.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Single readers info or not found.</returns>
    [HttpGet("/api/readers/get/{readersCardId}")]
    [ProducesResponseType(typeof(ReadersInfo), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReadersInfo(int readersCardId, CancellationToken cancellationToken)
    {
        var result = await _readersInfoRepository.GetReadersInfo(readersCardId, cancellationToken);

        if (result == null)
        {
            return NotFound($"Readers info for given id '{readersCardId}' wasn't found.");
        }
        
        return Ok(result);
    }
}