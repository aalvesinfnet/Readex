using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Readex.Infraestrutura;
using Readex.Models;

namespace Readex.Pages;

internal sealed record TituloRelacionado(string Capa, string Titulo, string CapaUrl, decimal Nota, int Leitores);

public class TitleModel : PageModel
{
    private readonly ReadexDbContext _banco;

    public TitleModel(ReadexDbContext banco)
    {
        _banco = banco;
    }

    public string Titulo { get; private set; } = "";
    public string Autor { get; private set; } = "";
    public string Tipo { get; private set; } = "";
    public string? CapaUrl { get; private set; }
    public int? Ano { get; private set; }
    public string? Formato { get; private set; }
    public string? Sinopse { get; private set; }
    public IReadOnlyList<string> Generos { get; private set; } = [];
    internal ObraDemonstracao? Previa { get; private set; }
    internal IReadOnlyList<TituloRelacionado> Relacionadas { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(string chave)
    {
        var cancelamento = HttpContext.RequestAborted;
        var obras = _banco.Obras.AsNoTracking();
        Obra? obra;

        if (Guid.TryParse(chave, out var id))
        {
            obra = await obras.SingleOrDefaultAsync(item => item.Id == id, cancelamento);
        }
        else
        {
            var slug = chave.ToLowerInvariant();
            obra = await obras.SingleOrDefaultAsync(item => item.Slug == slug, cancelamento);
        }

        if (obra is null)
            return NotFound();

        Titulo = obra.Titulo;
        Autor = obra.Autor;
        Tipo = obra.Tipo switch
        {
            TipoObra.LIVRO => "Book",
            TipoObra.HQ => "Comic",
            TipoObra.MANGA => "Manga",
            _ => "Title"
        };
        CapaUrl = string.IsNullOrWhiteSpace(obra.CapaUrl) ? null : Url.Content(obra.CapaUrl);
        Ano = obra.Ano;
        Formato = obra.Formato;
        Sinopse = obra.Sinopse;
        Generos = SepararGeneros(obra.Genero);

        var amostras = CatalogoDemonstracao.Obras;
        Previa = amostras.FirstOrDefault(item => string.Equals(item.Capa, obra.Slug, StringComparison.OrdinalIgnoreCase));

        var candidatas = await obras
            .Where(item => item.Id != obra.Id && item.Slug != null && item.CapaUrl != null)
            .Select(item => new { item.Slug, item.Titulo, item.Tipo, item.Genero, item.CapaUrl })
            .ToListAsync(cancelamento);
        var generosDaObra = Generos.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var candidatasPorSlug = candidatas
            .Where(item => !string.IsNullOrWhiteSpace(item.Slug) && !string.IsNullOrWhiteSpace(item.CapaUrl) &&
                (item.Tipo == obra.Tipo || SepararGeneros(item.Genero).Any(generosDaObra.Contains)))
            .ToDictionary(item => item.Slug!, StringComparer.OrdinalIgnoreCase);

        var relacionadas = new List<TituloRelacionado>();
        foreach (var amostra in amostras)
        {
            if (!candidatasPorSlug.TryGetValue(amostra.Capa, out var candidata))
                continue;

            relacionadas.Add(new TituloRelacionado(
                candidata.Slug!, candidata.Titulo, candidata.CapaUrl!, amostra.Nota, amostra.Leitores));
            if (relacionadas.Count == 6)
                break;
        }

        Relacionadas = relacionadas;
        return Page();
    }

    private static string[] SepararGeneros(string generos) =>
        generos.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
