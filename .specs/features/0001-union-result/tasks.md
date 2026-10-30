---

description: "Tarefas de implementação da migração de Result para unions nativas C# 15/.NET 11"
---

# Tasks: Migração de Result para unions nativas

**Input**: Design documents from `.specs/features/0001-union-result/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/public-api.md`, `quickstart.md`

**Organization**: As tarefas estão agrupadas por história para permitir implementação e validação incremental.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Fixar o baseline de linguagem, SDK e evidências da união nativa antes de alterar a biblioteca.

- [X] T001 Confirmar o SDK `11.0.100-rc.1.26425.128` e o TFM `net11.0` no `global.json`, `src/E7.EasyResult/E7.EasyResult.csproj` e `tests/E7.EasyResult.Tests/E7.EasyResult.Tests.csproj`
- [X] T002 [P] Executar a prova em `.specs/features/0001-union-result/proof/Consumer/Consumer.csproj` e registrar build sem warnings/erros e `Passed: 12 union proof checks.` em `.specs/features/0001-union-result/research.md`
- [X] T003 [P] Inventariar assinaturas, estados e overloads síncronos/assíncronos de `src/E7.EasyResult/Result.cs`, `src/E7.EasyResult/RailwayExtensions.cs` e `src/E7.EasyResult/Errors/` em `.specs/features/0001-union-result/contracts/public-api.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Preparar o contrato interno comum que todas as histórias dependem.

**⚠️ CRITICAL**: Nenhuma história deve ser considerada implementável até que o modelo de casos e as regras de migração estejam consistentes.

- [X] T004 Definir os casos públicos `Success`, `Success<T>` e `Failure` e o formato defensivo/imutável de `Failure.Notifications` em `src/E7.EasyResult/Result.cs` e `src/E7.EasyResult/Notification.cs`
- [X] T005 Definir `Result` e `Result<T>` como unions de casos explícitos em `src/E7.EasyResult/Result.cs`, incluindo o estado `default` inválido e a distinção entre `Success<T>(null)` e `default(Result<T>)`
- [X] T006 [P] Preservar a extensibilidade de `AppError`, `HttpError` e erros definidos pelo consumidor em `src/E7.EasyResult/Errors/AppError.cs` e `src/E7.EasyResult/Errors/HttpError.cs`
- [X] T007 Atualizar o modelo e as invariantes implementadas em `.specs/features/0001-union-result/data-model.md` após a primeira compilação da API union
- [X] T008 Compilar e executar os testes existentes para identificar todas as quebras de fonte causadas pela remoção de `Result<T> : Result` antes de migrar os testes em `tests/E7.EasyResult.Tests/`

**Checkpoint**: O modelo union compila, o estado inválido é detectável e os erros públicos continuam extensíveis.

---

## Phase 3: User Story 1 - Expressar resultados e preservar informações (Priority: P1) 🎯 MVP

**Goal**: Entregar resultados com/sem valor, falhas completas, railway síncrona e assíncrona sem perda de notificações ou bloqueio síncrono.

**Independent Test**: Executar `dotnet test E7.sln --filter FullyQualifiedName~RailwayExtensionsTests` e verificar sucesso, falha, erro customizado, notificações, callbacks, combinação e tarefas pendentes sem depender de publicação.

### Tests for User Story 1

- [X] T009 [P] [US1] Adicionar testes de criação, pattern matching, `default(Result<T>)`, `Success<T>(null)`, erro como valor e erro como falha em `tests/E7.EasyResult.Tests/ResultTests.cs`
- [X] T010 [P] [US1] Adicionar testes de falha só com erro, só com notificações, ambos, erro nulo, coleção vazia, duplicatas, ordem e cópia defensiva em `tests/E7.EasyResult.Tests/ResultTests.cs`
- [X] T011 [P] [US1] Adicionar testes de preservação de erro/notificações e não execução de callbacks para `Map`, `Bind`, `Ensure`, `Tap`, `MapFailure` e `Match` em `tests/E7.EasyResult.Tests/RailwayExtensionsTests.cs`
- [X] T012 [P] [US1] Adicionar testes de `Combine` com dois sucessos, falhas em cada operando, primeiro erro não nulo, ordem e multiplicidade de notificações em `tests/E7.EasyResult.Tests/RailwayExtensionsTests.cs`
- [X] T013 [P] [US1] Adicionar testes async com tarefa pendente, `Map`, `BindAsync`, `Ensure`, `Tap`, `Match` e falha completa sem `.Result` em `tests/E7.EasyResult.Tests/RailwayExtensionsTests.cs`
- [X] T014 [P] [US1] Adicionar testes de erro customizado e `HttpError`, verificando classificação, código, mensagens e status em `tests/E7.EasyResult.Tests/AppErrorTests.cs`

### Implementation for User Story 1

- [X] T015 [US1] Implementar factories/conversões explícitas de sucesso e falha, evitando ambiguidades quando `T` também for `AppError` ou `Notification`, em `src/E7.EasyResult/Result.cs`
- [X] T016 [US1] Implementar a propagação interna da falha completa, preservando `AppError?`, notificações, ordem, multiplicidade e cópia defensiva em `src/E7.EasyResult/RailwayExtensions.cs`
- [X] T017 [US1] Migrar `Ensure`, `Map`, `Bind`, `Tap`, `MapFailure`, `TryCatch` e `Match` para pattern matching sobre unions em `src/E7.EasyResult/RailwayExtensions.cs`
- [X] T018 [US1] Implementar `Combine` para retornar `(T1, T2)` em dois sucessos e, em falha, concatenar notificações esquerda-direita e escolher o primeiro erro não nulo em `src/E7.EasyResult/RailwayExtensions.cs`
- [X] T019 [US1] Migrar todos os overloads async para `async`/`await ConfigureAwait(false)`, remover `.Result`/`Task.Result` e preservar a exceção fora de `TryCatch` em `src/E7.EasyResult/RailwayExtensions.cs`
- [X] T020 [US1] Adaptar os helpers de consumidor e respostas de teste para o novo contrato em `tests/E7.EasyResult.Tests/Services/ResultSimulatorService.cs` e `tests/E7.EasyResult.Tests/Responses/ObjectResponse.cs`
- [X] T021 [US1] Executar os testes da história com `dotnet test E7.sln --filter FullyQualifiedName~RailwayExtensionsTests` e corrigir falhas de comportamento em `src/E7.EasyResult/` e `tests/E7.EasyResult.Tests/`

**Checkpoint**: US1 funciona isoladamente; falhas atravessam todas as operações sem perder dados e tarefas pendentes não bloqueiam o chamador.

---

## Phase 4: User Story 2 - Atualizar o pacote com migração compreensível (Priority: P1)

**Goal**: Permitir que um consumidor da API anterior migre para .NET 11/C# 15 usando somente a documentação e um consumidor representativo.

**Independent Test**: Compilar e executar um consumidor `net11.0` com sucesso, falha com notificações e erro customizado conforme `contracts/public-api.md`.

### Tests for User Story 2

- [X] T022 [P] [US2] Criar ou atualizar um consumidor representativo com sucesso, falha com notificações e erro customizado em `.specs/features/0001-union-result/proof/Consumer/Program.cs`
- [X] T023 [P] [US2] Verificar que os exemplos públicos de migração compilam contra a API union em `.specs/features/0001-union-result/proof/Consumer/Consumer.csproj`

### Implementation for User Story 2

- [X] T024 [US2] Atualizar a matriz antes/depois com requisito mínimo, impacto de fonte/ABI/runtime, substituto e exemplo para cada quebra em `.specs/features/0001-union-result/contracts/public-api.md`
- [X] T025 [US2] Documentar em `.specs/features/0001-union-result/contracts/public-api.md` a remoção de herança, troca de propriedades/flags por pattern matching, `default` inválido, conversões ambíguas e async com `await`
- [X] T026 [US2] Documentar no `README.md` a migração de `Result<T> : Result`, acessos `Value/Error/Notifications`, factories, `Combine`, railway e exceções explícitas
- [X] T027 [US2] Compilar e executar o consumidor representativo com `dotnet build .specs/features/0001-union-result/proof/Consumer/Consumer.csproj --configuration Release` e `dotnet run --project .specs/features/0001-union-result/proof/Consumer/Consumer.csproj --configuration Release --no-build`

**Checkpoint**: US2 fornece caminho de migração reproduzível e o consumidor representativo observa sucesso, falha e erro próprio.

---

## Phase 5: User Story 3 - Publicar pelo fluxo existente (Priority: P2)

**Goal**: Atualizar TFM, empacotamento e CI sem alterar gatilhos, origem de credenciais ou destino de publicação.

**Independent Test**: Gerar e inspecionar um pacote local em consumidor `net11.0`, verificando TFM, README, LICENSE e metadados; revisar o workflow sem publicar.

### Tests for User Story 3

- [X] T028 [P] [US3] Criar cenário de empacotamento e consumidor temporário conforme `.specs/features/0001-union-result/quickstart.md`, sem executar `dotnet nuget push`
- [X] T029 [P] [US3] Revisar condições, ordem de testes/tag/pack/publish e segredos do workflow em `.github/workflows/production.yml`

### Implementation for User Story 3

- [X] T030 [US3] Alterar `TargetFramework` para `net11.0` e revisar metadados, README, LICENSE e documentação XML em `src/E7.EasyResult/E7.EasyResult.csproj`
- [X] T031 [US3] Alterar `TargetFramework` para `net11.0` e manter referências xUnit, Shouldly, Test SDK e coverlet em `tests/E7.EasyResult.Tests/E7.EasyResult.Tests.csproj`
- [X] T032 [US3] Atualizar `.github/workflows/production.yml` para instalar/restaurar/testar .NET 11 mantendo trigger em `main`, `workflow_dispatch`, `GITHUB_TOKEN`, `PUSH_NUGET`, tag action, pack e `nuget.org`
- [X] T033 [US3] Garantir em `.github/workflows/production.yml` que testes obrigatórios falhem antes da tag/pack/publish e que `PackageVersion` continue vindo do output da tag action
- [X] T034 [US3] Executar `dotnet restore E7.sln`, `dotnet test E7.sln --configuration Release` e `dotnet build E7.sln --configuration Release` após a migração de TFM em `E7.sln`
- [X] T035 [US3] Gerar o pacote em cópia temporária com `dotnet pack src/E7.EasyResult/ --configuration Release -o packages`, inspecionar TFM/README/LICENSE/metadados e executar o consumidor temporário descrito em `.specs/features/0001-union-result/quickstart.md`

**Checkpoint**: O pacote local é instalável e o pipeline preserva sua mecânica; publicação real permanece fora da validação local.

---

## Phase 6: User Story 4 - Aprender o novo uso pela documentação (Priority: P2)

**Goal**: Explicar 100% das famílias públicas e fornecer exemplos executáveis para pelo menos 80% da matriz documental.

**Independent Test**: Revisar a matriz finita, reproduzir os exemplos executáveis e confirmar responsabilidades de diretórios no `README.md`.

### Tests for User Story 4

- [X] T036 [P] [US4] Enumerar a matriz por criação, inspeção, erros customizados/HTTP, notificações, transformação, encadeamento, validação, combinação, efeitos, falha, exceções, seleção, async e migração em `README.md`
- [X] T037 [P] [US4] Compilar os exemplos documentais representativos e registrar quais cenários atingem a meta arredondada de 80% em `.specs/features/0001-union-result/proof/Consumer/Program.cs`

### Implementation for User Story 4

- [X] T038 [US4] Atualizar o `README.md` com requisitos C# 15/.NET 11, arquitetura, responsabilidades de `src/`, `tests/`, `Errors/`, workflow e limitações de unions
- [X] T039 [US4] Adicionar ao `README.md` exemplos executáveis de criação, inspeção, falhas, notificações, transformação, encadeamento, validação, combinação, efeitos, `TryCatch`, `Match` e async
- [X] T040 [US4] Adicionar ao `README.md` tabela de migração antes/depois e explicar que `default` é inválido, sucesso nulo é explícito e `Failure` preserva erro/notificações
- [X] T041 [US4] Verificar no `README.md` que cada família pública é explicada e que pelo menos 80% da matriz tem exemplo reproduzível, corrigindo lacunas em `README.md` e `.specs/features/0001-union-result/proof/Consumer/Program.cs`

**Checkpoint**: US4 atende FR-006–FR-008 e SC-003–SC-005 com exemplos verificáveis.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Executar os gates finais, revisar o diff e deixar os artefatos consistentes.

- [X] T042 [P] Atualizar `.specs/features/0001-union-result/research.md`, `.specs/features/0001-union-result/data-model.md` e `.specs/features/0001-union-result/harness-review.md` com decisões e evidências finais da implementação
- [X] T043 [P] Executar `dotnet format whitespace E7.sln --verify-no-changes --no-restore` e corrigir apenas desvios introduzidos pela migração em `src/`, `tests/` e `.github/`
- [X] T044 Executar `dotnet test E7.sln --configuration Release` como gate final de comportamento em `E7.sln`
- [X] T045 Executar `dotnet build E7.sln --configuration Release` como gate final de compilação em `E7.sln`
- [X] T046 Revisar `git diff --check`, o conteúdo do pacote e o diff de `.github/workflows/production.yml`, registrando checks `passed`, `failed`, `not applicable` ou `unavailable` em `.specs/features/0001-union-result/quickstart.md`
- [X] T047 Confirmar a decisão aprovada de major version e a origem da versão gerada pela tag action em `.github/workflows/production.yml` e `.specs/features/0001-union-result/contracts/public-api.md`, sem executar publicação NuGet local

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Sem dependências; T002 e T003 podem executar em paralelo após T001 confirmar o SDK.
- **Foundational (Phase 2)**: Depende de T001–T003 e bloqueia todas as histórias.
- **US1 (Phase 3)**: Depende de T004–T008; é o MVP e deve estabilizar o contrato comportamental.
- **US2 (Phase 4)**: Depende de T015–T020 de US1 para documentar a API efetiva; T022/T023 podem preparar o consumidor após T008.
- **US3 (Phase 5)**: Depende de US1 para os testes Release e de US2 para o consumidor/migração; T029 pode revisar o workflow em paralelo.
- **US4 (Phase 6)**: Depende de US1 e US2; exemplos e matriz devem refletir a API final.
- **Polish (Phase 7)**: Depende das quatro histórias e dos artefatos de pacote.

### User Story Dependencies

- **US1 (P1)**: Depende apenas da fundação; não depende de outra história.
- **US2 (P1)**: Depende do contrato funcional de US1, mas sua preparação documental pode começar após a fundação.
- **US3 (P2)**: Depende da API estabilizada por US1 e do consumidor de US2.
- **US4 (P2)**: Depende da API efetiva de US1 e da matriz de migração de US2.

### Parallel Opportunities

- Setup: T002 e T003 em paralelo depois de T001.
- Fundação: T006 e tarefas documentais iniciais podem executar em paralelo, mas T004/T005 devem preceder a implementação railway.
- US1: T009–T014 são testes em paralelo; T016, T018 e T019 só devem ser separados se não editarem a mesma região de `RailwayExtensions.cs`.
- US2: T022/T023 e T024/T025 podem executar em paralelo, evitando edição simultânea do mesmo arquivo.
- US3: T028/T029 podem executar em paralelo; T030/T031 são arquivos distintos.
- US4: T036/T037 podem executar em paralelo; T038–T040 devem ser serializados por editarem `README.md`.
- Polish: T042/T043 podem executar em paralelo; T044–T047 são gates posteriores.

## Parallel Example: User Story 1

```text
Task: "Adicionar testes de criação, default, null e sobreposição em tests/E7.EasyResult.Tests/ResultTests.cs"
Task: "Adicionar testes de propagação e callbacks em tests/E7.EasyResult.Tests/RailwayExtensionsTests.cs"
Task: "Adicionar testes de erros customizados/HTTP em tests/E7.EasyResult.Tests/AppErrorTests.cs"
```

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Completar Setup e Foundational.
2. Implementar os casos union e a propagação integral de falhas.
3. Migrar railway síncrona e async sem `.Result`.
4. Executar os testes focados de US1 e parar para validar o comportamento independentemente.

### Incremental Delivery

1. Entregar US1 como núcleo funcional.
2. Entregar US2 com consumidor representativo e tabela de migração.
3. Entregar US3 com TFM, pacote e pipeline preservado.
4. Entregar US4 com matriz documental e exemplos executáveis.
5. Executar Polish e gates finais; não publicar durante validação local.

### Notes

- Todas as tarefas usam o formato obrigatório `- [ ] Txxx [P?] [US?] descrição com caminho`.
- `[P]` aparece somente quando a tarefa pode trabalhar em arquivo distinto sem depender de uma tarefa incompleta.
- Tarefas de teste são necessárias porque a especificação exige evidência para todos os requisitos públicos e os gates do harness.
- Testes de arquitetura, servidor, persistência e Jev são não aplicáveis conforme o plano.

## Phase 8: Convergence

- [X] T048 [US2] Migrar `.specs/features/0001-union-result/proof/Consumer/Consumer.csproj` e `Program.cs` para referenciar a biblioteca real ou o pacote local, substituir a implementação `UnionProof` duplicada e compilar/executar sucesso, falha com notificações e erro customizado per FR-002/SC-001 (partial)
- [X] T049 [US2] Completar `.specs/features/0001-union-result/contracts/public-api.md` com a matriz antes/depois de `Create`, fábricas, inspeção, `Ensure`, `Combine`, `Tap`, `Map`, `Bind`, `MapFailure`, `TryCatch`, `Match` e todas as variantes assíncronas, incluindo requisito mínimo, impacto ABI/runtime, substituto e exemplo per FR-006/FR-009/SC-002 (partial)
- [X] T050 [US4] Enumerar a matriz documental finita em `README.md` e `.specs/features/0001-union-result/proof/Program.cs`, adicionar exemplos executáveis para pelo menos 80% dos cenários e explicar todas as famílias públicas per FR-007/SC-003 (missing)
- [X] T051 [US3] Criar consumidor temporário `net11.0` conforme `.specs/features/0001-union-result/quickstart.md`, instalar o pacote gerado em `/tmp/e7-package`, executar cenários de sucesso, falha com notificações e erro customizado, e registrar o resultado per SC-004 (missing)
- [X] T052 [US1] Tornar a construção pública de `Failure` defensiva em `src/E7.EasyResult/Result.cs`, copiando notificações também quando o consumidor usa o construtor do caso diretamente, e adicionar regressão em `tests/E7.EasyResult.Tests/ResultTests.cs` per FR-003 (partial)
- [X] T053 [US3] Resolver e registrar a decisão aprovada de versionamento em `.github/workflows/production.yml` e `.specs/features/0001-union-result/contracts/public-api.md`, removendo o `DEFAULT_BUMP: major` não aprovado ou vinculando-o a evidência explícita do mantenedor per FR-009 (contradicts)
- [X] T054 Atualizar `.specs/features/0001-union-result/research.md`, `data-model.md`, `harness-review.md` e `quickstart.md` com as evidências finais do consumidor, pacote, testes, build, formatação, diff e publicação real `not applicable`, reconciliando os gaps desta fase per FR-011/SC-004 (partial)

## Phase 9: Convergence

- [X] T055 [US4] Documentar no `README.md` a arquitetura e a responsabilidade de `src/E7.EasyResult/`, `src/E7.EasyResult/Errors/`, `tests/E7.EasyResult.Tests/`, `.specs/features/0001-union-result/proof/` e `.github/workflows/production.yml` per FR-008/SC-005 (missing)
- [X] T056 [US4] Corrigir os snippets do `README.md` que não compilam, incluindo construtores reais de `ElementNotFoundError` e a assinatura assíncrona de `Tap`, e validar os exemplos contra o consumidor representativo per FR-006/FR-007/SC-002/SC-003 (partial)
- [X] T057 Atualizar `.specs/features/0001-union-result/quickstart.md` para registrar os 35 testes Release atualmente aprovados, mantendo os estados `passed`/`not applicable` dos demais gates per FR-011 (partial)
- [X] T058 [US3] Registrar em artefato aprovado pelo mantenedor a autorização para `DEFAULT_BUMP: major` ou remover o valor fixo de `.github/workflows/production.yml` até essa aprovação, preservando a decisão de versionamento no release per FR-009/plan: release versioning (partial)

## Phase 10: Convergence

- [X] T059 [US4] Corrigir o pipeline assíncrono em `README.md` para usar uma lambda compatível com `Tap(Task<Result<T>>, Func<T, Task>)` e validar o snippet contra a API real per FR-006/FR-007/SC-002/SC-003 (partial)
- [X] T060 [US1] Definir o contrato de `Match` para `Failure` sem `AppError`, ajustar `src/E7.EasyResult/RailwayExtensions.cs` e `tests/E7.EasyResult.Tests/RailwayExtensionsTests.cs` para cobrir falha somente com notificações sem dereferência nula per FR-003/FR-004/SC-001 (partial)
