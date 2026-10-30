# Validação da migração

Execute na raiz com o SDK do `global.json`.

## Prova de unions

```bash
dotnet build .specs/features/0001-union-result/proof/Consumer/Consumer.csproj --configuration Release --nologo --verbosity quiet
dotnet run --project .specs/features/0001-union-result/proof/Consumer/Consumer.csproj --configuration Release --no-build
```

Resultado esperado: build sem warnings/erros e `Passed: 12 union proof checks.`. A prova deve continuar cobrindo
conversões, casos sobrepostos, `default`, `null`, pattern matching, propagação de falha e tarefa incompleta.

## Biblioteca

```bash
dotnet restore E7.sln
dotnet test E7.sln --filter FullyQualifiedName~RailwayExtensionsTests
dotnet test E7.sln --configuration Release
dotnet build E7.sln --configuration Release
```

Os testes devem cobrir falhas com erro/notificações, `Combine`, callbacks, custom errors e async sem `.Result`.
Resultado observado: `Passed: 36, Failed: 0`.

## Pacote e consumidor

Em uma cópia temporária do checkout, copie `README.md` e `LICENSE` para `src/E7.EasyResult/`, execute:

```bash
dotnet pack src/E7.EasyResult/ --configuration Release -o packages
```

Inspecione o `.nupkg` para TFM `net11.0`, README, LICENSE e metadados. Crie um consumidor temporário `net11.0`,
instale o pacote localmente e execute sucesso, falha com notificações e erro customizado. Não faça `dotnet nuget push`.
Resultado observado: pacote local criado com `lib/net11.0`, README, LICENSE e nuspec válido. Consumidor externo
temporário `net11.0` instalado a partir da fonte local e executado com sucesso (`Package consumer passed.`).

## Pipeline

Revisar `.github/workflows/production.yml`: trigger em `main`/manual, SDK .NET 11, restore, testes antes da tag,
`github-tag-action`, `PackageVersion` derivada do output, credenciais existentes, pack, README/LICENSE e push para
nuget.org. A publicação hospedada só é `passed` após execução real; revisão local é evidência parcial.
Resultado local: `passed` para revisão estrutural; publicação real `not applicable` nesta execução. O consumidor da prova
também passou contra a biblioteca real com `Passed: 12 union proof checks.`.
