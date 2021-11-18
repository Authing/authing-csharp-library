﻿using Authing.ApiClient.Domain.Client;
using Authing.ApiClient.Test.Base;
using Xunit;
using ManagementClient = Authing.ApiClient.Domain.Client.Impl.ManagementBaseClient.ManagementClient;

namespace Authing.ApiClient.Netstandard20_up.Test.Users
{
    public class get_user_detail : BaseTest
    {
        [Fact]
        public async void should_get_user_detail_correct()
        {
            var client = managementClient;
            var user = await client.Users.Detail(TestUserId, true);
            Assert.NotNull(user);
            Assert.Equal("16666667777", user.Phone);
        }
    }
}