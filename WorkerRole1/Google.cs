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

namespace Bloodhound
{

    public class GoogleLoader
    {
        Helpers m_Helpers;

        public GoogleLoader(Helpers mh)
        {
            m_Helpers=mh;
            m_Helpers.m_Blob.CreateContainer("log");
            CloudBlobContainer container = m_Helpers.m_Blob.BlobClient.GetContainerReference("log");

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

}



    /*
    [JsonProperty("id")]
    public string ID { get; set; }
    [JsonProperty("label")]
    public string Label { get; set; }
    [JsonProperty("url")]
    public string URL { get; set; }
    [JsonProperty("item")]
    public List<Test2> Test2List { get; set; }
    */


    /*
    public class Task
    {
        [JsonProperty(PropertyName = "start")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Start { get; set; }
 
        [JsonProperty(PropertyName = "stop")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Stop { get; set; }
 
        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }
 
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
 
        [JsonProperty(PropertyName = "project")]
        public Project Project { get; set; }
    }
 
    public class Project
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
 
        [JsonProperty(PropertyName = "client_project_name")]
        public string Client { get; set; }
    }
    */




/*
            // First create a BookService object and execute query
            BooksService booksService = new BooksService("myCompany-myApp-1");
            String isbn = "9780552152679";
            URL url = new URL("http://www.google.com/books/feeds/volumes/?q=ISBN%3C" + isbn + "%3E");
            VolumeQuery volumeQuery = new VolumeQuery(url);
            VolumeFeed volumeFeed = booksService.query(volumeQuery, VolumeFeed.class);
            // Since we've used an ISBN in our query there will be only one entry in VolumeFeed
            List<VolumeEntry> volumeEntries = volumeFeed.getEntries();
            VolumeEntry entry = volumeEntries.get(0);
            // Get the Link for the small thumbnail and create a Link for the large thumbnail
            // Parsing/Replacing substring is terrible but so far there is no other option in gdata API
            Link thumbnailSmall = entry.getThumbnailLink();
             String smallThumbnailHref = thumbnailSmall.getHref();
            String largeThumbnailHref = smallThumbnailHref.replaceFirst("zoom=5", "zoom=1");
            Link thumbnailLarge = new Link(thumbnailSmall.getRel(), thumbnailSmall.getType(), largeThumbnailHref);
            // Get InputStream and save small thumbnail
            GDataRequest request = booksService.createLinkQueryRequest(thumbnailSmall);
            request.execute();
            InputStream inStream = request.getResponseStream();
            BufferedImage input = ImageIO.read(inStream);
            ImageIO.write(input, "PNG", new File("/home/user/android/test_small"));
            request.end();
            // Get InputStream and save large thumbnail
            request = booksService.createLinkQueryRequest(thumbnailLarge);
            request.execute();
            inStream = request.getResponseStream();
            input = ImageIO.read(inStream);
            ImageIO.write(input, "PNG", new File("/home/user/android/test_large"));
            request.end();



            BooksService booksService = new BooksService("UAH");
            String isbn = "9780262140874";
            URL url = new URL("http://www.google.com/books/feeds/volumes/?q=ISBN%3C" + isbn + "%3E");
            VolumeQuery volumeQuery = new VolumeQuery(url);
            VolumeFeed volumeFeed = booksService.query(volumeQuery, VolumeFeed.class);
            VolumeEntry bookInfo=volumeFeed.getEntries().get(0);
 
            System.out.println("Title: " + bookInfo.getTitles().get(0));
            System.out.println("Id: " + bookInfo.getId());
            System.out.println("Authors: " + bookInfo.getAuthors());
            System.out.println("Version: " + bookInfo.getVersionId());
            System.out.println("Description: "+bookInfo.getDescriptions()+"n");
            titleBook= bookInfo.getTitles().get(0).toString();
            titleBook=(String) titleBook.subSequence(titleBook.indexOf("="), titleBook.length()-1);
        }catch(Exception ex){System.out.println(ex.getMessage());}
    }

}




https://www.googleapis.com/books/v1/volumes?q=isbn:9780307588364
GET https://www.googleapis.com/books/v1/volumes?q=flowers+inauthor:keyes&key=yourAPIKey

intitle: Returns results where the text following this keyword is found in the title.
inauthor: Returns results where the text following this keyword is found in the author.
inpublisher: Returns results where the text following this keyword is found in the publisher.
subject: Returns results where the text following this keyword is listed in the category list of the volume.
isbn: Returns results where the text following this keyword is the ISBN number.
lccn: Returns results where the text following this keyword is the Library of Congress Control Number.
oclc: Returns results where the text following this keyword is the Online Computer Library Center number.


BooksService service = new BooksService("gdataSample-Books-1");
		VolumeQuery vv = new VolumeQuery(new URL("http://www.google.com/books/feeds/volumes?q=ISBN<"+ISBN+">"));
                VolumeFeed volumeFeed = service.query(vv, VolumeFeed.class);
 * 
 * 
 * 
 * 
 * 
 * 

public class AndroidBooksApiActivity extends Activity {
Button btnSearch;
TextView tvTitle,tvISBN,tvAuthor,tvDatePublished;
EditText etSearch;
@Override
public void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    setContentView(R.layout.main);
    btnSearch=(Button)findViewById(R.id.btnFindBook);
    tvTitle=(TextView)findViewById(R.id.tvTitle);
    tvISBN=(TextView)findViewById(R.id.tvISBN);
    tvAuthor=(TextView)findViewById(R.id.tvAuthor);
    tvDatePublished=(TextView)findViewById(R.id.tvDatePublished);
    etSearch=(EditText)findViewById(R.id.etSearch);
    btnSearch.setOnClickListener(new View.OnClickListener() {

        public void onClick(View v) {
            try
            {   
                //String isbn;
                //isbn=etSearch.getText().toString();
                String isbn = "9780262140874";          

                BooksService booksService = new BooksService("UAH");        

                URL url = new URL("http://www.google.com/books/feeds/volumes/?q=ISBN%3C" + isbn + "%3E");

                VolumeQuery volumeQuery = new VolumeQuery(url);

                VolumeFeed volumeFeed = booksService.query(volumeQuery, VolumeFeed.class);

                VolumeEntry bookInfo=volumeFeed.getEntries().get(0);

                tvTitle.setText("Title: " + bookInfo.getTitles().get(0));
                tvISBN.setText("ISBN: " + isbn);
                tvAuthor.setText("Authors: " + bookInfo.getCreators());
                tvDatePublished.setText("DatePublished: "+bookInfo.getDates()+"\n");
            }catch(Throwable ex){tvTitle.setText(ex.getMessage()); ex.printStackTrace();}

          }
    });
}
 * 
 * 
 * 
import java.net.URL;
import com.google.gdata.client.books.BooksService;
import com.google.gdata.client.books.VolumeQuery;
import com.google.gdata.data.books.VolumeEntry;
import com.google.gdata.data.books.VolumeFeed;
public class books 
{
public static void main(String[] args)
{       
    ////////////////////////////////////////////////
    try
    {                               
        BooksService booksService = new BooksService("UAH");
        String isbn = "9780262140874";
        URL url = new URL("http://www.google.com/books/feeds/volumes/?q=ISBN%3C" + isbn + "%3E");
        VolumeQuery volumeQuery = new VolumeQuery(url);
        VolumeFeed volumeFeed = booksService.query(volumeQuery, VolumeFeed.class);
        VolumeEntry bookInfo=volumeFeed.getEntries().get(0);

        System.out.println("Title: " + bookInfo.getTitles().get(0));
        System.out.println("Id: " + bookInfo.getId());
        System.out.println("Authors: " + bookInfo.getCreators());
        System.out.println("Description: "+bookInfo.getDescriptions()+"\n");
    }catch(Exception ex){System.out.println(ex.getMessage());}
    /////////////////////////////////////////////////                           
 }}
 * 
 * 
 * Google books API searching by ISBN
JsonFactory jsonFactory = new JacksonFactory();    
    final Books books = new Books(new NetHttpTransport(), jsonFactory);
    List volumesList = books.volumes.list("9780262140874");
 
    volumesList.setMaxResults((long) 2);
 
    volumesList.setFilter("ebooks");
    try
    {
        Volumes volumes = volumesList.execute();
        for (Volume volume : volumes.getItems())
        {
            VolumeVolumeInfo volumeInfomation = volume.getVolumeInfo();
            System.out.println("Title: " + volumeInfomation.getTitle());
            System.out.println("Id: " + volume.getId());
            System.out.println("Authors: " + volumeInfomation.getAuthors());
            System.out.println("date published: " + volumeInfomation.getPublishedDate());
            System.out.println();
        }
 
    } catch (Exception ex) {
        // TODO Auto-generated catch block
        System.out.println("didnt wrork "+ex.toString());
    }
       
String titleBook="";
 
    ////////////////////////////////////////////////
    try
    {                              
        BooksService booksService = new BooksService("UAH");
        String isbn = "9780262140874";
        URL url = new URL("http://www.google.com/books/feeds/volumes/?q=ISBN%3C" + isbn + "%3E");
        VolumeQuery volumeQuery = new VolumeQuery(url);
        VolumeFeed volumeFeed = booksService.query(volumeQuery, VolumeFeed.class);
        VolumeEntry bookInfo=volumeFeed.getEntries().get(0);
 
        System.out.println("Title: " + bookInfo.getTitles().get(0));
        System.out.println("Id: " + bookInfo.getId());
        System.out.println("Authors: " + bookInfo.getAuthors());
        System.out.println("Version: " + bookInfo.getVersionId());
        System.out.println("Description: "+bookInfo.getDescriptions()+"n");
        titleBook= bookInfo.getTitles().get(0).toString();
        titleBook=(String) titleBook.subSequence(titleBook.indexOf("="), titleBook.length()-1);
    }catch(Exception ex){System.out.println(ex.getMessage());}
    /////////////////////////////////////////////////
    JsonFactory jsonFactory = new JacksonFactory();    
    final Books books = new Books(new NetHttpTransport(), jsonFactory);
    List volumesList = books.volumes.list(titleBook);  
    try
    {
        Volumes volumes = volumesList.execute();
        Volume bookInfomation= volumes.getItems().get(0);
 
        VolumeVolumeInfo volumeInfomation = bookInfomation.getVolumeInfo();
        System.out.println("Title: " + volumeInfomation.getTitle());
        System.out.println("Id: " + bookInfomation.getId());
        System.out.println("Authors: " + volumeInfomation.getAuthors());
        System.out.println("date published: " + volumeInfomation.getPublishedDate());
        System.out.println();
 
    } catch (Exception ex) {
        System.out.println("didnt wrork "+ex.toString());
    }
       
https://www.googleapis.com/books/v1/volumes?q=isbn<your_isbn_here>
       
https://www.googleapis.com/books/v1/volumes?q=isbn0735619670
       
BooksService booksService = new BooksService("myCompany-myApp-1");
myService.setUserCredentials("user@domain.com", "secretPassword");
 
String isbn = "9780552152679";
URL url = new URL("http://www.google.com/books/feeds/volumes/?q=ISBN%3C" + isbn + "%3E");
VolumeQuery volumeQuery = new VolumeQuery(url);
VolumeFeed volumeFeed = booksService.query(volumeQuery, VolumeFeed.class);
 
// using an ISBN in query gives only one entry in VolumeFeed
List<VolumeEntry> volumeEntries = volumeFeed.getEntries();
VolumeEntry entry = volumeEntries.get(0);
 * 
 * 
 * 
 * // First create a BookService object and execute query
02
BooksService booksService = new BooksService("myCompany-myApp-1");
03
String isbn = "9780552152679";
04
URL url = new URL("http://www.google.com/books/feeds/volumes/?q=ISBN%3C" + isbn + "%3E");
05
VolumeQuery volumeQuery = new VolumeQuery(url);
06
VolumeFeed volumeFeed = booksService.query(volumeQuery, VolumeFeed.class);
07
 
08
// Since we've used an ISBN in our query there will be only one entry in VolumeFeed
09
List<VolumeEntry> volumeEntries = volumeFeed.getEntries();
10
VolumeEntry entry = volumeEntries.get(0);
11
 
12
// Get the Link for the small thumbnail and create a Link for the large thumbnail
13
// Parsing/Replacing substring is terrible but so far there is no other option in gdata API
14
Link thumbnailSmall = entry.getThumbnailLink();
15
String smallThumbnailHref = thumbnailSmall.getHref();
16
String largeThumbnailHref = smallThumbnailHref.replaceFirst("zoom=5", "zoom=1");
17
Link thumbnailLarge = new Link(thumbnailSmall.getRel(), thumbnailSmall.getType(), largeThumbnailHref);
18
 
19
// Get InputStream and save small thumbnail
20
GDataRequest request = booksService.createLinkQueryRequest(thumbnailSmall);
21
request.execute();
22
InputStream inStream = request.getResponseStream();
23
BufferedImage input = ImageIO.read(inStream);
24
ImageIO.write(input, "PNG", new File("/home/user/android/test_small"));
25
request.end();
26
 
27
 
28
// Get InputStream and save large thumbnail
29
request = booksService.createLinkQueryRequest(thumbnailLarge);
30
request.execute();
31
inStream = request.getResponseStream();
32
input = ImageIO.read(inStream);
33
ImageIO.write(input, "PNG", new File("/home/user/android/test_large"));
34
request.end();

 * 
 * 


namespace consoleGoogleResearch
{
    class Program
    {
        public static void Main(string[] args)
        {

            // Register the authenticator. The Client ID and secret have to be copied from the API Access
            // tab on the Google APIs Console.
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = "512897999421.apps.googleusercontent.com";
            provider.ClientSecret = "8evigZBt6SZgc7orc03xyjyt";
            // Create the service. This will automatically call the authenticator.
            var service = new CalendarService(new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthentication));

            Google.Apis.Calendar.v3.CalendarListResource.ListRequest clrq = service.CalendarList.List();
            var result = clrq.Fetch();

            if (result.Error != null)
            {
                Console.WriteLine(result.Error.Message);
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Calendars: ");
            foreach (CalendarListEntry calendar in result.Items)
            {
                Console.WriteLine("{0}", calendar.Id);
                Console.WriteLine("\tAppointments:");
                Google.Apis.Calendar.v3.EventsResource.ListRequest elr = service.Events.List(calendar.Id);
                var events = elr.Fetch();
                foreach (Event e in events.Items)
                {
                    Console.WriteLine("\t From: {0} To: {1} Description: {2}, Location: {3}", e.Start, e.End, e.Description, e.Location);
                }
            }
            Console.ReadKey();
        }

        private static IAuthorizationState GetAuthentication(NativeApplicationClient arg)
        {
            // Get the auth URL:
            //IAuthorizationState state = new AuthorizationState(new[] { CalendarService.Scopes.Calendar.ToString() });
            IAuthorizationState state = new AuthorizationState(new[] { "https://www.google.com/calendar/feeds" });
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
            Uri authUri = arg.RequestUserAuthorization(state);

            // Request authorization from the user (by opening a browser window):
            Process.Start(authUri.ToString());
            Console.Write("  Authorization Code: ");
            string authCode = Console.ReadLine();

            // Retrieve the access token by using the authorization code:
            return arg.ProcessUserAuthorization(authCode, state);
        }
    }
}



*/
