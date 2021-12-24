﻿using System;
using Newtonsoft.Json;
using Authing.ApiClient.Types;

namespace Authing.ApiClient.Domain.Model.Management.Resources
{

    public class UpdateResourceParam
    {
        [JsonProperty("type")]
        public ResourceType Type { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
        [JsonProperty("actions")]
        public ResourceAction[] Actions { get; set; }
        [JsonProperty("namespace")]
        public string NameSpace { get; set; }
    }
}
