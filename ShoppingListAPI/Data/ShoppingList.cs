using System;
using System.Collections.Generic;

namespace ShoppingListAPI.Data
{
    public class ShoppingList
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime? TripDate { get; set; }

        public bool IsCompleted { get; set; }

        public List<LineItem> LineItems { get; set; }
    }
}
