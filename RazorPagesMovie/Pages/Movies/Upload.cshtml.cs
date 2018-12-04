using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.WindowsAzure.Storage.Blob;

namespace RazorPagesMovie.Pages.Movies
{
    public class UploadModel : PageModel
    {

        // movie object for the data to be populated
        public Models.Movie _movie { get; set; }

        // get database contents, when page is returned we can populate with the right movie
        public RazorPagesMovie.Models.MovieContext _context;
        public CloudBlobClient _client;

        // bind backend properties with the input form
        [BindProperty]
        public int MovieID { get; set; }

        // hold the data of the file being uploaded
        [BindProperty]
        public Microsoft.AspNetCore.Http.IFormFile FileUpload { get; set; }

        public UploadModel(RazorPagesMovie.Models.MovieContext context, CloudBlobClient blobClient)
        {
            _context = context;
            _client = blobClient;
        }        
    
        // gets item id from the ui
        public void OnGet(int _id)
        {
            _movie = _context.Movie.Find(_id);
        }

        // specify result to return 
        // use Task every time we run anything in async
        public async Task<IActionResult> OnPostAsync()
        {
            var container = _client.GetContainerReference("ajmovie");
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlockBlobReference(FileUpload.FileName);
            await blob.UploadFromStreamAsync(FileUpload.OpenReadStream());

            return Redirect("index");
        }
    }
}