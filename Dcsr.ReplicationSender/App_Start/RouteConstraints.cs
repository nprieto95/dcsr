using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Dcsr.ReplicationSender
{
    public class QueryStringParameterPresentConstraint : IHttpRouteConstraint
    {

        protected readonly string queryStringParameterName;

        public QueryStringParameterPresentConstraint(string parameterName)
        {
            queryStringParameterName = parameterName;
        }

        public virtual bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            return request.RequestUri.ParseQueryString().Keys.OfType<string>().Contains(queryStringParameterName);
        }

    }

    public class QueryStringParameterValueConstraint : QueryStringParameterPresentConstraint
    {
        private readonly string queryStringParameterValue;
        public QueryStringParameterValueConstraint(string parameterName, string parameterValue) : base(parameterName)
        {
            queryStringParameterValue = parameterValue;
        }
        public override bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            return base.Match(request, route, parameterName, values, routeDirection) &&
                request.RequestUri.ParseQueryString()[queryStringParameterName] == queryStringParameterValue;
        }
    }

}