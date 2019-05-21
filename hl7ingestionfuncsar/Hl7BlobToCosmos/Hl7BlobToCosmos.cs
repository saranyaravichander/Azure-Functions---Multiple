using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Documents.Client;

namespace Hl7BlobToCosmos
{
    public static class Hl7BlobToCosmos
    {
        [FunctionName("Hl7BlobToCosmos")]
        public static async Task Run([BlobTrigger("hl7data/{name}", Connection = "hl7landingblobconn")]Stream myBlob, string name,
            [CosmosDB(
                databaseName:"hl7jsondb",
                collectionName :"hl7messages",
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log)
        {
            string requestBody = await new StreamReader(myBlob).ReadToEndAsync();

            JObject jobj = HL7ToXmlConverter.ConvertToJObject(requestBody);

            var inserted = await client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri("hl7jsondb", "hl7messages"), jobj);

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }

        private static string determinerhm(JObject obj)
        {
            string rhm = "932";
            string msgtype = getFirstField(obj["hl7message"]["MSH"]["MSH.9"]);
            msgtype = msgtype.ToLower();
            if (msgtype.Equals("adt"))
            {
                string instance = getFirstField(obj["hl7message"]["MSH"]["MSH.3"]);
                string source = getFirstField(obj["hl7message"]["MSH"]["MSH.4"]);
                if (instance.Equals("HQ") && source.Equals("C")) rhm = "204";
                else if (instance.Equals("HQ") && source.Equals("U")) rhm = "205";
            }
            else if (msgtype.Equals("oru"))
            {
                string pv139 = getFirstField(obj["hl7message"]["PV1"]["PV1.39"]);
                var rhm205 = "COE, COM, COC, CON, COS, COU";
                var rhm204 = "J, CH, M";
                if (rhm205.IndexOf(pv139) > -1) rhm = "205";
                if (rhm204.IndexOf(pv139) > -1) rhm = "204";
            }
            else if (msgtype.Equals("orm"))
            {
                string instance = getFirstField(obj["hl7message"]["MSH"]["MSH.3"]);
                string source = getFirstField(obj["hl7message"]["MSH"]["MSH.4"]);
                if (instance.Equals("HNAM") && source.Equals("AA")) rhm = "204";
                else if (instance.Equals("HNAM") && source.Equals("CO")) rhm = "205";
            }
            return rhm;

        }
        private static string getFirstField(JToken o)
        {
            if (o == null) return "";
            if (o.Type == JTokenType.String) return (string)o;
            if (o.Type == JTokenType.Object) return (string)o.First;
            return "";
        }
    }
}
