﻿using Authing.ApiClient.Domain.Client.Impl.AuthenticationClient;
using Authing.ApiClient.Domain.Client.Impl.ManagementBaseClient;
using Authing.ApiClient.Test.Base;

namespace Authing.ApiClient.Framework.Test
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
            managementClient = new ManagementClient(init: opt =>
            {
                opt.UserPoolId = UserPoolId;
                opt.Secret = Secret;
                opt.Host = Host;
            });
        }
    }
}