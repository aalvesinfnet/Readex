namespace Readex.Models;

public class Avaliacao {
    public Guid LeitorId { get; }
    public Guid ObraId { get; }
    public int Nota { get; private set; }

    public Avaliacao(Guid leitorId, Guid obraId, int nota)
    {
        if (leitorId == Guid.Empty || obraId == Guid.Empty)
            throw new ArgumentException(
                "Leitor e obra precisam ter identificadores válidos."
            );

        LeitorId = leitorId;
        ObraId = obraId;

        AlterarNota(nota);
    }

    public void AlterarNota(int nota)
    {
        if (nota < 1 || nota > 5)
            throw new ArgumentOutOfRangeException(
                nameof(nota),
                "A nota deve ser de 1 a 5."
            );

        Nota = nota;
    }
}