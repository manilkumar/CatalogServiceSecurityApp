﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogServiceSecurityApp.Models.DbModels
{
    public class UserEvent
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public string EventId { get; set; }
        public Event Event { get; set; }
    }
}
