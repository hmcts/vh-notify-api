using System.Collections.Generic;
using NotifyApi.Domain;

namespace Testing.Common.Models
{
    public class TestRun
    {
        public TestRun()
        {
            NotificationsCreated = new List<Notification>();
        }
        public List<Notification> NotificationsCreated { get; set; }
    }
}
