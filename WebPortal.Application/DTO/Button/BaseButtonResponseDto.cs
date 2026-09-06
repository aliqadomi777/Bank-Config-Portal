using System;

namespace WebPortal.Application.DTO.Button
{
    public class BaseButtonResponseDto
    {
        public int ButtonId { get; set; }

        public string ButtonNameEN { get; set; }

        public string ButtonNameAR { get; set; }

        public int ButtonType { get; set; }

        public string TypeName { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }
    }
}