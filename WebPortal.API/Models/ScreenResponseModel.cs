using System;
using System.Collections.Generic;

namespace WebPortal.API.Models
{
    public class ScreenResponseModel
    {
        public int ScreenId { get; set; }

        public string ScreenName { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }

        public List<BaseButtonResponseModel> Buttons { get; set; }
    }
}