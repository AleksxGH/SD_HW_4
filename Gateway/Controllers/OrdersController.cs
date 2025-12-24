using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Gateway.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public OrdersController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("http://orders-service:8080");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/orders", request);

        var content = await response.Content.ReadAsStringAsync();

        return StatusCode(
            (int)response.StatusCode,
            content
        );
    }
}