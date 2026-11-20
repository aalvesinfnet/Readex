namespace Readex.Models;

public class Curtida
{
    public Guid LeitorId { get; }
    public Guid ObraId { get; }
    public DateTimeOffset DataCurtida { get; }

    public Curtida(
        Guid leitorId,
        Guid obraId,
        DateTimeOffset agora)
    {
        if (leitorId == Guid.Empty || obraId == Guid.Empty)
            throw new ArgumentException(
                "Leitor e obra precisam ter identificadores válidos."
            );

        LeitorId = leitorId;
        ObraId = obraId;
        DataCurtida = agora.ToUniversalTime();
    }
}