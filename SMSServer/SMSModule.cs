using Nancy;
using Nancy.ModelBinding;

namespace SMSServer
{
    public class SMSModule : NancyModule
    {
        public SMSModule()
        {
            Post["/sms/echo"] = parameters =>
            {
                var sm = this.Bind<ShortMessage>();
                return sm.Message;
            };
            Post["/sms/gift", true] = async (ctx, token) =>
            {
                var sm = this.Bind<ShortMessage>();
                return await Santa.Reply(sm);
            };
        }
    }
}
