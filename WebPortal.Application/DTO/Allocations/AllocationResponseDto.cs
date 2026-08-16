namespace WebPortal.Application.DTO.Allocations
{
    public class AllocationResponseDto
    {
        public int AllocationId { get; set; }
        public int CounterId { get; set; }
        public int ServiceId { get; set; }

        public string ServiceNameEN { get; set; }
        public string ServiceNameAR { get; set; }
    }
}
