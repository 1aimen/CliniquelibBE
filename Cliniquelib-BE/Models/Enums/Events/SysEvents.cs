namespace Cliniquelib_BE.Models.Enums.Events
{
    public enum SysEvents
    {
        // General
        ApplicationStarted,
        ApplicationStopped,
        ApplicationCrashed,
        ConfigurationLoaded,
        ConfigurationReloaded,
        EnvironmentVariablesLoaded,

        // Infra
        DatabaseConnected,
        DatabaseDisconnected,
        DatabaseMigrationApplied,
        DatabaseMigrationFailed,
        CacheHit,
        CacheMiss,
        CacheRefreshed,
        RedisConnectionOpened,
        RedisConnectionClosed,

        // API / HTTP
        HttpRequestReceived,
        HttpRequestCompleted,
        HttpRequestFailed,
        ExternalApiCalled,
        ExternalApiResponseReceived,
        ExternalApiTimeout,

        // Auth
        UserLoginSucceeded,
        UserLoginFailed,
        UserLogout,
        UserRoleChanged,
        JwtTokenIssued,
        JwtTokenExpired,
        MFAChallengeSent,
        MFAChallengeValidated,

        // Notifications & Messaging
        EmailQueued,
        EmailSent,
        EmailDeliveryFailed,
        SmsSent,
        SmsDeliveryFailed,
        PushNotificationSent,
        PushNotificationFailed,
        WebhookSent,
        WebhookFailed,

        // Teleconsultation
        TeleconsultationStarted,
        TeleconsultationEnded,
        TeleconsultationReconnected,
        TeleconsultationDropped,
        TeleconsultationMessageSent,
        TeleconsultationMessageReceived,

        // Files
        FileUploaded,
        FileDeleted,
        FileAccessed,
        FileDownloadStarted,
        FileDownloadCompleted,
        FileCorrupted,

        // Audit
        PermissionGranted,
        PermissionRevoked,
        RoleCreated,
        RoleDeleted,
        SuspiciousActivityDetected,
        BruteForceLoginAttempt,
        DataExported,
        DataImported,

        // Jobs / Workers
        JobScheduled,
        JobStarted,
        JobCompleted,
        JobFailed,
        JobRetried,
        CronTriggered,

        // Monitoring / Health
        HealthCheckPassed,
        HealthCheckFailed,
        ServiceDegraded,
        ServiceRecovered,
        DiskSpaceLow,
        MemoryThresholdExceeded,
        CpuThresholdExceeded,
        NetworkLatencyHigh,

        // DevOps
        DeploymentStarted,
        DeploymentCompleted,
        DeploymentFailed,
        FeatureFlagEnabled,
        FeatureFlagDisabled,
        RollbackInitiated,
        RollbackCompleted
    }
}
