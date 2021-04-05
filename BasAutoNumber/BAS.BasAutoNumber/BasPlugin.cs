using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Linq;

namespace BAS.CRM.BasAutoNumber
{
    public class BasPlugin
    {
        public IOrganizationService Service { get; set; }
        public IPluginExecutionContext Context { get; set; }
        private ITracingService Tracing { get; set; }
        private IServiceProvider ServiceProvider { get; set; }


        public BasPlugin(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext _context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService _service = factory.CreateOrganizationService(_context.UserId);
            ITracingService _tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            this.Service = _service;
            this.Context = _context;
            this.Tracing = _tracing;
        }

        public static BasPlugin GetCRM(IServiceProvider serviceProvider)
        {
            BasPlugin basPlugin = new BasPlugin(serviceProvider);
            IPluginExecutionContext _context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService _service = factory.CreateOrganizationService(_context.UserId);
            ITracingService _tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            basPlugin.Service = _service;
            basPlugin.Context = _context;
            basPlugin.Tracing = _tracing;

            return basPlugin;
        }

        public static string RetrieveOptionSetText(string entityName, string optionSetName, int optionSetValue, IOrganizationService service)
        {
            OptionSetValue OptionsetValue = new OptionSetValue(optionSetValue);

            try
            {
                // Create the request
                RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityName,
                    LogicalName = optionSetName
                };

                // Execute the request
                RetrieveAttributeResponse attributeResponse = null;

                attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);


                // OptionSet metada
                PicklistAttributeMetadata metadata = attributeResponse.AttributeMetadata as PicklistAttributeMetadata;

                // selected label
                string selectedLabel = metadata.OptionSet.Options
                    .Where(x => x.Value == OptionsetValue.Value)
                    .Select(x => x.Label.UserLocalizedLabel.Label)
                    .FirstOrDefault();

                return selectedLabel;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }
        public void Trace(string message)
        {
            if (Tracing == null)
            {
                GetTracingService();
            }

            if (String.IsNullOrWhiteSpace(message) || String.IsNullOrEmpty(message)) { return; }

            Tracing.Trace(message);
        }

        private void GetTracingService()
        {
            //Extract the tracing service
            Tracing = (ITracingService)ServiceProvider.GetService(typeof(ITracingService));

            if (Tracing == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve the Tracing service.");
            }
        }
    }
}
