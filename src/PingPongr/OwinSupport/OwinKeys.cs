namespace PingPongr.OwinSupport
{
    internal static class OwinKeys
    {
        public const string RequestMethod = "owin.RequestMethod";
        public const string RequestPath = "owin.RequestPath";
        public const string RequestPathBase = "owin.RequestPathBase";
        public const string RequestProtocol = "owin.RequestProtocol";
        public const string RequestQueryString = "owin.RequestQueryString";
        public const string RequestScheme = "owin.RequestScheme";
        public const string RequestHeaders = "owin.RequestHeaders";
        public const string RequestBody = "owin.RequestBody";

        public const string ResponseBody = "owin.ResponseBody";
        public const string ResponseHeaders = "owin.ResponseHeaders";
        public const string StatusCode = "owin.ResponseStatusCode";
        public const string ReasonPhrase = "owin.ResponseReasonPhrase";
        public const string ResponseProtocol = "owin.ResponseProtocol";

        public const string Version = "owin.Version";
        public const string CallCancelled = "owin.CallCancelled";
    }
}
