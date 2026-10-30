# Revisão do harness — 2026-10-30

## Fontes e autoridade

Revisão do AGENTS.md no disco, perfil ativo, YAMLs de harness, skills, gates, arquitetura e confiança, guia de
verificação, configurações nativas e skills pertinentes. O perfil e decisões do proprietário prevalecem sobre defaults.
AGENTS contém descrições desatualizadas sobre formatação e CI somente .NET 10: `.editorconfig`, formatação opcional e
SDKs efetivamente declarados devem orientar a execução; alinhar a documentação durante a implementação.

## Skills e consequências

| Skill                                 | Aplicação                                                                                                                                     |
|---------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------|
| enfatiza7-project-rules-dotnet        | Preservar arquitetura, convenções, escopo documental e sequência de specs; contratos públicos deliberados.                                    |
| enfatiza7-software-craftsmanship      | Evolução incremental, simplicidade e abstrações por necessidade presente; sem limite obrigatório de linhas.                                   |
| enfatiza7-backend-architecture-dotnet | Referências de estilo C# e estratégia de testes pertinentes. Não impor CQRS, contextos ou Vertical Slice à biblioteca.                        |
| enfatiza7-security-engineering        | Revisão limitada a SDK/dependências/pipeline, preservando permissões e origem de segredos; sem scanners automáticos novos.                    |
| typesafe-ai                           | Integração do agente, não dependência do pacote. Não substituir compilador/testes por julgamento; chamadas live exigem permissão e orçamento. |
| speckit-clarify                       | Requer spec existente; não autoriza criar spec ausente nesse fluxo.                                                                           |
| enfatiza7-token-efficient-commands    | Inspeção direcionada, preservando diagnósticos e códigos de saída.                                                                            |

Monólito modular não corresponde à arquitetura declarada. TDD não foi escolhido. Setup não é repetido nesta revisão; a
implementação de mudança de SDK/TFM pode justificar refresh focado, preservando escolhas existentes. Model advice foi
recusado no perfil e avaliação de confiança está desativada; nenhum é gate adicional. Jev tem instrução própria no
projeto e não deve ser confundido com esse model advice opcional.

Usar identificadores em inglês, nullable, XML docs e erros em `Errors/`. A API NuGet justifica tipos públicos;
auxiliares podem permanecer internos. A preferência geral por versão estável não invalida a escolha explícita por SDK 11
prerelease. Evitar bloqueio async e null-forgiving sem justificativa; avaliar ConfigureAwait quando apropriado a
biblioteca. Não acrescentar CancellationToken, ValueTask ou interfaces a todos os métodos por preferência. Tratar
default e nulos explicitamente, sem presumir ganho de desempenho.

## Verificação finita

Comandos na raiz, orçamento de cinco minutos por execução central conforme perfil. Durante implementação:

| Check             | Comando / evidência                                                     | Condição                                                                                         |
|-------------------|-------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------|
| Railway           | `dotnet test E7.sln --filter FullyQualifiedName~RailwayExtensionsTests` | Obrigatório para mudança local; adaptar filtro por subject.                                      |
| API compartilhada | `dotnet test E7.sln --configuration Release`                            | Obrigatório para núcleo/contratos; incluir propagação de erro e notificações, callbacks e async. |
| Compilação        | `dotnet build E7.sln --configuration Release`                           | Um build final; repetir ao corrigir falha de build. Roslyn existente, sem analisador sobreposto. |
| Pacote            | Empacotamento e consumidor temporário conforme quickstart               | Evidência adicional necessária à mudança de TFM/API; sem publicação.                             |
| Formatação        | `dotnet format whitespace E7.sln --verify-no-changes --no-restore`      | Opcional, após restore válido.                                                                   |
| Cobertura         | Coverlet existente                                                      | Opcional, sem threshold; 80% dos exemplos não significa cobertura de código.                     |
| Arquitetura       | Nenhum teste existente declarado                                        | Não aplicável; novo teste só por regra explícita, determinística e valiosa.                      |

Falha ou indisponibilidade de gate obrigatório impede concluir a implementação. Repetir só checks afetados por correção.
Não usar --no-build sem artefatos comprovadamente frescos; resultados históricos do perfil não validam a migração
futura. Revisão independente somente para questão concreta ou impacto/incerteza material, sem quantidade fixa de agentes
ou iterações.

## Estado desta revisão

- Leitura das regras e resolução de caminhos: **passed**.
- Especificação e respostas de clarify: **passed**, `spec.md` existe, está pronta para planejamento e registra Q1
  (ruptura documentada) e Q2 (correção de notificações/async).
- Hooks pre/post: **not applicable**, `.specify/extensions.yml` ausente.
- Checklist de requisitos: **passed**, checklist existente e sem itens pendentes.
- Prova de unions: **passed**, SDK `11.0.100-rc.1.26425.128`, build sem warnings/erros e 12 verificações executadas.
- Build/testes da biblioteca: **not applicable nesta fase de planejamento**; são gates obrigatórios da implementação.

## Evidências da implementação

- Testes Release, incluindo railway focado e regressões de `Failure`: **passed**.
- Build Release e formatação whitespace: **passed**.
- Prova/consumidor representativo: **passed** contra a biblioteca real, sem implementação union duplicada.
- Pacote local: **passed** para TFM `net11.0`, README, LICENSE e nuspec.
- Publicação NuGet hospedada: **not applicable** localmente.

O script resolveu a feature por override explícito de diretório, sem trocar a branch Git `main`. A constituição ainda é
template e não cria princípios adicionais. O próximo passo é executar o plano atualizado; publicação hospedada e
aprovação de uma major version continuam evidências/decisões externas ao planejamento.
