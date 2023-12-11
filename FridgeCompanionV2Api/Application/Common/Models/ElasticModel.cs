﻿using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class ElasticModelRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CompletionField Suggest { get; set; }
    }
}
