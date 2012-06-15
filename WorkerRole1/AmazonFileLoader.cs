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


using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

using AmazonProductAdvtApi;


//namespace WorkerRole1
//{

    public class AmazonFileLoader
    {
        static private SignedRequestHelper m_helper;

        public static void Init(SignedRequestHelper  h) { m_helper=h; }

        //       private const string ITEM_ID = "B000MRA3T4";
       private const string ITEM_ID = "9780007391592";
       private const string NAMESPACE = "http://webservices.amazon.com/AWSECommerceService/2009-03-31";
       private const string MY_AWS_ASSOCIATE_TAG = "bookm02a-20";

       public void Run()
       {
                String requestUrl;
                String title;

                //title = FetchTitle("http://www.google.com");

                /*
                 * Here is an ItemLookup example where the request is stored as a dictionary.
                 */
                IDictionary<string, string> r1 = new Dictionary<string, String>();
                //r1["Service"] = "AWSECommerceService";
                //r1["Version"] = "2011-08-01";
  //               r1["ResponseGroup"] = "RelatedItems";
                //                r1["ResponseGroup"] = "Small";
//               r1["IdType"] = "ASIN";

                /* Random params for testing
                r1["AnUrl"] = "http://www.amazon.com/books";
                r1["AnEmailAddress"] = "foobar@nowhere.com";
                r1["AUnicodeString"] = "αβγδεٵٶٷٸٹٺチャーハン叉焼";
                r1["Latin1Chars"] = "ĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳ";
                 */

//                r1["RelationshipType"] = "AuthorityTitle";
                r1["Operation"] = "ItemLookup";
                r1["ItemId"] = ITEM_ID;
                r1["ResponseGroup"] = "Large,EditorialReview,AlternateVersions";
                r1["AssociateTag"] = MY_AWS_ASSOCIATE_TAG;
//                r1["IdType"] = "ASIN";
                r1["IdType"] = "ISBN";
                r1["MerchantId"] = "Amazon";
                r1["Availability"] = "Available";//                r1["SearchIndex"] = "Books";
                r1["SearchIndex"] = "Books";
                //

/*
//                r1["RelationshipType"] = "AuthorityTitle";
                r1["Operation"] = "ItemSearch";
//                r1["ItemId"] = ITEM_ID;
                r1["ResponseGroup"] = "EditorialReview,AlternateVersions";
                r1["AssociateTag"] = MY_AWS_ASSOCIATE_TAG;
//                r1["IdType"] = "ISBN";
                r1["SearchIndex"] = "Books";
                //r1["Title"] = "The Memoirs of Cleopatra";
                //r1["Aurhor"] = "Margaret George"; 
                r1["Keywords"] = "BK_BKOT_000832";
//                r1["Actor"] = "Donada Peters";
                //r1["MerchantId"] = "Amazon";
                //r1["Availability"] = "Available";
                //r1["Publisher"] = "Books on Tape";
           */
                requestUrl = m_helper.Sign(r1);
                title = FetchTitle(requestUrl);

                System.Console.WriteLine("Method 1: ItemLookup Dictionary form.");
                System.Console.WriteLine("Title is \"" + title + "\"");
                System.Console.WriteLine();

                /*
                 * Here is a CartCreate example where the request is stored as a dictionary.
            
                IDictionary<string, string> r2 = new Dictionary<string, String>();
                r2["Service"] = "AWSECommerceService";
                r2["Version"] = "2011-08-01";
                r2["Operation"] = "CartCreate";
                r2["Item.1.OfferListingId"] = "Ho46Hryi78b4j6Qa4HdSDD0Jhan4MILFeRSa9mK+6ZTpeCBiw0mqMjOG7ZsrzvjqUdVqvwVp237ZWaoLqzY11w==";
                r2["Item.1.Quantity"] = "1";
                r2["AssociateTag"] = MY_AWS_ASSOCIATE_TAG;
                r2["IdType"] = "ISBN";
                r2["SearchIndex"] = "Books";

                requestUrl = m_helper.Sign(r2);
                title = FetchTitle(requestUrl);

                System.Console.WriteLine("Method 1: CartCreate Dictionary form.");
                System.Console.WriteLine("Cart Item Title is \"" + title + "\"");
                System.Console.WriteLine();  
           
       */         
            //while (true)
            {
                // Do Some Work


                Thread.Sleep(100);
            }
        }



       private static string FetchTitle(string url)
       {
           try
           {
               WebRequest request = HttpWebRequest.Create(url);
               WebResponse response = request.GetResponse();

               Stream dataStream = response.GetResponseStream();
               // Open the stream using a StreamReader for easy access.
               StreamReader reader = new StreamReader(dataStream);
               // Read the content.
               string responseFromServer = reader.ReadToEnd();
               // Display the content.
               Console.WriteLine(responseFromServer);
               // Cleanup the streams and the response.
               reader.Close();
               dataStream.Close();
               response.Close();

               StreamWriter fs = new StreamWriter("c:/t.xml");
               fs.Write(responseFromServer);
               fs.Close();



               XmlDocument doc = new XmlDocument();
               doc.Load(response.GetResponseStream());

               XmlNodeList errorMessageNodes = doc.GetElementsByTagName("Message", NAMESPACE);
               if (errorMessageNodes != null && errorMessageNodes.Count > 0)
               {
                   String message = errorMessageNodes.Item(0).InnerText;
                   return "Error: " + message + " (but signature worked)";
               }

               XmlNode titleNode = doc.GetElementsByTagName("Title", NAMESPACE).Item(0);
               string title = titleNode.InnerText;
               return title;
           }
           catch (Exception e)
           {
               System.Console.WriteLine("Caught Exception: " + e.Message);
               System.Console.WriteLine("Stack Trace: " + e.StackTrace);
           }

           return null;
       }






    }

//}




/*


public class WorkerRole : ThreadedRoleEntryPoint
{
    public override void Run()
    {
        // This is a sample worker implementation. Replace with your logic.
        Trace.WriteLine("Worker Role entry point called", "Information");

        base.Run();
    }

    public override bool OnStart()
    {
        List<WorkerEntryPoint> workers = new List<WorkerEntryPoint>();

        workers.Add(new ImageSizer());
        workers.Add(new ImageSizer());
        workers.Add(new ImageSizer());
        workers.Add(new HouseCleaner());
        workers.Add(new TurkHandler());
        workers.Add(new Crawler());
        workers.Add(new Crawler());
        workers.Add(new Crawler());
        workers.Add(new Gardener());
        workers.Add(new Striker());

        return base.OnStart(workers.ToArray());
    }
}




internal class Striker : WorkerEntryPoint
{
    public override void Run()
    {
        while (true)
        {
            // Do Some Work

            Thread.Sleep(100);
        }
    }
}






   CookieContainer cookieJar = new CookieContainer();
   private void FireRequest()
   {
            var request = HttpWebRequest.Create(new Uri("http://www.google.se")) as HttpWebRequest;
 
            request.Method = "GET";
            request.CookieContainer = cookieJar;
 
            request.BeginGetResponse(ar =>
            {
                HttpWebRequest req2 = (HttpWebRequest)ar.AsyncState;
                var response = (HttpWebResponse)req2.EndGetResponse(ar);
                int numVisibleCookies = response.Cookies.Count;
 
            }, request);
        }
 Inspecting th
*/