using Readex.Models;
using Readex.Servicos;
using Readex.Infraestrutura;

int verificacoes = 0;

void Verificar(bool condicao, string descricao)
{
    verificacoes++;

    if (!condicao)
    {
        throw new Exception("Falhou: " + descricao);
    }
}

void DeveRejeitar<T>(Action acao, string descricao) where T : Exception
{
    verificacoes++;

    try
    {
        acao();
    }
    catch (T)
    {
        return;
    }

    throw new Exception("Não rejeitou: " + descricao);
}

var a = new Obra
{
    Id = Guid.NewGuid(),
    Titulo = "Exemplo; com \"aspas\"",
    Autor = "Autor A",
    Genero = "Romance",
    Tipo = TipoObra.LIVRO,
    Destaque = true
};

var b = new Obra
{
    Id = Guid.NewGuid(),
    Titulo = "Obra B",
    Autor = "Autor B",
    Genero = "Aventura",
    Tipo = TipoObra.HQ
};

var c = new Obra
{
    Id = Guid.NewGuid(),
    Titulo = "Obra C",
    Autor = "Autor C",
    Genero = "Aventura",
    Tipo = TipoObra.MANGA
};

a.Validar();

Verificar(a.CorrespondePesquisa("EXEMPLO"), "pesquisa título");
Verificar(a.CorrespondePesquisa("autor a"), "pesquisa autor");
Verificar(a.CorrespondePesquisa("romance"), "pesquisa gênero");
Verificar(!a.CorrespondePesquisa(" "), "termo vazio");

DeveRejeitar<InvalidDataException>(
    () => new Obra().Validar(), "obra inválida");

var catalogo = new Catalogo([a, b, c]);

Verificar(catalogo.Obras.Count == 3, "catálogo carregado");
Verificar(catalogo.ObterDestaques().Single().Id == a.Id, "destaques");
Verificar(catalogo.Pesquisar("aventura").Count == 2, "pesquisa catálogo");
Verificar(catalogo.ObterPorId(b.Id) == b, "detalhes obra");
Verificar(catalogo.ObterPorId(Guid.NewGuid()) is null, "obra indisponível");

DeveRejeitar<ArgumentException>(
    () => new Catalogo([a, a]), "catálogo duplicado");

var cadastro = new CadastroLeitores();
var leitor = cadastro.CriarConta("miguel", "miguel@example.com", "senha-teste");
var outro = cadastro.CriarConta("outro", "outro@example.com", "senha-teste");

Verificar(leitor.Listas.Count == 1 && leitor.ObterQueroLer().Tipo == TipoLista.QUERO_LER,
    "conta e única lista padrão");
Verificar(leitor.ObterQueroLer().Itens.Count == 0, "lista padrão vazia");
Verificar(leitor.NomeExibicao == "miguel", "perfil inicial");
Verificar(cadastro.ObterPorId(leitor.Id) == leitor, "consulta por id");
Verificar(cadastro.ObterPorNomeUsuario("MIGUEL") == leitor, "consulta por nome de usuário");

DeveRejeitar<InvalidOperationException>(
    () => cadastro.CriarConta("MIGUEL", "novo@example.com", "teste"),
    "nome de usuário duplicado");

DeveRejeitar<InvalidOperationException>(
    () => cadastro.CriarConta("novo", "MIGUEL@example.com", "teste"),
    "e-mail duplicado");

Verificar(cadastro.VerificarCredenciais("miguel", "senha-teste") == leitor,
    "credenciais válidas");
Verificar(cadastro.VerificarCredenciais("miguel", "senha-incorreta") is null,
    "senha incorreta");
Verificar(cadastro.VerificarCredenciais("inexistente", "senha-teste") is null,
    "conta inexistente");

var identificacao = leitor.ObterIdentificacaoPublica();

Verificar(identificacao.NomeUsuario == "miguel" && identificacao.NomeExibicao == "miguel",
    "identificação pública");

var quero = leitor.ObterQueroLer();
leitor.SalvarObraNaLista(quero.Id, a);

var nota = leitor.AtribuirOuAlterarNota(a, 1);
var mesma = leitor.AtribuirOuAlterarNota(a, 5);

Verificar(ReferenceEquals(nota, mesma) && leitor.Avaliacoes.Count == 1 && mesma.Nota == 5,
    "atualização de nota mantém um registro");

DeveRejeitar<ArgumentOutOfRangeException>(
    () => leitor.AtribuirOuAlterarNota(a, 0), "nota abaixo do intervalo");

DeveRejeitar<ArgumentOutOfRangeException>(
    () => leitor.AtribuirOuAlterarNota(a, 6), "nota acima do intervalo");

Verificar(mesma.Nota == 5 && quero.Itens.Count == 1, "nota inválida preserva dados");

var lista = leitor.CriarLista("Minha lista");

Verificar(lista.Itens.Count == 0 && leitor.Listas.Count == 2,
    "lista personalizada vazia");

leitor.SalvarObraNaLista(lista.Id, a);
leitor.SalvarObraNaLista(lista.Id, a);

Verificar(lista.Itens.Count == 1 && quero.Itens.Count == 1,
    "obra única por lista e permitida em várias");

Verificar(leitor.RemoverObraDaLista(quero.Id, a.Id), "remover vínculo selecionado");

Verificar(quero.Itens.Count == 0 && lista.Itens.Count == 1 && leitor.Avaliacoes.Count == 1,
    "remoção preserva outras listas e nota");

Verificar(!leitor.RemoverObraDaLista(quero.Id, a.Id), "remoção já ausente");

DeveRejeitar<InvalidOperationException>(
    () => outro.SalvarObraNaLista(lista.Id, a), "lista de outro leitor");

DeveRejeitar<ArgumentException>(
    () => leitor.CriarLista(" "), "nome de lista vazio");

Verificar(leitor.Listas.Count(x => x.Tipo == TipoLista.QUERO_LER) == 1,
    "lista padrão não duplicada");

leitor.PersonalizarPerfil("Miguel Alves", null, new Obra?[] { a, null, b });

Verificar(leitor.Favoritos.Count == 2
    && leitor.Favoritos[0].Posicao == 1
    && leitor.Favoritos[1].Posicao == 3,
    "favoritos com posição vazia");

DeveRejeitar<ArgumentException>(
    () => leitor.PersonalizarPerfil("Outro nome", null, new Obra?[] { a, a }),
    "favorito repetido");

DeveRejeitar<ArgumentException>(
    () => leitor.PersonalizarPerfil("Outro nome", null, new Obra?[] { a, b, c, null, null }),
    "quinta posição");

DeveRejeitar<ArgumentException>(
    () => leitor.PersonalizarPerfil(" ", null, new Obra?[] { a }),
    "nome de exibição vazio");

Verificar(leitor.NomeExibicao == "Miguel Alves" && leitor.Favoritos.Count == 2,
    "edição inválida preserva perfil inteiro");

leitor.PersonalizarPerfil("Miguel Alves", null, new Obra?[] { null, null, b });

Verificar(leitor.Favoritos.Count == 1 && leitor.Favoritos[0].Posicao == 3,
    "retirar favorito mantém espaços");

var antes = new DateTimeOffset(2026, 11, 19, 10, 0, 0, TimeSpan.FromHours(-3));

leitor.CurtirObra(a, antes);
leitor.CurtirObra(a, antes.AddMinutes(1));
leitor.CurtirObra(b, antes.AddMinutes(2));

Verificar(leitor.Curtidas.Count == 2
    && leitor.Curtidas.Single(x => x.ObraId == a.Id).DataCurtida == antes,
    "curtida única preserva instante");

Verificar(leitor.ObterCurtidasRecentes()[0].ObraId == b.Id, "likes recentes ordenados");
Verificar(leitor.Curtidas.All(x => x.DataCurtida.Offset == TimeSpan.Zero), "datas em UTC");
Verificar(leitor.RetirarCurtida(a.Id), "retirar like");
Verificar(leitor.ObterCurtidasRecentes().All(x => x.ObraId != a.Id),
    "like retirado ausente das consultas");

Verificar(leitor.Avaliacoes.Count == 1 && lista.Itens.Count == 1 && leitor.Favoritos.Count == 1,
    "like não altera demais registros");

leitor.CurtirObra(a, antes.AddMinutes(3));

Verificar(leitor.ObterCurtidasRecentes()[0].ObraId == a.Id,
    "nova curtida tem novo instante");

var pasta = Path.Combine(AppContext.BaseDirectory, "dados-verificacao");
Directory.CreateDirectory(pasta);

var caminho = Path.Combine(pasta, "obras-verificacao.csv");
var csv = new ObraCsv();

csv.Gravar(caminho, [a, b, c]);

var relidas = csv.Ler(caminho);

Verificar(relidas.Count == 3 && relidas[0].Titulo == a.Titulo && relidas[0].CapaUrl is null,
    "CSV preservado após mover validação para domínio");

var antesArquivo = File.ReadAllText(caminho);

DeveRejeitar<InvalidDataException>(
    () => csv.Gravar(caminho, [new Obra()]), "CSV rejeita obra inválida");

Verificar(File.ReadAllText(caminho) == antesArquivo,
    "CSV inválido preserva arquivo anterior");

Console.WriteLine($"SUCESSO: {verificacoes} verificações passaram.");
Console.WriteLine($"CSV de verificação: {caminho}");