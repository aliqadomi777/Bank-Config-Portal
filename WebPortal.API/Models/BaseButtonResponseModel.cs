using System;

namespace WebPortal.API.Models
{
    public class BaseButtonResponseModel
    {
        public int ButtonId { get; set; }

        public string ButtonNameEN { get; set; }

        public string ButtonNameAR { get; set; }

        public string TypeName { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }
        public int ButtonType { get; set; }

    }
}