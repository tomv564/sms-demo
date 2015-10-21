using Nancy;
using Nancy.Extensions;
using Nancy.ModelBinding;

namespace SMSServer
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = parameters => "Hello World";
            Post["/"] = parameters => "";
            Post["/echo"] = parameters => Request.Body.AsString();
            Post["/gift", true] = async (ctx, token) =>
            {
                var sm = this.Bind<ShortMessage>();
                return await Santa.Reply(sm);
            };
        }
    }
}
