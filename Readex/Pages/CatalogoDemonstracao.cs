namespace Readex.Pages;

internal sealed record ObraDemonstracao(
    string Capa,
    decimal Nota,
    int Leitores,
    int QuantidadeAvaliacoes,
    IReadOnlyList<int> Distribuicao,
    string CorCapa);

internal static class CatalogoDemonstracao
{
    internal static IReadOnlyList<ObraDemonstracao> Obras { get; } =
    [
        new("piranesi", 4.3m, 412, 184_200, [1, 2, 6, 31, 60], "#1d3a4a"),
        new("vagabond", 4.7m, 201, 96_300, [0, 1, 3, 18, 78], "#3a2a1c"),
        new("saga", 4.5m, 288, 132_800, [1, 2, 7, 28, 62], "#4a1d2e"),
        new("the-left-hand-of-darkness", 4.1m, 330, 151_000, [2, 4, 14, 38, 42], "#2b3440"),
        new("monster", 4.6m, 176, 88_400, [0, 1, 4, 24, 71], "#3d1414"),
        new("watchmen", 4.4m, 452, 210_500, [1, 3, 9, 30, 57], "#3f3a12"),
        new("tomorrow-and-tomorrow", 4.2m, 389, 168_900, [2, 4, 11, 35, 48], "#183c3a"),
        new("frieren", 4.6m, 158, 71_200, [0, 1, 4, 24, 71], "#26385a"),
        new("sandman", 4.5m, 260, 119_000, [1, 2, 6, 29, 62], "#2a1d3f"),
        new("the-remains-of-the-day", 4.0m, 241, 98_700, [2, 5, 16, 39, 38], "#3b3226"),
        new("berserk", 4.7m, 219, 102_400, [1, 1, 3, 16, 79], "#2a0f0f"),
        new("maus", 4.5m, 330, 143_000, [1, 2, 6, 27, 64], "#2c2c2c")
    ];
}
