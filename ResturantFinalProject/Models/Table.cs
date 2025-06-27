namespace ResturantFinalProject.Models
{
    public class Table : BaseEntity
    {
        public int Number { get; set; }

        public int Capacity { get; set; }

        public bool IsReserved { get; set; }

        public DateTime? ReservationTime { get; set; }

        public string? ReservedBy { get; set; }
    }
}