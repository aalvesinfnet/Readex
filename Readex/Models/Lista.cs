namespace Readex.Models;

public class Lista
{
    private readonly List<ItemLista> _itens = [];

    public Guid Id { get; private set; }
    public Guid LeitorId { get; private set; }
    public string Nome { get; private set; } = "";
    public TipoLista Tipo { get; private set; }

    public IReadOnlyList<ItemLista> Itens => _itens.AsReadOnly();

    private Lista()
    {
    }

    public Lista(Guid id, Guid leitorId, string nome, TipoLista tipo)
    {
        if (id == Guid.Empty || leitorId == Guid.Empty)
            throw new ArgumentException(
                "Lista e leitor precisam ter identificadores válidos.");

        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length > 80)
            throw new ArgumentException(
                "O nome da lista é obrigatório e aceita até 80 caracteres.");

        if (!Enum.IsDefined(tipo))
            throw new ArgumentException("Tipo de lista inválido.");

        if (tipo == TipoLista.QUERO_LER && nome.Trim() != "Quero ler")
            throw new ArgumentException(
                "A lista padrão deve se chamar Quero ler.");

        Id = id;
        LeitorId = leitorId;
        Nome = nome.Trim();
        Tipo = tipo;
    }

    public void AdicionarObra(Obra obra)
    {
        obra.Validar();

        if (_itens.Any(item => item.ObraId == obra.Id))
            return;

        _itens.Add(new ItemLista(Id, obra.Id));
    }

    public bool RemoverObra(Guid obraId)
    {
        var item = _itens.SingleOrDefault(
            item => item.ObraId == obraId);

        return item is not null && _itens.Remove(item);
    }
}
