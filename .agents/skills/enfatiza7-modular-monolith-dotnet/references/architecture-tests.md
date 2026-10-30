# Architecture Tests

Use when adding or changing module boundaries, host composition, public contracts, provider adapters, or frontend/backend references.

## Guardrails To Consider

Architecture tests should verify repository-specific versions of these rules:

- modules do not depend on the host/API project;
- modules do not depend on other module internals unless explicitly allowed;
- domain projects/namespaces do not depend on infrastructure, EF Core, provider SDKs, HTTP clients, queues, or host APIs;
- frontend does not reference backend module internals;
- host/API does not contain business endpoints or business rules when endpoints belong in modules;
- implementation types remain internal unless they intentionally cross a boundary;
- provider SDKs do not leak outside owning infrastructure/provider modules;
- architecture style per module does not drift into unrelated ceremony.

## When To Add Or Update

Consider adding or updating an architecture test when an explicit project rule could be violated by:

- creating a new module or bounded context;
- creating a new public contract;
- adding a module integration;
- adding a provider adapter;
- moving endpoint ownership;
- introducing a shared package/project;
- changing frontend/backend dependency direction.

## Test Naming

Prefer names that describe the boundary:

```txt
Modules_Should_Not_Depend_On_ApiHost
Domain_Should_Not_Depend_On_Infrastructure
Frontend_Should_Not_Reference_Module_Internals
Implementation_Types_Should_Be_Internal_By_Default
```

Use the repository's existing test framework and naming convention. Add a new test only when it deterministically protects a valuable rule that is not already covered. Do not introduce a new architecture-test library when one is already standardized.
