# Composition Root

Use when organizing host/API startup, `Program.cs`, service registration, module registration, endpoint mapping, migrations, public boundary DI calls, or repository-approved host-owned wiring.

## Role

The composition root owns application assembly:

```txt
Host/API
├── reads configuration
├── registers modules
├── registers cross-cutting middleware
├── maps module endpoint groups
├── runs migrations/startup tasks
└── registers approved public boundary DI extensions when needed
```

It must not become a business layer.

## Program.cs Rule

Keep `Program.cs` thin. It may contain:

- host builder creation;
- configuration binding at the host level;
- high-level module registration calls;
- high-level public boundary DI registration calls when needed;
- middleware pipeline;
- endpoint mapping calls;
- migration/startup orchestration.

Avoid inline lambdas, mapping logic, provider calls, or DTO translation in `Program.cs` when the logic is specific to inter-module behavior.

Prefer:

```csharp
builder.Services
    .AddSalesModule(builder.Configuration)
    .AddBillingModule(builder.Configuration)
    .AddBillingPublicBoundary();
```

instead of:

```csharp
builder.Services.AddSalesBillingIntegration(async (...) =>
{
    // provider lookup, query, filtering, mapping, fallback
});
```

## Host Structure

Default host shape keeps composition focused on bootstrap:

```txt
src/Backend/<HostProject>/
  Program.cs
  <Bootstrap|Startup|CompositionRoot|DependencyInjection>/
```

Do not create host-owned `ModuleIntegrations` folders for ordinary BC-to-BC calls. The consumer module should call the provider module's `Public/` surface directly when the dependency is allowed.

If repository rules explicitly require host-owned orchestration, reuse the local host convention and keep the orchestration thin.

## Naming

Provider modules may expose public boundary DI extension methods from their own `Public/DependencyInjection` folder, for example:

```csharp
public static IServiceCollection Add<BcName>PublicBoundary(this IServiceCollection services)
```

These methods register only public boundary contracts. Full module bootstrap and internal dependency injection stay in module-internal registration methods such as `Add<BcName>Module`.
