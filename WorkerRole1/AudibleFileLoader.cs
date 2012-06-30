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


        public class AudibleRecord : TableServiceEntity
        {
            public AudibleRecord(string lastName, string firstName)
            {
                this.PartitionKey = lastName;
                this.RowKey = firstName;
            }

            public AudibleRecord() { }

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

    public class AudibleFileLoader
    {

        TableHelper m_TableHelper;
        BlobHelper m_LogBlob;

        public AudibleFileLoader(TableHelper tc,BlobHelper bh) 
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

            for(int i=0;i<100;i++)
            {
                string Data = sr.ReadLine();
                string [] records = Data.Split(new Char [] {'\t' });
                AudibleRecord ar=new AudibleRecord();

     /*         foreach (string s in records) 
                {

                if (s.Trim() != "")
                    Console.WriteLine(s);
                }
    */
         
                ar.last_updated=records[0];
                ar.name=records[1];
                ar.category=records[2];
                ar.keywords=records[3];
                ar.short_description=records[4];
                ar.long_description=records[5];
                ar.sku=records[6];
                ar.asin=records[7];
                ar.isbn=records[8];
                ar.our_price=records[9];
                ar.retail_price=records[10];
                ar.buy_url=records[11];
                ar.thumb_nail_url=records[12];
                ar.large_image_url=records[13];
                ar.average_customer_rating=records[14];
                ar.author=records[15];
                ar.publisher=records[16];
                ar.audio_length=records[17];
                ar.sample_url=records[18];
                ar.release_date=records[19];
                ar.narrator=records[20];
                ar.faithfulness=records[21];
                ar.number_of_credits=records[22];


                if (ar.sku.Substring(0, 2)!= "BK")
                    continue;

                string sss = ar.ToString();


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

//}


