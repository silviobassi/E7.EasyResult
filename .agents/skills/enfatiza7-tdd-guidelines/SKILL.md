---
name: enfatiza7-tdd-guidelines
description: Enfatiza7 Red-Green-Refactor guidance. Use only when the user or project has chosen TDD for the work.
---

# Enfatiza7 TDD Guidelines

Use this skill when TDD has been chosen. Tests specify observable behavior first, then production code follows in small verifiable steps. TDD does not determine the project's test pyramid, diamond, or inverted pyramid; use the project-declared test strategy.

## Required Reference

Before giving advice or changing code, read:

- `references/tdd-guidelines.md`

## Operating Rules

- Always follow Red -> Green -> Refactor.
- Start by identifying the expected observable behavior.
- Write one failing test before production code.
- Keep each step small and behavior-focused.
- Write the simplest production code that makes the test pass.
- Refactor only after all tests pass.
- Treat difficult tests as design feedback.
- Prefer real implementations over mocks when practical.
- Mock only true collaborators.
- Do not test private methods, framework behavior, implementation details, or meaningless coverage targets.
- Explain trade-offs when multiple testing approaches are valid.
