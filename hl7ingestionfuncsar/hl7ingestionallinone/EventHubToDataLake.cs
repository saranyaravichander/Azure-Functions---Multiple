using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DataLake;
using Microsoft.Extensions.Logging;

namespace hl7ingestionallinone
{
    public static class EventHubToDataLake
    {
            [FunctionName("EventHubToDataLake")]
        public static void Run([EventHubTrigger("hl7msgseh", Connection = "hl7eventhub", ConsumerGroup = "hl7readerdatalake")] EventData eventData,
          [DataLakeStore(AccountFQDN = "%fqdn%", ApplicationId = "%applicationid%", ClientSecret = "%clientsecret%", TenantID = "%tentantid%")]out DataLakeStoreOutput dataLakeStoreOutput,
            ILogger log)
        {
            string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

            dataLakeStoreOutput = new DataLakeStoreOutput()
            {
                FileName = "hl7data/" + DateTime.UtcNow.ToString("yyyy/MM/dd") + "/" + Guid.NewGuid().ToString("N") + ".txt",
                FileStream = new MemoryStream(eventData.Body.Array)
            };

            // Replace these two lines with your processing logic.
            log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
        }
    }
}
