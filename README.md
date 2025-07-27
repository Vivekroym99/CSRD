# CSRD & ESG Reporting System

A comprehensive Windows Forms application for Corporate Sustainability Reporting Directive (CSRD) compliance and European Sustainability Reporting Standards (ESRS) data management.

## Overview

This application assists organizations in collecting, managing, analyzing, and reporting sustainability data in accordance with CSRD requirements and ESRS standards, with specific consideration for Polish legal transposition and general ESG principles.

## Key Features Implemented

✅ **Project Structure & Setup**
- Complete .NET 6.0 Windows Forms project
- NuGet packages: SQLite, Dapper, DataVisualization

✅ **Data Models (ESRS Compliant)**
- EmissionRecord (ESRS E1 Climate Change)
- EnergyConsumption (ESRS E1 Energy)
- WorkforceDiversity (ESRS S1 Own Workforce)  
- MaterialityAssessment (Double Materiality)

✅ **Database Layer**
- SQLite database with automatic initialization
- ESRS-compliant table schemas
- Database connection management

✅ **Main Application Interface**
- Tabbed navigation for different ESRS areas
- Menu system with comprehensive functionality
- Dashboard overview and module access

✅ **Emissions Data Entry (ESRS E1)**
- Complete form for GHG emissions data entry
- Scope 1, 2, 3 emissions with real-time calculation
- Data quality and verification tracking
- Input validation and user guidance

✅ **Data Access Layer**
- Repository pattern implementation
- Async/await data operations
- CRUD operations for all entities
- Business logic separation

✅ **Reporting System**
- ESRS E1 Climate Change report generation
- Compliance checklist automation
- Export capabilities (CSV, formatted reports)
- Trend analysis and statistics

✅ **Validation & Error Handling**
- Comprehensive input validation
- ESRS-specific business rules
- Centralized error logging
- User-friendly error messages

## Technical Architecture

### Technology Stack
- **Framework**: .NET 6.0 Windows Forms
- **Database**: SQLite (embedded)
- **Data Access**: Dapper ORM with Repository pattern
- **Architecture**: Layered design (Presentation, Business Logic, Data Access)

### Build Instructions
```bash
dotnet build CSRDReporting.csproj
dotnet run
```

## CSRD Compliance Features

### Double Materiality Assessment
- Impact materiality scoring (1-5 scale)
- Financial materiality evaluation
- Stakeholder consultation tracking
- Management response documentation

### ESRS E1 Climate Change
- Scope 1, 2, 3 GHG emissions tracking
- Location-based and market-based Scope 2
- Energy consumption monitoring
- Data quality and verification status

### Data Quality Framework
- Quality levels: High, Medium, Low, Estimated
- Verification status tracking
- Third-party verification support
- Methodology documentation

This application provides a solid foundation for CSRD compliance while maintaining flexibility for Polish regulatory requirements and future ESRS standard updates. 
