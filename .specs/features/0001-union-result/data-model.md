# Modelo de dados

## `Result<T>` e `Result`

Unions fechadas que representam exatamente os casos de sucesso com valor, sucesso sem valor e falha:

- `Result<T>`: `Success<T>` ou `Failure`.
- `Result`: `Success` ou `Failure`.
- `Success<T>.Value`: pode ser `null` quando `T` for anulável; é diferente de `default(Result<T>)`.
- `Failure.Error`: `AppError?`, preservando subclasses de consumidor e `HttpError`.
- `Failure.Notifications`: coleção ordenada, com duplicatas preservadas e cópia defensiva/imutável após criação.

`default` de uma union pode não conter caso. Operações públicas que precisam de um caso devem rejeitá-lo explicitamente
com a exceção de estado inválido documentada; não deve ser interpretado como sucesso ou falha silenciosa.

`Failure` copia a sequência recebida em seu construtor público e expõe somente uma leitura, portanto alterações
posteriores na coleção original não alteram o resultado.

## Operações

`Map`, `Bind`, `Ensure`, `Tap`, `MapFailure` e `Match` preservam `Failure` integralmente e não executam callbacks de
sucesso em falha. `Combine` retorna `(T1, T2)` para dois sucessos; em falha concatena notificações esquerda-direita e
seleciona o primeiro erro não nulo. Variantes async aguardam a tarefa sem bloqueio síncrono.

## Validações

Testar falha apenas com erro, apenas com notificações, com ambos, com erro nulo, coleção vazia, notificações repetidas e
ordem. Testar `Result<AppError>` tanto com `Success<AppError>` quanto com `Failure` para comprovar a desambiguação.
