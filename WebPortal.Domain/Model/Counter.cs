using System;

namespace WebPortal.Domain.Model
{
    public class CounterModel
    {
        public int CounterId { get; set; }
        public string CounterNameEN { get; set; }
        public string CounterNameAR { get; set; }
        public bool CounterStatus { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public int BranchId { get; set; }
        public int TypeID { get; set; }
        public string TypeName { get; set; }

    }
}
