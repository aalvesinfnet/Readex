# Readex

Readex é uma aplicação web para explorar livros, mangás e HQs. Foi desenvolvida como Projeto de Bloco do Instituto Infnet em 2026.

## Funcionalidades

- Navegação pública pelo catálogo, com busca por título, autor ou gênero e filtros por categoria.
- Página de detalhes com capa, autoria e informações da obra.
- Cadastro, login e saída da conta. Cada conta recebe a lista padrão **Quero ler**.

As seções **Popular this week**, **Highest rated** e **Popular lists** exibem dados de demonstração.

## Como rodar

O projeto usa C#, ASP.NET Core Razor Pages e EF Core com SQLite. Instale o SDK do .NET 10 e execute os comandos na raiz do repositório.

```sh
dotnet tool install --global dotnet-ef --version 10.0.15
dotnet ef database update --project Readex --startup-project Readex --context ReadexDbContext
dotnet run --project Readex --launch-profile https
```

Se já tiver o `dotnet-ef` 10 instalado, comece pelo segundo comando. As migrações criam o banco local e carregam o catálogo inicial. Acesse [https://localhost:7163](https://localhost:7163). Se o navegador pedir um certificado de desenvolvimento confiável, execute `dotnet dev-certs https --trust`.
