using Microsoft.AspNetCore.Identity;
using Readex.Models;

namespace Readex.Servicos;

public class CadastroLeitores
{
    private readonly List<Leitor> _leitores = [];
    private readonly IPasswordHasher<Leitor> _hasher;

    public CadastroLeitores()
    {
        _hasher = new PasswordHasher<Leitor>();
    }

    public Leitor CriarConta(
        string nomeUsuario,
        string email,
        string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            throw new ArgumentException("Senha obrigatória.");

        var leitor = new Leitor(
            Guid.NewGuid(),
            nomeUsuario,
            email);

        bool jaCadastrado = _leitores.Any(l =>
            string.Equals(
                l.NomeUsuario,
                leitor.NomeUsuario,
                StringComparison.OrdinalIgnoreCase)
            || string.Equals(
                l.Email,
                leitor.Email,
                StringComparison.OrdinalIgnoreCase));

        if (jaCadastrado)
            throw new InvalidOperationException(
                "Nome de usuário ou e-mail já cadastrado.");

        leitor.DefinirSenhaHash(
            _hasher.HashPassword(leitor, senha));

        _leitores.Add(leitor);

        return leitor;
    }

    public Leitor? VerificarCredenciais(
        string nomeUsuario,
        string senha)
    {
        if (string.IsNullOrWhiteSpace(nomeUsuario)
            || string.IsNullOrEmpty(senha))
            return null;

        var leitor = ObterPorNomeUsuario(nomeUsuario);

        if (leitor is null)
            return null;

        var resultado = _hasher.VerifyHashedPassword(
            leitor,
            leitor.SenhaHash,
            senha);

        if (resultado == PasswordVerificationResult.Failed)
            return null;

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            leitor.DefinirSenhaHash(
                _hasher.HashPassword(leitor, senha));
        }

        return leitor;
    }

    public Leitor? ObterPorId(Guid leitorId)
    {
        return _leitores.SingleOrDefault(
            leitor => leitor.Id == leitorId);
    }

    public Leitor? ObterPorNomeUsuario(string nomeUsuario)
    {
        return _leitores.SingleOrDefault(leitor =>
            string.Equals(
                leitor.NomeUsuario,
                nomeUsuario.Trim(),
                StringComparison.OrdinalIgnoreCase));
    }
}