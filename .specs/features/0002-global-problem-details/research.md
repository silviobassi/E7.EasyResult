# Research: Tratamento global de Problem Details

## Contexto histórico

- O commit imediatamente anterior à migração (`121065a`) ainda continha a hierarquia de resultados baseada em classes.
- O commit atual (`5306adc`) introduziu `Success<T>`/`Failure` e exemplos com `error switch` nos limites HTTP do README.
- Antes da migração, `Match` entregava `AppError` ao callback de falha; após a migração, o callback recebe `AppError?`,
  enquanto as notificações permanecem em `Result<T>.Notifications`.

## Decisão: manter o núcleo agnóstico de HTTP

**Decision**: não adicionar dependência de ASP.NET Core ou retornar `ProblemDetails` diretamente pela biblioteca.

**Rationale**: `E7.EasyResult` é uma biblioteca de resultados usada também fora de HTTP. Acoplar o núcleo a um framework
web reduziria a reutilização e transferiria para ele uma responsabilidade da aplicação consumidora.

**Alternatives considered**:

- Adicionar `Microsoft.AspNetCore.*`: rejeitado por aumentar o acoplamento e o grafo de dependências.
- Fazer cada endpoint manter seu próprio `switch`: rejeitado por repetir a política e divergir do comportamento anterior.
- Lançar exceções para que um middleware as converta: rejeitado por alterar falhas como valores.

## Decisão: expor a falha completa ao adaptador

**Decision**: adicionar um overload de `Match` cujo callback recebe `Failure`, mantendo o overload existente cujo
callback recebe `AppError?`; o adaptador central usa o overload novo.

**Rationale**: um callback que recebe somente `AppError?` não consegue preservar notificações no Problem Details sem
consultar o resultado por fora. `Failure` já é a unidade que contém todas as informações necessárias.

**Example**:

```csharp
return result.Match(
    onSuccess: order => Results.Ok(order),
    onFailure: failure => problemDetailsHandler.Handle(failure));
```

**Alternatives considered**:

- Continuar passando apenas `AppError?`: rejeitado porque falhas somente com notificações ficam incompletas.
- Expor uma classe específica de resposta HTTP: rejeitado pelo acoplamento.
- Serializar a falha dentro da biblioteca: rejeitado porque formato e extensões pertencem ao consumidor.

## Decisão: compatibilidade de `Match`

**Decision**: manter o callback legado baseado em `AppError?` e adicionar uma variante explícita baseada em `Failure`.

**Rationale**: consumidores existentes continuam compilando, enquanto novos handlers conseguem acessar erro e
notificações em uma única chamada.

**Example**:

```csharp
result.Match(
    onSuccess: value => Format(value),
    onFailure: error => FormatLegacy(error));
```

O novo overload deve evitar ambiguidade de inferência e documentar que `Failure.Error` pode ser nulo.

## Decisão: política de status

**Decision**: o adaptador usa o status de `HttpError`; usa `400 Bad Request` para `AppError` comum ou falha somente
com notificações, preservando a política demonstrada no README anterior.

**Rationale**: mantém o comportamento observado no exemplo legado e torna a migração determinística.

**Alternatives considered**:

- Mapear todo erro comum para `500`: rejeitado porque validações e erros de domínio já são respostas de cliente.
- Introduzir tabela configurável de status no pacote: rejeitado por escopo; aplicações podem adaptar erros próprios.

## Decisão: validação

**Decision**: cobrir a política com testes unitários do contrato de `Failure`/`Match` e com consumidor de prova que
simula o adaptador global, além de atualizar os exemplos de migração.

**Rationale**: não há aplicação HTTP hospedada no repositório; uma prova de consumidor verifica o contrato público sem
transformar a biblioteca em um web service.
