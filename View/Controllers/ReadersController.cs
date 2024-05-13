﻿using Contracts.Models;
using Microsoft.AspNetCore.Mvc;

namespace View.Controllers;

[Route("readers")]
public class ReadersController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private HttpClient _createClient;

    public ReadersController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<ActionResult> GetReaders()
    {
        using (_createClient = CreateClient())
        {
            try
            {
                return Ok(await _createClient.GetFromJsonAsync<List<ReadersInfo>>("/api/readers/select"));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
    }

    [HttpGet("{readersCardId:int}")]
    public async Task<ActionResult<ReadersInfo>> GetReader(int readersCardId)
    {
        try
        {
            using (_createClient = CreateClient())
            {

                var result = await _createClient.GetFromJsonAsync<ReadersInfo>($"/api/readers/get/{readersCardId}");

                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error retrieving data from the database");
        }
    }
    
    private HttpClient CreateClient()
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://localhost:7227");
        return httpClient;
    }
}