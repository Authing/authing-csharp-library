﻿using System.Threading.Tasks;
using Authing.ApiClient.Auth;
using Authing.ApiClient.Mgmt;
using Authing.ApiClient.Test.Base;

namespace Authing.ApiClient.Netstandard20.Test
{
    public class BaseTest : TestBase
    {
        protected AuthenticationClient authenticationClient { get; set; }
        protected ManagementClient managementClient { get; set; }

        public BaseTest()
        {
            authenticationClient = new AuthenticationClient(
                opt =>
                {
                    opt.AppId = AppId;
                    opt.Host = Host;
                }
            );
        }

        public async Task<ManagementClient> GetManagementClient()
        {
            managementClient = await ManagementClient.InitManagementClient(init: opt =>
            {
                opt.UserPoolId = UserPoolId;
                opt.Secret = Secret;
                opt.Host = Host;
            });
            return managementClient;
        }
    }
}