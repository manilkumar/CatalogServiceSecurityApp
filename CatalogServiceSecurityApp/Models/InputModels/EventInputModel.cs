﻿using CatalogServiceSecurityApp.Models.DbModels;

namespace CatalogServiceSecurityApp.Models.InputModels
{
    public class EventInputModel
    {
        public string Id { get; set; }

        public string? Title { get; set; }

        public string? Category { get; set; }

        public DateTime Date { get; set; }

        public IList<string> UserEmails { get; set; }
    }
}
