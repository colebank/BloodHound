
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ServiceModel;
using System.Xml;
using System.Diagnostics;
using System.IO;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;


class Debugger
{
    static Helpers m_Helpers;
    static TextWriter m_Tw;


    public static void Init(Helpers mh)
    {
        m_Helpers = mh;

        m_Helpers.m_Blob.CreateContainer("log");
        CloudBlobContainer container = m_Helpers.m_Blob.BlobClient.GetContainerReference("log");
        CloudBlob blob = container.GetBlobReference("Bloodhound.Log");
        BlobStream bs = blob.OpenWrite();
        m_Tw = new StreamWriter(bs);
        //content = new UTF8Encoding().GetString(data);
        //bs.Close();
    }

    public static void WriteThree(string s)
    {
        Console.WriteLine(s);
        Trace.WriteLine(s);
        m_Tw.WriteLine(s);
        m_Tw.Flush();
    }

}



class IsbnConverter
{
    /// <summary>
    /// Converts ISBN13 code to ISBN10 code
    /// </summary>
    /// <param name="isbn13">code to convert</param>
    /// <returns>empty if the parameter is invalid, otherwise the converted value</returns>
    public static string ConvertTo10(string isbn13)
    {
        string isbn10 = string.Empty;
        long temp;

        // *************************************************
        // Validation of isbn13 code can be done by        *
        // using this snippet found here:                  *
        // http://www.dreamincode.net/code/snippet5385.htm *
        // *************************************************

        if (!string.IsNullOrEmpty(isbn13) &&
                isbn13.Length == 13 &&
                Int64.TryParse(isbn13, out temp))
        {
            isbn10 = isbn13.Substring(3, 9);
            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += Int32.Parse(isbn10[i].ToString()) * (i + 1);

            int result = sum % 11;
            char checkDigit = (result > 9) ? 'X' : result.ToString()[0];
            isbn10 += checkDigit;
        }

        return isbn10;
    }

    /// <summary>
    /// Converts ISBN10 code to ISBN13 code
    /// </summary>
    /// <param name="isbn10">code to convert</param>
    /// <returns>empty if the parameter is invalid, otherwise the converted value</returns>
    public static string ConvertTo13(string isbn10)
    {
        string isbn13 = string.Empty;
        long temp;

        // *************************************************
        // Validation of isbn10 code can be done by        *
        // using this snippet found here:                  *
        // http://www.dreamincode.net/code/snippet5385.htm *
        // *************************************************

        if (!string.IsNullOrEmpty(isbn10) &&
                isbn10.Length == 10 &&
                Int64.TryParse(isbn10, out temp))
        {
            int result = 0;
            isbn13 = "978" + isbn10.Substring(0, 9);
            for (int i = 0; i < isbn13.Length; i++)
                result += int.Parse(isbn13[i].ToString()) * ((i % 2 == 0) ? 1 : 3);

            int checkDigit = 10 - (result % 10);
            isbn13 += checkDigit.ToString();
        }

        return isbn13;
    }
}


public class NameParts
{
    public NameParts()
    {
        Salutation="";
        FirstMid = "";
        Last = "";
        Suffix = "";
        Role = "";
    }
    
    public string Salutation;
    public string FirstMid;
    public string Last;
    public string Suffix;
    public string Role;

    public string BTOrderedName()
    {
        string end = Salutation + (Suffix != "" && Salutation != "" ? ", " : "") 
            +Suffix;

        return Last+", "+FirstMid+(end != "" ? ", "+end: "") +(Role != "" ? " ("+Role+")" : "");

    }
}


public static class NameExtensions
{
        private static List<string> salutations = new List<string>() {
        "hon","judge","dr","doctor","miss","fr","misses","mr","mister","mrs","ms","sir",
        "rev","madam","madame","ab","2ndlt","amn","1stlt","a1c","capt","sra","maj",
        "ssgt","ltcol","tsgt","col","briggen","1stsgt","majgen","smsgt","ltgen",
        "1stsgt","gen","cmsgt","1stsgt","ccmsgt","cmsaf","pvt","2lt","pv2","1lt",
        "pfc","cpt","spc","maj","cpl","ltc","sgt","col","ssg","bg","sfc","mg",
        "msg","ltg","1sgt","gen","sgm","csm","sma","wo1","wo2","wo3","wo4","wo5",
        "ens","sa","ltjg","sn","lt","po3","lcdr","po2","cdr","po1","capt","cpo",
        "radm(lh)","scpo","radm(uh)","mcpo","vadm","mcpoc","adm","mpco-cg","cwo-2",
        "cwo-3","cwo-4","pvt","2ndlt","pfc","1stlt","lcpl","capt","cpl","maj","sgt",
        "ltcol","ssgt","col","gysgt","bgen","msgt","majgen","1stsgt","ltgen","mgysgt",
        "gen","sgtmaj","sgtmajmc","wo-1","cwo-2","cwo-3","cwo-4","cwo-5","ens","sa",
        "ltjg","sn","lt","po3","lcdr","po2","cdr","po1","capt","cpo","rdml","scpo",
        "radm","mcpo","vadm","mcpon","adm","fadm","wo1","cwo2","cwo3","cwo4","cwo5",
        "prof","professor","freiherr","dame","bishop","baroness","baron","count",
        "countess","duke","dutchess","graf","gräfin","margrave","margravine","earl","marquis"};

        private static List<string> preficies = new List<string>() {
        "abu","bon","ben","bin","da","dal","de","del","della","der","de","di","dí","du","e","ibn",
        "la","le","pietro","san","st","ste","ter","van","vel","von","vere","vanden"};

         private static List<string> suffices = new List<string>() { 
         "esq","esquire","jr","sr","i","ii","iii","iv","v","clu","chfc","cfp","md","phd","cpa",
         "senior","Junior","apr","rph","pe","ma","dmd","cme","dds"};

          private static List<string> conjunctions = new List<string>() {"&", "and", "et", "e", "und", "y"};

    static private string FixCommas(string s)
    {
        string r = "";
        bool lastspace = false;

        foreach (char c in s)
        {
            if (c == ' ')
            {
                if (!lastspace)
                {
                    r += c;
                    lastspace = true;
                }
            }
            else
            if (c == ',')
            {
                r += ", ";
                lastspace = true;
            }  
            else
                r += c;
        }

        return r;
    }

    static private string RemoveChar(string s, char x)
    {
        string r="";
        foreach (char c in s)
            if (c != x) r+=c;

        return r;
    }


    static NameExtensions()
    {
    }

    static private List<string> SplitPlus(string name, char x)
    {
        List<string> Ret=new List<string>();

        string[] csplit = (name.Split(new Char[] { x }));
        foreach (string s in csplit)
        {
            string t = s.Trim();
            if (t.Length > 0)
                Ret.Add(t);

        }
        return Ret;

    }

    //static private List<string> SplitPlus(string name, char x)

 
    public static NameParts BTOneNameParser(string OneName) // in format of pre-last[conj last2], first middle suffix, salutation (role)
    {
        NameParts np=new NameParts();

        if (OneName == "Not Available (NA)")
            return np;

        int ct =OneName.IndexOf('(');
        if(ct>0)
        {
            OneName=OneName.Substring(0,ct-1);
            np.Role=OneName.Substring(ct+1,OneName.Length);
        }

        List<string> clist = SplitPlus(OneName,',');

        int i=0;

        if(i < clist.Count)
            np.Last=clist[i++];

        if(i < clist.Count)
            np.FirstMid=clist[i++];

        while(i < clist.Count )
        {
            string test = RemoveChar(clist[i],'.').ToLower();

            if (preficies.IndexOf(test) >= 0)
            {
                if(np.Salutation!="")
                    np.Salutation+=", ";
                np.Salutation+=clist[i];
            }
            else if (suffices.IndexOf(test) >= 0)
            {
                if(np.Suffix!="")
                    np.Suffix+=", ";
                np.Suffix+=clist[i];
            }
            else
            {
                Trace.WriteLine("BT parse error",clist[i]);
            }
            i++;
        }

        return np;

    }

    public static List<NameParts> BTNameListParser(string MultiName)
    {
        List<string> Authors = SplitPlus(MultiName,'/');
        List<NameParts> Parsed = new List<NameParts>();

        foreach (string s in Authors)
           Parsed.Add(BTOneNameParser(s));

        return Parsed;
    }

}


/*

namespace Spike.ContractBuilder
{
    static internal class ContractHelper
    {
        static Random r = new Random();

        /// <summary>
        /// Determines the namespace associated with the method
        /// </summary>
        /// <param name="contractType"></param>
        /// <returns></returns>
        internal static string GetNamespace(Type contractType)
        {
            string ns = string.Empty;
            object[] attributes = contractType.GetCustomAttributes(typeof(ServiceContractAttribute), true);
            if (attributes.Length == 1)
            {
                ns = ((ServiceContractAttribute)attributes[0]).Namespace;
            }
            return ns;
        }
        internal static string GenerateSampleXml(ParameterInfo info)
        {
            if (info.ParameterType.IsClass)
            {
                object parameter = null;

                if (info.ParameterType.FullName == "System.String" ||
                    (info.ParameterType.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract)
                    return info.Name ?? info.ParameterType.FullName;
                else
                {
                    parameter = CreateFullInstance(info.ParameterType);

                    Type ser = typeof(SerializationHelper<>).MakeGenericType(info.ParameterType);
                    string s = (string)ser.InvokeMember("SerializeServiceObject", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { parameter });

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(s);

                    return "\r\n" + doc.DocumentElement.InnerXml;
                }
            }
            else
            {
                object o = GetDefaultValue(info.Name, info.ParameterType);
                return o.ToString();
            }
        }
        internal static object CreateFullInstance(Type type)
        {
            object parameter = type.IsArray ? Activator.CreateInstance(type, new object[] { 1 }) : Activator.CreateInstance(type);

            if (type.IsArray)
            {
                // TODO: figure this out
            }
            else
            {
                foreach (FieldInfo field in type.GetFields())
                {
                    if (field.IsLiteral != true && field.IsPublic == true)
                    {
                        object value = GetDefaultValue(field.Name, field.FieldType);
                        field.SetValue(parameter, value);
                    }
                }
                foreach (PropertyInfo property in type.GetProperties())
                {
                    object value = GetDefaultValue(property.Name, property.PropertyType);
                    property.SetValue(parameter, value, null);
                }
            }
            return parameter;
        }
        internal static object GetDefaultValue(string name, Type type)
        {
            object value = null;
            switch (type.FullName)
            {
                case "System.Boolean":
                    return false;
                case "System.Guid":
                    return Guid.NewGuid();
                case "System.Void":
                    return type.FullName;
                case "System.Single":
                case "System.Double":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                    value = 0;
                    break;
                case "System.DateTime":
                    value = DateTime.Now;
                    break;
                case "System.String":
                    value = name;
                    break;
                case "System.Runtime.Serialization.ExtensionDataObject":
                    value = null;
                    break;
                default:
                    if (type.IsEnum)
                    {
                        value = Enum.GetValues(type).GetValue(0);
                    }
                    else if (type.IsClass)
                    {
                        value = CreateFullInstance(type);
                    }
                    else if (type.FullName.StartsWith("System.Nullable"))
                    {
                        // TODO: determine the type
                        return null;
                    }
                    else
                    {
                        throw new Exception("Not Supported Type: " + name);
                    }
                    break;
            }
            return value;
        }
        internal static object GetRandomObject(Type type)
        {
            // type code
            switch (type.FullName)
            {
                case "System.Void":
                    return null;
                case "System.Boolean":
                    return r.Next(0, 1) == 1;
                case "System.Single":
                case "System.Double":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                    return r.Next(1000);
                case "System.DateTime":
                    return DateTime.Now.AddMinutes(r.Next(400000) - 200000);
                case "System.String":
                    return "Stub returned a value of " + r.Next(1000).ToString() + ".";
            }

            // should now be a reference type or array

            // ending code
            if (type.IsArray == false)
            {
                return type.GetConstructor(new Type[] { }).Invoke(null);
            }
            else
            {
                // TODO: a better array than this...
                return Array.CreateInstance(type.GetElementType(), 0);
            }
        }

        internal static object DeserializeObject(Type type, string content)
        {
            return DeserializeObject(type, content, null);
        }
        internal static object DeserializeObject(Type type, string content, Type[] extraTypes)
        {
            if (typeof(Exception).IsAssignableFrom(type))
            {
                ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(string) });
                throw (Exception)constructor.Invoke(new object[] { content });
            }
            if (type.Name == "Void")
            {
                return null;
            }
            if(type==typeof(string))
            {
                return content;
            }
            if (type.IsEnum)
            {
                return Enum.Parse(type, content);
            }
            if (type.IsClass)
            {
                Type ser = typeof(SerializationHelper<>).MakeGenericType(type);
                if(extraTypes==null)
                    return ser.InvokeMember("Deserialize", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { content});
                else
                    return ser.InvokeMember("Deserialize", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { content, extraTypes });
            }
            else
            {
                MethodInfo parse = type.GetMethod("Parse", new Type[] { typeof(string) });

                if (parse != null)
                {
                    return parse.Invoke(null, new object[] { content });
                }
                else
                    throw new Exception("Type is not supported: " + type.FullName);
            }
        }
    }
}



*/

