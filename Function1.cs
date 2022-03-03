using Azure.Storage.Blobs;
using Iwp.Services.RabbitMQConsumerForBlob.Dtos;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Iwp.Services.RabbitMQConsumerForBlob
{
    public static class Function1
    {
        private const string Id = "/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Storage/storageAccounts/{2}";
        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};";
        private static IAzure azure { get; set; }

        [FunctionName("Function1")]
        public static void Run([RabbitMQTrigger("blob-antivirus-scan", ConnectionStringSetting = "Rabit_Mq_Host")]string myQueueItem, ILogger log)
        {
            var credentials = Microsoft.Azure.Management.ResourceManager.Fluent.SdkContext.AzureCredentialsFactory.FromServicePrincipal("a843f57d-6e2e-41eb-a471-1b208331c5be", "A9FJozRp_0RS9rXZ.zX4dumv78hoFh9sFx", "0b2d0fd9-c9fc-49a3-ab38-77e6f7d3b008", Microsoft.Azure.Management.ResourceManager.Fluent.AzureEnvironment.AzureGlobalCloud);
            azure = Microsoft.Azure.Management.Fluent.Azure.Authenticate(credentials)
                    .WithDefaultSubscription();

            RabbitMqMessageDto rabbitMqMessageDto = new RabbitMqMessageDto();
            rabbitMqMessageDto = System.Text.Json.JsonSerializer.Deserialize<RabbitMqMessageDto>(myQueueItem);
            
            BlobServiceClient blobServiceClientSrc = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=commonusertempspace;AccountKey=uNV6sDfGlv2cgntJFOD0tzwTUY6XqVZvvB9uDY6FW3bRP8SZLG5cBYl8wot1oVgEqZmPz2xBvxqXNoXx5EpHdA==;EndpointSuffix=core.windows.net");
            string[] arrStorage = rabbitMqMessageDto.FileDto.ClientStorage.Replace("_", "").Split(' ');
            string storageNameManaged = (arrStorage[0] + arrStorage[arrStorage.Length - 1]).ToLower();
            string managedContainerName = storageNameManaged + rabbitMqMessageDto.FileDto.CaseName.Replace("_", "");
            string connectionstringDest = GetStorageKey(storageNameManaged);

            BlobServiceClient blobServiceClientDest = new BlobServiceClient(connectionstringDest);
            // Create the container and return a container client object
            BlobContainerClient containerClientSrc = blobServiceClientSrc.GetBlobContainerClient("temp-data");
            BlobContainerClient containerClientDest = blobServiceClientDest.GetBlobContainerClient(managedContainerName);

            // Get a reference to a blob
            BlobClient blobClientSrc = containerClientSrc.GetBlobClient(rabbitMqMessageDto.FileDto.FileName);
            BlobClient blobClientDest = containerClientDest.GetBlobClient(rabbitMqMessageDto.FileDto.FileName);

            //Checking file extension and mime type
            //if (rabbitMqMessageDto.FileDto.FileName.Contains(".docx"))
            //{
            //    var memoryStream = new MemoryStream();
            //    blobClientSrc.DownloadTo(memoryStream);
            //    FileStream fileStream = File.Create("Files/dataDoc.docx");
            //    memoryStream.Seek(0, SeekOrigin.Begin);
            //    memoryStream.CopyTo(fileStream);
            //    memoryStream.Dispose();
            //    fileStream.Dispose();
            //    fileStream.Close();

            //    var client = new RestClient("http://localhost:10001/api/v1/drd_processing/convert_to_pdf");
            //    client.Timeout = -1;
            //    var request = new RestRequest(Method.PUT);
            //    request.AddFile("file", "Files/dataDoc.docx");
            //    IRestResponse response = client.Execute(request);
            //    Console.WriteLine(response.Content);

            //    FileStream fileStream2 = File.Create("Files/dataDoc.pdf");
            //    var ms = new MemoryStream(response.RawBytes);
            //    ms.Seek(0, SeekOrigin.Begin);
            //    ms.CopyTo(fileStream2);
            //    ms.Dispose();
            //    fileStream2.Dispose();
            //    fileStream2.Close();
            //    //fileStream2
            //}
            //else
            //{
            //    var memoryStream = new MemoryStream();
            //    blobClientSrc.DownloadTo(memoryStream);
            //    FileStream fileStream = File.Create("Files/dataDoc.pdf");
            //    memoryStream.Seek(0, SeekOrigin.Begin);
            //    memoryStream.CopyTo(fileStream);
            //    memoryStream.Dispose();
            //    fileStream.Dispose();
            //    fileStream.Close();
            //}
            //AddDocumentCoordinates($"https://localhost:9200/{rabbitMqMessageDto.FileDto.CaseName}/_bulk", blobClientSrc);
            //AddCaseIndex($"https://localhost:9200/{rabbitMqMessageDto.FileDto.CaseName}");

            var status = blobClientDest.StartCopyFromUri(blobClientSrc.Uri);
            status.WaitForCompletionAsync();
            while (status.HasCompleted == false)
            {
                Task.Delay(300);
            }
            TaskRequestDto taskRequestDto = new TaskRequestDto()
            {
                Content = "Uploading",
                CreatedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                ParentDocumentId = rabbitMqMessageDto.FileDto.DocumentID,
                TempResponse = "",
                TaskType = 6,
                Status = 1,
                RequestID = rabbitMqMessageDto.FileDto.DocumentID,
                CaseId = rabbitMqMessageDto.FileDto.CaseID
            };
            //NpgsqlConnection connection = new NpgsqlConnection("User ID=dbadmin_staging@noetic-db-staging;Password=fC$U&44JdkQj;Host=noetic-db-staging.postgres.database.azure.com;Port=5432;Database=iwp_db;SSL Mode=Require;Trust Server Certificate=true");
            NpgsqlConnection connection = new NpgsqlConnection("User ID=dbadmindemo@noeticdb-demo;Password=N03t!cdb@dm!n;Host=noeticdb-demo.postgres.database.azure.com;Port=5432;Database=iwp_db;SSL Mode=Require;Trust Server Certificate=true");

            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = $"INSERT INTO public.tasks VALUES ('{Guid.NewGuid()}',6,null,1,'Uploading','{DateTime.Now}','4fce0759-8342-457b-a294-1250424f39aa',null,null,'{taskRequestDto.ParentDocumentId}','{taskRequestDto.ParentDocumentId}','{DateTime.Now.AddDays(30)}','')";
            command.ExecuteNonQuery();
            command.Dispose();
            connection.Close();

        }

        static string GetStorageKey(string storageAccountName)
        {
            string id = string.Format(Id, "790d297d-7d54-482f-9346-d1bc14fecf31", "DefaultResourceGroup-EUS", storageAccountName);
            var azureStorageKey = azure.StorageAccounts.GetById(id).GetKeys().FirstOrDefault().Value;
            return string.Format(connectionString, storageAccountName, azureStorageKey);
        }

        static void AddCaseIndex(string url)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            HttpClient client = new HttpClient();
            string inputData = getInputData();
            var content = new StringContent(inputData, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Basic ZWxhc3RpYzo2Wnplc3FzWldjVFQxRkI3NnpmWQ==");
            var result = client.PutAsync(url, content).Result;
            client.Dispose();            
        }

        static void AddDocumentCoordinates(string url, BlobClient blobClientSrc)
        {
            //memoryStream.ToArray
            var clientDRDProcessing = new RestClient("http://localhost:10001/api/v1/drd_processing/");
            clientDRDProcessing.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddFile("files", "Files/dataDoc.pdf");
            //request.AddUrlSegment("files", "https://commonusertempspace.blob.core.windows.net/temp-data/ARCANDAnswers.%2520TO%25201ST%2520CASE%2520SPEC.%2520PREM.%2520INTERROG%28Flint%29.pdf");
            IRestResponse response = clientDRDProcessing.Execute(request);
            Console.WriteLine(response.Content);
            var documentRequests = JsonConvert.DeserializeObject<List<DocumentRequestDTO>>(response.Content);
            foreach(DocumentRequestDTO documentRequest in documentRequests)
            {
                if (!string.IsNullOrEmpty(documentRequest.Content))
                {
                    HttpClient client = new HttpClient();
                    string inputData = @"{ ""create"" : { } }
{ ""doc"":" + JsonConvert.SerializeObject(documentRequest) + "}\n";
                    var content = new StringContent(inputData, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue("elastic", "6ZzesqsZWcTT1FB76zfY");
                    var result = client.PostAsync(url, content).Result;
                    client.Dispose();
                }
            }
           
        }
        
//        private static string getDocumentInput()
//        {
//            string strData= @"
//{ ""create"" : { } }
//{ ""doc"":{ ""Id"": ""ID00000001d"",    ""ContentType"": ""Interrogatory"",    ""Content"": ""  \nPage 1 of 22  \n16-L-195  \n  \nIN THE CIRCUIT COURT  \n  \n  \n\f\n"",    ""CaseId"": [      ""16-L-195      "",      [        290,        304      ]    ],    ""Plaintiffs"": """",    ""Defendants"": """",    ""Court"": """",    ""DueDate"": """",    ""ServiceDate"": """"  } }
//";
//            return strData;
//        }
        private static string getInputData()
        {
            return @"{
" + "\n" +
            @"""settings"": {
" + "\n" +
            @"""index"": {
" + "\n" +
            @"""routing"": {
" + "\n" +
            @"""allocation"": {
" + "\n" +
            @"""include"": {
" + "\n" +
            @"""_tier_preference"": ""data_content""
" + "\n" +
            @"}
" + "\n" +
            @"}
" + "\n" +
            @"},
" + "\n" +
            @"""number_of_shards"": ""3"",
" + "\n" +
            @"""number_of_replicas"": ""1""
" + "\n" +
            @"}
" + "\n" +
            @"},
" + "\n" +
            @"""mappings"": {
" + "\n" +
            @"""dynamic"": ""strict"",
" + "\n" +
            @"""_meta"": {
" + "\n" +
            @"""InactiveFields"": [],
" + "\n" +
            @"""SimilarityDataTypes"": [
" + "\n" +
            @"""Lsh"",
" + "\n" +
            @"""Lsa""
" + "\n" +
            @"]
" + "\n" +
            @"},
" + "\n" +
            @"""properties"": {
" + "\n" +
            @"""CaseNum"": {
" + "\n" +
            @"""type"": ""keyword""
" + "\n" +
            @"},
" + "\n" +
            @"""Content"": {
" + "\n" +
            @"""type"": ""text"",
" + "\n" +
            @"""term_vector"": ""with_positions_offsets""
" + "\n" +
            @"},
" + "\n" +
            @"""TempPlainContent"": {
" + "\n" +
            @"""type"": ""text""
" + "\n" +
            @"},
" + "\n" +
            @"""TempFormattedContent"": {
" + "\n" +
            @"""type"": ""text""
" + "\n" +
            @"},
" + "\n" +
            @"""Status"": {
" + "\n" +
            @"""type"": ""text""
" + "\n" +
            @"},
" + "\n" +
            @"""ContentItemNum"": {
" + "\n" +
            @"""type"": ""integer""
" + "\n" +
            @"},
" + "\n" +
            @"""ContentItemNumStr"": {
" + "\n" +
            @"""type"": ""text""
" + "\n" +
            @"},
" + "\n" +
            @"""ContentType"": {
" + "\n" +
            @"""type"": ""keyword""
" + "\n" +
            @"},
" + "\n" +
            @"""FileName"": {
" + "\n" +
            @"""type"": ""text""
" + "\n" +
            @"},
" + "\n" +
            @"""FilePath"": {
" + "\n" +
            @"""type"": ""keyword""
" + "\n" +
            @"},
" + "\n" +
            @"""FormattedContent"": { ""type"": ""text"" },
" + "\n" +
            @"""CourtName"": { ""type"": ""text"" },
" + "\n" +
            @"""ServiceDate"": { ""type"": ""date"" },
" + "\n" +
            @"""Plaintiffs"": { ""type"": ""text"" },
" + "\n" +
            @"""Defendants"": { ""type"": ""text"" },
" + "\n" +
            @"""Id"": {
" + "\n" +
            @"""type"": ""keyword""
" + "\n" +
            @"},
" + "\n" +
            @"""ParentId"": {
" + "\n" +
            @"""type"": ""keyword""
" + "\n" +
            @"},
" + "\n" +
            @"""SelfGroupingLsa"": {
" + "\n" +
            @"""enabled"": false,
" + "\n" +
            @"""properties"": {
" + "\n" +
            @"""Id"": {
" + "\n" +
            @"""type"": ""keyword""
" + "\n" +
            @"},
" + "\n" +
            @"""Similarity"": {
" + "\n" +
            @"""type"": ""double""
" + "\n" +
            @"}
" + "\n" +
            @"}
" + "\n" +
            @"}
" + "\n" +
            @"}
" + "\n" +
            @"}
" + "\n" +
            @"}
";
        }
    }
}
