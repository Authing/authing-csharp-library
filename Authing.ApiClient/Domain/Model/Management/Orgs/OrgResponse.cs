﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Authing.ApiClient.Domain.Model.Management.Orgs
{
    public class OrgResponse
    {

        [JsonProperty("org")]
        public Org Result { get; set; }
    }
}
