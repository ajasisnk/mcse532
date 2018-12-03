using Microsoft.WindowsAzure.Storage;
using System;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=vcdemostorage;AccountKey=TguKNdWUMGa4iYYDi3FbbYCTxkjOIAtPiA6Q5r/qn06/k1VVO/ytL7BAMyUJFwHOf5dnxYhuZceGIQ3QJ+R/zg==;EndpointSuffix=core.windows.net";
            CloudStorageAccount cloudStorageAccount;
            cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

            // storage account client
            var client = cloudStorageAccount.CreateCloudBlobClient();

            // create container ref 
            var container = client.GetContainerReference("ajhelloworld");

            // create container if does not exist
            container.CreateIfNotExistsAsync().Wait();

            // get block blob reference
            var blob = container.GetBlockBlobReference("hello.txt");

            // write async to blob
            var output = blob.OpenWriteAsync().Result;

            // creat new writer and write output
            var sw = new StreamWriter(output);
            sw.WriteLine("Hello world!");

            // clean up resources
            sw.Close();
            output.Close();

        }
    }
}
