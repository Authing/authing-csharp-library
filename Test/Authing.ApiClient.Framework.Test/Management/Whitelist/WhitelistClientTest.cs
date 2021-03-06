using Authing.ApiClient.Types;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Authing.ApiClient.Framework.Test.Management.Whitelist
{
    public class WhitelistClienTtest : BaseTest
    {
        [Fact]
        public async void enable_whitelist_Func()
        {
            var client = managementClient;
            var res = await client.Whitelist.Enable(WhitelistType.EMAIL | WhitelistType.PHONE | WhitelistType.USERNAME);
            Assert.True(res.Result.Whitelist.EmailEnabled & res.Result.Whitelist.PhoneEnabled & res.Result.Whitelist.UsernameEnabled);
        }

        [Fact]
        public async void disable_whitelist_Func()
        {
            var client = managementClient;
            var res = await client.Whitelist.Disable(WhitelistType.EMAIL | WhitelistType.PHONE | WhitelistType.USERNAME);
            Assert.False(res.Result.Whitelist.EmailEnabled & res.Result.Whitelist.PhoneEnabled & res.Result.Whitelist.UsernameEnabled);
        }

        [Fact]
        public async void add_whitelist_Func()
        {
            List<string> phones = new List<string>();
            phones.Add("188888888888");

            var client = managementClient;
            var result = await client.Whitelist.Add(WhitelistType.PHONE, phones);
            foreach (var phone in phones)
            {
                Assert.NotNull(result.FirstOrDefault(c => c.Value == phone));
            }
            await client.Whitelist.Remove(WhitelistType.PHONE, phones);
        }

        [Fact]
        public async void remove_whitelist_Func()
        {
            List<string> phones = new List<string>();
            phones.Add("188888888888");

            var client = managementClient;
            await client.Whitelist.Add(WhitelistType.PHONE, phones);
            var result = await client.Whitelist.Remove(WhitelistType.PHONE, phones);
             result = await client.Whitelist.List(WhitelistType.PHONE);
            foreach (var phone in phones)
            {
                Assert.Null(result.FirstOrDefault(c => c.Value == phone));
            }
        }

        [Fact]
        public async void get_whitelist_Func()
        {
            List<string> phones = new List<string>();
            phones.Add("188888888888");

            var client = managementClient;
            await client.Whitelist.Add(WhitelistType.PHONE, phones);
            var result = await client.Whitelist.List(WhitelistType.PHONE);
            Assert.NotEmpty(result);
            await client.Whitelist.Remove(WhitelistType.PHONE, phones);
        }
    }
}