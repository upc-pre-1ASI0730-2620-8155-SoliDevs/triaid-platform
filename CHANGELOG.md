# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.0] - 2026-11-12

### Added
- **First Stable Release:**
  - Seven bounded contexts and a shared kernel serving the complete clinical workflow: patient registration, vital signs recording, NT-158 triage classification, automatic alerting, specialty referral, voucher issuance, subscriptions, and device inventory.
  - Full web client contract alignment (ADR-010): session management, anonymous staff registration, string episode identifiers, specialty-by-name referrals, and institution-scoped resources.
- **Release Engineering:**
  - Merge-based version history from 0.1.0 to 0.11.0, with one annotated tag per feature merge into `develop` and this changelog as the release record.

### Changed
- Nothing yet.

### Fixed
- **Approval Flow:**
  - Preserved the client approval state between the approve and confirm steps of classification updates, ensuring alerts are generated exactly once per confirmed classification.

### Deprecated
- Nothing yet.

### Removed
- Nothing yet.

### Security
- Nothing yet.

---

## [0.11.0] - 2026-11-11

### Added
- **Devices Bounded Context:**
  - Domain layer: `Device` aggregate with `EDeviceType`, online state, triage episode linking (`LinkedEpisodeId`) and institution tagging; `VitalSignsReading` aggregate for captured measurements.
  - Application layer: `CreateDeviceCommand` and `UpdateDeviceCommand` accepting the frontend device record on create.
  - Infrastructure layer: `DeviceRepository` and `VitalSignsReadingRepository` with `DeviceConfiguration` and `VitalSignsReadingConfiguration` EF Core mappings; conversion of the last-use timestamp between the frontend time format and the stored date.
  - Interfaces layer: `DevicesController` with `PUT /api/v1/devices/{id}` to update the full device record and `DELETE /api/v1/devices/{id}` to remove a device from the inventory.
- **Endpoint Documentation:**
  - Documented parameters and responses of the device endpoints.

### Changed
- **Resource Mapping:**
  - Moved device resource mapping to a resource-from-entity assembler.
  - Made the device serial number optional and stamped the registration date when a device is created.

### Fixed
- Nothing yet.

### Deprecated
- Nothing yet.

### Removed
- Nothing yet.

### Security
- Nothing yet.

---

## [0.10.0] - 2026-11-11

### Added
- **Alerting Queries & Commands:**
  - Domain layer: `CreateAlertCommand`, `UpdateAlertCommand`, `AcknowledgeAlertCommand` and `EscalateAlertCommand` implementing the severity state machine; `GetActiveAlertsQuery`, `ListAllAlertsQuery` and `GetAlertByIdQuery`.
  - Application layer: `AlertCommandService` and `AlertQueryService` handlers; `ListAsync` and `GetByIdAsync` methods on `IAlertRepository`.
  - Interfaces layer: enriched `AlertResource` with patient details and severity representation.
- **RESTful Alert Endpoints:**
  - Added `AlertsController` with full CRUD under `/api/v1/alerts`, plus input resources (`CreateAlertResource`, `UpdateAlertResource`, `EscalateAlertResource`) and the command assemblers (`AlertCommandFromResourceAssemblers`, `AcknowledgeAlertCommandFromResourceAssembler`, `EscalateAlertCommandFromResourceAssembler`, `AlertResourceFromEntityAssembler`).

### Changed
- Nothing yet.

### Fixed
- **Query Reliability:**
  - Evaluated active alert queries in memory to avoid Entity Framework Core translation errors on complex filters.

### Deprecated
- Nothing yet.

### Removed
- Nothing yet.

### Security
- Nothing yet.

---

## [0.9.0] - 2026-11-11

### Added
- **Voucher Persistence & Symptom Catalog:**
  - Domain layer: `Voucher` aggregate with `IVoucherRepository`.
  - Infrastructure layer: `VoucherRepository` with EF Core configuration.
  - Interfaces layer: `VouchersController` for voucher issuance and `SymptomsController` exposing the sixteen-symptom clinical catalog under `/api/v1/symptoms`.
- **Referral Records for the Web Client:**
  - Exposed referral records under `/api/v1/specialty-assignment/referrals`, accepting the specialty by name plus symptom and institution data.

### Changed
- **Contract Alignment:**
  - Aligned triage classification episode identifiers with the web client (string identifiers).
  - Opened subscription endpoints to the web client contract.

### Fixed
- Nothing yet.

### Deprecated
- Nothing yet.

### Removed
- Nothing yet.

### Security
- Nothing yet.

---

## [0.8.0] - 2026-11-11

### Added
- **Session Management:**
  - Domain layer: `Session` aggregate for work sessions.
  - Interfaces layer: `SessionsController` with `GET /api/v1/sessions`, `POST /api/v1/sessions` and `DELETE /api/v1/sessions/{id}` for web client session handling.
  - Anonymous staff registration and plain password exposure scoped to the course authentication model.

### Changed
- **Web Client Contract:**
  - Opened staff endpoints to the web client contract.

### Fixed
- Nothing yet.

### Deprecated
- Nothing yet.

### Removed
- Nothing yet.

### Security
- Nothing yet.

---

## [0.7.0] - 2026-11-11

### Changed
- **API Conventions:**
  - Applied a global `api/v1` route prefix through `RoutePrefixConvention`, with camelCase enum serialization via `JsonStringEnumConverter` for all resources.

### Fixed
- **Startup:**
  - Ensured the database schema is created before seeding on startup.

### Deprecated
- Nothing yet.

### Removed
- Nothing yet.

### Added
- Nothing yet.

### Security
- Nothing yet.

---

## [0.6.0] - 2026-11-11

### Added
- **Cross-Context Integration:**
  - Integrated the Triage Classification, Specialty Assignment, Devices, and Subscription bounded contexts into `develop`.
  - Added `VitalSignsReadingAssembler` mapping helper for device-to-triage vital sign transformation.

### Changed
- Nothing yet.

### Fixed
- **Merge Hygiene:**
  - Resolved `Program.cs` and `AppDbContext` conflicts across bounded contexts during the integration branch.

### Deprecated
- Nothing yet.

### Removed
- Nothing yet.

### Security
- Nothing yet.

---

## [0.5.0] - 2026-11-10

### Added
- **Subscription Bounded Context:**
  - Domain layer: `Subscription` and `Institution` aggregates with `ISubscriptionRepository` and `IInstitutionRepository`.
  - Domain services: `IPaymentGateway` port.
  - Infrastructure layer: `SubscriptionRepository`, `PlanRepository`, `InstitutionRepository` and the `SimulatedPaymentGateway`.
  - Application layer: `SubscriptionCommandService` and `SubscriptionQueryService`.
  - Interfaces layer: `SubscriptionsController`, `PlansController` and `InstitutionsController` with the subscription REST facade compatible with the web client.
- **IAM REST Facade:**
  - Interfaces layer: `UsersController` with IAM REST resources, assemblers and controllers, with role-based access control.

### Changed
- **Host Composition:**
  - Wired JWT authentication and the database context in the host.
  - Consolidated EF model configuration across bounded contexts after the devices merge.

### Fixed
- Nothing yet.

### Deprecated
- Nothing yet.

### Removed
- Nothing yet.

### Security
- **Endpoint Protection:**
  - Authenticated subscription endpoints through the IAM middleware.

---

## [0.4.0] - 2026-11-10

### Added
- **Specialty Assignment Bounded Context:**
  - Domain layer: `Specialty`, `Referral` and `Voucher` aggregates; `ReferEpisodeCommand`, `ReassignReferralCommand`; `ReferralCreatedEvent` and `ReferralReassignedEvent`; `SuggestSpecialtyQuery` and `GetReferralByEpisodeQuery`; `SpecialtyRules` clinical referral rules (US30-US33).
  - Application layer: `SpecialtyAssignmentService`.
  - Infrastructure layer: `SpecialtyRepository`, `ReferralRepository` and `VoucherRepository` with EF Core configuration and `SpecialtyCatalogSeed`.
  - Interfaces layer: REST facade with `ReferEpisodeResource` and `ReassignReferralResource`.

### Changed
- Nothing yet.

### Fixed
- **Alerting Persistence:**
  - Corrected the alerting owned-type configuration while wiring the specialty assignment services.

### Deprecated
- Nothing yet.

### Removed
- Nothing yet.

### Security
- Nothing yet.

---

## [0.3.0] - 2026-11-10

### Added
- **Triage Classification Bounded Context:**
  - Domain layer: `Classification` aggregate with `ETriageLevel`, `EClassificationState` and `VitalSignsSummary` value objects; `NT158Engine` normative triage engine with the normative rule table (US19).
  - Commands and queries: `SuggestClassificationCommand`, `ModifyClassificationCommand`, `ConfirmClassificationCommand`, `GetAllClassificationsQuery` and `GetClassificationByEpisodeQuery`.
  - Domain events: `ClassificationConfirmedEvent` and `ClassificationModifiedEvent`.
  - Application layer: `ClassificationCommandService` and `ClassificationQueryService`.
  - Infrastructure layer: `IClassificationRepository` contract and `ClassificationRepository` with EF Core mapping.
  - Interfaces layer: `/triage-classifications` facade in `ClassificationsController` with `ClassificationResource`, `SuggestClassificationResource`, `ModifyClassificationResource` and the resource/command assemblers.

### Changed
- Nothing yet.

### Fixed
- Nothing yet.

### Deprecated
- Nothing yet.

### Removed
- Nothing yet.

### Security
- Nothing yet.

---

## [0.2.0] - 2026-11-06

### Added
- **Alerting Bounded Context:**
  - Domain layer: `Alert` aggregate with clinical context (patient, episode, priority, `EAlertLevel` and `AlertMetric`), `AlertStatus` and `ClinicId` value objects.
  - Commands: `CreateAlertCommand`, `UpdateAlertCommand`, `AcknowledgeAlertCommand` and `EscalateAlertCommand` to retrieve active alerts, acknowledge them, and escalate them to a destination.
  - Infrastructure layer: `IAlertRepository` contract and `AlertRepository` with EF Core persistence configuration.

### Changed
- Nothing yet.

### Fixed
- **Initialization:**
  - Initialized alert metric properties to prevent null reference exceptions.

### Deprecated
- Nothing yet.

### Removed
- Nothing yet.

### Security
- **Clinic Data Isolation:**
  - Clinic-scoped alert queries and commands to isolate alert data by clinic.

---

## [0.1.0] - 2026-11-04

### Added
- **API Foundation:**
  - Initial TriAid Platform API with ASP.NET Core and .NET 10 configuration.
  - Data access layer integration with MySQL via Entity Framework Core and database schema auto-creation at startup.
  - OpenAPI support with the interactive Scalar documentation UI for local development.
- **Patient Registration Bounded Context:**
  - Domain layer: `Patient` and `TriageEpisode` aggregates with `Sex` and `EEpisodeState` value objects.
  - Commands: `CreatePatientCommand`, `CreateTemporaryProfileCommand`, `RegularizeIdentityCommand`, `UpdateContactCommand` and `OpenTriageEpisodeCommand`.
  - Domain events: `PatientRegisteredEvent`.
  - Application layer: `PatientRegistrationService` and `TriageEpisodeService` for patient listing, retrieval, DNI lookup, registration flows, temporary profile creation, identity regularization, contact updates, episode opening, patient episode queries, triage start, and attended or referred status transitions.
  - Infrastructure layer: `IPatientRepository`/`ITriageEpisodeRepository` contracts with their EF Core repositories.
- **Shared Kernel:**
  - `AggregateRoot`/`IAggregateRoot`, `Error`, `IEvent`, `IEntityId`, `IAuditableEntity`, `IBaseRepository` and `IUnitOfWork` patterns supporting repository-based persistence and consistent API conventions, plus the `Result` application model.

### Changed
- **Route Conventions:**
  - Standardized API route naming to kebab-case and enabled lowercase URLs for cleaner endpoint conventions.

### Fixed
- Nothing yet.

### Deprecated
- Nothing yet.

### Removed
- Nothing yet.

### Security
- **Baseline Configuration:**
  - Baseline configuration for HTTPS redirection and protected API exposure in the ASP.NET Core pipeline.
