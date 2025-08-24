namespace Cliniquelib_BE.Models.Enums
{
    public enum AppEvents
    {

        // Application Status
        ApplicationOnline,
        ApplicationOffline,
        ApplicationSynced,
        ApplicationBackup,
        LogRquest,

        // System Administration
        OrganizationCreated,
        OrganizationUpdated,
        OrganizationDeleted,
        OrganizationDeactivated,
        ClinicCreated,
        ClinicUpdated,
        ClinicDeleted,
        ClinicDeactivated,

        // Authentication & Authorization - User Registration
        UserRegistered,
        EmailVerificationSent,
        EmailVerified,
        UserLoggedIn,
        UserLoggedOut,
        PasswordResetRequested,
        PasswordResetCompleted,
        MFASetupCompleted,
        MFALoginCompleted,
        AdminUserCreated,
        UserRoleAssigned,
        UserRoleRemoved,
        UserPermissionsModified,
        UserDeactivated,
        UserReactivated,
        UserSessionInvalidated,
        NewUserInvited,

        // Notifications
        NotificationTypeChange,
        NotificationSent,
        NotificationRead,

        // Documents
        DocumentCreated,
        DocumentImported,
        DocumentDeleted,
        DocumentUpdated,
        DocumentExported,


        // Patient Engagement & Portal - Patient Profile Management
        PatientProfileViewed,
        PatientProfileUpdated,
        NotificationPreferencesUpdated,
        DoctorSearched,
        AvailableSlotsSearched,
        AppointmentBooked,
        AppointmentRescheduled,
        AppointmentCancelled,
        LateCancellationWarningIssued,

        // Patient Engagement & Portal - Medical Records Access
        MedicalHistoryViewed,
        LabResultViewedByPatient,
        ImagingResultViewedByPatient,

        // Patient Engagement & Portal - Billing & Payments
        InvoiceViewedByPatient,
        InvoiceDownloadedByPatient,
        BillPaymentInitiated,
        PaymentReceived,

        // Patient Engagement & Portal - Consent Management
        ConsentGranted,
        ConsentRevoked,

        // Patient Engagement & Portal - Secure Messaging
        MessageThreadCreated,
        MessageSent,
        MessageRead,

        // Clinical Workflow & EHR - Clinician Dashboard
        DashboardViewed,

        // Clinical Workflow & EHR - Encounter Management
        EncounterStarted,
        EncounterEnded,

        // Clinical Workflow & EHR - Clinical Documentation
        VitalsRecorded,
        ConditionAdded,
        ConditionUpdated,
        ConditionRemoved,
        AllergyAdded,
        AllergyUpdated,
        AllergyRemoved,
        ClinicalNoteTemplateApplied,

        // Clinical Workflow & EHR - Medication Management
        PrescriptionCreated,
        PrescriptionUpdated,
        PrescriptionFilled,
        DrugInteractionAlertRaised,
        PrescriptionHistoryViewed,

        // Clinical Workflow & EHR - Orders & Results Management
        LabOrderCreated,
        LabResultReceived,
        LabResultReviewedAndFinalized,
        ImagingOrderCreated,
        ImagingResultReceived,
        ImagingResultReviewedAndFinalized,

        // Front-Office & Scheduling
        AppointmentSlotsGenerated,
        AppointmentSlotsUpdated,
        PatientCheckedIn,
        WaitlistEntryAdded,
        WaitlistEntryMatched,

        // Billing & Financials
        InvoiceGenerated,
        InvoiceUpdated,
        InvoiceIssued,
        InvoiceCompleted,
        FinancialReportGenerated,

        // Analytics & Reporting
        OperationalReportGenerated,

        // Teleconsultation
        TeleconsultationSessionInitiated,
        TeleconsultationSessionStarted,
        TeleconsultationSessionJoinedByPatient,
        TeleconsultationSessionEnded,
        TeleconsultationChatMessageSent,
        TeleconsultationSessionRecorded,


    }
}
