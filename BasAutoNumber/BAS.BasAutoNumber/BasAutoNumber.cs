using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Xml;

namespace BAS.CRM.BasAutoNumber
{
    /// <summary>
    /// Plug-in is responsible to handle the auto numbering of a new record
    /// A pre-event operation step should be registered for each Entity where autonumber will be set
    /// </summary>
    public class BasAutoNumber : IPlugin
    {
        private readonly XmlDocument _pluginUnsecureConfig = new XmlDocument();
        public BasAutoNumber(string sUnsecureConfig, string sSecureConfig)
        {
            if (!String.IsNullOrEmpty(sUnsecureConfig))
            {
                _pluginUnsecureConfig.LoadXml(sUnsecureConfig);
            }
        }
        public void Execute(IServiceProvider serviceProvider)
        {
            var Plugin = new BasPlugin(serviceProvider);            
            SetAutoNumber(Plugin);
        }

        /// <summary>
        /// Function sets auto number with newly created unique ID to a specific field
        /// </summary>
        private void SetAutoNumber(BasPlugin Plugin)
        {
            if (!(Plugin.Context.InputParameters["Target"] is Entity)) { return; }
            var entity = (Entity)Plugin.Context.InputParameters["Target"];
            var sLogicalName = Plugin.Context.PrimaryEntityName;
            var sFieldName = string.Empty;
            var sNewId = string.Empty;

            LockAutoNumberConfig(Plugin);
            GetNewId(Plugin, sLogicalName, out sNewId, out sFieldName);

            if (String.IsNullOrEmpty(sNewId) || String.IsNullOrEmpty(sFieldName)) { return; }
            entity[sFieldName] = sNewId;

        }

        /// <summary>
        /// Retrieves Auto ID configuration Entity from CRM
        /// </summary>
        /// <param name="sEntityName"></param>
        /// <param name="iService"></param>
        /// <returns></returns>
        private Entity GetLockAutoIDConfig(BasPlugin basPlugin, string sEntityName)
        {
            var ecBasAutoNumber = basPlugin.Service.RetrieveMultiple(
                new FetchExpression(string.Format(FetchXML.RetrieveAutoNumbre(sEntityName), sEntityName)));

            return ecBasAutoNumber.Entities.FirstOrDefault();
        }

        private void GetNewId(BasPlugin Plugin, string sEntityName, out string sNewId, out string sFieldName)
        {
            var _lockingObject = new object();
            lock (_lockingObject)
            {
                sNewId = string.Empty;
                sFieldName = string.Empty;
                var wNextId = 0;

                var eAutoNumber = GetLockAutoIDConfig(Plugin, sEntityName);
                if (eAutoNumber == null) { return; }

                sFieldName = eAutoNumber.GetAttributeValue<string>("_fieldname");
                wNextId = eAutoNumber.GetAttributeValue<int>("_nextid");

                var wIncrement = eAutoNumber.GetAttributeValue<int>("_incrementvalue");
                var wFirstId = eAutoNumber.GetAttributeValue<int>("_firstid");
                var sPrefix = eAutoNumber.GetAttributeValue<string>("_prefix");
                var sSuffix = eAutoNumber.GetAttributeValue<string>("_suffix");
                var wLength = eAutoNumber.GetAttributeValue<int>("_length");
                var wLastId = eAutoNumber.GetAttributeValue<int>("_lastid");


                if ((wNextId > wLastId || wNextId < wFirstId))
                {
                    throw new InvalidPluginExecutionException("Next ID is out of range");
                }

                sNewId = WrapNewId(wNextId, sPrefix, sSuffix, wLength);
                wNextId += wIncrement;

                eAutoNumber["_nextid"] = wNextId;
                Plugin.Service.Update(eAutoNumber);

            }
        }

        /// <summary>
        /// Formats new ID into a specific format: Prefix + New nuique ID + Suffix
        /// </summary>
        /// <param name="wNewId"></param>
        /// <param name="sPrefix"></param>
        /// <param name="sSuffix"></param>
        /// <param name="wLength"></param>
        /// <returns></returns>
        private static string WrapNewId(int wNewId, string sPrefix, string sSuffix, int wLength)
        {
            string sNewIdPadded = wNewId.ToString().PadLeft(wLength, '0');
            string sNewIdResult = sPrefix + sNewIdPadded + sSuffix;

            return sNewIdResult;
        }

        /// <summary>
        /// Forces a SQL lock on the record to ensure the uniqueness of the next ID to be used on the sequential numbering
        /// </summary>
        /// <param name="Plugin"></param>
        private void LockAutoNumberConfig(BasPlugin Plugin)
        {
            var node = _pluginUnsecureConfig.SelectSingleNode("LockGuid");
            Guid gAutoNumberId = new Guid();
            if (node == null) { return; }
            bool bIsGuidValid = Guid.TryParse(node.InnerText, out gAutoNumberId);
            if (!bIsGuidValid) { return; }

            Entity eAutoNumber = new Entity("_autonumber", gAutoNumberId);
            Plugin.Service.Update(eAutoNumber);

        }

        //TO-DO
        // Automatically create plug-in step when autonumber record is created in CRM


    }
}
