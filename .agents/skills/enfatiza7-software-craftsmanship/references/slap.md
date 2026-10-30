# SLAP

Reference: Single Level of Abstraction Principle.

## Guidelines

Each method should operate at a single level of abstraction.

- High-level methods orchestrate.
- Low-level methods implement details.

## Example

Prefer orchestration like:

```text
ProcessOrder()
    ValidateOrder()
    CalculateTotals()
    PersistOrder()
    PublishEvents()
```

## Avoid

- Mixing orchestration with implementation details.
- Extracting methods that reduce readability.
