
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
using HtmlAgilityPack;


using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

using AmazonProductAdvtApi;


public class xAudibleRecord : TableServiceEntity
{
    public xAudibleRecord(string lastName, string firstName)
    {
        this.PartitionKey = lastName;
        this.RowKey = firstName;
    }

    public xAudibleRecord() { }

    public string last_updated { get; set; }
    public string name { get; set; }
    public string category { get; set; }
    public string keywords { get; set; }
    public string short_description { get; set; }
    public string long_description { get; set; }
    public string sku { get; set; }
    public string asin { get; set; }
    public string isbn { get; set; }
    public string our_price { get; set; }
    public string retail_price { get; set; }
    public string buy_url { get; set; }
    public string thumb_nail_url { get; set; }
    public string large_image_url { get; set; }
    public string average_customer_rating { get; set; }
    public string author { get; set; }
    public string publisher { get; set; }
    public string audio_length { get; set; }
    public string sample_url { get; set; }
    public string release_date { get; set; }
    public string narrator { get; set; }
    public string faithfulness { get; set; }
    public string number_of_credits { get; set; }

}


//namespace WorkerRole1
//{

public class GoodReadsExtractor
{

    TableHelper m_TableHelper;
    BlobHelper m_LogBlob;

    public GoodReadsExtractor(TableHelper tc, BlobHelper bh)
    {
        m_TableHelper = tc;
        m_LogBlob = bh;
        m_TableHelper.CreateTable("ISBN");

        m_LogBlob.CreateContainer("log");
        //m_LogBlob.SetContainerACL("log", "private");
        CloudBlobContainer container = m_LogBlob.BlobClient.GetContainerReference("log");
        /*
                    CloudBlobContainer blobContainer = blobClient.GetContainerReference("azurecontainer");
                    CloudBlob blob = Container.GetBlobReference(blobname + ".res");
                    BlobStream bs = blob.OpenWrite();
                    TextWriter tw = new StreamWriter(bs);
                    string append = resultsLine + ", ";
                    tw.WriteLine(append);
        */


        CloudBlob blob = container.GetBlobReference("AudibleLoader.Log");
        BlobStream bs = blob.OpenWrite();
        TextWriter tw = new StreamWriter(bs);
        tw.WriteLine("test");
        tw.Flush();
        //content = new UTF8Encoding().GetString(data);
        bs.Close();
        //BlobStream OpenWrite ()

    }




    public int LoadFromFile(string fn)
    {
        StreamReader sr = new StreamReader(fn);
        string Header = sr.ReadLine();
        string Expected = "last_updated\tname\tcategory\tkeywords\tshort_description\tlong_description\tsku\tasin\tisbn\tour_price\tretail_price\tbuy_url\tthumb_nail_url\tlarge_image_url\taverage_customer_rating\tauthor\tpublisher\taudio_length\tsample_url\trelease_date\tnarrator\tfaithfulness\tnumber_of_credits";
        if (Header != Expected)
            return -1;

        HtmlWeb hw = new HtmlWeb();
        //            hw.UserAgent = " Mozilla/5.0 (compatible; bingbot/2.0; +http://www.bing.com/bingbot.htm)";
        hw.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.56 Safari/536.5";

        for (int i = 0; i < 100; i++)
        {
            string Data = sr.ReadLine();
            string[] records = Data.Split(new Char[] { '\t' });
            xAudibleRecord ar = new xAudibleRecord();

            /*         foreach (string s in records) 
                       {

                       if (s.Trim() != "")
                           Console.WriteLine(s);
                       }
           */

            ar.last_updated = records[0];
            ar.name = records[1];
            ar.category = records[2];
            ar.keywords = records[3];
            ar.short_description = records[4];
            ar.long_description = records[5];
            ar.sku = records[6];
            ar.asin = records[7];
            ar.isbn = records[8];
            ar.our_price = records[9];
            ar.retail_price = records[10];
            ar.buy_url = records[11];
            ar.thumb_nail_url = records[12];
            ar.large_image_url = records[13];
            ar.average_customer_rating = records[14];
            ar.author = records[15];
            ar.publisher = records[16];
            ar.audio_length = records[17];
            ar.sample_url = records[18];
            ar.release_date = records[19];
            ar.narrator = records[20];
            ar.faithfulness = records[21];
            ar.number_of_credits = records[22];


            if (ar.sku.Substring(0, 2) != "BK")
                continue;

            string url = "http://www.amazon.com/s/url=search-alias%3Dstripbooks&field-keywords=" + ar.sku;

            HtmlDocument doc = hw.Load(url);


            string xp = "//h1[@id=\"noResultsTitle\"]";
            HtmlNode Node = doc.DocumentNode.SelectSingleNode(xp);//.InnerText;

            if (Node != null)
                continue;

            xp = "//div[@id=\"result_0\"]";

            HtmlNode ProdNode = doc.DocumentNode.SelectSingleNode(xp);
            if (ProdNode == null)
                continue;
            string Asin = ProdNode.GetAttributeValue("name", "");

            //xp = "//a[@class=\"title\"]";
            xp = ".//a";    //the . means from current node, otherwise searches entire tree

            Node = ProdNode.SelectSingleNode(xp);
            if (Node == null)
                continue;

            string FamillyName = "";
            string[] LinkList;
            string MainLink;

            MainLink = Node.GetAttributeValue("href", "");
            LinkList = MainLink.Split(new Char[] { '/' });
            string fam = LinkList[3];
            //myparent/mychild[text() = 'foo']

            xp = "//div[@class=\"otherEditions\"]/a";
            Node = doc.DocumentNode.SelectSingleNode(xp);
            //doc.Save("/site.xml");

            if (Node != null)
            {
                MainLink = Node.GetAttributeValue("href", "");
                string t = Node.InnerText;
                LinkList = MainLink.Split(new Char[] { '/' });
                FamillyName = LinkList[3];
                if (FamillyName == "gp")
                    FamillyName = "";
            }

            if (FamillyName == "")
                FamillyName = fam;

            Console.WriteLine(Asin);

            ar.PartitionKey = FamillyName;
            ar.RowKey = Asin;
        }

        sr.Close();
        /*
                    StreamWriter fs = new StreamWriter("c:/site.xml");
                    fs.Write(responseFromServer);

        */


        return 0;
    }






}


/*


See a series
Info on a series 
URL: http://www.goodreads.com/series/show.xml?id=ID    (sample url) 
HTTP method: GET 

See all series by an author
List of all series by an author 
URL: /series/list?format=xml&id=AUTHOR_ID    (sample url) 
HTTP method: GET 

See all series a work is in
List of all series a work is in 
URL: http://www.goodreads.com/series/work.xml?id=WORK_ID    (sample url) 
HTTP method: GET 



Find books by title, author, or ISBN
Get an xml file with the most popular books for the given query. This will search all books in the title/author/ISBN fields and show matches, sorted by popularity on Goodreads. There will be cases where a result is shown on the Goodreads site, but not through the API. This happens when the result is an Amazon-only edition and we have to honor Amazon's terms of service. 
URL: http://www.goodreads.com/search.xml    (sample url) 
HTTP method: GET 
Parameters: 
q: The query text to match against book title, author, and ISBN fields. Supports boolean operators and phrase searching.
page: Which page to return (default 1, optional)
key: Developer key (required).
search[field]: Field to search, one of 'title', 'author', or 'genre' (default is 'all')






Find an author by name
Get an xml file with the Goodreads url for the given author name. 
URL: http://www.goodreads.com/api/author_url.xml    (sample url) 
HTTP method: GET 
Parameters: 
id: Author name
key: Developer key (required).



 Paginate an author's books
Get an xml file with a paginated list of an authors books. 
URL: http://www.goodreads.com/author/list.xml    (sample url) 
HTTP method: GET 
Parameters: 
key: Developer key (required).
id: Goodreads Author id (required)
page: 1-N (default 1)

Get info about an author by id
Get an xml file with info about an author. 
URL: http://www.goodreads.com/author/show.xml    (sample url) 
HTTP method: GET 
Parameters: 
key: Developer key (required).
id: Goodreads Author id.

Get the Goodreads book ID given an ISBN
Get the Goodreads book ID given an ISBN. Response contains the ID without any markup. 
URL: http://www.goodreads.com/book/isbn_to_id    (sample url) 
HTTP method: GET 
Parameters: 
key: Developer key (required).
isbn: The ISBN of the book to lookup.

Get review statistics given a list of ISBNs
Get review statistics for books given a list of ISBNs. ISBNs can be specified as an array (e.g. isbns[]=0441172717&isbns[]=0141439602) or a single, comma-separated string (e.g. isbns=0441172717,0141439602). You can mix ISBN10s and ISBN13s, but you'll receive a 422 error if you don't specify any, and you'll receive a 404 if none are found. 
URL: http://www.goodreads.com/book/review_counts.json    (sample url) 
HTTP method: GET 
Parameters: 
key: Developer key (required).
isbns: Array of ISBNs or a comma seperated string of ISBNs (1000 ISBNs per request max.)
format: json
callback: function to wrap JSON response

 
 


See all editions by work
List of all the available editions of a particular work. This API requires extra permission please contact us 
URL: /work/editions.xml?id=WORK_ID    (sample url) 
HTTP method: GET 
Parameters: 
key: Developer key (required).


Get the listopia lists for a given book
XML version of list/book. This API requires extra permission please contact us 
URL: http://www.goodreads.com/list/book.xml?id=BOOK_ID    (sample url) 
HTTP method: GET 
Parameters: 
key: Developer key (required).

Get the books from a listopia list. This API requires extra permission please contact us
XML version of list/show 
URL: http://www.goodreads.com/list/show.xml?id=LIST_ID    (sample url) 
HTTP method: GET 
Parameters: 
key: Developer key (required).

Get the listopia lists for a given tag. This API requires extra permission please contact us
XML version of list/tag 
URL: http://www.goodreads.com/list/tag.xml?id=tag_name    (sample url) 
HTTP method: GET 
Parameters: 
key: Developer key (required).








Paginate an author's books
Get an xml file with a paginated list of an authors books. 
URL: http://www.goodreads.com/author/list.xml    (sample url) 
HTTP method: GET 
Parameters: 
key: Developer key (required).
id: Goodreads Author id (required)
page: 1-N (default 1)

Get info about an author by id
Get an xml file with info about an author. 
URL: http://www.goodreads.com/author/show.xml    (sample url) 
HTTP method: GET 
Parameters: 
key: Developer key (required).
id: Goodreads Author id.

Get the Goodreads book ID given an ISBN
Get the Goodreads book ID given an ISBN. Response contains the ID without any markup. 
URL: http://www.goodreads.com/book/isbn_to_id    (sample url) 
HTTP method: GET 
Parameters: 
key: Developer key (required).
isbn: The ISBN of the book to lookup.

Get review statistics given a list of ISBNs
Get review statistics for books given a list of ISBNs. ISBNs can be specified as an array (e.g. isbns[]=0441172717&isbns[]=0141439602) or a single, comma-separated string (e.g. isbns=0441172717,0141439602). You can mix ISBN10s and ISBN13s, but you'll receive a 422 error if you don't specify any, and you'll receive a 404 if none are found. 
URL: http://www.goodreads.com/book/review_counts.json    (sample url) 
HTTP method: GET 
Parameters: 
key: Developer key (required).
isbns: Array of ISBNs or a comma seperated string of ISBNs (1000 ISBNs per request max.)
format: json
callback: function to wrap JSON response

Get the reviews for a book given a Goodreads book id
Get an xml file that contains embed code for the iframe reviews widget, shelves, and meta-data about the book. The reviews widget show an excerpt (first 300 characters) of the most popular reviews of a book for a given internal Goodreads book_id. The reviews are from all known editions of the book. 

It's important to note that the Goodreads API gives you full access to Goodreads-owned data. It does not give you full access to all book meta-data, such as images, descriptions, etc. Goodreads gets much of this data from 3rd party sources such as Amazon and Barnes & Noble, and we do not have a license to distribute this data via our API. You may easily sign up for a developer key at Amazon if you need book meta-data that Goodreads is not able to provide. 
URL: http://www.goodreads.com/book/show?format=FORMAT    (sample url) 
HTTP method: GET 
Parameters: 
format: xml or json
key: Developer key (required).
id: A Goodreads internal book_id
text_only: Only show reviews that have text (default false)
rating: Show only reviews with a particular rating (optional)




*/



















