using System;
using FluentAssertions;
using NotificationApi.Domain;
using NotificationApi.Domain.Enums;
using NUnit.Framework;

namespace NotificationApi.UnitTests.Domain.Notifications
{
    public class AssignExternalIdTests
    {
        [Test]
        public void should_update_delivery_status()
        {
            const NotificationType notificationType = NotificationType.CreateIndividual;
            const string toEmail = "test@unit.com";
            var patId = Guid.NewGuid();
            var hearingId = Guid.NewGuid();
            var externalId = Guid.NewGuid().ToString();
            var notificationId = Guid.NewGuid();
            var notification = new EmailNotification(notificationId, notificationType, toEmail, patId, hearingId);
            notification.ExternalId.Should().BeNull();
            notification.AssignExternalId(externalId);
            notification.ExternalId.Should().Be(externalId);
        }
    }
}
