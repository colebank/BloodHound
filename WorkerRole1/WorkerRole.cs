

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


namespace WorkerRole1
{

    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("$projectname$ entry point called", "Information");

            //while (true)
            {
                FetchPage("http://edelweiss.abovethetreeline.com/ProductDetailPage.aspx?sku=0345527356");
                Thread.Sleep(10000);
                Trace.WriteLine("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }


        private static string FetchPage(string url)
        {
            try
            {

                WebRequest request = WebRequest.Create(url);
                // If required by the server, set the credentials.
                //request.Credentials = CredentialCache.DefaultCredentials;
                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // Display the status.
                Console.WriteLine(response.StatusDescription);
                // Get the stream containing content returned by the server.
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

                /*

                                WebRequest request = HttpWebRequest.Create(url);
                                WebResponse response = request.GetResponse();
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
                 */
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Caught Exception: " + e.Message);
                System.Console.WriteLine("Stack Trace: " + e.StackTrace);
            }

            return null;
        }
    }

}

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