using WsLocalizacao.Models.DTO;

var builder = WebApplication.CreateBuilder(args);

// Adicione serviços ao contêiner.
builder.Services.AddControllers();

// Configuração do Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registre o serviço KML para injeção de dependência
builder.Services.AddSingleton<KmlService>(provider =>
{
    string projectPath = Directory.GetCurrentDirectory();
    var kmlPath = Path.Combine(projectPath, "Models/DataBase/DIRECIONADORES1.kml");

    return new KmlService(kmlPath);
});

var app = builder.Build();

// Configure o pipeline de requisição HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Middleware para gerar documentação Swagger
    app.UseSwaggerUI(); // Interface gráfica para Swagger
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); // Mapear endpoints do controlador

app.Run();