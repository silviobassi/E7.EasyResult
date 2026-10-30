# Quickstart: Tratamento global de Problem Details

## Pré-requisitos

- SDK .NET configurado pelo `global.json`.
- Repositório restaurado.
- Contrato em [public-api.md](contracts/public-api.md).

## Validar o contrato da biblioteca

Na raiz do repositório:

```bash
dotnet test E7.sln --configuration Release --filter FullyQualifiedName~RailwayExtensionsTests
dotnet test E7.sln --configuration Release --filter FullyQualifiedName~ResultTests
dotnet build E7.sln --configuration Release
```

Resultados esperados: falhas HTTP preservam o erro principal; falhas somente com notificações preservam todas as
notificações; o overload novo recebe `Failure`; o overload legado com `AppError?` continua compilando; e o caminho de
sucesso não é executado para falhas.

## Validar o adaptador consumidor

1. Crie ou use um consumidor HTTP de prova.
2. Centralize a conversão de `Failure` para Problem Details em um único handler:

   ```csharp
   return result.Match(
       order => Results.Ok(order),
       onFailure: (Failure failure) => problemDetailsHandler.Handle(failure));
   ```

3. Execute um caso com `HttpError`, um `AppError` comum e uma falha somente com notificações.
4. Confirme, respectivamente, o status do erro, `400`, e `400`, além de detalhes e notificações preservados.
5. Compile também um consumidor usando `onFailure: error => HandleLegacyError(error)` para validar compatibilidade.
6. Adicione outro `HttpError` sem modificar os endpoints e confirme que o status é aplicado pelo handler central.

O consumidor de prova deste recurso pode ser executado diretamente da raiz:

```bash
dotnet run --project .specs/features/0002-global-problem-details/proof/Consumer/Consumer.csproj --configuration Release
```

Uma execução sem saída e código `0` confirma os cenários de `404`, `400` com notificações e `409` para um
`HttpError` adicionado sem alterar os endpoints.

## Critérios de conclusão

- Nenhum endpoint de prova contém um `switch` por tipo de erro.
- O handler central é o único lugar que conhece a política HTTP.
- Um consumidor não HTTP compila e executa sem registrar o handler.
