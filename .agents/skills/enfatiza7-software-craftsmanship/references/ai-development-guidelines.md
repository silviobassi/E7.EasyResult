# AI Development Guidelines

## Purpose

Guide AI-generated code toward production-quality engineering decisions.

## General

- Generate production-ready code.
- Do not generate tutorial code.
- Preserve architectural consistency.
- Prefer modifying existing code over introducing new abstractions.

## Before Introducing Any Abstraction

Ask internally:

- Does this reduce complexity?
- Does this solve a current problem?
- Is there more than one implementation?
- Does it improve maintainability?

If any answer is "no", prefer the simpler solution.

## Avoid

- Interfaces with one implementation.
- Factories with one implementation.
- Strategies with one implementation.
- Generic repositories by default.
- Generic services.
- Utility classes containing unrelated behavior.
- Artificial architectural layers.
- Pattern-oriented programming.

## Prefer

- Explicit code.
- Cohesive classes.
- Incremental refactoring.
- Evolutionary architecture.
- Consistency over cleverness.

## Multiple Valid Solutions

- Explain the trade-offs.
- Recommend the simplest solution meeting current requirements.
