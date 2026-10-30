# Feature Specification: Evolução de resultados com unions

**Feature Branch**: `main` (feature `0001-union-result`; nenhuma branch criada)

**Created**: 2026-10-30

**Status**: Ready for planning — decisões de compatibilidade e comportamento registradas

**Input**: [refactor.md](../../../refactor.md), com aplicação das regras do repositório e
da [revisão do harness](harness-review.md).

## Clarifications

### Session 2026-10-30

- Q: Qual compatibilidade a nova versão deve oferecer? → A: Q1: A — permitir mudanças documentadas de API e runtime;
  consumidores migram, avaliando versão principal e preservando a mecânica de publicação. Não é exigida camada de
  compatibilidade com usos ou runtimes anteriores.
- Q: Como tratar as divergências existentes de notificações e operações assíncronas? → A: Q2: A — corrigir nesta
  migração, preservando informações de falha e eliminando bloqueios assíncronos indevidos, com mudanças documentadas.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Expressar resultados e preservar informações (Priority: P1)

Como desenvolvedor consumidor, quero representar operações bem-sucedidas ou malsucedidas e encadear operações,
preservando valores, erros e notificações conforme o contrato acordado.

**Why this priority**: É o propósito do pacote; perder informações pode alterar decisões das aplicações consumidoras.

**Independent Test**: Um consumidor executa operações com e sem valor, falhas simples e notificações, verificando
resultados e callbacks sem depender do pipeline de publicação.

**Acceptance Scenarios**:

1. **Given** uma operação bem-sucedida com valor, **When** o consumidor transforma ou encadeia o resultado, **Then**
   recebe o valor esperado e cada callback aplicável é chamado uma vez.
2. **Given** uma falha com erro e notificações, **When** uma operação que deve interromper o encadeamento é aplicada,
   **Then** o callback de sucesso não é executado e a falha é tratada conforme FR-003 e FR-010.
3. **Given** dois resultados combinados, **When** ambos são sucesso, **Then** os dois valores são disponibilizados;
   **When** há falha, **Then** as notificações são concatenadas na ordem dos operandos e o primeiro erro não nulo é o
   principal.
4. **Given** um erro definido pelo consumidor, **When** usado como falha, **Then** sua classificação, código e mensagens
   continuam disponíveis.
5. **Given** uma operação assíncrona, **When** concluída, **Then** o consumidor observa valor ou falha equivalente à
   operação síncrona correspondente, ressalvadas apenas diferenças explicitamente acordadas.
6. **Given** uma falha com erro e notificações, **When** atravessa validação ou transformação síncrona ou assíncrona,
   **Then** ambos são preservados e callbacks de sucesso não são executados.
7. **Given** uma tarefa ainda não concluída, **When** uma operação assíncrona a recebe, **Then** devolve controle ao
   chamador sem esperar sincronicamente pela conclusão e entrega o resultado após a conclusão da tarefa.

### User Story 2 - Atualizar o pacote com migração compreensível (Priority: P1)

Como consumidor atual, quero conhecer os requisitos de atualização e identificar todas as mudanças que afetam meu código
antes de adotar a nova versão.

**Why this priority**: Uma refatoração do contrato público pode impedir atualização direta mesmo preservando os
conceitos do pacote.

**Independent Test**: Migrar um consumidor representativo utilizando apenas as instruções fornecidas e validar sucesso,
falha e erro personalizado.

**Acceptance Scenarios**:

1. **Given** um consumidor da versão anterior, **When** avalia a atualização, **Then** encontra requisitos mínimos e uma
   relação das mudanças incompatíveis, se houver.
2. **Given** uma mudança de uso público, **When** segue a documentação de migração, **Then** encontra o uso anterior e o
   substituto, conforme a política escolhida em FR-009.

### User Story 3 - Publicar pelo fluxo existente (Priority: P2)

Como mantenedor, quero que alterações elegíveis continuem produzindo e publicando o pacote pelo fluxo existente, com
validação compatível com a nova versão.

**Why this priority**: A entrega precisa continuar operacional sem novas etapas manuais para o mantenedor.

**Independent Test**: Comparar as condições e etapas de publicação, gerar um pacote local e consumi-lo; a publicação
real exige evidência do pipeline hospedado.

**Acceptance Scenarios**:

1. **Given** uma alteração elegível, **When** o fluxo é acionado, **Then** preserva as condições de disparo, mecanismo
   de tag, origem das credenciais e destino de publicação existentes.
2. **Given** uma verificação obrigatória com falha, **When** o pipeline executa, **Then** não prossegue para publicação.
3. **Given** o pacote gerado, **When** inspecionado e instalado por um consumidor suportado, **Then** contém
   documentação, licença e metadados esperados e permite executar os cenários publicados.

### User Story 4 - Aprender o novo uso pela documentação (Priority: P2)

Como desenvolvedor, quero exemplos representativos e orientação de organização para compreender o pacote e migrar meus
usos existentes.

**Why this priority**: A mudança só é utilizável quando os consumidores conseguem reproduzir o novo contrato.

**Independent Test**: Conferir a matriz de cenários documentais e reproduzir seus exemplos no ambiente suportado.

**Acceptance Scenarios**:

1. **Given** a matriz de cenários definida nesta especificação, **When** a documentação é revisada, **Then** pelo menos
   80% dos cenários têm exemplos executáveis e todos os grupos públicos estão explicados.
2. **Given** um mantenedor novo, **When** consulta o material, **Then** encontra a arquitetura recomendada e a
   responsabilidade dos diretórios e arquivos principais.

### Edge Cases

- Sucesso sem valor, valor nulo explícito e resultado em estado padrão devem ter significado documentado e verificado;
  qualquer diferença em relação à versão anterior deve aparecer na migração.
- Falha pode conter apenas erro, apenas notificações ou ambos; caracterizar também erro ausente e coleção vazia antes de
  restringir estados existentes.
- Notificações repetidas e sua ordem devem ser preservadas na combinação; alterações posteriores da coleção original
  exigem comportamento explicitamente documentado.
- Tipos de valor do consumidor que também sejam tipos de erro não devem ser interpretados silenciosamente como o caso
  errado.
- Exceções de callbacks e tarefas devem seguir o contrato da operação; não converter indiscriminadamente falhas técnicas
  em resultados esperados.
- Compatibilidade de atribuições, herança, conversões e observação de valor deve ser avaliada conforme FR-009.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O pacote DEVE representar sucesso e falha, com e sem valor, e permitir ao consumidor distinguir os casos.
- **FR-002**: O pacote DEVE manter capacidades de criação, transformação, encadeamento, validação, combinação, efeitos
  colaterais, transformação de falha, tratamento explícito de exceções e seleção de caso, inclusive variantes
  assíncronas existentes.
- **FR-003**: O pacote DEVE preservar erro e notificações na propagação de falhas e manter ordem e multiplicidade na
  agregação, corrigindo as perdas legadas conforme FR-010.
- **FR-004**: Consumidores DEVEM continuar podendo definir erros próprios e consultar suas mensagens, classificação e
  código; erros associados a HTTP devem manter seu significado.
- **FR-005**: O fluxo DEVE manter condições de acionamento, mecanismo de tag, credenciais e destino da publicação
  atuais, ajustando a validação e o empacotamento aos requisitos da nova versão.
- **FR-006**: O README DEVE explicar requisitos, uso, limitações, todos os grupos públicos e migração, incluindo
  correspondências antes/depois para mudanças incompatíveis.
- **FR-007**: A documentação DEVE conter exemplos executáveis para ao menos 80% da matriz finita de cenários descrita em
  Assumptions; nenhuma família pública pode ficar sem explicação.
- **FR-008**: A entrega de design DEVE apresentar arquitetura e estrutura de diretórios/arquivos recomendadas, com
  responsabilidades e justificativas proporcionais a uma biblioteca.
- **FR-009**: O pacote PODE alterar API pública e runtime mínimo nesta migração, sem obrigação de camada de
  compatibilidade anterior. Todas as quebras DEVEM ter impacto e caminho de migração documentados. O planejamento DEVE
  avaliar versão principal e explicitar como a versão será obtida pelo mecanismo de tag existente, preservando a
  mecânica de publicação e sem presumir aprovação de uma tag específica.
- **FR-010**: O pacote DEVE corrigir nesta migração a perda de notificações na propagação de falhas e os bloqueios
  assíncronos indevidos. Operações assíncronas DEVEM aguardar a conclusão de tarefas sem bloqueio síncrono do chamador,
  e as diferenças em relação ao legado DEVEM ser documentadas.
- **FR-011**: Todos os requisitos públicos DEVEM ter evidência de comportamento e as verificações obrigatórias
  aplicáveis do harness devem passar antes da conclusão da implementação.

### Key Entities

- **Resultado**: desfecho de uma operação, distinguindo sucesso e falha e disponibilizando valor quando aplicável.
- **Falha**: erro principal opcional acompanhado de notificações; a combinação mantém ambos os tipos de informação.
- **Erro**: classificação, código identificador e mensagens; pode ser definido pelo consumidor.
- **Notificação**: código e mensagem de uma ocorrência de validação; ordem e multiplicidade são observáveis.
- **Cenário documental**: uso enumerado do pacote, associado a uma família pública e a exemplo reproduzível.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% dos cenários de comportamento acordados para sucesso, falha, composição e operações assíncronas
  produzem o resultado esperado, incluindo preservação das informações aplicáveis.
- **SC-002**: 100% das mudanças incompatíveis identificadas têm requisito mínimo, impacto e caminho de migração
  documentados; um consumidor representativo consegue migrar e executar sucesso, falha e erro próprio.
- **SC-003**: Pelo menos 80% dos cenários da matriz documental têm exemplos reproduzíveis e 100% das famílias públicas
  são explicadas.
- **SC-004**: O pacote gerado pode ser instalado em um consumidor suportado com documentação, licença e metadados
  presentes; as condições e etapas de publicação preservadas são verificadas, distinguindo validação local de publicação
  real.
- **SC-005**: O mantenedor consegue identificar no design a responsabilidade de cada diretório principal e a razão das
  alterações estruturais propostas.

## Assumptions

- C# 15, .NET 11 e unions nativas são restrições explícitas do solicitante, não escolhas inferidas. Sintaxe, suporte e
  detalhes de implementação pertencem à pesquisa/plano e precisam ser confirmados nas fontes oficiais solicitadas em
  refactor.md.
- O escopo é a biblioteca existente, testes, documentação e pipeline. Não inclui novo servidor, persistência, frontend,
  integração de modelos no pacote ou funcionalidades alheias à migração.
- Reutiliza-se a feature existente `0001-union-result`; não se cria outra numeração para o mesmo trabalho. O plano
  anterior permanece preliminar até incorporar estas decisões.
- A matriz documental será enumerada a partir das famílias de criação, inspeção, erros personalizados/HTTP,
  notificações, transformação, encadeamento, validação, combinação, efeitos colaterais, transformação de falha,
  exceções, seleção de casos, async e migração. Cada variante pública com comportamento distinto gera um cenário; não se
  contam combinações arbitrárias de tipos ou dados. A meta é arredondada para cima e exige exemplo representativo em
  cada família.
- Não há meta quantitativa nova de desempenho nem percentual mínimo de cobertura de testes. Concisão e expressividade
  serão avaliadas no design por redução de duplicação e clareza dos casos, sem impor contagem de linhas.
- O harness ativo rege a seleção das skills e verificações: ferramentas existentes, TDD opcional, nenhuma arquitetura ou
  scanner adicional imposto e nenhuma chamada externa a Jev necessária à especificação.
- A constituição local ainda contém um template; seus exemplos não constituem regras aprovadas.
