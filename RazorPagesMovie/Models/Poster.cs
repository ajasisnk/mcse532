using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesMovie.Models 
{
    public class Poster : TableEntity
    {
        public int MovieID => int.Parse(this.PartitionKey);
        public string FileName => this.RowKey;

        public string URL { get; set; }
        public long FileSize { get; set; }

        // default constructor
        public Poster(){}

        // constructor for the new object with the movie id and filename

        public Poster(int movieid, string filename)
        {
            // those two are automatically saved as columns in the table
            // need to look at this as part of the design of the application
            this.PartitionKey = movieid.ToString();
            this.RowKey = filename;
        }
    }
}
