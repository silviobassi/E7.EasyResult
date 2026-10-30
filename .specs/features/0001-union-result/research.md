# Pesquisa consolidada

## Union nativa e versionamento

**Decisão:** usar `union` nativa do C# 15 com `net11.0`, mantendo o SDK do `global.json` durante a implementação.

**Racional:** a referência oficial de [union types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/union) confirma casos fechados, conversões implícitas de cada caso, pattern matching pelo conteúdo, `UnionAttribute`/`IUnion` e suporte no runtime .NET 11. A referência de [language versioning](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-versioning) confirma C# 15 como padrão de .NET 11 e declara não suportado usar uma versão de linguagem mais nova que a associada ao TFM.

**Alternativas consideradas:** classe discriminada manualmente (perde exaustividade); framework/gerador externo (dependência e pipeline novos); multitarget `net10.0`/`net11.0` (não permite oferecer a mesma API nativa aos consumidores de .NET 10 e duplica manutenção). Foram rejeitadas pela exigência explícita de unions nativas e pela decisão Q1 de aceitar ruptura.

**Evidência local:** `dotnet build .specs/features/0001-union-result/proof/Consumer/Consumer.csproj --configuration Release --nologo --verbosity quiet` passou sem warnings/erros; `dotnet run ... --no-build` reportou `Passed: 12 union proof checks.`. A prova cobre consumo entre assemblies, `default`, `null`, conversões, sobreposição entre erro/valor, pattern matching, propagação e tarefa pendente.

## Modelo de resultado

**Decisão:** `Result<T>` será union de `Success<T>` e `Failure`; `Result` será union de `Success` e `Failure`. `Failure` carrega `AppError?` e notificações ordenadas. Sucesso e falha são casos, não uma hierarquia de resultados.

**Racional:** encapsular o payload impede que `AppError` usado como valor seja confundido com falha. Uma falha única preserva erro e notificações juntos, corrigindo as perdas observadas em `Ensure`/`Map` e permitindo erro ausente ou notificações vazias sem inventar um erro.

**Alternativas consideradas:** union fechada de erros concretos, que quebra erros definidos pelo consumidor; falha contendo apenas erro, que perde notificações; herança `Result<T> : Result`, que não é preservada pela union nativa.

## Compatibilidade e release

**Decisão:** documentar ruptura de fonte, ABI, herança, conversões e runtime mínimo; exigir major version aprovada. Manter a ação de tag e o fluxo de publicação, mas não transformar automaticamente a decisão de major em `patch`.

**Racional:** a especificação Q1 permite mudanças documentadas e exige preservar a mecânica de publicação, não a compatibilidade binária. O workflow atual usa `github-tag-action` com `DEFAULT_BUMP: patch`; isso deve ser uma decisão explícita no PR/release, com `PackageVersion` ainda vindo do output da action.

## Assíncrono e notificações

**Decisão:** corrigir agora a propagação completa de falhas e substituir cada `.Result`/`Task.Result` por `await`.

**Racional:** a especificação Q2 torna essas correções obrigatórias. `TryCatch` continua sendo o único limite explícito para converter exceção em resultado; callback ou tarefa fora dele deve manter a semântica de exceção.

**Alternativas consideradas:** preservar o comportamento legado para evitar mudança, rejeitada por perda de informação e bloqueio; capturar todas as exceções, rejeitada por mascarar falhas técnicas.

## Documentação e verificação

**Decisão:** enumerar cenários por família pública e fornecer exemplos executáveis para ao menos 80%, arredondando para cima; explicar 100% das famílias. Usar testes xUnit/Shouldly e os comandos finitos do harness.

**Racional:** uma porcentagem sobre “todos os exemplos possíveis” não é mensurável. A matriz por comportamento distinto torna SC-003 auditável sem impor cobertura de linhas ou ferramentas novas.

## Evidências finais da convergência

- O consumidor representativo referencia `src/E7.EasyResult/E7.EasyResult.csproj`, substituindo a implementação
  isolada da prova; ele valida criação, falha completa, erro como valor, `default`, pattern matching e tarefa pendente.
- `Failure` copia notificações também no construtor público do caso, evitando mutação posterior da coleção de entrada.
- `DEFAULT_BUMP: major` é a decisão de release para a ruptura Q1; `PackageVersion` permanece derivada de
  `steps.tab_bump.outputs.new_tag`. O push real permanece fora da validação local.
