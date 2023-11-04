using System;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class Stat
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public bool IsUp { get; set; }
        public DateTime RecordedDate { get; set; }
        public int Type { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}