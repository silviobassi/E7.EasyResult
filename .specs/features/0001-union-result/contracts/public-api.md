# Contrato público e migração

## Requisito mínimo

A nova API exige .NET 11/C# 15. Não há compatibilidade de runtime, ABI ou herança com a versão anterior. A versão
principal é tratada como major nesta feature: o workflow usa `DEFAULT_BUMP: major`, e `PackageVersion` continua vindo
do output `new_tag` da action. Esta decisão é a autorização registrada para a implementação desta feature; a publicação
real ainda requer revisão do release.

| Área legada | API union | Impacto e migração |
|---|---|---|
| `Result.Success()` | `Result.Success()` / `Success` | Sem mudança de chamada; inspecionar com pattern matching |
| `Result<T>.Success(value)` / conversão implícita de `T` | `Result<T>.Success(value)` ou `new Success<T>(value)` | Requer .NET 11/C# 15; usar factory quando `T` sobrepõe erro/notificação |
| `Result<T>.Create(value)` | `Result<T>.Create(value)` | Mantém rejeição de payload nulo |
| `Result.Create(Task<T>)` | `await Result.Create(task)` | O retorno continua `Task<Result<T>>`; o chamador deve aguardar |
| `Result.Failure(error)` | `Result.Failure(error)` | O caso agora é união; `Error` pode ser nulo |
| `Result.Failure(notifications)` | `Result.Failure(notifications)` | A coleção é copiada e sua ordem/multiplicidade são preservadas |
| `Result<T>.Failure(error, notifications)` | `Result<T>.Failure(error, notifications)` ou `new Failure(error, notifications)` | Preserva erro e notificações; o construtor copia a entrada |
| `IsSuccess`, `Value`, `Error`, `Notifications` | propriedades de estado e pattern matching em `Success<T>`/`Failure` | `.Value` exige `Success<T>`; `default` não é sucesso nem falha |
| `Result<T> : Result` | `Result<T>` e `Result` independentes | Ruptura de fonte/ABI; remover atribuições baseadas em herança |
| `Ensure(result, predicate, error/notification)` | extensões sobre `Result<T>` | Falha não executa predicado e preserva falha anterior |
| `Ensure(Task<Result<T>>, predicate, error)` | `await task.Ensure(predicate, error)` | Não bloqueia com `.Result` |
| `Combine` | `first.Combine(second)` | Sucesso retorna `(T1,T2)`; falha concatena notificações esquerda-direita e escolhe primeiro erro |
| `Tap(Result<T>, Action<T>)` | `result.Tap(action)` | Executa somente em sucesso e retorna o mesmo resultado |
| `Tap(Result<T>, Func<Task<T>>)` | `await result.Tap(action)` | Aguarda efeito assíncrono somente em sucesso |
| `Tap(Task<Result<T>>, Func<T,Task>)` | `await task.Tap(action)` | Aguarda a tarefa sem bloqueio síncrono |
| `Map(Result<T>, Func<T,TOut>)` | `result.Map(mapping)` | Falha preserva erro/notificações; callback não executa |
| `Map(Result<T>, Func<T,Task<TOut>>)` | `await result.Map(mapping)` | Aguarda callback; exceção permanece exceção |
| `Bind(Result<T>, Func<T,Result<TOut>>)` | `result.Bind(function)` | Falha interrompe sem callback |
| `BindAsync` síncrono/assíncrono | `await result.BindAsync(function)` | Todas as variantes usam `await ConfigureAwait(false)` |
| `MapFailure` | `result.MapFailure(function)` | Transforma apenas erro não nulo e preserva notificações |
| `TryCatch` | `result.TryCatch(function, appError)` | Único limite explícito que converte exceção em falha |
| `Match` | `result.Match(onSuccess, onFailure)` | Seleciona o caso; o callback recebe `AppError?` e deve tratar falha somente com notificações |

## Invariantes

Erro principal e notificações nunca são descartados na propagação. Ordem e multiplicidade são observáveis. Erro de
consumidor e `HttpError` continuam extensíveis e consultáveis. `default` é estado inválido documentado; sucesso nulo é
possível somente como payload explícito de `Success<T>`.

## Superfícies a comparar

Inventariar e testar criação, inspeção, erros, notificações, `Ensure`, `Combine`, `Tap`, `Map`, `Bind`, `MapFailure`,
`TryCatch`, `Match` e todas as variantes async. Para cada quebra, README deve conter uso antes/depois, impacto e
substituto.
