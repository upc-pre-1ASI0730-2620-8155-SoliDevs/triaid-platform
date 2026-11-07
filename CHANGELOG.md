# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.2.0] - 2026-11-06

### Added
- Alerting workflows to retrieve active alerts, acknowledge them, and escalate them to a destination.
- REST endpoints and resources for alert operations, with Entity Framework Core persistence configuration.

### Security
- Clinic-scoped alert queries and commands to isolate alert data by clinic.

### Fixed
- Initialized alert metric properties to prevent null reference exceptions.

## [0.1.0] - 2026-11-04

### Added
- Initial TriAid Platform API foundation with ASP.NET Core and .NET 10 configuration.
- Patient Registration bounded context, including patient listing, retrieval, DNI lookup, registration flows, temporary profile creation, identity regularization, and contact updates.
- Triage Episodes bounded context for opening episodes, patient episode queries, triage start, and status transitions for attended or referred cases.
- Shared application, domain, and infrastructure patterns to support repository-based persistence and consistent API conventions.
- Data access layer integration with MySQL via Entity Framework Core and database schema auto-creation at startup.
- OpenAPI support and API documentation configuration for local development.

### Changed
- Standardized API route naming to kebab-case and enabled lowercase URLs for cleaner endpoint conventions.

### Security
- Baseline configuration for HTTPS redirection and protected API exposure in the ASP.NET Core pipeline.
