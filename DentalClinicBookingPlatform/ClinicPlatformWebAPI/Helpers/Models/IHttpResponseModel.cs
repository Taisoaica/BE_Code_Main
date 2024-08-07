﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClinicPlatformWebAPI.Helpers.Models
{
    public interface IHttpResponseModel<T> where T : class
    {

        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }
        public T? Content { get; set; }
        public string ToString();
    }
}
