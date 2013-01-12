// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR.Owin.Handlers
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class HubDispatcherHandler
    {
        private readonly AppFunc _next;
        private readonly string _path;
        private readonly HubConfiguration _config;
        private readonly IDependencyResolver _resolver;

        public HubDispatcherHandler(AppFunc next, string path, HubConfiguration configuration, IDependencyResolver resolver)
        {
            _next = next;
            _path = path;
            _config = configuration;
            _resolver = resolver;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            var path = environment.Get<string>(OwinConstants.RequestPath);
            if (path == null || !path.StartsWith(_path, StringComparison.OrdinalIgnoreCase))
            {
                return _next(environment);
            }

            var pathBase = environment.Get<string>(OwinConstants.RequestPathBase);
            var dispatcher = new HubDispatcher(pathBase + _path, _config);

            var handler = new CallHandler(_resolver, dispatcher);
            return handler.Invoke(environment);
        }
    }
}
