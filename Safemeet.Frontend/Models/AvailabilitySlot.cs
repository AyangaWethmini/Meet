namespace SafeMeet.Frontend.Models
{
           public class AvailabilitySlot
           {
               public string Id { get; set; }
               public string UserId { get; set; }
               public DateTime Date { get; set; }
               public TimeSpan StartTime { get; set; }
               public TimeSpan EndTime { get; set; }
               public int PreferenceRating { get; set; }
           }
       }