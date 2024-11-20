using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WsLocalizacao.Models.DTO;

using System.Xml.Linq;

public class KmlService
{
    private readonly string _kmlPath;

    public KmlService(string kmlPath)
    {
        _kmlPath = kmlPath;
    }

    public List<Placemark> GetPlacemarks()
    {
        var placemarks = new List<Placemark>();
        var xdoc = XDocument.Load(_kmlPath);

        foreach (var placemarkElement in xdoc.Descendants().Where(x => x.Name.LocalName == "Placemark"))
        {
            var extendedData = placemarkElement.Descendants().FirstOrDefault(x => x.Name.LocalName == "ExtendedData");

            if (extendedData != null)
            {
                placemarks.Add(new Placemark
                {
                    Cliente = extendedData.Descendants().FirstOrDefault(x => x.Attribute("name")?.Value == "CLIENTE")
                        ?.Value,
                    Situacao = extendedData.Descendants().FirstOrDefault(x => x.Attribute("name")?.Value == "SITUAÇÃO")
                        ?.Value,
                    Bairro = extendedData.Descendants().FirstOrDefault(x => x.Attribute("name")?.Value == "BAIRRO")
                        ?.Value,
                    Referencia = extendedData.Descendants()
                        .FirstOrDefault(x => x.Attribute("name")?.Value == "REFERENCIA")?.Value,
                    RuaCruzamento = extendedData.Descendants()
                        .FirstOrDefault(x => x.Attribute("name")?.Value == "RUA/CRUZAMENTO")?.Value,
                });
            }
        }

        return placemarks;
    }

    public List<Placemark> FilterPlacemarks(List<Placemark> placemarks, Placemark filter)
    {
        return placemarks.Where(p =>
            (string.IsNullOrEmpty(filter.Cliente) ||
             p.Cliente?.Contains(filter.Cliente, StringComparison.OrdinalIgnoreCase) == true) &&
            (string.IsNullOrEmpty(filter.Situacao) ||
             p.Situacao?.Contains(filter.Situacao, StringComparison.OrdinalIgnoreCase) == true) &&
            (string.IsNullOrEmpty(filter.Bairro) ||
             p.Bairro?.Contains(filter.Bairro, StringComparison.OrdinalIgnoreCase) == true) &&
            (string.IsNullOrEmpty(filter.Referencia) ||
             p.Referencia?.Contains(filter.Referencia, StringComparison.OrdinalIgnoreCase) == true) &&
            (string.IsNullOrEmpty(filter.RuaCruzamento) ||
             p.RuaCruzamento?.Contains(filter.RuaCruzamento, StringComparison.OrdinalIgnoreCase) == true)
        ).ToList();
    }

    public string ExportToKml(List<Placemark> filteredPlacemarks)
    {
        var kml = new XDocument(
            new XElement("kml",
                new XElement("Document",
                    filteredPlacemarks.Select(p =>
                        new XElement("Placemark",
                            new XElement("name", p.Cliente),
                            new XElement("description", $"Situacao: {p.Situacao}, Bairro: {p.Bairro}"),
                            new XElement("ExtendedData",
                                new XElement("Data", new XAttribute("name", "CLIENTE"),
                                    new XElement("value", p.Cliente)),
                                new XElement("Data", new XAttribute("name", "SITUAÇÃO"),
                                    new XElement("value", p.Situacao)),
                                new XElement("Data", new XAttribute("name", "BAIRRO"), new XElement("value", p.Bairro)),
                                new XElement("Data", new XAttribute("name", "REFERENCIA"),
                                    new XElement("value", p.Referencia)),
                                new XElement("Data", new XAttribute("name", "RUA/CRUZAMENTO"),
                                    new XElement("value", p.RuaCruzamento))
                            )
                        )
                    )
                )
            )
        );
        
        string projectPath = Directory.GetCurrentDirectory();
        var exportPath = Path.Combine(projectPath, "Models/DataBase/ExportedPlacemarks.kml");
        
        kml.Save(exportPath);
        return kml.ToString();
    }
}

public class Placemark
{
    public string Cliente { get; set; }
    public string? Situacao { get; set; }
    public string Bairro { get; set; }

    [MinLength(3, ErrorMessage = "O campo 'Referencia' deve ter pelo menos 3 caracteres.")]
    public string Referencia { get; set; }

    [MinLength(3, ErrorMessage = "O campo 'RuaCruzamento' deve ter pelo menos 3 caracteres.")]
    public string RuaCruzamento { get; set; }
}