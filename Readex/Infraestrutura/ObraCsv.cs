using System.Text;
using Microsoft.VisualBasic.FileIO;
using Readex.Models;

namespace Readex.Infraestrutura;

public class ObraCsv {
    private static readonly string[] Cabecalho = [
        "id", "titulo", "autor", "genero",
        "tipo", "capaUrl", "destaque"
    ];

    public List<Obra> Ler(string caminho) {
        using var leitor = new TextFieldParser(
            caminho,
            new UTF8Encoding(false, true),
            detectEncoding: true
        );

        leitor.TextFieldType = FieldType.Delimited;
        leitor.SetDelimiters(";");
        leitor.HasFieldsEnclosedInQuotes = true;
        leitor.TrimWhiteSpace = false;

        try {
            var campos = leitor.ReadFields();

            if (campos is null || !campos.SequenceEqual(Cabecalho))
                throw new InvalidDataException("Cabeçalho CSV inválido.");

            var obras = new List<Obra>();
            var ids = new HashSet<Guid>();

            while (!leitor.EndOfData) {
                campos = leitor.ReadFields();

                if (campos is null || campos.Length != Cabecalho.Length)
                    throw new InvalidDataException(
                        "Registro CSV com quantidade de campos inválida."
                    );

                if (!Guid.TryParseExact(campos[0], "D", out var id))
                    throw new InvalidDataException(
                        "O id deve ser um UUID válido no formato com hífens."
                    );

                if (!Enum.GetNames<TipoObra>().Contains(campos[4]))
                    throw new InvalidDataException(
                        "O tipo deve ser LIVRO, HQ ou MANGA."
                    );

                if (!bool.TryParse(campos[6], out var destaque))
                    throw new InvalidDataException(
                        "O destaque deve ser true ou false."
                    );

                var obra = new Obra {
                    Id = id,
                    Titulo = campos[1],
                    Autor = campos[2],
                    Genero = campos[3],
                    Tipo = Enum.Parse<TipoObra>(campos[4]),
                    CapaUrl = campos[5].Length == 0 ? null : campos[5],
                    Destaque = destaque
                };

                Validar(obra);

                if (!ids.Add(obra.Id))
                    throw new InvalidDataException(
                        "Há ids de obras repetidos no CSV."
                    );

                obras.Add(obra);
            }

            return obras;
        }
        catch (MalformedLineException) {
            throw new InvalidDataException(
                "CSV malformado: confira delimitadores e aspas."
            );
        }
    }

    public void Gravar(string caminho, IEnumerable<Obra> obras) {
        var registros = obras.ToList();
        var ids = new HashSet<Guid>();

        foreach (var obra in registros)
        {
            Validar(obra);

            if (!ids.Add(obra.Id))
                throw new InvalidDataException(
                    "Há ids de obras repetidos para gravar."
                );
        }

        var destino = Path.GetFullPath(caminho);
        Directory.CreateDirectory(Path.GetDirectoryName(destino)!);

        var temporario =
            destino + "." + Guid.NewGuid().ToString("N") + ".tmp";

        try
        {
            using (var escritor = new StreamWriter(
                temporario,
                false,
                new UTF8Encoding(false)))
            {
                escritor.WriteLine(string.Join(";", Cabecalho));

                foreach (var obra in registros)
                {
                    string[] campos =
                    [
                        obra.Id.ToString("D"),
                    obra.Titulo,
                    obra.Autor,
                    obra.Genero,
                    obra.Tipo.ToString(),
                    obra.CapaUrl ?? "",
                    obra.Destaque ? "true" : "false"
                    ];

                    escritor.WriteLine(
                        string.Join(";", campos.Select(Escapar))
                    );
                }
            }

            File.Move(temporario, destino, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporario))
                File.Delete(temporario);
        }
    }

    private static string Escapar(string valor)
    {
        if (valor.IndexOfAny([';', '"', '\r', '\n']) < 0)
            return valor;

        return "\"" + valor.Replace("\"", "\"\"") + "\"";
    }

    private static void Validar(Obra obra) {
        obra.Validar();
    }
}