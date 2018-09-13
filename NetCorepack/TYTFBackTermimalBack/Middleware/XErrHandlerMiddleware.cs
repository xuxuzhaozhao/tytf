﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TYTFBackTermimalBack.Models;

namespace TYTFBackTermimalBack.Middleware
{
    public class XErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public XErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            //if (exception is MyNotFoundException) code = HttpStatusCode.NotFound;
            //else if (exception is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (exception is MyException) code = HttpStatusCode.BadRequest;

            var r = new XResult();
            r.code = 10000;
            r.message = exception.Message;
            var result = JsonConvert.SerializeObject(r);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            return context.Response.WriteAsync(result);
        }
    }
}
