﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Authing.ApiClient.Domain.Model;
using Authing.ApiClient.Domain.Model.Management.Acl;
using Authing.ApiClient.Extensions;
using Authing.ApiClient.Infrastructure.GraphQL;
using Authing.ApiClient.Types;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace Authing.ApiClient.Domain.Client.Impl.ManagementBaseClient
{
    public partial class ManagementClient
    {
        /// <summary>
        /// 权限控制模块
        /// </summary>
        public AclManagementClient acl { get; set; }

        /// <summary>
        /// 权限控制类
        /// </summary>
        public class AclManagementClient
        {
            private readonly ManagementClient client;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="client"></param>
            public AclManagementClient(ManagementClient client)
            {
                this.client = client;
            }

            /// <summary>
            /// 创建权限分组
            /// </summary>
            /// <param name="code">权限分组唯一标识符</param>
            /// <param name="name">权限分组名</param>
            /// <param name="description">可选，权限分组描述</param>
            /// <returns></returns>
            public async Task<NameSpace> CreateNamespace(string code, string name, string description)
            {
                var res = await client.Post<NameSpace>($"api/v2/resource-namespace/{client.UserPoolId}",
                    new Dictionary<string, string>()
                    {
                        { nameof(code), code },
                        { nameof(name), name },
                        { nameof(description), description },
                    });
                return res.Data;
            }

            /// <summary>
            /// 获取权限分组列表
            /// </summary>
            /// <param name="page">页码，默认为 1</param>
            /// <param name="limit">每页个数，默认为 10</param>
            /// <returns></returns>
            public async Task<Namespaces> ListNamespaces(int page = 1, int limit = 10)
            {
                var res = await client.Get<Namespaces>(
                    $"api/v2/resource-namespace/{client.UserPoolId}?limit={limit}&page={page}"
                    , new GraphQLRequest());
                return res.Data;
            }

            /// <summary>
            /// 更新权限分组
            /// </summary>
            /// <param name="code">权限分组 Code</param>
            /// <param name="updateNamespaceParam">需要更新的数据
            /// updates.code <String> 可选，权限分组唯一标识符。
            /// updates.name<String> 可选，权限分组名称。
            /// updates.description<String> 可选，权限分组描述。
            /// </param>
            /// <returns></returns>
            public async Task<NameSpace> UpdateNamespace(string code, UpdateNamespaceParam updateNamespaceParam)
            {
                if (updateNamespaceParam == null) throw new ArgumentNullException(nameof(updateNamespaceParam));

                var res = await client.Post<NameSpace>($"api/v2/resource-namespace/{client.UserPoolId}/code/{code}",
                    new Dictionary<string, string>()
                    {
                        { nameof(updateNamespaceParam.Code), updateNamespaceParam.Code },
                        { nameof(updateNamespaceParam.Name), updateNamespaceParam.Name },
                        { nameof(updateNamespaceParam.Description), updateNamespaceParam.Description }
                    });

                return res.Data;
            }

            /// <summary>
            /// 删除权限分组
            /// </summary>
            /// <param name="code">权限分组 ID</param>
            /// <returns></returns>
            public async Task<int> DeleteNamespace(int code)
            {
                var res = await client.Delete<object>($"api/v2/resource-namespace/{client.UserPoolId}/{code}",
                    new GraphQLRequest());
                return res.Code;
            }

            /// <summary>
            /// 获取资源列表
            /// </summary>
            /// <param name="resourceQueryFilter">过滤参数类</param>
            /// <returns></returns>
            public async Task<ListResourcesRes> ListResources(ResourceQueryFilter resourceQueryFilter)
            {
                if (resourceQueryFilter == null) throw new ArgumentNullException(nameof(resourceQueryFilter));
                string endPoint = $"api/v2/resources?";
                endPoint += string.IsNullOrWhiteSpace(resourceQueryFilter.NameSpaceCode)
                    ? ""
                    : $"&namespace={resourceQueryFilter.NameSpaceCode}";
                endPoint += $"&type={resourceQueryFilter.Type}";
                endPoint += $"&limit={(resourceQueryFilter.FetchAll ? "-1" : $"{resourceQueryFilter.Limit}")}";
                endPoint += $"&page={resourceQueryFilter.Page}";

                var res = await client.Get<ListResourcesRes>(endPoint, new GraphQLRequest());
                return res.Data;
            }

            /// <summary>
            /// 创建资源
            /// </summary>
            /// <param name="createResourceParam">创建资源参数类</param>
            /// <returns></returns>
            public async Task<Resources> CreateResource(ResourceParam createResourceParam)
            {
                if (createResourceParam == null) throw new ArgumentNullException(nameof(createResourceParam));

                if (createResourceParam.Code == null)
                {
                    throw new ArgumentException("请为资源设定一个资源标识符");
                }

                if (createResourceParam.Actions?.Count() < 1)
                {
                    throw new ArgumentException("请至少定义一个资源操作");
                }

                if (createResourceParam.NameSpace == null)
                {
                    throw new ArgumentException("请传入权限分组标识符");
                }


                var res = await client.Post<Resources>("api/v2/resources",
                    new Dictionary<string, string>()
                    {
                        {nameof(createResourceParam.Code).ToLower(),createResourceParam.Code},
                        {nameof(createResourceParam.Actions).ToLower(),createResourceParam.Actions.ConvertJson()},
                        {nameof(createResourceParam.ApiIdentifier).ToLower(),createResourceParam.ApiIdentifier ?? ""},
                        {nameof(createResourceParam.NameSpace).ToLower(),createResourceParam.NameSpace},
                        {nameof(createResourceParam.Type).ToLower(),createResourceParam.Type.ToString()},
                        {nameof(createResourceParam.Description).ToLower(),createResourceParam.Description},
                    });

                return res.Data;
            }

            /// <summary>
            /// 根据资源代码查询资源
            /// </summary>
            /// <param name="code">资源标识符</param>
            /// <param name="nameSpace">权限分组命名空间</param>
            /// <returns></returns>
            public async Task<Resources> FindResourceByCode(string code, string nameSpace = "")
            {
                string endPoint = $"api/v2/resources/by-code/{code}";
                endPoint += string.IsNullOrWhiteSpace(nameSpace) ? "" : $"?namespace={nameSpace}";
                var resut = await client.Get<Resources>(endPoint, new GraphQLRequest());
                return resut.Data;
            }

            /// <summary>
            /// 根据资源 ID 查询资源
            /// </summary>
            /// <param name="id">资源 ID</param>
            /// <returns></returns>
            public async Task<Resources> GetResourceById(string id)
            {
                string endPoint = $"api/v2/resources/detail?id={id}";
                var resut = await client.Get<Resources>(endPoint, new GraphQLRequest());
                return resut.Data;

            }

            /// <summary>
            /// 更新资源
            /// </summary>
            /// <param name="code">资源标识符</param>
            /// <param name="options">资源信息对象
            ///  options.Namespace <String> 资源所在的权限分组标识
            ///  options.Type 资源类型，可选值为 ResourceType.DATA、ResourceType.API、ResourceType.MENU、ResourceType.UI、ResourceType.BUTTON。
            ///  options.Actions<List<ResourceAction>> 资源操作对象数组。其中 name 为操作名称，填写一个动词，description 为操作描述，填写描述信息。
            ///  options.description<String> 资源描述信息
            ///  ResourceAction : Name<String> 操作名称
            ///  ResourceAction : Description<String> 描述信息
            /// </param>
            /// <returns></returns>
            public async Task<Resources> UpdateResource(string code, ResourceParam options)
            {
                if (options == null) throw new ArgumentNullException(nameof(options));
                if (string.IsNullOrWhiteSpace(options.NameSpace)) throw new ArgumentException($"Parameter {nameof(options.NameSpace)} is request!");
                string endPoint = $"api/v2/resources/{code}";
                var result = await client.Post<Resources>(endPoint,
                    new Dictionary<string, string>()
                    {
                        {nameof(options.Code).ToLower(),code},
                        {nameof(options.Actions).ToLower(),options.Actions.ConvertJson()},
                        {nameof(options.ApiIdentifier).ToLower(),options.ApiIdentifier ?? ""},
                        {nameof(options.NameSpace).ToLower(),options.NameSpace},
                        {nameof(options.Type).ToLower(),options.Type.ToString()},
                        {nameof(options.Description).ToLower(),options.Description},
                    });
                return result.Data;
            }

            /// <summary>
            /// 删除资源
            /// </summary>
            /// <param name="code">资源标识符</param>
            /// <param name="namespacecode">资源所在的权限分组标识</param>
            /// <returns></returns>
            public async Task<bool> DeleteResource(string code, string namespacecode)
            {
                string endPoint = $"api/v2/resources/{code}?namespace={namespacecode}";
                var result = await client.Delete<bool>(endPoint, new GraphQLRequest());
                return result.Code == 200;
            }

            /// <summary>
            /// 允许某个用户对某个资源进行某个操作
            /// </summary>
            /// <param name="userid">用户 ID</param>
            /// <param name="action"> 操作名称，推荐使用 <resourceType>:<actionName> 的格式，如 books:edit，books:list</param>
            /// <param name="resource">资源名称，必须为 <resourceType>:<resourceId> 格式或者为 _，如 _，books:123，books:*</param>
            /// <param name="nameSpace">权限分组命名空间</param>
            /// <returns></returns>
            public async Task<CommonMessage> Allow(string userid, string nameSpace, string resource, string action)
            {
                Regex temp = new Regex(@"^[0-9a-z]+:[0-9a-z]+$", RegexOptions.IgnoreCase);
                action.CheckParameter();
                resource.CheckParameter();
                var param = new AllowParam(resource, action)
                {
                    UserId = userid,
                    Resource = resource,
                    Namespace = nameSpace
                };
                var res = await client.Request<AllowResponse>(param.CreateRequest());
                return res.Data.Result;
            }

            /// <summary>
            /// 批量撤回资源
            /// </summary>
            /// <param name="options">
            /// options.namespace <String> 权限分组的 Code，详情请见使用权限分组管理权限资源。
            /// options.resource<String> 资源名称，必须为<resourceType>:<resourceId> 格式或者为 _，如 _，books:123，books:*。
            /// options.opts<List<RevokeResourceOpt>>
            /// options.opts.targetType. <PolicyAssignmentTargetType> 被授权主体类型
            /// options.opts.targetIdentifier. <String> 被授权主体唯一标识
            /// </param>
            /// <returns></returns>
            public async Task<CommonMessage> RevokeResource(RevokeResourceParams options)
            {
                options.Resource.CheckParameter();
                string endPoint = "api/v2/acl/revoke-resource";
                var result = await client.Post<CommonMessage>(endPoint,
                    new Dictionary<string, string>()
                    {
                        {nameof(options.NameSpace).ToLower(),options.NameSpace},
                        {nameof(options.Opts).ToLower(),options.Opts.ConvertJson()},
                        {nameof(options.Resource).ToLower(),options.Resource ?? ""},
                    });

                return result.Data;
            }

            /// <summary>
            /// 判断某个用户是否对某个资源有某个操作权限
            /// </summary>
            /// <param name="userId">用户ID</param>
            /// <param name="action">操作名称，推荐使用 <resourceType>:<actionName> 的格式，如 books:edit，books:list</param>
            /// <param name="resource">资源名称，必须为 <resourceType>:<resourceId> 格式或者为 _，如 _，books:123，books:*</param>
            /// <param name="namespacecode">资源组CODE</param>
            /// <returns></returns>
            public async Task<bool> IsAllowed(string userId, string action, string resource, string namespacecode = "")
            {
                action.CheckParameter();
                resource.CheckParameter();
                var result = await client.Request<IsActionAllowedResponse>(new IsActionAllowedParam(resource, action, userId, namespacecode).CreateRequest());
                return result.Data.Result;
            }

            public async Task<PaginatedAuthorizedResources> listAuthorizedResources(PolicyAssignmentTargetType targetType, string targetIdentifier, string namespacecode, ListAuthorizedResourcesOptions options)
            {
                var param = new ListAuthorizedResourcesParam(targetType, targetIdentifier, namespacecode,
                    options.ResourceType);
                var result = await client.Request<ListAuthorizedResourcesResponse>(param.CreateRequest());
                return result.Data.AuthorizedResources;
            }
        }
    }


}
