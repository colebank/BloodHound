
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

namespace Bloodhound
{

public class BTRecord : TableServiceEntity
{
    public BTRecord(string lastName, string firstName)
    {
        this.PartitionKey = lastName;
        this.RowKey = firstName;
    }

    public BTRecord() { }
    
    public string ISBN { get; set; }
    public string Title { get; set; }
    public string RespParty { get; set; }
    public string Series { get; set; }
    public string Publisher { get; set; }
    public string VendorCode { get; set; }
    public string ImprintCode { get; set; }
    public string PubDate { get; set; }
    public string Subject { get; set; }
    public string SubjectCode1 { get; set; }
    public string SubjectCode2 { get; set; }
    public string SubjectCode3 { get; set; }
    public string AdultJuv { get; set; }
    public string Mention { get; set; }
    public string BookJacket { get; set; }
    public string Binding { get; set; }
    public string PageCount { get; set; }
    public string TrimSize { get; set; }
    public string FirstPrinting { get; set; }
    public string USListPrice { get; set; }
    public string UKListPrice { get; set; }
    public string UnitSales { get; set; }
    public string ListSales { get; set; }
    public string NetSales { get; set; }
    public string AdBudget { get; set; }
    public string Returns { get; set; }
    public string Demand { get; set; }
    public string OnHand { get; set; }
    public string OnOrder { get; set; }
    public string OnBackorder { get; set; }
    public string Speedstock { get; set; }
    public string Status { get; set; }
    public string BTKey { get; set; }




}




public class BTFileLoader
{
    Helpers m_Helpers;
    public BTFileLoader(Helpers mh)
    {
        m_Helpers = mh;

        

        m_Helpers.m_Blob.CreateContainer("log");
        //m_LogBlob.SetContainerACL("log", "private");
        CloudBlobContainer container = m_Helpers.m_Blob.BlobClient.GetContainerReference("log");
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
        string Header = sr.ReadLine().Trim();
        //string Expected = "last_updated\tname\tcategory\tkeywords\tshort_description\tlong_description\tsku\tasin\tisbn\tour_price\tretail_price\tbuy_url\tthumb_nail_url\tlarge_image_url\taverage_customer_rating\tauthor\tpublisher\taudio_length\tsample_url\trelease_date\tnarrator\tfaithfulness\tnumber_of_credits";
        string Expected = "ISBN	Title\tResp Party\tSeries\tPublisher\tVendor Code\tImprint Code\tPub Date\tSubject\tSubject Code 1\tSubject Code 2\tSubject Code 3\tAdult/Juv\tMention\tBook Jacket\tBinding\tPage Count\tTrim Size\t1st Printing\tUS List Price\tUK List Price\tUnit Sales\tList Sales\tNet Sales\tAd Budget\tReturns\tDemand\tOn Hand\tOn Order\tOn Backorder\tSpeedstock\tStatus\tBT Key";

        if (Header != Expected)
            return -1;

        HtmlWeb hw = new HtmlWeb();
        //            hw.UserAgent = " Mozilla/5.0 (compatible; bingbot/2.0; +http://www.bing.com/bingbot.htm)";
        hw.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.56 Safari/536.5";

        for (int i = 0; i < 100; i++)
        {
            string Data = sr.ReadLine();
            string[] records = Data.Split(new Char[] { '\t' });
            BTRecord ar = new BTRecord();

            /*         foreach (string s in records) 
                       {

                       if (s.Trim() != "")
                           Console.WriteLine(s);
                       }
           */

            ar.ISBN = records[0];
            ar.Title = records[1];
            ar.RespParty = records[2];
            ar.Series = records[3];
            ar.Publisher = records[4];
            ar.VendorCode = records[5];
            ar.ImprintCode = records[6];
            ar.PubDate = records[7];
            ar.Subject = records[8];
            ar.SubjectCode1 = records[9];
            ar.SubjectCode2 = records[10];
            ar.SubjectCode3 = records[11];
            ar.AdultJuv = records[12];
            ar.Mention = records[13];
            ar.BookJacket = records[14];
            ar.Binding = records[15];
            ar.PageCount = records[16];
            ar.TrimSize = records[17];
            ar.FirstPrinting = records[18];
            ar.USListPrice = records[19];
            //ar.UKListPrice = records[20];
            ar.UnitSales = records[21];
            ar.ListSales = records[22];
            ar.NetSales = records[23];
            ar.AdBudget = records[24];
            //ar.Returns = records[25];
            ar.Demand = records[26];
            ar.OnHand = records[27];
            ar.OnOrder = records[28];
            //ar.OnBackorder = records[29];
            //ar.Speedstock = records[30];
            ar.Status = records[31];
            ar.BTKey = records[32];
    
            if (ar.ISBN.Substring(0, 3) != "978")
                continue;

            m_Helpers.m_Queue.PutMessage("goodreads-workid", new CloudQueueMessage(ar.ISBN));

/*
            serviceContext.UpdateObject(customer5);

            serviceContext.UpdateObject(specificEntity);
            serviceContext.AddObject("people", customer1);
            serviceContext.SaveChangesWithRetries();
            serviceContext.SaveChangesWithRetries(SaveChangesOptions.ReplaceOnUpdate);
            
            context.MergeOption = MergeOption.NoTracking;
            And when updating (or removing) an entity we call:

            context.AttachTo(entitySetName, entity, eTag);

            _LogEntryServiceContext.MergeOption = MergeOption.PreserveChanges;
            AppendOnly
            OverwriteChanges
            NoTracking


            AppendOnly: This is the default value where the DataServiceContext does not load the entity instance from the server if it is already present in its cache.
            OverwriteChanges: DataServiceContext always loads the entity instance from the server hence keeping it up-to-date and overwriting the previously tracked entity.
            PreserveChanges: When an entity instance exists in the DataServiceContext, it is not loaded from persistent storage. Any property changes made to objects in the DataServiceContext are preserved but the ETag is updated hence making it a good option when recovering from optimistic concurrency errors.
            NoTracking: Entity instances are not tracked by the DataServiceContext. To update an entity on a context that is not tracking will require the use of AttachTo to update the etag to use for the update and hence is not recommended.
                context.AttachTo("Blogs", blog, "etag to use");
                context.UpdateObject(blog);
                context.SaveChanges();

            _LogEntryServiceContext.AttachTo("LogEntry", itemToDelete, "*");
            _LogEntryServiceContext.DeleteObject(itemToDelete);
            _LogEntryServiceContext.SaveChanges();
            _LogEntryServiceContext.Detach(itemToDelete);


            var someCust = db.Customers
            .SingleOrDefault(c=>c.ID == 5); //unlikely(?) to be more than one, but technically COULD BE

            var bobbyCust = db.Customers
            .FirstOrDefault(c=>c.FirstName == "Bobby"); //clearly could be one or many, so use First?

            var latestCust = db.Customers
            .OrderByDescending(x=> x.CreatedOn)
            .FirstOrDefault();//Single or First, or does it matter?



            svc = new TestContext();
            var item = (from i in svc.TestTable
                        where i.PartitionKey == "item1pk"
                        && i.RowKey == "item1rk" select i).Single();
            svc.DeleteObject(item);
            svc.SaveChanges();

            However, this means that a delete takes two roundtrips.  In some cases, you may know the unique key (partition key plus row key) of an entity already, and you’d like to delete without first querying.  There is a way to do this.  You need to construct a dummy local entity with the right partition and row keys, and then AttachTo this object, specifying an ETag of “*” (to prevent a conflict when your delete encounters the ETag of the real entity).  The code looks like this:

            svc = new TestContext();
            item = new TestEntity("item2pk", "item2rk");
            svc.AttachTo("TestTable", item, "*");
            svc.DeleteObject(item);
            svc.SaveChanges();

*/


            string url = "http://www.amazon.com/s/url=search-alias%3Dstripbooks&field-keywords=" + ar.ISBN;

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

}


/*


ISBN	Title	Resp Party	Series	Publisher	Vendor Code	Imprint Code	Pub Date	Subject Code 1	Subject Code 2	Subject Code 3	Binding	1st Printing	Unit Sales	List Sales	Net Sales	Ad Budget	Returns	Demand	On Hand	On Order	On Backorder	Speedstock	BT Key


*/



















