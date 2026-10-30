# Plano: migração de Result para unions nativas

**Feature:** `0001-union-result`  
**Branch de trabalho:** `0001-union-result`  
**Entrada:** [spec.md](spec.md), [implementation-examples.md](implementation-examples.md), [harness-review.md](harness-review.md) e `refactor.md`  
**Estado:** pronto para implementação; Q1 e Q2 resolvidas

## 1. Decisões de design

- Usar `union` nativa do C# 15 em projetos `net11.0`, sem framework de unions, fonte gerada ou camada de compatibilidade com a API anterior. A prova em `proof/` compila e executa 12 verificações com o SDK fixado no `global.json`.
- Modelar os estados com casos explícitos: sucesso com valor (`Success<T>`), sucesso sem valor (`Success`) e falha (`Failure`). `Failure` contém `AppError?` e uma coleção imutável/defensiva de `Notification`; erro e notificações podem coexistir.
- Encapsular o valor de sucesso evita que `Result<AppError>` confunda um erro usado como valor com uma falha. Conversões implícitas só serão mantidas quando não criarem ambiguidade; fábricas nomeadas e pattern matching serão o caminho documentado para casos ambíguos.
- `default(Result<T>)` é um estado sem caso válido. A API deve detectá-lo e lançar a exceção de estado inválido em inspeções/operações que exigem um caso; `Success<T>(null)` continua distinto de `default` quando `T` é anulável.
- Q1: a ruptura de fonte, ABI, herança, conversões e TFM é aceita e será publicada como mudança principal, após aprovação do mantenedor. Não preservar `Result<T> : Result` nem acessos/propriedades legados por fachada.
- Q2: corrigir na mesma migração a perda de notificações e os bloqueios síncronos (`Task.Result`), preservando a falha inteira e usando `await ConfigureAwait(false)`. Exceções de callbacks/tarefas não serão convertidas indiscriminadamente.

## 2. Contexto técnico e gates

- Atualizar biblioteca e testes de `net10.0` para `net11.0`; manter `global.json` e SDK RC fixados até existir decisão de atualização. C# 15 é o padrão de .NET 11 e não deve ser forçado em `net10.0`.
- Manter xUnit, Shouldly, Roslyn e a estrutura de dois projetos. Não adicionar analisador, pacote de union, arquitetura modular, Jev ou TDD obrigatório.
- A constituição é um template não ratificado; portanto não há gate constitucional adicional. Os gates efetivos são testes Release, build Release, empacotamento/consumidor temporário e revisão do pipeline.
- O pipeline hospedado não pode ser declarado validado por checks locais. Falha de teste/build deve impedir tag e publicação; push para NuGet não será executado durante a implementação.

## 3. Fase 0 — pesquisa e prova

1. Usar a prova existente em `proof/` como contrato executável de linguagem. Reexecutar build e consumidor e preservar evidência para: union genérica e não genérica entre assemblies, pattern matching, conversão de casos, `default`, `null`, tipos de valor, erro como sucesso, erro como falha, falha inteira em `Map`, `MapAsync` e tarefa incompleta.
2. Registrar em [research.md](research.md) as fontes oficiais de union e versionamento, as restrições de `UnionAttribute`/`IUnion`, conversões ambíguas, boxing e nulabilidade. Se a documentação ficar indisponível, marcar a fonte como unavailable, sem substituir a prova local por suposição.
3. Inventariar as assinaturas atuais em `Result.cs`, `RailwayExtensions.cs`, `Errors/`, testes, README e workflow. Para cada método, registrar estado de sucesso, falha, callback, exceção e comportamento async antes da edição.

## 4. Fase 1 — modelo e contrato

1. Atualizar [data-model.md](data-model.md) com invariantes de `Success`, `Success<T>`, `Failure`, `Notification` e `AppError`, incluindo ordem/multiplicidade, cópia defensiva e estado `default`.
2. Atualizar [contracts/public-api.md](contracts/public-api.md) com a matriz antes/depois. A matriz deve cobrir `Create`, sucesso/falha, inspeção, `Ensure`, `Combine`, `Tap`, `Map`, `Bind`, `MapFailure`, `TryCatch`, `Match` e todas as variantes assíncronas.
3. Para cada quebra, documentar: requisito mínimo (.NET 11/C# 15), impacto de fonte/binário/runtime, substituto e exemplo antes/depois. Destacar remoção de herança, mudanças de pattern matching, acesso a `Value/Error/Notifications`, conversões implícitas e retorno `Task<Result<T>>`.
4. Fixar a matriz documental por família pública, contando uma variante por comportamento distinto. Exigir exemplos compiláveis para pelo menos 80% da matriz, arredondando para cima, e explicação para 100% das famílias.

## 5. Fase 2 — implementação incremental

1. Criar os casos de resultado e factories/conversões mínimas no projeto principal, mantendo XML docs, nullable e namespace/nomes existentes quando não conflitarem com unions. Validar primeiro criação, pattern matching, `default`, `null` e tipos sobrepostos.
2. Centralizar uma operação interna de propagação que copie a falha completa. Aplicá-la a `Map`, `Bind`, `Ensure`, `MapFailure`, `Match` e `Tap`; callback de sucesso não roda em falha.
3. Implementar `Combine`: dois sucessos retornam tupla; falhas concatenam notificações na ordem esquerda-direita, preservam duplicatas e escolhem o primeiro erro não nulo. Não mutar a coleção de entrada.
4. Migrar todos os overloads async para `async`/`await`. Remover `Task.Result`/`.Result`; garantir que uma tarefa incompleta devolva controle, que o callback só execute após sucesso e que falhas preservem erro/notificações. Manter `ConfigureAwait(false)` onde a biblioteca aguarda tarefas.
5. Migrar erros existentes e subclasses de consumidor sem fechar a extensibilidade de `AppError`/`HttpError`. Tratar exceções somente em `TryCatch`, conforme seu contrato.
6. Adaptar testes existentes e adicionar casos determinísticos para cada requisito: falha só com erro, só com notificações, ambos, erro nulo, lista vazia, duplicatas/ordem, erro customizado/HTTP, `default`, `null`, erro como valor, callbacks, exceções e tarefas pendentes.

## 6. Fase 3 — API, documentação e migração

1. Atualizar README com requisito mínimo, arquitetura de diretórios, criação/inspeção por pattern matching, railway síncrona/assíncrona, falhas agregadas e limitações de unions.
2. Incluir uma seção “Migração da API” com tabela antes/depois e exemplos compiláveis. Explicar que `Result<T>` deixa de herdar de `Result`, que consumidores devem trocar propriedades/flags por casos, que `default` é inválido, que conversões ambíguas exigem `Success<T>`/`Failure` explícitos e que chamadas async devem ser aguardadas.
3. Incluir a matriz de cenários e marcar quais exemplos são executáveis; verificar a meta de 80% e a cobertura de todas as famílias públicas antes do build final.

## 7. Fase 4 — TFM, pacote e pipeline

1. Alterar os dois `.csproj` para `net11.0`, revisar metadados, README, LICENSE e documentação XML, e confirmar que o pacote não contém artefatos `bin/obj` ou a prova isolada.
2. Atualizar `.github/workflows/production.yml` para instalar/restaurar/testar .NET 11 e manter: trigger em `main`, `workflow_dispatch`, `GITHUB_TOKEN` no action de tag, cópia de README/LICENSE, `dotnet pack`, `PUSH_NUGET` e `nuget.org`.
3. Manter o mecanismo de tag existente, mas não publicar silenciosamente uma ruptura como patch: registrar no PR/release a decisão do mantenedor sobre major version. O plano não fixa uma tag específica; a implementação deve confirmar que o output de `github-tag-action` alimenta `PackageVersion` e que uma verificação falha antes de publicação.
4. Validar localmente em cópia temporária: restore, testes Release, pack, inspeção do `.nupkg` (TFM, README, LICENSE, metadados) e consumidor temporário que execute sucesso, falha e erro customizado. Não executar `dotnet nuget push`.

## 8. Verificação finita e critérios de saída

Na raiz, com o SDK selecionado:

1. **Passed obrigatório:** `dotnet test E7.sln --filter FullyQualifiedName~RailwayExtensionsTests` durante a migração, adaptando o filtro para novos testes.
2. **Passed obrigatório:** `dotnet test E7.sln --configuration Release` após alterações compartilhadas.
3. **Passed obrigatório:** `dotnet build E7.sln --configuration Release` uma vez no final (repetir somente após corrigir falha de build).
4. **Passed obrigatório para esta mudança de TFM/API:** pack e consumidor temporário conforme [quickstart.md](quickstart.md).
5. **Optional:** `dotnet format whitespace E7.sln --verify-no-changes --no-restore`; cobertura sem threshold.
6. **Not applicable:** testes de arquitetura, servidor, persistência, publicação real e chamada live a Jev.

Saída somente quando SC-001–SC-005 da especificação estiverem evidenciados, todas as clarificações resolvidas, a migração documentada e os gates obrigatórios em estado `passed`. Qualquer check obrigatório indisponível bloqueia a conclusão e deve ser reportado como `unavailable`.
