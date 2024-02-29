﻿namespace TaktTusur.Media.Domain.Common
{
    public class Event
    {
        public int Id { get; set; }

        public long EventStartDateTime { get; set; }

        public int MemberStatus { get; set; }

        public bool IsFavorite { get; set; }

        public string Address { get; set; }

        public string Text { get; set; }

        public string ButtonText { get; set; }

        public int[] Friends { get; set; }
    }
}