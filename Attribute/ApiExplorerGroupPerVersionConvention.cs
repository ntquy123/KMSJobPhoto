using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace erpsolution.api.Attribute
{
    public class ApiExplorerGroupPerVersionConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller.ControllerType.Namespace; // e.g. "Controllers.HR"
            var apiVersion = controllerNamespace.Split('.').Last().ToLower();
            if (apiVersion != null && apiVersion != string.Empty && apiVersion != "controllers")
                controller.ApiExplorer.GroupName = apiVersion;
            else controller.ApiExplorer.GroupName = "auth";
        }
    }
}
