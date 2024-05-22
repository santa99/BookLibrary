using System.Net.Mime;
using Contracts;
using Contracts.Exceptions;
using Contracts.Models;
using Contracts.Models.Responses;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <inheritdoc />
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[ProducesResponseType( StatusCodes.Status401Unauthorized)]
[Produces( MediaTypeNames.Application.Json )]
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
    [ProducesResponseType( typeof(ErrorCodeModel),StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReadersInfo(int readersCardId, CancellationToken cancellationToken)
    {
        var result = await _readersInfoRepository.GetReadersInfo(readersCardId, cancellationToken);

        if (result == null)
        {
            throw new ReadersCardIdNotFoundException(readersCardId);
        }
        
        return Ok(result);
    }
}