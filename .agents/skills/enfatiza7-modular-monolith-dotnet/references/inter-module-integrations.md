# Inter-Module Integrations

Use when one module needs a capability owned by another module.

## Default Pattern

```txt
<ConsumerModule>
  references provider module Public/ contracts only when direct dependency is allowed
  translates provider contract models to consumer-owned models inside the consumer adapter/application flow

<ProviderModule>
  exposes minimal provider contract from Public/
  binds only public boundary interfaces to internal implementations through Public/DependencyInjection

<HostProject>
  registers modules
  maps endpoints
  runs migrations/provisioning
  does not own BC-to-BC integration wiring unless repository rules explicitly require host-owned orchestration
```

The consumer may know the provider module's intentional `Public/` surface. It must not know provider internals such as
`Application`, `Domain`, `Infrastructure`, persistence models, SDK models, handlers, or validators.

When a provider exposes interfaces, contracts, events, or injectable public services, those types live under the provider
module's `Public/` folder. Public DI extensions live under `Public/DependencyInjection` only for public boundary contract registrations and may bind to internal implementations.

Do not move full module bootstrap or module-internal dependency-injection code into `Public/DependencyInjection`. Module bootstrap, persistence, options, endpoints, migrations, workers, and internal-only service registration remain in the owning module's normal bootstrap/dependency-injection structure.

Name public module interfaces by capability. Prefer `I<Capability>Provider` for cross-module provider capabilities, for
example `IConsultantEligibilityProvider` or `IConsultantAvailabilityProvider`. Avoid broad names such as
`IConsultantManagementService` unless the contract truly represents a coarse service boundary approved by the project.

Ports are for infrastructure or external systems such as databases, Redis, AI, WhatsApp, e-mail, queues, storage, and
provider SDKs. Do not model another module as a port when that module can expose an intentional `Public/` contract.

Use `Public/Events` when communication can be asynchronous or represents a notification between modules.

## Physical Shape

Provider module:

```txt
<ProviderModule>/
  Public/
    Interfaces/
      I<Capability>Provider.cs
    Contracts/
      Requests/
      Responses/
      Dtos/
      Records/
      Enums/
      Errors/
    Events/
    DependencyInjection/  # public boundary contract registrations only
```

Consumer module:

```txt
<ConsumerModule>/
  <Application|Features|Infrastructure>/
    <Capability>Adapter.cs
```

The adapter belongs to the consumer because it translates provider public models into consumer-owned models and failure
policy.

## Naming

Use provider public contract names by capability:

```csharp
I<Capability>Provider
Get<Capability>Query
<Capability>Snapshot
<Capability>Response
```

Use consumer adapter names by consumed capability:

```csharp
<ProviderCapability>Adapter
<ProviderCapability>Resolver
```

Avoid `I<ModuleName>Service` for cross-module public contracts unless the project explicitly approves a coarse service
facade.

## Translation

Provider models and consumer models should remain separate unless there is an explicitly shared contract.

Translate in the consumer module adapter/application flow:

```txt
ProviderSnapshot -> ConsumerSnapshot/Candidate
ProviderError -> Consumer fallback/error policy
Provider identifiers -> Consumer strongly typed ids/value objects
```

Do not leak provider persistence models, SDK models, or internal domain objects into the consumer module.

## Failure Policy

Keep failure handling explicit in the consumer adapter/application flow:

- return an empty candidate set only when conservative denial/no-op is the intended business fallback;
- propagate or translate errors when the consumer must fail fast;
- document non-obvious fallback behavior in the adapter method or test name.