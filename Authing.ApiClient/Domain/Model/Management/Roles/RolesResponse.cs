﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Authing.ApiClient.Domain.Model.Management.Roles
{
    public class RolesResponse
    {

        [JsonProperty("roles")]
        public PaginatedRoles Result { get; set; }
    }
}
