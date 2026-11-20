namespace Readex.Models;

public class ItemLista {
    public Guid ListaId { get; }
    public Guid ObraId { get; }

    public ItemLista(Guid listaId, Guid obraId)
    {
        if (listaId == Guid.Empty || obraId == Guid.Empty)
            throw new ArgumentException(
                "Lista e obra precisam ter identificadores válidos."
            );

        ListaId = listaId;
        ObraId = obraId;
    }
}