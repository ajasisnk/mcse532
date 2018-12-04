using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Movies
{
    public class PosterModel : PageModel
    {

        public Models.Movie _movie { get; set; }

        // get database contents, when page is returned we can populate with the right movie
        public RazorPagesMovie.Models.MovieContext _context;
        private CloudTable _cloudTable;
        private CloudBlobClient _BlobClient;
        public IList<Poster> _posters = new List<Poster>();

        public PosterModel(CloudTable CloudTable, CloudBlobClient BlobClient)
        {
            _cloudTable = CloudTable;
            _BlobClient = BlobClient;
        }

        public async Task<string> GetSas(string BlobUri)
        {
            var blob = await _BlobClient.GetBlobReferenceFromServerAsync(new Uri(BlobUri));
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy();
            policy.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24);
            policy.Permissions = SharedAccessBlobPermissions.Read;

            string sasToken = blob.GetSharedAccessSignature(policy);
            return BlobUri + sasToken;
        }



        public void OnGet(int id)
        {
            // create new query object
            // generate condition
            // filter on partitionkey == id

            TableQuery<Poster> query = new TableQuery<Poster>()
                .Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()
                ));

            TableContinuationToken continuationToken = null;

            // execute the query
            do
            {

                var segmentResult = _cloudTable.ExecuteQuerySegmentedAsync(query, continuationToken);

                // iterate through all results
                foreach (var item in segmentResult.Result)
                {
                    // populate model list 
                    _posters.Add(item);

                }

                continuationToken = segmentResult.Result.ContinuationToken;

                // this will terminate when segment is null
            } while (continuationToken != null);

        }
    }
}