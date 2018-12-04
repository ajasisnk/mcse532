using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesMovie.Models 
{
    public class Poster : TableEntity
    {
        public int MovieID
        {
            get { return int.Parse(PartitionKey); }
            set { this.PartitionKey = value.ToString(); }
        }

        public string FileName
        {
            get { return RowKey; }
            set { this.RowKey = value.ToString(); }
        }

        public string URL;
        public int FileSize;

        // default constructor
        public Poster(){}

        // constructor for the new object with the movie id and filename

        public Poster(int movieid, string filename)
        {
            this.PartitionKey = movieid.ToString();
            this.RowKey = filename;
        }
    }
}
