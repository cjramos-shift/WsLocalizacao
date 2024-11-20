using Microsoft.AspNetCore.Mvc;
using WsLocalizacao.Models.DTO;

namespace WsLocalizacao.Controllers;

[ApiController]
[Route("api/placemarks")]
public class ApiController : Controller
{
    private readonly KmlService _kmlService;

    public ApiController()
    {
        string projectPath = Directory.GetCurrentDirectory();
        var kmlPath = Path.Combine(projectPath, "Models/DataBase/DIRECIONADORES1.kml");

        _kmlService = new KmlService(kmlPath);
    }
    
    [HttpPost("export")]
    public IActionResult ExportPlacemarks([FromBody] Placemark filter)
    {
        var placemarks = _kmlService.GetPlacemarks();
        var filteredPlacemarks = _kmlService.FilterPlacemarks(placemarks, filter);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        if (!filteredPlacemarks.Any())
        {
            return BadRequest("Nenhum placemark encontrado com os filtros fornecidos.");
        }

        var exportPath = _kmlService.ExportToKml(filteredPlacemarks);
        return Ok(new { message = "Arquivo exportado com sucesso!", path = exportPath });
    }

    [HttpGet("")]
    public IActionResult GetPlacemarks([FromQuery] Placemark filter)
    {
        var placemarks = _kmlService.GetPlacemarks();
        var filteredPlacemarks = _kmlService.FilterPlacemarks(placemarks, filter);

        if (!filteredPlacemarks.Any())
        {
            return BadRequest("Nenhum placemark encontrado com os filtros fornecidos.");
        }

        return Ok(filteredPlacemarks);
    }

    [HttpGet("filters")]
    public IActionResult GetAvailableFilters()
    {
        var placemarks = _kmlService.GetPlacemarks();

        var clientes = placemarks.Select(p => p.Cliente).Distinct().ToList();
        var situacoes = placemarks.Select(p => p.Situacao).Distinct().ToList();
        var bairros = placemarks.Select(p => p.Bairro).Distinct().ToList();

        return Ok(new { clientes, situacoes, bairros });
    }
}