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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;


namespace Bloodhound
{

    public class Int64_Setting : TableServiceEntity
    {

        public Int64_Setting() {m_Value=0;}
        public Int64 m_Value { get; set; }

    }

    private class SettingsTable
    {
        Helpers m_Helpers; 
        Int64 m_NextWorkId { get; set; }
        Int64 m_NextAuthorId { get; set; }

        public SettingsTable(Helpers mh)
        {  m_Helpers = mh; m_NextAuthorId = m_NextWorkId = 0x10000000;}


        public Int64 GetAndIncrement(string Item)
        {
            Int64_Setting v=new Int64_Setting();
            for(int i=0;i<5;i++)
            {
                m_Helpers.m_Table.GetEntity<Int64_Setting>("Settings",Item, "Biblio",out v);
                v.m_Value++;
                bool ret = m_Helpers.m_Table.ReplaceUpdateEntity<Int64_Setting>("Settings",Item, "Biblio",v);
                return v.m_Value-1;
            }

            throw new Exception("GetAndIncrement lock failed");

        }

        public Int64 GetNextAuthorID() {return GetAndIncrement("NextWorkId"); }
        public Int64 GetNextWorkID() {return GetAndIncrement("NextAuthorId"); }

    }


    private class __Settings
    {
        public __Settings()
        { m_NextAuthorId = m_NextWorkId = 0x10000000;}

        Int64 m_NextWorkId { get; set; }
        Int64 m_NextAuthorId { get; set; }

    }

    public class Admin
    {
        public Admin(Helpers mh)
        { m_Helpers = mh; m_a = new __Settings(); }

        [JsonIgnore]
        Helpers m_Helpers;
        __Settings m_a;

        public bool Load()
        {            
            try
            {
                CloudBlobContainer container = m_Helpers.m_Blob.BlobClient.GetContainerReference("admin");
                CloudBlob blob = container.GetBlobReference("settings");
                BlobStream bs = blob.OpenRead();
                TextReader tw = new StreamReader(bs);
                string jsonResult =tw.ReadToEnd();
                bs.Close();
                m_a = JsonConvert.DeserializeObject<__Settings>(jsonResult);
                return true;
            }
            catch (Exception e)
            {
                //System.
                Debugger.WriteThree("Caught Exception: " + e.Message);
                Debugger.WriteThree("Stack Trace: " + e.StackTrace);
            }
            return false;
        }

        public bool Save()
        {
            try
            {
                string jsonResult = JsonConvert.SerializeObject(m_a);
                CloudBlobContainer container = m_Helpers.m_Blob.BlobClient.GetContainerReference("admin");
                CloudBlob blob = container.GetBlobReference("settings");
                BlobStream bs = blob.OpenWrite();
                TextWriter tw = new StreamWriter(bs);
                tw.WriteLine(jsonResult);
                tw.Flush();
                bs.Close();
                return true;
            }
            catch (Exception e)
            {
                //System.
                Debugger.WriteThree("Caught Exception: " + e.Message);
                Debugger.WriteThree("Stack Trace: " + e.StackTrace);
            }
            return false;
        }

    }


    public class WorkIdRec : TableServiceEntity
    {
        public WorkIdRec(string WorkId, string Isbn)
        {
            this.PartitionKey = WorkId;
            this.RowKey = Isbn;
        }

        public string Title { get; set; }
        public string RespParty { get; set; }
        public string Series { get; set; }
        public string Publisher { get; set; }
    }


    public class IsbnRec : TableServiceEntity
    {
        public IsbnRec(string WorkId, string Isbn)
        {
            this.PartitionKey = WorkId;
            this.RowKey = Isbn;
        }

        public string Title { get; set; }
        public string RespParty { get; set; }
        public string Series { get; set; }
        public string Publisher { get; set; }
 
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

    public class BisacRec : TableServiceEntity
    {
        public BisacRec(string WorkId, string BCode)
        {
            this.PartitionKey = WorkId;
            this.RowKey = BCode;
        }

        public string Isbn { get; set; }
        public string CodeType { get; set; }
        public string Source { get; set; }

    }

    public class AuthorsRec : TableServiceEntity
    {
        public AuthorsRec(string WorkId, string LastName)
        {
            this.PartitionKey = WorkId;
            this.RowKey = LastName;
        }
        
        public string Isbn { get; set; }
        //public string Last { get; set; }

        public string Salutation { get; set; }
        public string FirstMid { get; set; }
        public string Suffix { get; set; }
        public string Role { get; set; }
    }

    public class PriceRec : TableServiceEntity
    {
        public PriceRec(string WorkId, string Isbn)
        {
            this.PartitionKey = WorkId;
            this.RowKey = Isbn;
        }

        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Store { get; set; }
        public DateTime LastUpdate { get; set; }

    }

    public class RatingRec : TableServiceEntity
    {
        public RatingRec(string WorkId, string Isbn)
        {
            this.PartitionKey = WorkId;
            this.RowKey = Isbn;
        }

        public decimal Rating { get; set; }
        public int NumRatings { get; set; }
        public string Source { get; set; }
        public DateTime LastUpdate { get; set; }
    }


    public class PopularityRec : TableServiceEntity
    {
        public PopularityRec(string WorkId, string Isbn)
        {
            this.PartitionKey = WorkId;
            this.RowKey = Isbn;
        }

        public string Rank { get; set; }
        public string Source { get; set; }
        public string Standardized { get; set; }
        public DateTime LastUpdate { get; set; }

    }

    public class KeywordRec : TableServiceEntity
    {
        public KeywordRec(string WorkId, string Keyword)
        {
            this.PartitionKey = WorkId;
            this.RowKey = Keyword;
        }

        public string Isbn { get; set; }
        public string Source { get; set; }
        public DateTime LastUpdate { get; set; }

    }
}





/*
context.MergeOption = MergeOption.NoTracking;
And when updating (or removing) an entity we call:

context.AttachTo(entitySetName, entity, eTag);
tableService.updateEntity('tasktable',serverEntity, {checkEtag: true},
    function(error, updateResponse) {
    if(!error){
        console.log('success');
    } else {
        console.log(error);
   }
});


class Person
{
    private string myName ="N/A";
    private int myAge = 0;

    // Declare a Name property of type string:
    public string Name
    {
        get 
        {
           return myName; 
        }
        set 
        {
           myName = value; 
        }
    }

    // Declare an Age property of type int:
    public int Age
    {
        get 
        { 
           return myAge; 
        }
        set 
        { 
           myAge = value; 
        }
    }

    public override string ToString()
    {
        return "Name = " + Name + ", Age = " + Age;
    }

public static string AcquireLease(this CloudBlob blob)
{
    var creds = blob.ServiceClient.Credentials;
    var transformedUri = new Uri(creds.TransformUri(blob.Uri.ToString()));
    var req = BlobRequest.Lease(transformedUri,
        90, // timeout (in seconds)
        LeaseAction.Acquire, // as opposed to "break" "release" or "renew"
        null); // name of the existing lease, if any
    blob.ServiceClient.Credentials.SignRequest(req);
    using (var response = req.GetResponse())
    {
        return response.Headers["x-ms-lease-id"];
    }
}
var account = CloudStorageAccount.DevelopmentStorageAccount;
var blob = account.CreateCloudBlobClient().GetBlobReference("container/blob");
var leaseId = blob.AcquireLease();
blob.UploadText("new content", leaseId);
blob.ReleaseLease(leaseId);

// NOTE: This method doesn't do everything that the regular UploadText does.
// Notably, it doesn't update the BlobProperties of the blob (with the new
// ETag and LastModifiedTimeUtc). It also, like all the methods in this file,
// doesn't apply any retry logic. Use this at your own risk!

public static void UploadText(this CloudBlob blob, string text, string leaseId)
{
    string url = blob.Uri.ToString();
    if (blob.ServiceClient.Credentials.NeedsTransformUri)
    {
        url = blob.ServiceClient.Credentials.TransformUri(url);
    }
    var req = BlobRequest.Put(new Uri(url), 90, new BlobProperties(), BlobType.BlockBlob, leaseId, 0);
    using (var writer = new StreamWriter(req.GetRequestStream()))
    {
        writer.Write(text);
    }
    blob.ServiceClient.Credentials.SignRequest(req);
    req.GetResponse().Close();
}
private static void DoLeaseOperation(CloudBlob blob, string leaseId, LeaseAction action)
{
    var creds = blob.ServiceClient.Credentials;
    var transformedUri = new Uri(creds.TransformUri(blob.Uri.ToString()));
    var req = BlobRequest.Lease(transformedUri, 90, action, leaseId);
    creds.SignRequest(req);
    req.GetResponse().Close();
}

public static void ReleaseLease(this CloudBlob blob, string leaseId)
{
    DoLeaseOperation(blob, leaseId, LeaseAction.Release);
}

public static void RenewLease(this CloudBlob blob, string leaseId)
{
    DoLeaseOperation(blob, leaseId, LeaseAction.Renew);
}

public static void BreakLease(this CloudBlob blob)
{
    DoLeaseOperation(blob, null, LeaseAction.Break);
}

*/


