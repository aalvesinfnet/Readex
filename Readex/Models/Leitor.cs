using System.ComponentModel.DataAnnotations;

namespace Readex.Models;

public class Leitor
{
    private readonly List<Avaliacao> _avaliacoes = [];
    private readonly List<Lista> _listas = [];
    private readonly List<Curtida> _curtidas = [];
    private readonly List<Favorito> _favoritos = [];

    public Guid Id { get; }
    public string NomeUsuario { get; }
    public string Email { get; }

    internal string SenhaHash { get; private set; } = "";

    public string NomeExibicao { get; private set; }
    public string? FotoUrl { get; private set; }

    public IReadOnlyList<Avaliacao> Avaliacoes => _avaliacoes.AsReadOnly();
    public IReadOnlyList<Lista> Listas => _listas.AsReadOnly();
    public IReadOnlyList<Curtida> Curtidas => _curtidas.AsReadOnly();
    public IReadOnlyList<Favorito> Favoritos => _favoritos.AsReadOnly();

    internal Leitor(Guid id, string nomeUsuario, string email)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id do leitor inválido.");

        if (string.IsNullOrWhiteSpace(nomeUsuario)
            || nomeUsuario.Trim().Length > 30)
            throw new ArgumentException(
                "Nome de usuário obrigatório, com até 30 caracteres.");

        if (string.IsNullOrWhiteSpace(email)
            || email.Trim().Length > 254
            || !new EmailAddressAttribute().IsValid(email.Trim()))
            throw new ArgumentException("E-mail inválido.");

        Id = id;
        NomeUsuario = nomeUsuario.Trim();
        Email = email.Trim();
        NomeExibicao = NomeUsuario;

        _listas.Add(
            new Lista(
                Guid.NewGuid(),
                Id,
                "Quero ler",
                TipoLista.QUERO_LER));
    }

    internal void DefinirSenhaHash(string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senhaHash)
            || senhaHash.Length > 512)
            throw new ArgumentException("Hash de senha inválido.");

        SenhaHash = senhaHash;
    }

    public (
        string NomeUsuario,
        string NomeExibicao,
        string? FotoUrl) ObterIdentificacaoPublica()
    {
        return (NomeUsuario, NomeExibicao, FotoUrl);
    }

    public void PersonalizarPerfil(
        string nomeExibicao,
        string? fotoUrl,
        IReadOnlyList<Obra?> favoritosPorPosicao)
    {
        if (string.IsNullOrWhiteSpace(nomeExibicao)
            || nomeExibicao.Trim().Length > 60)
            throw new ArgumentException(
                "Nome de exibição obrigatório, com até 60 caracteres.");

        if (fotoUrl is not null && fotoUrl.Length > 500)
            throw new ArgumentException(
                "A referência da foto aceita até 500 caracteres.");

        if (favoritosPorPosicao.Count > 4)
            throw new ArgumentException(
                "O perfil aceita até quatro espaços de favoritos.");

        var novos = new List<Favorito>();
        var ids = new HashSet<Guid>();

        for (int i = 0; i < favoritosPorPosicao.Count; i++)
        {
            var obra = favoritosPorPosicao[i];

            if (obra is null)
                continue;

            obra.Validar();

            if (!ids.Add(obra.Id))
                throw new ArgumentException(
                    "Favoritos não podem se repetir.");

            novos.Add(new Favorito(Id, obra.Id, i + 1));
        }

        NomeExibicao = nomeExibicao.Trim();
        FotoUrl = string.IsNullOrWhiteSpace(fotoUrl) ? null : fotoUrl;

        _favoritos.Clear();
        _favoritos.AddRange(novos);
    }

    public Avaliacao AtribuirOuAlterarNota(Obra obra, int nota)
    {
        obra.Validar();

        var avaliacao = _avaliacoes.SingleOrDefault(
            a => a.ObraId == obra.Id);

        if (avaliacao is not null)
        {
            avaliacao.AlterarNota(nota);
            return avaliacao;
        }

        avaliacao = new Avaliacao(Id, obra.Id, nota);
        _avaliacoes.Add(avaliacao);

        return avaliacao;
    }

    public Lista CriarLista(string nome)
    {
        var lista = new Lista(
            Guid.NewGuid(),
            Id,
            nome,
            TipoLista.PERSONALIZADA);

        _listas.Add(lista);

        return lista;
    }

    public Lista ObterLista(Guid listaId)
    {
        return _listas.SingleOrDefault(lista => lista.Id == listaId)
            ?? throw new InvalidOperationException(
                "A lista não pertence a este leitor ou não existe.");
    }

    public Lista ObterQueroLer()
    {
        return _listas.Single(
            lista => lista.Tipo == TipoLista.QUERO_LER);
    }

    public void SalvarObraNaLista(Guid listaId, Obra obra)
    {
        ObterLista(listaId).AdicionarObra(obra);
    }

    public bool RemoverObraDaLista(Guid listaId, Guid obraId)
    {
        return ObterLista(listaId).RemoverObra(obraId);
    }

    public void CurtirObra(Obra obra, DateTimeOffset agora)
    {
        obra.Validar();

        if (_curtidas.Any(c => c.ObraId == obra.Id))
            return;

        _curtidas.Add(new Curtida(Id, obra.Id, agora));
    }

    public bool RetirarCurtida(Guid obraId)
    {
        var curtida = _curtidas.SingleOrDefault(
            c => c.ObraId == obraId);

        return curtida is not null && _curtidas.Remove(curtida);
    }

    public IReadOnlyList<Curtida> ObterCurtidasRecentes()
    {
        return _curtidas
            .OrderByDescending(c => c.DataCurtida)
            .ToList()
            .AsReadOnly();
    }
}