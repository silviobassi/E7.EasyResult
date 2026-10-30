# Feature Specification: Tratamento global de Problem Details

**Feature Branch**: `main`

**Created**: 2026-10-30

**Status**: Draft

**Input**: User description: "então isso deve ser feito pois a versão anterior fazia exatamente isso"

## Clarifications

### Session 2026-10-30

- Q: Onde deve viver a implementação reutilizável do tratamento de falhas em Problem Details? → A: O pacote expõe `Failure` completo; cada aplicação HTTP registra um handler global próprio, sem `switch` nos endpoints.
- Q: Qual assinatura deve ser o caminho recomendado para o `Match` usado pelo handler global? → A: Adicionar um overload que receba `Failure` completo e manter o overload atual que recebe `AppError?`.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Delegar falhas ao tratamento central (Priority: P1)

Como desenvolvedor consumidor da biblioteca, quero retornar um resultado de falha sem distinguir manualmente cada tipo
de erro em cada endpoint, para que o tratamento HTTP permaneça consistente e sem código repetido.

**Why this priority**: O comportamento centralizado existia na versão anterior e evita que endpoints produzam respostas
incompatíveis para erros equivalentes.

**Independent Test**: Um consumidor registra o tratamento global e executa endpoints que retornam falhas de domínio,
falhas HTTP e falhas com notificações, verificando que cada resposta é produzida pelo mesmo fluxo central.

**Acceptance Scenarios**:

1. **Given** um resultado de falha contendo um erro HTTP, **When** o endpoint entrega a falha ao tratamento global,
   **Then** a resposta usa o status HTTP definido pelo erro e apresenta detalhes no formato Problem Details.
2. **Given** um resultado de falha contendo um erro de aplicação sem status HTTP específico, **When** a falha é entregue ao
   tratamento global, **Then** a resposta usa o status padrão acordado para falhas de cliente e preserva a mensagem do
   erro.
3. **Given** um resultado de falha contendo notificações, **When** a falha é entregue ao tratamento global, **Then** as
   notificações permanecem disponíveis na resposta e não são descartadas.

### User Story 2 - Remover mapeamento repetido nos consumidores (Priority: P1)

Como mantenedor de uma aplicação que usa a biblioteca, quero que o consumidor não precise escrever um `switch` para
cada endpoint, para que a evolução do catálogo de erros não exija alterações espalhadas pela aplicação.

**Why this priority**: A duplicação torna o comportamento dependente de cada endpoint e pode reintroduzir divergências em
relação ao comportamento da versão anterior.

**Independent Test**: Um endpoint com sucesso e falha usa apenas o fluxo padrão de resultado; adicionar um novo erro HTTP
não exige editar o endpoint para escolher o status da resposta.

**Acceptance Scenarios**:

1. **Given** um endpoint que retorna sucesso ou falha, **When** seu resultado é processado, **Then** o endpoint não
   precisa selecionar manualmente o tipo de erro para produzir a resposta de falha.
2. **Given** dois endpoints que retornam a mesma falha, **When** ambos são executados, **Then** produzem respostas
   equivalentes quanto a status, detalhes e notificações.
3. **Given** um novo tipo de erro HTTP compatível com o contrato existente, **When** ele é usado por um endpoint,
   **Then** o tratamento global aplica seu status sem alteração no endpoint.

### User Story 3 - Migrar consumidores da versão atual (Priority: P2)

Como consumidor atual, quero instruções claras para configurar e usar o tratamento global, para que eu possa substituir
os mapeamentos locais sem perder informações de erro.

**Why this priority**: A mudança atravessa a fronteira entre a biblioteca e a aplicação consumidora e precisa ser
adotada sem interpretações diferentes.

**Independent Test**: Um consumidor representativo segue a documentação e verifica uma falha HTTP, uma falha comum e uma
falha com notificações.

**Acceptance Scenarios**:

1. **Given** a documentação de migração, **When** um consumidor substitui o mapeamento local, **Then** encontra o uso
   anterior, o uso recomendado e os limites de responsabilidade de cada camada.
2. **Given** uma aplicação sem tratamento global configurado, **When** ela usa o resultado de falha, **Then** a
   documentação identifica explicitamente o comportamento e a configuração necessária.

### Edge Cases

- Uma falha pode conter erro HTTP, erro de aplicação, apenas notificações ou nenhuma informação de erro; o resultado
  deve seguir uma resposta definida para cada caso.
- Um erro HTTP pode declarar status inválido ou inesperado; o tratamento deve rejeitar ou normalizar o valor de forma
  previsível, sem produzir resposta de sucesso.
- Mensagens nulas, vazias ou múltiplas notificações não devem causar perda silenciosa de informação.
- O mesmo erro pode atravessar mais de um componente de tratamento; a resposta não deve ser duplicada nem convertida
  novamente.
- O consumidor pode usar o resultado fora de HTTP; o contrato de resultado não deve exigir uma resposta HTTP nesse
  cenário.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: A biblioteca DEVE expor o caso `Failure` completo para que uma aplicação HTTP registre um fluxo global de conversão para Problem Details, sem exigir que a biblioteca dependa de tipos HTTP.
- **FR-002**: O fluxo global DEVE usar o status associado a um erro HTTP quando esse status estiver disponível.
- **FR-003**: O fluxo global DEVE aplicar um status padrão documentado para erros de aplicação sem status HTTP específico.
- **FR-004**: O handler global da aplicação DEVE preservar a mensagem, a classificação, o código e as notificações
  disponíveis na falha, conforme o formato de resposta acordado.
- **FR-005**: Consumidores DEVEM conseguir processar resultados de sucesso e falha sem escrever uma seleção manual por
  tipo de erro em cada endpoint.
- **FR-005a**: A biblioteca DEVE oferecer um overload de `Match` cujo callback de falha receba `Failure` completo e
  DEVE manter o overload existente baseado em `AppError?` para permitir migração gradual.
- **FR-006**: Erros HTTP novos que respeitem o contrato existente DEVEM ser tratados pelo fluxo global sem alteração nos
  consumidores existentes.
- **FR-007**: O comportamento de falhas sem erro principal, incluindo falhas apenas com notificações, DEVE ser
  definido, testado e documentado.
- **FR-008**: O handler global DEVE ser opcional para consumidores que usam a biblioteca fora de HTTP e não DEVE alterar
  a semântica de sucesso, falha ou propagação do resultado; a biblioteca não DEVE exigir dependências HTTP.
- **FR-009**: A documentação DEVE apresentar a migração de mapeamentos locais para o tratamento global, incluindo
  configuração, exemplos de resposta e limites de responsabilidade.
- **FR-010**: O pacote DEVE fornecer evidência automatizada para erros HTTP, erros comuns, notificações, equivalência entre
  endpoints e uso fora de HTTP.

### Key Entities

- **Resultado de falha**: desfecho que contém um erro principal opcional e uma coleção ordenada de notificações.
- **Erro HTTP**: erro de aplicação que informa o status apropriado para uma resposta HTTP.
- **Problem Details**: representação pública de uma falha HTTP, contendo detalhes e extensões relevantes.
- **Tratamento global**: componente responsável por aplicar uma política única de resposta às falhas recebidas.
- **Consumidor**: aplicação ou biblioteca que usa resultados e pode, ou não, expô-los por HTTP.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% dos cenários de falha HTTP, falha comum e falha com notificações produzem o status e os detalhes
  documentados.
- **SC-002**: 100% dos endpoints representativos avaliados deixam de conter seleção manual por tipo de erro para o
  fluxo coberto pela configuração global.
- **SC-003**: Adicionar um novo erro HTTP compatível não exige alterar nenhum endpoint consumidor existente.
- **SC-004**: Um consumidor representativo consegue concluir a migração seguindo somente a documentação fornecida,
  cobrindo sucesso, falha HTTP, falha comum e notificações.
- **SC-005**: Consumidores não HTTP continuam executando os mesmos cenários de resultado sem configurar o tratamento
  global.

## Assumptions

- O handler global é uma responsabilidade da aplicação que expõe HTTP; a biblioteca expõe o contrato de resultados e
  erros sem fornecer uma resposta HTTP pronta.
- O uso recomendado para novos handlers é `Match(..., failure => HandleFailure(failure))`, em que `failure.Error` pode
  ser nulo e `failure.Notifications` permanece disponível; consumidores existentes podem continuar usando o callback
  baseado em `AppError?`.
- Problem Details é o formato público adotado para respostas de falha HTTP.
- O status padrão para erro de aplicação sem status HTTP específico será definido no planejamento com base no
  comportamento da versão anterior e nas convenções do consumidor.
- A entrega inclui o contrato do pacote, testes, documentação e um consumidor de prova do handler; não inclui criar um
  novo servidor ou alterar consumidores externos não presentes no repositório.
- A compatibilidade com a versão anterior é comportamental para o tratamento centralizado; alterações incompatíveis
  inevitáveis devem ser explicitadas na documentação de migração.
