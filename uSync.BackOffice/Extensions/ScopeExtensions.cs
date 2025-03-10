﻿using System;

using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Scoping;

using uSync.BackOffice.Configuration;
using uSync.BackOffice.Notifications;
using uSync.BackOffice.SyncHandlers;

namespace uSync.BackOffice.Extensions;
internal static class ScopeExtensions
{
    public static IDisposable SuppressScopeByConfig(this ICoreScope scope, uSyncConfigService configService)
        => configService.Settings.DisableNotificationSuppression
            ? new DummyDisposable()
            : scope.Notifications.Suppress();


    public static ICoreScope CreateNotificationScope(
        this ICoreScopeProvider scopeProvider,
        IEventAggregator eventAggregator,
        ILoggerFactory loggerFactory,
        SyncUpdateCallback callback)
    {
        
        var notificationPublisher = new SyncScopedNotificationPublisher(
            eventAggregator, loggerFactory.CreateLogger<SyncScopedNotificationPublisher>(), callback);

        return scopeProvider.CreateCoreScope(
            scopedNotificationPublisher: notificationPublisher, 
            autoComplete: true);
    }
}

/// <summary>
///  a dummy disposable class, so we can use it when we don't suppress notifications.
/// </summary>
internal class DummyDisposable : IDisposable
{
    public void Dispose()
    {
        // nothing to dispose. 
    }
}
