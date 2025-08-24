namespace Cliniquelib_BE.Models.Enums.Events
{
    public enum AppErrors
    {
        // General
        UnknownError,
        OperationFailed,
        DatabaseError,
        TimeoutError,
        ConcurrencyConflict,
        ValidationError,
        ServiceUnavailable,
        ExternalServiceFailure,
        LogFail,
        LogRequestFailed,

        // Application
        ApplicationSyncFailed,
        ApplicationBackupFailed,

        // Authentication & Authorization
        RegistrationFailed,
        LoginFailed,
        MFALoginFailed,
        EmailVerificationFailed,
        PasswordResetFailed,
        UnauthorizedAction,
        UserDeactivationFailed,

        // Notifications
        NotificationSendFailed,
        NotificationReadFailed,
        NotificationTypeChangeFailed,
        SmsDeliveryFailed,
        EmailDeliveryFailed,

        // Documents / Files
        DocumentUploadFailed,
        DocumentDownloadFailed,
        DocumentNotFound,
        DocumentAccessDenied,
        DocumentCorrupted,
        FileFormatNotSupported,
        DocumentExportFailed,
        DocumentImportFailed,

        // Clinics & Organizations
        ClinicCreationFailed,
        ClinicUpdateFailed,
        ClinicDeletionFailed,
        OrganizationCreationFailed,
        OrganizationUpdateFailed,
        OrganizationDeletionFailed,
        OrganizationDeactivationFailed,

        // Patient
        PatientProfileUpdateFailed,
        AppointmentBookingFailed,
        AppointmentRescheduleFailed,
        AppointmentCancellationFailed,
        ConsentRevokeFailed,
        PaymentFailed,
        InvoiceGenerationFailed,

        // Teleconsultation
        TeleconsultationSessionFailed,
        TeleconsultationdMessageDeliveryFailed,
        TeleconsultationSessionJoinFailed,
        TeleconsultationRecordingFailed,
    }
}
