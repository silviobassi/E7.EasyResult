# Public API Contract: Tratamento global de Problem Details

## Result matching

O pacote deve adicionar um overload de seleção em que o callback de falha recebe o caso `Failure` completo:

```csharp
result.Match(
    onSuccess: value => HandleSuccess(value),
    onFailure: failure => HandleFailure(failure));
```

`failure.Error` pode ser nulo. `failure.Notifications` deve conter a coleção ordenada associada ao resultado.

O overload existente baseado em `AppError?` permanece disponível para compatibilidade:

```csharp
result.Match(
    onSuccess: value => HandleSuccess(value),
    onFailure: error => HandleLegacyError(error));
```

O handler global deve usar a variante com `Failure`:

```csharp
static IResult HandleFailure(Failure failure)
{
    var error = failure.Error;
    var statusCode = error is HttpError httpError
        ? (int)httpError.GetHttpStatusCode()
        : StatusCodes.Status400BadRequest;

    return Results.Problem(
        detail: error?.Message ?? "Validation failed.",
        statusCode: statusCode,
        extensions: new Dictionary<string, object?>
        {
            ["notifications"] = failure.Notifications
        });
}
```

## Consumer-owned HTTP adapter

O adaptador central do consumidor deve seguir esta política:

| Falha | Status | Conteúdo |
|---|---:|---|
| `Failure.Error` é `HttpError` | Status retornado pelo erro | Mensagem, código/classificação e notificações |
| `Failure.Error` é outro `AppError` | 400 | Mensagem, código/classificação e notificações |
| Sem erro principal | 400 | Fallback e notificações |

O adaptador é a fronteira HTTP; o pacote não deve depender de tipos de resposta HTTP.

## Compatibility contract

- O fluxo de sucesso não muda.
- Falhas somente com notificações continuam sendo falhas e não invocam o callback de sucesso.
- Consumidores não HTTP podem ignorar o adaptador e continuar usando `Result`, `Map`, `Bind`, `Ensure` e `Match`.
- O callback legado baseado somente em `AppError?` permanece disponível; ele não substitui o callback completo quando
  notificações precisam ser serializadas.
