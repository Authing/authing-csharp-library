﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Authing.ApiClient.Domain.Model.Management.WhiteList
{
    public class AddWhitelistResponse
    {

        [JsonProperty("addWhitelist")]
        public IEnumerable<WhiteList> Result { get; set; }
    }
}
