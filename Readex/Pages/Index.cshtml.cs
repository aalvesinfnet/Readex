using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Readex.Infraestrutura;
using Readex.Models;

namespace Readex.Pages;

public sealed record ResultadoCatalogo(string Chave, string Titulo, string Autor, string CapaUrl);

public class IndexModel : PageModel
{
    private readonly ReadexDbContext _banco;

    public IndexModel(ReadexDbContext banco)
    {
        _banco = banco;
    }

    public IReadOnlyList<ResultadoCatalogo> Resultados { get; private set; } = [];
    public IReadOnlyDictionary<string, Obra> ObrasPorSlug { get; private set; } =
        new Dictionary<string, Obra>(StringComparer.OrdinalIgnoreCase);
    public bool ExibindoCatalogo { get; private set; }
    public string TituloCatalogo { get; private set; } = "Browse titles";

    public async Task OnGetAsync(string? q, TipoObra? tipo, bool todos = false)
    {
        var termo = q?.Trim() ?? "";
        if (tipo is { } tipoSolicitado && !Enum.IsDefined(tipoSolicitado))
            tipo = null;

        ExibindoCatalogo = todos || termo.Length > 0 || tipo.HasValue;

        var consulta = _banco.Obras.AsNoTracking().Where(obra => obra.CapaUrl != null);
        if (tipo is { } tipoFiltrado)
            consulta = consulta.Where(obra => obra.Tipo == tipoFiltrado);

        var obras = await consulta.ToListAsync(HttpContext.RequestAborted);
        var obrasComCapa = obras.Where(obra => !string.IsNullOrWhiteSpace(obra.CapaUrl));

        if (!ExibindoCatalogo)
        {
            ObrasPorSlug = obrasComCapa.Where(obra => !string.IsNullOrWhiteSpace(obra.Slug))
                .ToDictionary(obra => obra.Slug!, StringComparer.OrdinalIgnoreCase);
            return;
        }

        TituloCatalogo = termo.Length > 0 ? "Search results" : tipo switch
        {
            TipoObra.LIVRO => "Books",
            TipoObra.HQ => "Comics",
            TipoObra.MANGA => "Manga",
            _ => "Browse titles"
        };

        Resultados = obrasComCapa
            .Where(obra => termo.Length == 0 || obra.CorrespondePesquisa(termo))
            .OrderBy(obra => obra.Titulo, StringComparer.OrdinalIgnoreCase)
            .Select(obra => new ResultadoCatalogo(
                string.IsNullOrWhiteSpace(obra.Slug) ? obra.Id.ToString() : obra.Slug,
                obra.Titulo, obra.Autor, obra.CapaUrl!))
            .ToList();
    }
}
