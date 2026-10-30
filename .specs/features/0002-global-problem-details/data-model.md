# Data Model: Tratamento global de Problem Details

## Failure

Representa o caso de falha do resultado.

| Campo | Tipo conceitual | Regras |
|---|---|---|
| `Error` | erro de aplicação opcional | Pode ser nulo quando há apenas notificações |
| `Notifications` | lista ordenada de notificações | Preserva ordem e multiplicidade; é copiada na criação |

## AppError

Representa o erro principal da operação.

| Campo | Tipo conceitual | Regras |
|---|---|---|
| `Message` | texto | Disponível ao adaptador HTTP |
| `AppErrorType` | classificação | Preservada na conversão |
| `ErrorCodeName` | código | Preservado na conversão |

## HttpError

Especialização de `AppError` que informa o status de transporte.

| Campo | Tipo conceitual | Regras |
|---|---|---|
| `HttpStatusCode` | status HTTP | O adaptador central usa esse status |

## Problem Details resultante

Representação no limite HTTP, criada pelo consumidor.

| Campo | Fonte | Regras |
|---|---|---|
| Status | `HttpError` ou política padrão | `HttpError` vence; caso geral usa 400 |
| Detail | `Error.Message` ou fallback | Nunca produz resposta de sucesso por falha |
| Code/type | `ErrorCodeName` e `AppErrorType` | Preservados quando disponíveis |
| Extensions | `Notifications` | Inclui todas na ordem original |

## Transições

1. `Result<T>` em sucesso: executa a resposta de sucesso.
2. `Result<T>` em `Failure` com `HttpError`: cria Problem Details com o status do erro.
3. `Result<T>` em `Failure` com outro `AppError`: cria Problem Details com status 400.
4. `Result<T>` em `Failure` sem erro: cria Problem Details com status 400 e notificações/fallback.
5. Resultado fora de HTTP: permanece `Success<T>` ou `Failure`, sem conversão obrigatória.
