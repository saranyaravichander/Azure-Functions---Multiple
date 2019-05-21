using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace hl7ingestionallinone
{
    public static class EventHubToBlob
    {
        [FunctionName("EventHubToBlob")]
        public static async Task Run(
            [EventHubTrigger("hl7msgseh", Connection = "hl7eventhub", ConsumerGroup ="hl7reader")] EventData eventData, 
            Binder binder,
            ILogger log)
        {
            var exceptions = new List<Exception>();

                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

            var attributes = new Attribute[]
        {
                new BlobAttribute("hl7data/" + DateTime.UtcNow.ToString("yyyy/MM/dd") + "/" + Guid.NewGuid().ToString("N") + ".txt"),
                new StorageAccountAttribute("hl7landingconnstring"),
        };

            using (var writer = await binder.BindAsync<TextWriter>(attributes))
            {
                writer.Write(messageBody);
            }

            // Replace these two lines with your processing logic.
            log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    await Task.Yield();
        }
    }
}
