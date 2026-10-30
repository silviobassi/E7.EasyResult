# Configuration ownership by bounded context in .NET

Use this guidance when the project declares bounded contexts. Each context defines and validates the options governing its behavior, persistence, providers, and resilience. Configuration for shared infrastructure has a named owner and is consumed through an explicit contract.

- Bind context-owned sections to typed options and validate required values using the project's established .NET options approach.
- Let the host compose configuration sources and register options. The host does not own each context's business meaning.
- Centralized physical `ConnectionStrings` are compatible with context ownership: the context declares which connection it consumes.
- Keep credentials in environment-appropriate secret sources; versioned examples may show keys, never live secrets.
- Use one consistent section shape within a project where it helps binding and readability. Do not create empty sections or separate files and databases for symmetry.
- Test local binding and behavior for a local option change. Widen verification when shared infrastructure or a module boundary changes.
