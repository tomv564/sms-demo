using System;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using Nancy.TinyIoc;

namespace SMSServer
{
    public class BootStrapper : DefaultNancyBootstrapper
    {
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = @"dsiuy2323" }; }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            pipelines.AfterRequest +=
                (ctx) => Console.WriteLine("{0}: {1} {2} {3}", DateTime.Now, ctx.Request.Method, ctx.Request.Path,
                    ctx.Response.StatusCode);

            pipelines.OnError += (ctx, ex) =>
            {
                Console.WriteLine("{0}: {1} {2} {3}", DateTime.Now, ctx.Request.Method, ctx.Request.Path, HttpStatusCode.InternalServerError);
                Console.WriteLine(ex.Message);
            
                return null;
            };
        }
    }
}
