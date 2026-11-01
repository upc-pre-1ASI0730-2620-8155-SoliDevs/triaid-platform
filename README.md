# Vital Signs Capture - Bounded Context

This repository and module implement the **Vital Signs Capture** Bounded Context, responsible for medical device inventory management, real-time data synchronization, and vital signs capture (both automated and via contingency manual entry) for patient clinical episodes.
this is an early version of what the tri-aid platform can do
---

## 📋 Overview

The **Vital Signs Capture** bounded context operates under the principles of **Domain-Driven Design (DDD)** and Clean Architecture. Its primary function is to ensure medical personnel can link measurement devices (blood pressure monitors, pulse oximeters, heart rate monitors, and thermometers) to a triage episode, record readings securely, and validate clinical range boundaries before confirming a preliminary diagnosis.

---

## System Architecture

The project is structured into well-defined layers to separate business rules from technical infrastructure:

- **`Domain`**: Contains pure business entities, such as the device model (`Device`) and the catalogue of vital sign measurement types with their respective clinical limits and units of measure (`DeviceType`).
- **`Application`**: Manages use cases, device state logic, and linking/unlinking business rules for clinical episodes.
- **`Infrastructure`**: Houses data persistence mechanisms using Entity Framework Core and database context configurations.
- **`Presentation`**:
    - **Views & Panels**: Interactive user interface components for device management and the operational vital signs panel.
    - **API Controllers**: RESTful endpoints for integration with the frontend.

---

## ✨ Key Features & Capabilities

- **Device Inventory Management**:
    - Real-time connectivity status tracking (*online* / *offline*) for medical equipment.
    - Dynamic linking and unlinking of specific devices to an active triage episode.
- **Vital Signs Data Capture**:
    - Simulated automated reading capture from linked hardware.
    - Contingency manual entry mode with strict clinical range validation for:
        - **Blood Pressure (BP)** (Systolic and Diastolic)
        - **Oxygen Saturation ($\text{SpO}_2$)**
        - **Heart Rate (HR)**
        - **Body Temperature**

---

## 🚀 Tech Stack & Tooling

- **Back-end**: .NET Core 8, C#, Entity Framework Core
- **Front-end**: Vue.js 3 (`<script setup>`), Vite