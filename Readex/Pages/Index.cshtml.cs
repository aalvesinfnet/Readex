using Microsoft.AspNetCore.Mvc.RazorPages;
using Readex.Infraestrutura;
using Readex.Models;

namespace Readex.Pages;

public class IndexModel : PageModel {
    private readonly string _pastaDados;
    private readonly ObraCsv _csv = new();
    public List<Obra> Obras { get; private set; } = [];
    public string? Mensagem { get; private set; }
    public string? Erro { get; private set; }
    public IndexModel(IWebHostEnvironment ambiente) {
        _pastaDados = Path.Combine(
            ambiente.ContentRootPath,
            "Dados"
        );
    }

    public void OnGet() {
        try {
            Obras = _csv.Ler(
                Path.Combine(_pastaDados, "obras.csv")
            );
        }
        catch (Exception erro) when (
            erro is IOException
            or UnauthorizedAccessException
            or InvalidDataException
            or System.Text.DecoderFallbackException) {
            Erro = erro.Message;
        }
    }

    public void OnPost() {
        try {
            var origem = Path.Combine(
                _pastaDados,
                "obras.csv"
            );

            var destino = Path.Combine(
                _pastaDados,
                "obras-copia.csv"
            );

            var lidas = _csv.Ler(origem);

            _csv.Gravar(destino, lidas);

            Obras = _csv.Ler(destino);

            Mensagem =
                $"{Obras.Count} obra(s) lida(s), gravada(s) e relida(s). " +
                "Arquivo gerado: Dados/obras-copia.csv.";
        }
        catch (Exception erro) when (
            erro is IOException
            or UnauthorizedAccessException
            or InvalidDataException
            or System.Text.DecoderFallbackException) {
            Erro = erro.Message;
        }
    }
}