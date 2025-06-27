using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Razor;

namespace day1.Middlewares
{
    public class WorkingHoursMiddleware
    {
        private readonly RequestDelegate _next;

        public WorkingHoursMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var now = DateTime.Now;
            if (now.Hour < 9 || now.Hour >= 23)
            {
                context.Response.StatusCode = 403;

                // Render a Razor View instead of plain text
                var viewHtml = await RenderViewAsync(context, "AccessDenied");
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(viewHtml);
                return;
            }

            await _next(context);
        }

        private async Task<string> RenderViewAsync(HttpContext context, string viewName)
        {
            var serviceProvider = context.RequestServices;
            var razorViewEngine = serviceProvider.GetRequiredService<IRazorViewEngine>();
            var tempDataProvider = serviceProvider.GetRequiredService<ITempDataProvider>();

            // Create a fake ActionContext for rendering
            var actionContext = new ActionContext(
                context,
                new RouteData(),
                new ActionDescriptor()
            );

            var viewResult = razorViewEngine.FindView(actionContext, viewName, isMainPage: false);
            if (!viewResult.Success)
            {
                return $"Error: View '{viewName}' not found.";
            }

            using var sw = new StringWriter();
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewData,
                new TempDataDictionary(context, tempDataProvider),
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }
}
