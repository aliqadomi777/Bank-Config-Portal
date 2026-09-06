using System;
using System.Collections.Generic;
using WebPortal.Application.DTO.Button;

namespace WebPortal.Application.DTO.Screen
{
    public class ScreenResponseDto
    {
        public int ScreenId { get; set; }
        public string ScreenName { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public IList<BaseButtonResponseDto> Buttons { get; set; } = new List<BaseButtonResponseDto>();
        public bool IsActive { get; set; }
    }
}
