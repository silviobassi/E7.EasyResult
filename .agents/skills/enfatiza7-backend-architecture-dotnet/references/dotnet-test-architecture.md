
# Enfatiza7 .NET Test Architecture

Choose tests from the repository's declared architecture, behavior, and risk. Use the existing test framework and naming conventions. TDD, when chosen, changes the implementation loop; it does not require one distribution of test levels.

## Choosing a test shape

- A rich domain often benefits from fast domain tests plus integration tests around persistence and contracts.
- Vertical slices need behavior tests close to the feature and integration tests for real boundaries.
- Active Record may need more integration coverage because persistence and behavior meet in the record.
- Transaction Script may need more use-case or integration coverage than isolated domain tests.
- CQRS and Event Sourcing need replay, event-version, and projection checks where those mechanisms exist.
- Blazor can use component and critical-journey tests when the project uses those tools.

These are starting points, not fixed pyramids, diamonds, or quotas. Match the existing project's strategy and the change's risk.

## Verification and architecture rules

Run the narrowest useful tests during local implementation. Widen tests for shared code, database, or boundary changes and run one project build at the end of the implementation scope. Use architecture tests only for explicit, deterministic, valuable rules that existing tools do not already cover. An architecture style preference alone is not a test requirement. Keep library choice and metric thresholds in the project.

