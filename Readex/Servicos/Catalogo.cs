using Readex.Models;

namespace Readex.Servicos;

public class Catalogo
{
    private readonly List<Obra> _obras;

    public IReadOnlyList<Obra> Obras => _obras.AsReadOnly();

    public Catalogo(IEnumerable<Obra> obras)
    {
        _obras = obras.ToList();

        var ids = new HashSet<Guid>();

        foreach (var obra in _obras)
        {
            obra.Validar();

            if (!ids.Add(obra.Id))
                throw new ArgumentException(
                    "Obra repetida no catálogo.");
        }
    }

    public IReadOnlyList<Obra> ObterDestaques()
    {
        return _obras
            .Where(obra => obra.Destaque)
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyList<Obra> Pesquisar(string termo)
    {
        return _obras
            .Where(obra => obra.CorrespondePesquisa(termo))
            .ToList()
            .AsReadOnly();
    }

    public Obra? ObterPorId(Guid obraId)
    {
        return _obras.SingleOrDefault(
            obra => obra.Id == obraId);
    }
}