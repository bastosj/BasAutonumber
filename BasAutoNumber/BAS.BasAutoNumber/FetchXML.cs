using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAS.CRM.BasAutoNumber
{
    public static class FetchXML
    {
        public static string RetrieveAutoNumbre(string sEntityName)
        {
          var sFetchRetrieveAutoNumber = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='1'>
  <entity name='bas_basautonumber'>
    <attribute name='bas_basautonumberid' />
    <attribute name='bas_suffix' />
    <attribute name='bas_prefix' />
    <attribute name='bas_nextid' />
    <attribute name='bas_length' />
    <attribute name='bas_lastid' />
    <attribute name='bas_incrementvalue' />
    <attribute name='bas_firstid' />
    <attribute name='bas_fieldname' />
    <attribute name='bas_entityname' />
    <filter type='and'>
      <condition attribute='bas_entityname' operator='eq' value='{0}' />
    </filter>
  </entity>
</fetch>";

            return sFetchRetrieveAutoNumber;
        }
    }
}
