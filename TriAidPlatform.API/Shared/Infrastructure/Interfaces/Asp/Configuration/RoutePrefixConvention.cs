using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace TriAidPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration;

/// <summary>
///     Prepends a global URL prefix to every attribute-routed controller that does not
///     already declare it, so REST clients can rely on a single base path.
/// </summary>
/// <param name="prefix">The global route prefix, without leading or trailing slashes.</param>
public class RoutePrefixConvention(string prefix) : IApplicationModelConvention
{
    /// <summary>Applies the prefix to every controller route.</summary>
    /// <param name="application">The application model being conventioned.</param>
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
            Apply(controller);
    }

    /// <summary>Applies the prefix to a single controller when its route lacks it.</summary>
    /// <param name="controller">The controller model being conventioned.</param>
    private void Apply(ControllerModel controller)
    {
        foreach (var selector in controller.Selectors)
        {
            var template = selector.AttributeRouteModel?.Template;
            if (template != null && template.TrimStart('/').StartsWith(prefix.Trim('/'), StringComparison.OrdinalIgnoreCase))
                return;
        }
        foreach (var selector in controller.Selectors)
            selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                new AttributeRouteModel(new RouteAttribute(prefix.Trim('/'))),
                selector.AttributeRouteModel);
    }
}
