# Exemplos da implementação proposta

Proposta para discussão, alinhada às respostas Q1: A e Q2: A. Não altera a biblioteca atual nem fixa o contrato final.

## 1. Resultado com casos explícitos

```csharp
public sealed record Success<T>(T Value);

public sealed record Failure(
    AppError? Error,
    IReadOnlyList<Notification> Notifications);

public union Result<T>(Success<T>, Failure);
```

O consumidor distingue sucesso e falha pelo caso. `AppError` e `Notification` continuam representando os conceitos existentes. `Failure` reúne erro e notificações para que possam ser propagados juntos. A hierarquia de erros permanece extensível pelo consumidor.

Encapsular o valor em `Success<T>` também distingue sucesso com valor do tipo `AppError` de uma falha. A nova API não depende da herança antiga entre resultados genéricos e não genéricos. O equivalente sem valor será desenhado como sucesso sem payload ou a mesma falha.

## 2. Uso pelo consumidor

```csharp
Result<int> success = new Success<int>(42);

Result<int> failure = new Failure(
    new ElementNotFoundError(),
    new[] { new Notification("order.missing", "Order not found.") });

string message = success switch
{
    Success<int> value => $"Created: {value.Value}",
    Failure error => error.Error?.Message ?? "Validation failed.",
    _ => throw new InvalidOperationException("Uninitialized result.")
};
```

Os tipos de erro existentes devem continuar disponíveis. O tratamento acima de resultado não inicializado é uma proposta: a especificação exige que esse caso seja definido e documentado, mas ainda não escolheu essa exceção como contrato. O acesso ao valor passa pelo caso de sucesso, em vez de assumir que todo resultado contém um valor útil.

## 3. Map preservando a falha inteira

```csharp
public static Result<TOut> Map<TIn, TOut>(
    this Result<TIn> result,
    Func<TIn, TOut> map)
    => result switch
    {
        Success<TIn> success => new Success<TOut>(map(success.Value)),
        Failure failure => failure,
        _ => throw new InvalidOperationException("Uninitialized result.")
    };
```

Quando há falha, o callback não executa. O mesmo payload de falha é propagado, incluindo erro e notificações, sem recriar uma falha contendo apenas o erro. Essa abordagem será aplicada às operações relevantes, incluindo validação e encadeamento.

## 4. MapAsync sem bloqueio síncrono

```csharp
public static async Task<Result<TOut>> MapAsync<TIn, TOut>(
    this Task<Result<TIn>> task,
    Func<TIn, Task<TOut>> map)
{
    var result = await task.ConfigureAwait(false);

    return result switch
    {
        Success<TIn> success => new Success<TOut>(
            await map(success.Value).ConfigureAwait(false)),
        Failure failure => failure,
        _ => throw new InvalidOperationException("Uninitialized result.")
    };
}
```

O encadeamento usa `await`, preserva a falha inteira e não envolve captura indiscriminada de exceções. A nomenclatura `Async` segue o harness. O planejamento deverá enumerar mudanças dos overloads antigos e apresentar a migração correspondente.

## 5. Evidência comportamental prevista

- Falha com erro e duas notificações atravessa Map/MapAsync/Ensure/Bind sem perder dados; callbacks de sucesso não executam.
- Tarefa pendente permite que a operação assíncrona devolva controle ao chamador; após sua conclusão, o resultado esperado é observado.
- Combine preserva ordem e multiplicidade das notificações e escolhe o primeiro erro não nulo.
- Um erro definido no consumidor continua utilizável.
- Valor nulo, resultado padrão, coleções e conversões recebem contratos explícitos antes de implementar a API final.

Usar xUnit/Shouldly existentes. Rodar testes focados durante mudanças, suíte Release por alteração de contrato compartilhado e um build final conforme o harness. Não adicionar novas camadas, frameworks de union ou ferramentas de análise para esses exemplos.

## Verificação e limites

**Passed:** prova isolada em `/tmp/e7-union-example`, com union genérica, Map e MapAsync, compilada por `dotnet build /tmp/e7-union-example/Example.csproj --configuration Release --nologo --verbosity quiet`, SDK `11.0.100-rc.1.26425.128`, zero warnings e zero erros. A prova usa tipos mínimos equivalentes para erro e notificação; não comprova integração com a biblioteca nem todos os exemplos de consumidor.

**Unavailable:** nova consulta à referência oficial retornou HTTP 503. A evidência desta rodada é a compilação local; o suporte e a documentação oficial precisam ser reconfirmados no planejamento.

**Not applicable:** build e testes da biblioteca atual, pois não foi alterada. Nenhum teste de comportamento foi executado para a prova; compilar não comprova os cenários acima.

## Resultado do clarify

Nenhuma nova pergunta de requisito foi necessária para apresentar estes exemplos. A especificação e o checklist permanecem inalterados, com 16/16 itens; os exemplos são material de design, a consolidar em `$speckit-plan`. Hooks pre/post não aplicáveis: `.specify/extensions.yml` ausente. As escolhas de implementação, incluindo o tratamento de resultado padrão, ficam para o planejamento e não são decisões aprovadas por este documento.
