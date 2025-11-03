# 🏥 YourCarePortal

A **secure, user-friendly platform** designed to help **family members and caregivers** monitor schedules, appointments, and documentation related to their loved one’s home care.

Developed for the **Care Industry**, this solution emphasizes **transparency, coordination,** and **peace of mind** for families involved in ongoing care.

---

## ✨ Features

- 📅 **Schedule Management** – View and track care visits, upcoming appointments, and key milestones.  
- 📂 **Document Access** – Securely upload and manage reports, forms, and care documents.  
- 👪 **Family & Caregiver Collaboration** – Shared access for family members and professional caregivers.  
- 🔐 **Secure & Compliant** – Built with strong authentication and privacy standards.  
- 📊 **Transparency Dashboard** – Real-time overview of care activities and updates.

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-------------|
| Frontend | React.js (TypeScript, Vite) |
| Backend | ASP.NET Core API |
| Database | SQL Server / Azure SQL |
| Authentication | Azure AD B2C / JWT Tokens |
| Hosting | Azure App Services |
| Version Control | GitHub + Git Branching Strategy |

---

## 🚀 Goals

- Ensure **clarity and transparency** in home care processes.  
- Provide **real-time access** to essential information.  
- Simplify communication between families and care providers.  
- Strengthen **trust and accountability** through shared visibility.

---

## 🧭 Vision

Empowering families and caregivers with **a single, transparent digital platform** to oversee care operations, fostering confidence and collaboration across the home-care journey.

---

## 📸 Screenshots *(Coming Soon)*

_Add screenshots of key pages such as Dashboard, Appointments, and Document Viewer here._

---

## ⚙️ Installation *(Optional Section)*

1. Clone the repository:
   ```bash
   git clone https://github.com/<your-username>/YourCarePortal.git
   cd YourCarePortal

    ```bash

   └── YourCarPortal/
    ├── YourCarePortal/
    │   ├── Controllers/
    │   │   ├── AppointmentsController.cs
    │   │   ├── BudgetController.cs
    │   │   ├── ClientDetailsController.cs
    │   │   ├── CustomFormsController.cs
    │   │   ├── HomeController.cs
    │   │   ├── NDISQuotesController.cs
    │   │   ├── NDISStatementController.cs
    │   │   ├── SettingsController.cs
    │   │   ├── StatementsController.cs
    │   │   ├── SupportPlanController.cs
    │   │   └── TemplateController.cs
    │   ├── Data/
    │   │   └── DatabaseContext.cs
    │   ├── Models/
    │   │   ├── Appointment/
    │   │   │   ├── Appointment.cs
    │   │   │   ├── AppointmentDetail.cs
    │   │   │   ├── AppointmentFilter.cs
    │   │   │   ├── AppointmentListItem.cs
    │   │   │   └── AppointmentSummary.cs
    │   │   ├── Budget/
    │   │   │   ├── Budget.cs
    │   │   │   ├── BudgetCategory.cs
    │   │   │   ├── BudgetItem.cs
    │   │   │   └── BudgetSummary.cs
    │   │   ├── Client/
    │   │   │   ├── Client.cs
    │   │   │   ├── ClientContact.cs
    │   │   │   ├── ClientDetail.cs
    │   │   │   └── ClientSummary.cs
    │   │   ├── Common/
    │   │   │   ├── ApiEnvelope.cs
    │   │   │   ├── ApiError.cs
    │   │   │   ├── ApiResult.cs
    │   │   │   ├── KeyValue.cs
    │   │   │   └── Paging.cs
    │   │   ├── Forms/
    │   │   │   ├── CustomForm.cs
    │   │   │   ├── CustomFormField.cs
    │   │   │   └── CustomFormSubmission.cs
    │   │   ├── Ndis/
    │   │   │   ├── NdisQuote.cs
    │   │   │   ├── NdisQuoteItem.cs
    │   │   │   └── NdisStatement.cs
    │   │   ├── Settings/
    │   │   │   └── UserSettings.cs
    │   │   ├── Statements/
    │   │   │   ├── Statement.cs
    │   │   │   ├── StatementDetail.cs
    │   │   │   └── StatementSummary.cs
    │   │   └── Template/
    │   │       ├── Template.cs
    │   │       └── TemplatePreview.cs
    │   ├── Program.cs
    │   ├── Properties/
    │   │   └── launchSettings.json
    │   ├── Services/
    │   │   ├── APIResponseHelper.cs
    │   │   ├── ApiAuthService.cs
    │   │   ├── ApiClientFactory.cs
    │   │   ├── ApiRequestBuilder.cs
    │   │   ├── AppointmentService.cs
    │   │   ├── BudgetService.cs
    │   │   ├── ClientDetailsService.cs
    │   │   ├── CustomFormsService.cs
    │   │   ├── DateFormatService.cs
    │   │   ├── HttpLoggingHandler.cs
    │   │   ├── JsonService.cs
    │   │   ├── NdisQuotesPdfService.cs
    │   │   ├── NdisQuotesService.cs
    │   │   ├── NdisStatementService.cs
    │   │   ├── QueryStringService.cs
    │   │   ├── RequestContextService.cs
    │   │   ├── RouteHelper.cs
    │   │   ├── ScheduleService.cs
    │   │   ├── SecureStringService.cs
    │   │   ├── SessionBootstrapService.cs
    │   │   ├── SessionService.cs
    │   │   ├── SettingsService.cs
    │   │   ├── StatementService.cs
    │   │   ├── StaticAssetsVersionService.cs
    │   │   ├── TemplateService.cs
    │   │   ├── TimeZoneService.cs
    │   │   ├── UrlHelperService.cs
    │   │   └── UserAgentService.cs
    │   ├── Views/
    │   │   ├── Appointments/
    │   │   │   ├── Appointments.cshtml
    │   │   │   ├── _AppointmentFilters.cshtml
    │   │   │   ├── _AppointmentList.cshtml
    │   │   │   └── _AppointmentSummary.cshtml
    │   │   ├── Budget/
    │   │   │   ├── Budget.cshtml
    │   │   │   └── _BudgetSummary.cshtml
    │   │   ├── ClientDetails/
    │   │   │   ├── ClientDetails.cshtml
    │   │   │   └── _ClientSummary.cshtml
    │   │   ├── CustomForms/
    │   │   │   ├── CustomForms.cshtml
    │   │   │   └── _CustomFormList.cshtml
    │   │   ├── Home/
    │   │   │   └── Index.cshtml
    │   │   ├── NDISQuotes/
    │   │   │   ├── NDISQuotes.cshtml
    │   │   │   └── _NdisQuoteList.cshtml
    │   │   ├── NDISStatement/
    │   │   │   └── NDISStatement.cshtml
    │   │   ├── Settings/
    │   │   │   └── Settings.cshtml
    │   │   ├── Shared/
    │   │   │   ├── _Layout.cshtml
    │   │   │   ├── _Nav.cshtml
    │   │   │   └── _ValidationScriptsPartial.cshtml
    │   │   ├── Statements/
    │   │   │   ├── Statements.cshtml
    │   │   │   └── _StatementList.cshtml
    │   │   └── Template/
    │   │       ├── Template.cshtml
    │   │       └── _TemplatePreview.cshtml
    │   ├── YourCarePortal.csproj
    │   ├── appsettings.Development.json
    │   ├── appsettings.json
    │   └── wwwroot/
    │       ├── css/
    │       │   ├── site.css
    │       │   └── vendor/
    │       │       ├── bootstrap.min.css
    │       │       └── sb-admin.css
    │       ├── js/
    │       │   ├── site.js
    │       │   └── vendor/
    │       │       └── bootstrap.bundle.min.js
    │       ├── fonts/
    │       │   ├── Metropolis-Black.otf
    │       │   ├── Metropolis-BlackItalic.otf
    │       │   ├── Metropolis-Bold.otf
    │       │   └── Metropolis-Regular.otf
    │       └── images/
    │           ├── logos/
    │           │   └── yourcareportal-logo.png
    │           └── icons/
    │               └── favicon.ico
    └── YourCarPortal.sln


