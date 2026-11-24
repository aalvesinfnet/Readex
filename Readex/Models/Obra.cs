using System.ComponentModel.DataAnnotations;

namespace Readex.Models;

public class Obra
{
    public Guid Id { get; set; }

    [Required, StringLength(200)]
    public string Titulo { get; set; } = "";

    [Required, StringLength(150)]
    public string Autor { get; set; } = "";

    [Required, StringLength(80)]
    public string Genero { get; set; } = "";

    [EnumDataType(typeof(TipoObra))]
    public TipoObra Tipo { get; set; }

    [StringLength(500)]
    public string? CapaUrl { get; set; }

    public bool Destaque { get; set; }

    [StringLength(100)]
    public string? Slug { get; set; }

    public int? Ano { get; set; }

    [StringLength(80)]
    public string? Formato { get; set; }

    public string? Sinopse { get; set; }

    public void Validar()
    {
        if (Id == Guid.Empty)
            throw new InvalidDataException(
                "O id da obra não pode ser vazio."
            );

        var erros = new List<ValidationResult>();

        bool valida = Validator.TryValidateObject(
            this,
            new ValidationContext(this),
            erros,
            validateAllProperties: true
        );

        if (!valida)
            throw new InvalidDataException(
                "Obra inválida: " +
                string.Join(" ", erros.Select(erro => erro.ErrorMessage))
            );
    }

    public bool CorrespondePesquisa(string termo)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return false;

        termo = termo.Trim();

        return Titulo.Contains(termo, StringComparison.OrdinalIgnoreCase)
            || Autor.Contains(termo, StringComparison.OrdinalIgnoreCase)
            || Genero.Contains(termo, StringComparison.OrdinalIgnoreCase);
    }
}