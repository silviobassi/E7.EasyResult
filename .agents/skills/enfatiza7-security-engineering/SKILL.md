---
name: enfatiza7-security-engineering
description: Enfatiza7 guidance for proportionate software security design and review. Use when a change affects authentication, authorization, sensitive data, untrusted input, secrets, dependencies, the build supply chain, or a trust boundary.
---

# Enfatiza7 Security Engineering

Apply this skill only to security-relevant changes. Keep the analysis at the task's scale. A local change may need a
short explanation; a new trust boundary or broad privilege change may need a deeper review before implementation.

## Decide what matters

- Identify the asset or behavior to protect, the trust boundary, and a plausible threat introduced or affected by the
  change.
- Name a relevant [CWE](https://cwe.mitre.org/) when it clarifies a possible software weakness. Choose a control and the
  smallest useful test or other evidence for that control. Do not require a CWE identifier when none fits.
- Use the project's existing security checks and required gates. A semantic review cannot replace a failed or
  unavailable deterministic check. Do not add a scanner, threshold, or checklist solely because this skill was loaded.
- Report material findings with the affected boundary, consequence, evidence, and an actionable correction. Escalate
  unresolved security design or product decisions.

## Select references by context

- Use [OWASP Threat Modeling](https://owasp.org/projects/threat-modeling-project/) for design questions. For affected
  web or API controls, select relevant requirements from [ASVS](https://owasp.org/projects/asvs/)
  or [API Security](https://owasp.org/projects/api-security-project/) and relevant verification ideas from
  the [Web Security Testing Guide](https://owasp.org/projects/web-security-testing-guide/).
- Use [NIST SSDF](https://csrc.nist.gov/pubs/sp/800/218/final) as general secure-development
  guidance. [SAFECode Fundamental Practices](https://safecode.org/resource-secure-development-practices/fundamental-practices-secure-software-development-2/)
  and the current [CWE Top 25](https://cwe.mitre.org/top25/) are optional references, not per-task checklists.
- For affected components, assess known [CVEs](https://www.cve.org/) reported by existing tools or advisories.
  Use [CVSS](https://www.first.org/cvss/) and known exploitation such
  as [CISA KEV](https://www.cisa.gov/known-exploited-vulnerabilities-catalog) alongside actual exposure and impact; no
  single score decides priority.
- Use a matching [CIS Benchmark](https://www.cisecurity.org/cis-benchmarks-overview) only when the project owns the
  affected platform configuration. Consult [NIST SP 800-53](https://csrc.nist.gov/pubs/sp/800/53/r5/upd1/final) controls
  only when the project declares them. BSIMM concerns organization-level maturity, not an ordinary task gate.

The primary reviewer can apply this guidance to a bounded change. Consult a security specialist when a new trust
boundary, broad privilege change, or high-consequence threat needs deeper independent analysis. Neither this skill nor
that specialist can waive the project's required gates.
