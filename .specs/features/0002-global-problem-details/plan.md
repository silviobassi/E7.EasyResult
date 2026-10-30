# Implementation Plan: Tratamento global de Problem Details

**Branch**: `0002-global-problem-details` | **Date**: 2026-10-30 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `.specs/features/0002-global-problem-details/spec.md`

## Summary

Restaurar a conversão centralizada de falhas para Problem Details sem repetir `switch` em cada endpoint. A biblioteca
continuará agnóstica de HTTP, adicionará um overload de `Match` com `Failure` completo e manterá o overload legado com
`AppError?`; o consumidor implementará um único adaptador global, preservando erro, código, classificação e notificações.

## Technical Context

**Language/Version**: C# 15 / .NET 11

**Primary Dependencies**: SDK .NET, xUnit, Shouldly; sem dependência HTTP no pacote

**Storage**: N/A

**Testing**: xUnit/Shouldly, testes unitários e consumidor de prova

**Target Platform**: Biblioteca .NET 11; consumidores HTTP ou não HTTP

**Project Type**: Biblioteca NuGet

**Performance Goals**: O caminho global não deve introduzir bloqueio síncrono nem processamento duplicado de falhas

**Constraints**: Manter o núcleo sem dependência de ASP.NET; preservar notificações; manter os dois overloads de `Match`
sem ambiguidade de compilação

**Scale/Scope**: `src/E7.EasyResult`, `tests/E7.EasyResult.Tests`, README e prova de consumidor da feature

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Nenhuma regra foi ratificada: `.specify/memory/constitution.md` ainda contém apenas o template. Gate aprovado por não
haver violação constitucional aplicável. As regras do repositório exigem testes Release, build Release e documentação
para mudanças públicas; serão executadas na implementação.

## Project Structure

### Documentation (this feature)

```text
.specs/features/0002-global-problem-details/
├── plan.md              # This file ($speckit-plan command output)
├── research.md          # Phase 0 output ($speckit-plan command)
├── data-model.md        # Phase 1 output ($speckit-plan command)
├── quickstart.md        # Phase 1 output ($speckit-plan command)
├── contracts/           # Phase 1 output ($speckit-plan command)
└── tasks.md             # Phase 2 output ($speckit-tasks command - NOT created by $speckit-plan)
```

### Source Code (repository root)

```text
src/E7.EasyResult/
├── Result.cs
├── RailwayExtensions.cs
└── Errors/
tests/
└── E7.EasyResult.Tests/
    ├── ResultTests.cs
    └── RailwayExtensionsTests.cs
README.md
```

**Structure Decision**: manter a biblioteca, os testes e o README existentes. A política HTTP será demonstrada em
`.specs/features/0002-global-problem-details/proof/Consumer/Program.cs`, sem mover o código do núcleo para uma aplicação
web e sem adicionar persistência.

## Implementation outline

1. Adicionar o overload de `Match` com `Func<Failure, TOut>` em `RailwayExtensions.cs`, mantendo o overload com
   `Func<AppError?, TOut>` e documentando a diferença.
2. Garantir que propagação, `Map`, `Bind`, `Ensure`, `Combine` e operações assíncronas mantenham erro e notificações
   ao produzir um novo `Failure`.
3. Adicionar testes para erro HTTP, erro comum, falha somente com notificações, overload novo e compatibilidade legada.
4. Atualizar README com exemplos do handler global, política de status, overloads e migração do `error switch` local.
5. Criar consumidor de prova em `.specs/features/0002-global-problem-details/proof/Consumer/` para demonstrar que
   novos `HttpError` não exigem alteração nos endpoints.
6. Executar testes focados, testes Release e build Release.

## Post-design Constitution Check

Passou. O design mantém a biblioteca sem dependência de ASP.NET, não cria persistência ou novos módulos e conserva a
responsabilidade HTTP no consumidor. Os artefatos de pesquisa, modelo, contrato e validação não introduzem violações
das regras disponíveis no repositório.
