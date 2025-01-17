﻿using HSMCommon.Constants;
using HSMSensorDataObjects;
using Microsoft.AspNetCore.Http;
using NLog;
using System.Threading.Tasks;

namespace HSMServer.Middleware
{
    internal sealed class LoggingExceptionMiddleware
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly RequestDelegate _next;


        public LoggingExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch
            {
                if (context.Connection.LocalPort == ConfigurationConstants.SensorsPort)
                {
                    var request = context.Request;
                    request.Headers.TryGetValue(nameof(BaseRequest.Key), out var key);

                    _logger.Error($"Path {request.Path}, access key = {key}, remote id, port: {context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}, content size = {request.ContentLength}");
                }

                throw;
            }
        }
    }
}
