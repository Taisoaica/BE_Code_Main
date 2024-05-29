﻿using System.Text.Json;

namespace WebAPI.Models
{
    public class ExceptionDetailResponse
    {
        public int ErrorCode { get; set; }
        public string? Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
