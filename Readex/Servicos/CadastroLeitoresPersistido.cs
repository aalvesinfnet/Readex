using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Readex.Infraestrutura;
using Readex.Models;

namespace Readex.Servicos;

public sealed class CadastroLeitoresPersistido
{
    private const int TamanhoMinimoSenha = 8;
    private const int TamanhoMaximoSenha = 128;
    private readonly ReadexDbContext _banco;
    private readonly IPasswordHasher<Leitor> _hasher;

    public CadastroLeitoresPersistido(ReadexDbContext banco, IPasswordHasher<Leitor> hasher)
    {
        _banco = banco;
        _hasher = hasher;
    }

    public async Task<bool> CriarContaAsync(
        string nomeUsuario, string email, string senha, CancellationToken cancelamento)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length is < TamanhoMinimoSenha or > TamanhoMaximoSenha)
            throw new ArgumentException("Password must have between 8 and 128 characters.", nameof(senha));

        var leitor = new Leitor(Guid.NewGuid(), nomeUsuario, email);

        var jaCadastrado = await _banco.Leitores.AnyAsync(item =>
            item.NomeUsuarioNormalizado == leitor.NomeUsuarioNormalizado ||
            item.EmailNormalizado == leitor.EmailNormalizado, cancelamento);
        if (jaCadastrado)
            return false;

        leitor.DefinirSenhaHash(_hasher.HashPassword(leitor, senha));
        _banco.Leitores.Add(leitor);
        try
        {
            await _banco.SaveChangesAsync(cancelamento);
        }
        catch (DbUpdateException erro) when (erro.InnerException is SqliteException { SqliteExtendedErrorCode: 2067 })
        {
            // A simultaneous signup can succeed after the duplicate check above.
            _banco.Entry(leitor.ObterQueroLer()).State = EntityState.Detached;
            _banco.Entry(leitor).State = EntityState.Detached;
            return false;
        }

        return true;
    }

    public async Task<Leitor?> VerificarCredenciaisAsync(
        string nomeUsuario, string senha, CancellationToken cancelamento)
    {
        if (string.IsNullOrWhiteSpace(nomeUsuario) || nomeUsuario.Trim().Length > 30 ||
            string.IsNullOrEmpty(senha) || senha.Length > TamanhoMaximoSenha)
            return null;

        var nomeNormalizado = nomeUsuario.Trim().ToUpperInvariant();
        var leitor = await _banco.Leitores.SingleOrDefaultAsync(
            item => item.NomeUsuarioNormalizado == nomeNormalizado, cancelamento);
        if (leitor is null || string.IsNullOrEmpty(leitor.SenhaHash))
            return null;

        var resultado = _hasher.VerifyHashedPassword(leitor, leitor.SenhaHash, senha);
        if (resultado == PasswordVerificationResult.Failed)
            return null;

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            leitor.DefinirSenhaHash(_hasher.HashPassword(leitor, senha));
            await _banco.SaveChangesAsync(cancelamento);
        }

        return leitor;
    }
}
