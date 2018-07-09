using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace XRMPlugins
{
    public class AccountCreatePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext) serviceProvider.GetService(typeof(IPluginExecutionContext));
            if(context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity account = (Entity)context.InputParameters["Target"];

                if (account.LogicalName == "account")
                {

                    Entity followupTask = new Entity("task");
                    followupTask["subject"] = "Send e-mail to the new customer.";
                    followupTask["description"] = "Follow up with the customer.";
                    followupTask["scheduledstart"] = DateTime.Now.AddDays(7);
                    followupTask["scheduledend"] = DateTime.Now.AddDays(7);
                    followupTask["category"] = context.PrimaryEntityName;

                    if (context.OutputParameters.Contains("id"))
                    {

                        Guid regardingObjectId = new Guid(context.OutputParameters["id"].ToString());
                        string regardingObjectType = "account";
                        followupTask["regardingobjectid"] = new EntityReference(regardingObjectType, regardingObjectId);
                    }

                    IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = factory.CreateOrganizationService(context.UserId);
                    service.Create(followupTask);
                }
            }
        }

    }
}
