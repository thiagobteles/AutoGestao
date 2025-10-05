namespace AutoGestao.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                   request.Query.ContainsKey("ajax") ||
                   request.ContentType?.Contains("application/json") == true;
        }
    }
}
