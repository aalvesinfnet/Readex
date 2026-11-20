namespace Readex.Models;

public class Favorito
{
    public Guid LeitorId { get; }
    public Guid ObraId { get; }
    public int Posicao { get; }

    public Favorito(Guid leitorId, Guid obraId, int posicao)
    {
        if (leitorId == Guid.Empty || obraId == Guid.Empty)
            throw new ArgumentException(
                "Leitor e obra precisam ter identificadores válidos.");

        if (posicao < 1 || posicao > 4)
            throw new ArgumentOutOfRangeException(
                nameof(posicao),
                "A posição deve ser de 1 a 4.");

        LeitorId = leitorId;
        ObraId = obraId;
        Posicao = posicao;
    }
}