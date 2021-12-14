﻿using System.Collections.Generic;
using Authing.ApiClient.Types;
using Newtonsoft.Json;

namespace Authing.ApiClient.Domain.Model.Management.Acl
{
    public class PolicyStatement
    {
        #region members
        [JsonProperty("resource")]
        public string Resource { get; set; }

        [JsonProperty("actions")]
        public IEnumerable<string> Actions { get; set; }

        [JsonProperty("effect")]
        public PolicyEffect? Effect { get; set; }

        [JsonProperty("condition")]
        public IEnumerable<PolicyStatementCondition> Condition { get; set; }

        [JsonProperty("resourceType")]
        public ResourceType? ResourceType { get; set; }
        #endregion
    }
}