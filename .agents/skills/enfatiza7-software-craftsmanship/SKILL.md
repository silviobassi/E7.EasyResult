---
name: enfatiza7-software-craftsmanship
description: Enfatiza7 guidance for pragmatic, maintainable production code. Use for implementation or review when simplicity, abstraction choices, or code clarity matter.
---

# Enfatiza7 Software Craftsmanship

Use this skill to produce code that is simple, readable, maintainable, evolvable, testable, consistent with the existing
architecture, and ready for production.

## References

Read only the references relevant to the current task:

- `references/software-craftsmanship.md`
- `references/pragmatism.md`
- `references/clean-code.md`
- `references/solid.md`
- `references/slap.md`
- `references/object-calisthenics.md`
- `references/ai-development-guidelines.md`

## Operating Rules

- Prefer pragmatic, explicit code over clever or pattern-heavy code.
- Preserve the architecture and style already present in the codebase.
- Modify existing code before adding new abstractions.
- Introduce abstractions only when they reduce current complexity or protect a real boundary.
- Treat principles as heuristics, not obligations.
- Optimize first for pragmatism, then simplicity, readability, maintainability, extensibility, and performance.
- Explain trade-offs when more than one valid solution exists, then recommend the simplest solution that satisfies
  current requirements.

## Contextual sizing

Prefer cohesive files that are easy to understand. A project may adopt 200 lines as a review signal for relevant
authored production files, including C# backend, C#/Razor frontend, Java and JavaScript/TypeScript components.
This is not a completion gate or a universal recommendation for each class or method. Extract responsibilities when
that reduces current complexity or protects a real boundary; never split merely to satisfy a count. Declarative UI may
remain longer when cohesive. Tests, docs, configuration, styles and generated/vendor files are excluded. Review affected
code only; do not scan legacy files or install a line-count tool. Existing project policies remain authoritative.

## Review

When reviewing an implementation, report concrete defects, avoidable complexity, and material test gaps. Request
independent review when the task or project requires it, or when material impact or uncertainty justifies it. Follow the
project's verification contract; this skill does not require a particular agent, verifier, commit cadence, or
context-meter threshold.
