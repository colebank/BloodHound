using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Threading;
using System.Web;
using System.Globalization;

//using Google.Apis.Authentication;
//using Google.Apis.Authentication.OAuth2;
//using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
//using Google.Apis.Books.v1;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

//using DotNetOpenAuth.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


public class GoogleLoader
{
    TableHelper m_TableHelper;
    BlobHelper m_LogBlob;

    public GoogleLoader(TableHelper tc, BlobHelper bh)
    {
        m_TableHelper = tc;
        m_LogBlob = bh;
        m_TableHelper.CreateTable("ISBN");

        m_LogBlob.CreateContainer("log");
        CloudBlobContainer container = m_LogBlob.BlobClient.GetContainerReference("log");

        CloudBlob blob = container.GetBlobReference("AudibleLoader.Log");
        BlobStream bs = blob.OpenWrite();
        TextWriter tw = new StreamWriter(bs);
        tw.WriteLine("test");
        tw.Flush();
        //content = new UTF8Encoding().GetString(data);
        bs.Close();
        //BlobStream OpenWrite ()

    }


    public void Run()
    {

        //try{          

        string url = "https://www.googleapis.com/books/v1/volumes?projection=full&q=flowers+inauthor:keyes&key=" + "AIzaSyDSU_HCunq22GVxgSzSjutxmGWxpnMs24Y";
        WebRequest request = HttpWebRequest.Create(url);
        WebResponse response = request.GetResponse();

        Stream dataStream = response.GetResponseStream();

        var jsonResult = string.Empty;

        using (var reader = new StreamReader(dataStream, Encoding.UTF8))
        {
            jsonResult = reader.ReadToEnd();
            reader.Close();
        }

        //        var tasks = JsonConvert.DeserializeObject<Task[]>(jsonResult);
        var tasks = JsonConvert.DeserializeObject<GoogleBookRec>(jsonResult);

        int a = 0;

        /*        foreach (var task in tasks)
                {
                    Console.WriteLine(
                        "{0} - {1}: {2} starting {3:yyyy-MM-dd HH:mm}", 
                        task.Project.Name, 
                        task.Description,
                        TimeSpan.FromSeconds(task.Duration),
                        task.Start);
                }
         */

    }

    public class GoogleBookRec
    {
        //        [JsonProperty(PropertyName = "kind")]
        public string kind { get; set; }

        //        [JsonProperty(PropertyName = "totalItems")]
        public string totalItems { get; set; }

        //        [JsonProperty(PropertyName = "items")]
        public List<OneBookRec> items { get; set; }
    }

    public class OneBookRec
    {
        public string kind { get; set; }
        public string id { get; set; }
        public string etag { get; set; }
        public string selflink { get; set; }
        public VolumeInfoRec volumeInfo;
        public SaleInfoRec saleInfo;
        public AccessInfoRec accessInfo;
        public SearchInfoRec searchInfo { get; set; }
    }

    public class SearchInfoRec
    {
        public string textSnippet { get; set; }
    }

    public class DownloadRec
    {
        public bool isAvailable { get; set; }
        public string acsTokenLink { get; set; }
    }


    public class SaleInfoRec
    {
        public string country { get; set; }
        public string saleability { get; set; }
        public bool isEbook { get; set; }
        public PriceRec listPrice { get; set; }
        public PriceRec retailPrice { get; set; }
        public string buyLink { get; set; }
        public DateTime onSaleDate { get; set; }

    }

    public class AccessInfoRec
    {
        public string country { get; set; }
        public string viewability { get; set; }
        public bool embeddable { get; set; }
        public bool publicDomain { get; set; }
        public string textToSpeechPermission { get; set; }
        public DownloadRec epub;
        public DownloadRec pdf;
        public string webReaderLink { get; set; }
        public string accessViewStatus { get; set; }
    }


    public class DownloadAccessRec
    {
        public string kind { get; set; }
        public string volumeId { get; set; }
        public bool restricted { get; set; }
        public bool deviceAllowed { get; set; }
        public bool justAcquired { get; set; }
        public int maxDownloadDevices { get; set; }
        public int downloadsAcquired { get; set; }
        public string nonce { get; set; }
        public string source { get; set; }
        public string reasonCode { get; set; }
        public string message { get; set; }
        public string signature { get; set; }
    }


    public class PriceRec
    {
        public decimal amount { get; set; }
        public string currencyCode { get; set; }
    }

    public class VolumeInfoRec
    {
        public string title { get; set; }
        public List<string> authors { get; set; }
        public string publisher { get; set; }
        public string publishedDate { get; set; }
        public string description { get; set; }
        public List<IndustIdsoRec> industryIdentifiers { get; set; }
        public int pageCount { get; set; }
        public string dimensions { get; set; }
        public string printType { get; set; }
        public List<string> categories { get; set; }
        public double averageRating { get; set; }
        public int ratingsCount { get; set; }
        public string contentVersion { get; set; }
        public ImageLinksRec imageLinks { get; set; }
        public string language { get; set; }
        public string mainCategory { get; set; }
        public string previewLink { get; set; }
        public UserInfoRec userInfo { get; set; }
        public string canonicalVolumeLink { get; set; }

    }


    public class UserInfoRec
    {
        public string review { get; set; }
        public string readingPosition { get; set; }
        public bool isPurchased { get; set; }
        public DateTime updated { get; set; }
        public bool isPreordered { get; set; }

    }

    public class DimensionsRec
    {
        public decimal height { get; set; }
        public decimal width { get; set; }
        public decimal thickness { get; set; }
    }

    public class ImageLinksRec
    {
        public string smallThumbnail { get; set; } //(width of ~80 pixels
        public string small { get; set; } //(width of ~300 pixels)
        public string medium { get; set; } //(width of ~575 pixels)
        public string large { get; set; }  //(width of ~800 pixels
        public string thumbnail { get; set; } //(width of ~128 pixels)
        public string extraLarge { get; set; } //(width of ~1280 pixels)
    }


    public class IndustIdsoRec
    {
        public string type { get; set; }
        public string identifier { get; set; }
    }
}
