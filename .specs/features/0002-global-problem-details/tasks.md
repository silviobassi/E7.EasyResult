---

description: "Tarefas de implementação para tratamento global de Problem Details"
---

# Tasks: Tratamento global de Problem Details

**Input**: Design documents from `.specs/features/0002-global-problem-details/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/public-api.md`, `quickstart.md`

**Tests**: Incluídos porque a especificação exige evidência automatizada para erros, notificações, matching e consumidor.

## Phase 1: Setup

**Purpose**: Confirmar a estrutura existente e os pontos públicos que serão alterados.

- [X] T001 Confirmar o baseline do commit `5306adc` e os pontos públicos de `Match`, `Failure` e `Notifications` em `src/E7.EasyResult/Result.cs` e `src/E7.EasyResult/RailwayExtensions.cs`
- [X] T002 [P] Catalogar os exemplos HTTP com `error switch` em `README.md` e os cenários correspondentes em `.specs/features/0001-union-result/contracts/public-api.md`

## Phase 2: Foundational

**Purpose**: Fixar o contrato compartilhado antes das histórias de consumidor.

- [X] T003 Definir no contrato público de `src/E7.EasyResult/RailwayExtensions.cs` o overload recomendado `Func<Failure, TOut>` e manter o overload legado `Func<AppError?, TOut>`, incluindo `Error` anulável e `Notifications` ordenadas
- [X] T004 [P] Definir os casos de status e conteúdo do adaptador central em `.specs/features/0002-global-problem-details/contracts/public-api.md`, mantendo `HttpError` como fonte de status e `400` como fallback
- [X] T005 [P] Preparar tipos de erro de teste em `tests/E7.EasyResult.Tests/RailwayExtensionsTests.cs` para cobrir `HttpError`, `AppError` comum e falha somente com notificações

**Checkpoint**: Contrato e casos de teste compartilhados definidos; histórias podem ser implementadas.

## Phase 3: User Story 1 - Delegar falhas ao tratamento central (Priority: P1) 🎯 MVP

**Goal**: Permitir que um adaptador central receba a falha completa e preserve status, erro e notificações.

**Independent Test**: Executar resultados com `HttpError`, `AppError` comum e somente notificações; verificar status `HttpError` ou `400`, detalhes e notificações sem executar o callback de sucesso.

### Tests for User Story 1

- [X] T006 [P] [US1] Testar em `tests/E7.EasyResult.Tests/RailwayExtensionsTests.cs` que o novo overload entrega `Failure` completo e que o overload legado continua recebendo `AppError?`
- [X] T007 [P] [US1] Testar preservação de `Error`, código, classificação e notificações na adaptação de falhas em `tests/E7.EasyResult.Tests/ResultTests.cs`
- [X] T008 [P] [US1] Testar falha somente com notificações e callback de sucesso não executado em `tests/E7.EasyResult.Tests/RailwayExtensionsTests.cs`

### Implementation for User Story 1

- [X] T009 [US1] Adicionar em `src/E7.EasyResult/RailwayExtensions.cs` o overload genérico de `Match` com callback `Func<Failure, TOut>` sem acoplar o pacote a ASP.NET
- [X] T010 [US1] Manter e documentar em `src/E7.EasyResult/RailwayExtensions.cs` o overload legado com `Func<AppError?, TOut>`, evitando ambiguidade de inferência entre os dois callbacks
- [X] T011 [US1] Garantir em `src/E7.EasyResult/Result.cs` que a construção e propagação de `Failure` preservem ordem, multiplicidade e cópia de `Notifications`
- [X] T012 [US1] Implementar fixture de adaptador central em `tests/E7.EasyResult.Tests/Services/ResultSimulatorService.cs` que mapeie `HttpError` ao status declarado e demais falhas a `400`, preservando mensagem, código, classificação e notificações

**Checkpoint**: US1 funciona isoladamente e demonstra o comportamento centralizado sem dependência HTTP no pacote.

## Phase 4: User Story 2 - Remover mapeamento repetido nos consumidores (Priority: P1)

**Goal**: Substituir o `error switch` repetido nos exemplos por um único adaptador global reutilizável.

**Independent Test**: Dois endpoints de prova processam a mesma falha com a mesma resposta; adicionar um novo `HttpError` não altera os endpoints.

### Tests for User Story 2

- [X] T013 [P] [US2] Adicionar teste do consumidor de prova para dois endpoints sem `switch` local em `.specs/features/0002-global-problem-details/proof/Consumer/Program.cs`
- [X] T014 [P] [US2] Adicionar cenário de novo `HttpError` tratado sem alteração do endpoint em `.specs/features/0002-global-problem-details/proof/Consumer/Program.cs`

### Implementation for User Story 2

- [X] T015 [US2] Criar o consumidor de prova em `.specs/features/0002-global-problem-details/proof/Consumer/Consumer.csproj` para referenciar a biblioteca sem dependência ASP.NET adicional
- [X] T016 [US2] Implementar o adaptador central e os endpoints de prova em `.specs/features/0002-global-problem-details/proof/Consumer/Program.cs`, mantendo o `switch` de tipos somente no handler global
- [X] T017 [US2] Remover os exemplos de `error switch` por endpoint e documentar o handler único em `README.md`
- [X] T018 [US2] Verificar no consumidor de prova que `HttpError`, `AppError` e falha somente com notificações produzem respostas equivalentes às regras de `contracts/public-api.md`

**Checkpoint**: US1 e US2 funcionam independentemente após a fundação; endpoints não conhecem o catálogo de erros.

## Phase 5: User Story 3 - Migrar consumidores da versão atual (Priority: P2)

**Goal**: Explicar a migração do callback baseado em `AppError?` e do `switch` local para o adaptador global.

**Independent Test**: Um consumidor segue apenas a documentação e cobre sucesso, falha HTTP, falha comum e notificações.

### Tests for User Story 3

- [X] T019 [P] [US3] Validar o quickstart executável e os cenários de migração descritos em `.specs/features/0002-global-problem-details/quickstart.md`
- [X] T020 [P] [US3] Revisar os exemplos de `Match(Failure)`, `Match(AppError?)`, `Failure` e notificações em `README.md` contra `.specs/features/0002-global-problem-details/contracts/public-api.md`

### Implementation for User Story 3

- [X] T021 [US3] Documentar em `README.md` a configuração do tratamento global, a política `HttpError`/`400` e a preservação de notificações
- [X] T022 [US3] Documentar em `README.md` a migração antes/depois do `error switch`, incluindo limites entre biblioteca agnóstica e consumidor HTTP
- [X] T023 [US3] Atualizar `.specs/features/0002-global-problem-details/quickstart.md` com comandos e resultados observáveis do consumidor de prova

**Checkpoint**: US3 permite migrar um consumidor representativo sem consultar código adicional.

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Validar o conjunto completo e manter os artefatos sincronizados.

- [X] T024 [P] Atualizar `.specs/features/0002-global-problem-details/data-model.md` e `contracts/public-api.md` para refletir o overload `Match(Failure)` e a compatibilidade `Match(AppError?)`
- [X] T025 [P] Executar `dotnet test E7.sln --configuration Release --filter FullyQualifiedName~RailwayExtensionsTests` e registrar falhas em `tests/E7.EasyResult.Tests/RailwayExtensionsTests.cs`
- [X] T026 Executar `dotnet test E7.sln --configuration Release` após as alterações compartilhadas
- [X] T027 Executar `dotnet build E7.sln --configuration Release` e validar o fluxo descrito em `.specs/features/0002-global-problem-details/quickstart.md`

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: sem dependências.
- **Foundational (Phase 2)**: depende de T001; bloqueia as histórias.
- **US1 (Phase 3)**: depende de T003-T005; é o MVP.
- **US2 (Phase 4)**: depende de T009-T012; usa o contrato de US1.
- **US3 (Phase 5)**: depende de T016-T018; documenta o comportamento final.
- **Polish (Phase 6)**: depende das histórias desejadas; T026-T027 são gates finais.

### User Story Dependencies

- **US1 (P1)**: independente após a fundação.
- **US2 (P1)**: depende do contrato implementado em US1, mas sua prova é independente do servidor HTTP.
- **US3 (P2)**: depende do comportamento final demonstrado por US1 e US2.

### Parallel Opportunities

- T002 e T005 podem executar em paralelo com T001 após a inspeção inicial.
- T004 e T005 podem executar em paralelo.
- T006-T008 podem executar em paralelo antes de T009-T012.
- T013-T014 podem executar em paralelo; T015 pode ser preparado enquanto os testes são escritos.
- T019-T020 podem executar em paralelo.
- T024-T025 podem executar em paralelo após a implementação; T026-T027 devem ser executados ao final.

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Concluir Setup e Foundational.
2. Implementar US1 e executar T006-T012.
3. Validar o callback completo e a preservação de falhas.
4. Parar para demonstrar o adaptador central antes de alterar documentação ampla.

### Incremental Delivery

1. Entregar US1 como contrato de falha completa.
2. Entregar US2 com consumidor de prova sem `switch` nos endpoints.
3. Entregar US3 com migração documentada.
4. Executar todos os gates de Polish.

## Phase 7: Convergence

- [X] T028 Adicionar ao adaptador e à prova um cenário de `AppError` comum que preserve mensagem, código e classificação na resposta Problem Details per FR-004 (partial)
- [X] T029 Adicionar dois endpoints de prova que retornem a mesma falha e verificar equivalência de status, detalhes e notificações per US2/AC2 (missing)
- [X] T030 Definir, documentar e testar a rejeição ou normalização previsível de status HTTP inválido ou inesperado per Edge Cases (partial)

## Phase 8: Convergence

- [X] T031 Atualizar o exemplo do handler global em `README.md` para serializar também código e classificação do erro per FR-004 e US3/AC1 (partial)
