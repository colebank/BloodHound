
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
    
    
    
    public static class StringExtensions
    {
        private static readonly Dictionary<char, string> Replacements = new Dictionary<char, string>();
        /// <summary>Returns the specified string with characters not representable in ASCII codepage 437 converted to a suitable representative equivalent.  Yes, this is lossy.</summary>
        /// <param name="s">A string.</param>
        /// <returns>The supplied string, with smart quotes, fractions, accents and punctuation marks 'normalized' to ASCII equivalents.</returns>
        /// <remarks>This method is lossy. It's a bit of a hack that we use to get clean ASCII text for sending to downlevel e-mail clients.</remarks>
        public static string Asciify(this string s)
        {
            return (string.Join(string.Empty, s.Select(c => Asciify(c)).ToArray()));
        }

        private static string Asciify(char x)
        {
            return Replacements.ContainsKey(x) ? (Replacements[x]) : (x.ToString());
        }

        public static string AlphaNum(this string s)
        {
            string asc = s.Asciify();

            return (string.Join(string.Empty, asc.Select(c => AlphaNum(c)).ToArray()));
        }

        private static string AlphaNum(char x)
        {
            return char.IsLetter(x) || char.IsNumber(x) || x=='_' || x=='-' ? x.ToString() : "" ;
        }

        //ISO-8859-1 == (Western European (ISO) 28591).
        //Νεκρός στη λήθη (Τα Βαμπίρ του Νότου, #4)

 
        static StringExtensions()
        {
            Replacements['’'] = "'"; // 75151 occurrences
            Replacements['–'] = "-"; // 23018 occurrences
            Replacements['‘'] = "'"; // 9783 occurrences
            Replacements['"'] = "\""; // 6938 occurrences
            Replacements['"'] = "\""; // 6165 occurrences
            Replacements['…'] = "..."; // 5547 occurrences
            Replacements['£'] = "GBP"; // 3993 occurrences
            Replacements['•'] = "*"; // 2371 occurrences
            Replacements[' '] = " "; // 1529 occurrences
            Replacements['é'] = "e"; // 878 occurrences
            Replacements['ï'] = "i"; // 328 occurrences
            Replacements['´'] = "'"; // 226 occurrences
            Replacements['—'] = "-"; // 133 occurrences
            Replacements['·'] = "*"; // 132 occurrences
            Replacements['„'] = "\""; // 102 occurrences
            Replacements['€'] = "EUR"; // 95 occurrences
            Replacements['®'] = "(R)"; // 91 occurrences
            Replacements['¹'] = "(1)"; // 80 occurrences
            Replacements['«'] = "\""; // 79 occurrences
            Replacements['è'] = "e"; // 79 occurrences
            Replacements['á'] = "a"; // 55 occurrences
            Replacements['™'] = "TM"; // 54 occurrences
            Replacements['»'] = "\""; // 52 occurrences
            Replacements['ç'] = "c"; // 52 occurrences
            Replacements['½'] = "1/2"; // 48 occurrences
            Replacements['­'] = "-"; // 39 occurrences
            Replacements['°'] = " degrees "; // 33 occurrences
            Replacements['ä'] = "a"; // 33 occurrences
            Replacements['É'] = "E"; // 31 occurrences
            Replacements['‚'] = ","; // 31 occurrences
            Replacements['ü'] = "u"; // 30 occurrences
            Replacements['í'] = "i"; // 28 occurrences
            Replacements['ë'] = "e"; // 26 occurrences
            Replacements['ö'] = "o"; // 19 occurrences
            Replacements['à'] = "a"; // 19 occurrences
            Replacements['¬'] = " "; // 17 occurrences
            Replacements['ó'] = "o"; // 15 occurrences
            Replacements['â'] = "a"; // 13 occurrences
            Replacements['ñ'] = "n"; // 13 occurrences
            Replacements['ô'] = "o"; // 10 occurrences
            Replacements['¨'] = ""; // 10 occurrences
            Replacements['å'] = "a"; // 8 occurrences
            Replacements['ã'] = "a"; // 8 occurrences
            Replacements['ˆ'] = ""; // 8 occurrences
            Replacements['©'] = "(c)"; // 6 occurrences
            Replacements['Ä'] = "A"; // 6 occurrences
            Replacements['Ï'] = "I"; // 5 occurrences
            Replacements['ò'] = "o"; // 5 occurrences
            Replacements['ê'] = "e"; // 5 occurrences
            Replacements['î'] = "i"; // 5 occurrences
            Replacements['Ü'] = "U"; // 5 occurrences
            Replacements['Á'] = "A"; // 5 occurrences
            Replacements['ß'] = "ss"; // 4 occurrences
            Replacements['¾'] = "3/4"; // 4 occurrences
            Replacements['È'] = "E"; // 4 occurrences
            Replacements['¼'] = "1/4"; // 3 occurrences
            Replacements['†'] = "+"; // 3 occurrences
            Replacements['³'] = "'"; // 3 occurrences
            Replacements['²'] = "'"; // 3 occurrences
            Replacements['Ø'] = "O"; // 2 occurrences
            Replacements['¸'] = ","; // 2 occurrences
            Replacements['Ë'] = "E"; // 2 occurrences
            Replacements['ú'] = "u"; // 2 occurrences
            Replacements['Ö'] = "O"; // 2 occurrences
            Replacements['û'] = "u"; // 2 occurrences
            Replacements['Ú'] = "U"; // 2 occurrences
            Replacements['Œ'] = "Oe"; // 2 occurrences
            Replacements['º'] = "?"; // 1 occurrences
            Replacements['‰'] = "0/00"; // 1 occurrences
            Replacements['Å'] = "A"; // 1 occurrences
            Replacements['ø'] = "o"; // 1 occurrences
            Replacements['˜'] = "~"; // 1 occurrences
            Replacements['æ'] = "ae"; // 1 occurrences
            Replacements['ù'] = "u"; // 1 occurrences
            Replacements['‹'] = "<"; // 1 occurrences
            Replacements['±'] = "+/-"; // 1 occurrences



            Replacements['\u00C0']=Replacements['\u00C1']=Replacements['\u00C2']=Replacements['\u00C3']=Replacements['\u00C4']=Replacements['\u00C5']=Replacements['\u0100']=Replacements['\u0102']=Replacements['\u0104']=Replacements['\u018F']=Replacements['\u01CD']=Replacements['\u01DE']=Replacements['\u01E0']=Replacements['\u01FA']=Replacements['\u0200']=Replacements['\u0202']=Replacements['\u0226']=Replacements['\u023A']=Replacements['\u1D00']=Replacements['\u1E00']=Replacements['\u1EA0']=Replacements['\u1EA2']=Replacements['\u1EA4']=Replacements['\u1EA6']=Replacements['\u1EA8']=Replacements['\u1EAA']=Replacements['\u1EAC']=Replacements['\u1EAE']=Replacements['\u1EB0']=Replacements['\u1EB2']=Replacements['\u1EB4']=Replacements['\u1EB6']=Replacements['\u24B6']=Replacements['\uFF21']="A";
            Replacements['\u00E0']=Replacements['\u00E1']=Replacements['\u00E2']=Replacements['\u00E3']=Replacements['\u00E4']=Replacements['\u00E5']=Replacements['\u0101']=Replacements['\u0103']=Replacements['\u0105']=Replacements['\u01CE']=Replacements['\u01DF']=Replacements['\u01E1']=Replacements['\u01FB']=Replacements['\u0201']=Replacements['\u0203']=Replacements['\u0227']=Replacements['\u0250']=Replacements['\u0259']=Replacements['\u025A']=Replacements['\u1D8F']=Replacements['\u1D95']=Replacements['\u1E01']=Replacements['\u1E9A']=Replacements['\u1EA1']=Replacements['\u1EA3']=Replacements['\u1EA5']=Replacements['\u1EA7']=Replacements['\u1EA9']=Replacements['\u1EAB']=Replacements['\u1EAD']=Replacements['\u1EAF']=Replacements['\u1EB1']=Replacements['\u1EB3']=Replacements['\u1EB5']=Replacements['\u1EB7']=Replacements['\u2090']=Replacements['\u2094']=Replacements['\u24D0']=Replacements['\u2C65']=Replacements['\u2C6F']=Replacements['\uFF41']="a";
            Replacements['\uA732']="AA";
            Replacements['\u00C6']=Replacements['\u01E2']=Replacements['\u01FC']=Replacements['\u1D01']="AE";
            Replacements['\uA734']="AO";
            Replacements['\uA736']="AU";
            Replacements['\uA738']=Replacements['\uA73A']="AV";
            Replacements['\uA73C']="AY";
            Replacements['\u249C']="(a)";
            Replacements['\uA733']="aa";
            Replacements['\u00E6']=Replacements['\u01E3']=Replacements['\u01FD']=Replacements['\u1D02']="ae";
            Replacements['\uA735']="ao";
            Replacements['\uA737']="au";
            Replacements['\uA739']=Replacements['\uA73B']="av";
            Replacements['\uA73D']="ay";
            Replacements['\u0181']=Replacements['\u0182']=Replacements['\u0243']=Replacements['\u0299']=Replacements['\u1D03']=Replacements['\u1E02']=Replacements['\u1E04']=Replacements['\u1E06']=Replacements['\u24B7']=Replacements['\uFF22']="B";
            Replacements['\u0180']=Replacements['\u0183']=Replacements['\u0253']=Replacements['\u1D6C']=Replacements['\u1D80']=Replacements['\u1E03']=Replacements['\u1E05']=Replacements['\u1E07']=Replacements['\u24D1']=Replacements['\uFF42']="b";
            Replacements['\u249D']="(b)";
            Replacements['\u00C7']=Replacements['\u0106']=Replacements['\u0108']=Replacements['\u010A']=Replacements['\u010C']=Replacements['\u0187']=Replacements['\u023B']=Replacements['\u0297']=Replacements['\u1D04']=Replacements['\u1E08']=Replacements['\u24B8']=Replacements['\uFF23']="C";
            Replacements['\u00E7']=Replacements['\u0107']=Replacements['\u0109']=Replacements['\u010B']=Replacements['\u010D']=Replacements['\u0188']=Replacements['\u023C']=Replacements['\u0255']=Replacements['\u1E09']=Replacements['\u2184']=Replacements['\u24D2']=Replacements['\uA73E']=Replacements['\uA73F']=Replacements['\uFF43']="c";
            Replacements['\u249E']="(c)";
            Replacements['\u00D0']=Replacements['\u010E']=Replacements['\u0110']=Replacements['\u0189']=Replacements['\u018A']=Replacements['\u018B']=Replacements['\u1D05']=Replacements['\u1D06']=Replacements['\u1E0A']=Replacements['\u1E0C']=Replacements['\u1E0E']=Replacements['\u1E10']=Replacements['\u1E12']=Replacements['\u24B9']=Replacements['\uA779']=Replacements['\uFF24']="D";
            Replacements['\u00F0']=Replacements['\u010F']=Replacements['\u0111']=Replacements['\u018C']=Replacements['\u0221']=Replacements['\u0256']=Replacements['\u0257']=Replacements['\u1D6D']=Replacements['\u1D81']=Replacements['\u1D91']=Replacements['\u1E0B']=Replacements['\u1E0D']=Replacements['\u1E0F']=Replacements['\u1E11']=Replacements['\u1E13']=Replacements['\u24D3']=Replacements['\uA77A']=Replacements['\uFF44']="d";
            Replacements['\u01C4']=Replacements['\u01F1']="DZ";
            Replacements['\u01C5']=Replacements['\u01F2']="Dz";
            Replacements['\u249F']="(d)";
            Replacements['\u0238']="db";
            Replacements['\u01C6']=Replacements['\u01F3']=Replacements['\u02A3']=Replacements['\u02A5']="dz";
            Replacements['\u00C8']=Replacements['\u00C9']=Replacements['\u00CA']=Replacements['\u00CB']=Replacements['\u0112']=Replacements['\u0114']=Replacements['\u0116']=Replacements['\u0118']=Replacements['\u011A']=Replacements['\u018E']=Replacements['\u0190']=Replacements['\u0204']=Replacements['\u0206']=Replacements['\u0228']=Replacements['\u0246']=Replacements['\u1D07']=Replacements['\u1E14']=Replacements['\u1E16']=Replacements['\u1E18']=Replacements['\u1E1A']=Replacements['\u1E1C']=Replacements['\u1EB8']=Replacements['\u1EBA']=Replacements['\u1EBC']=Replacements['\u1EBE']=Replacements['\u1EC0']=Replacements['\u1EC2']=Replacements['\u1EC4']=Replacements['\u1EC6']=Replacements['\u24BA']=Replacements['\u2C7B']=Replacements['\uFF25']="E";
            Replacements['\u00E8']=Replacements['\u00E9']=Replacements['\u00EA']=Replacements['\u00EB']=Replacements['\u0113']=Replacements['\u0115']=Replacements['\u0117']=Replacements['\u0119']=Replacements['\u011B']=Replacements['\u01DD']=Replacements['\u0205']=Replacements['\u0207']=Replacements['\u0229']=Replacements['\u0247']=Replacements['\u0258']=Replacements['\u025B']=Replacements['\u025C']=Replacements['\u025D']=Replacements['\u025E']=Replacements['\u029A']=Replacements['\u1D08']=Replacements['\u1D92']=Replacements['\u1D93']=Replacements['\u1D94']=Replacements['\u1E15']=Replacements['\u1E17']=Replacements['\u1E19']=Replacements['\u1E1B']=Replacements['\u1E1D']=Replacements['\u1EB9']=Replacements['\u1EBB']=Replacements['\u1EBD']=Replacements['\u1EBF']=Replacements['\u1EC1']=Replacements['\u1EC3']=Replacements['\u1EC5']=Replacements['\u1EC7']=Replacements['\u2091']=Replacements['\u24D4']=Replacements['\u2C78']=Replacements['\uFF45']="e";
            Replacements['\u24A0']="(e)";
            Replacements['\u0191']=Replacements['\u1E1E']=Replacements['\u24BB']=Replacements['\uA730']=Replacements['\uA77B']=Replacements['\uA7FB']=Replacements['\uFF26']="F";
            Replacements['\u0192']=Replacements['\u1D6E']=Replacements['\u1D82']=Replacements['\u1E1F']=Replacements['\u1E9B']=Replacements['\u24D5']=Replacements['\uA77C']=Replacements['\uFF46']="f";
            Replacements['\u24A1']="(f)";
            Replacements['\uFB00']="ff";
            Replacements['\uFB03']="ffi";
            Replacements['\uFB04']="ffl";
            Replacements['\uFB01']="fi";
            Replacements['\uFB02']="fl";
            Replacements['\u011C']=Replacements['\u011E']=Replacements['\u0120']=Replacements['\u0122']=Replacements['\u0193']=Replacements['\u01E4']=Replacements['\u01E5']=Replacements['\u01E6']=Replacements['\u01E7']=Replacements['\u01F4']=Replacements['\u0262']=Replacements['\u029B']=Replacements['\u1E20']=Replacements['\u24BC']=Replacements['\uA77D']=Replacements['\uA77E']=Replacements['\uFF27']="G";
            Replacements['\u011D']=Replacements['\u011F']=Replacements['\u0121']=Replacements['\u0123']=Replacements['\u01F5']=Replacements['\u0260']=Replacements['\u0261']=Replacements['\u1D77']=Replacements['\u1D79']=Replacements['\u1D83']=Replacements['\u1E21']=Replacements['\u24D6']=Replacements['\uA77F']=Replacements['\uFF47']="g";
            Replacements['\u24A2']="(g)";
            Replacements['\u0124']=Replacements['\u0126']=Replacements['\u021E']=Replacements['\u029C']=Replacements['\u1E22']=Replacements['\u1E24']=Replacements['\u1E26']=Replacements['\u1E28']=Replacements['\u1E2A']=Replacements['\u24BD']=Replacements['\u2C67']=Replacements['\u2C75']=Replacements['\uFF28']="H";
            Replacements['\u0125']=Replacements['\u0127']=Replacements['\u021F']=Replacements['\u0265']=Replacements['\u0266']=Replacements['\u02AE']=Replacements['\u02AF']=Replacements['\u1E23']=Replacements['\u1E25']=Replacements['\u1E27']=Replacements['\u1E29']=Replacements['\u1E2B']=Replacements['\u1E96']=Replacements['\u24D7']=Replacements['\u2C68']=Replacements['\u2C76']=Replacements['\uFF48']="h";
            Replacements['\u01F6']="HV";
            Replacements['\u24A3']="(h)";
            Replacements['\u0195']="hv";
            Replacements['\u00CC']=Replacements['\u00CD']=Replacements['\u00CE']=Replacements['\u00CF']=Replacements['\u0128']=Replacements['\u012A']=Replacements['\u012C']=Replacements['\u012E']=Replacements['\u0130']=Replacements['\u0196']=Replacements['\u0197']=Replacements['\u01CF']=Replacements['\u0208']=Replacements['\u020A']=Replacements['\u026A']=Replacements['\u1D7B']=Replacements['\u1E2C']=Replacements['\u1E2E']=Replacements['\u1EC8']=Replacements['\u1ECA']=Replacements['\u24BE']=Replacements['\uA7FE']=Replacements['\uFF29']="I";
            Replacements['\u00EC']=Replacements['\u00ED']=Replacements['\u00EE']=Replacements['\u00EF']=Replacements['\u0129']=Replacements['\u012B']=Replacements['\u012D']=Replacements['\u012F']=Replacements['\u0131']=Replacements['\u01D0']=Replacements['\u0209']=Replacements['\u020B']=Replacements['\u0268']=Replacements['\u1D09']=Replacements['\u1D62']=Replacements['\u1D7C']=Replacements['\u1D96']=Replacements['\u1E2D']=Replacements['\u1E2F']=Replacements['\u1EC9']=Replacements['\u1ECB']=Replacements['\u2071']=Replacements['\u24D8']=Replacements['\uFF49']="i";
            Replacements['\u0132']="IJ";
            Replacements['\u24A4']="(i)";
            Replacements['\u0133']="ij";
            Replacements['\u0134']=Replacements['\u0248']=Replacements['\u1D0A']=Replacements['\u24BF']=Replacements['\uFF2A']="J";
            Replacements['\u0135']=Replacements['\u01F0']=Replacements['\u0237']=Replacements['\u0249']=Replacements['\u025F']=Replacements['\u0284']=Replacements['\u029D']=Replacements['\u24D9']=Replacements['\u2C7C']=Replacements['\uFF4A']="j";
            Replacements['\u24A5']="(j)";
            Replacements['\u0136']=Replacements['\u0198']=Replacements['\u01E8']=Replacements['\u1D0B']=Replacements['\u1E30']=Replacements['\u1E32']=Replacements['\u1E34']=Replacements['\u24C0']=Replacements['\u2C69']=Replacements['\uA740']=Replacements['\uA742']=Replacements['\uA744']=Replacements['\uFF2B']="K";
            Replacements['\u0137']=Replacements['\u0199']=Replacements['\u01E9']=Replacements['\u029E']=Replacements['\u1D84']=Replacements['\u1E31']=Replacements['\u1E33']=Replacements['\u1E35']=Replacements['\u24DA']=Replacements['\u2C6A']=Replacements['\uA741']=Replacements['\uA743']=Replacements['\uA745']=Replacements['\uFF4B']="k";
            Replacements['\u24A6']="(k)";
            Replacements['\u0139']=Replacements['\u013B']=Replacements['\u013D']=Replacements['\u013F']=Replacements['\u0141']=Replacements['\u023D']=Replacements['\u029F']=Replacements['\u1D0C']=Replacements['\u1E36']=Replacements['\u1E38']=Replacements['\u1E3A']=Replacements['\u1E3C']=Replacements['\u24C1']=Replacements['\u2C60']=Replacements['\u2C62']=Replacements['\uA746']=Replacements['\uA748']=Replacements['\uA780']=Replacements['\uFF2C']="L";
            Replacements['\u013A']=Replacements['\u013C']=Replacements['\u013E']=Replacements['\u0140']=Replacements['\u0142']=Replacements['\u019A']=Replacements['\u0234']=Replacements['\u026B']=Replacements['\u026C']=Replacements['\u026D']=Replacements['\u1D85']=Replacements['\u1E37']=Replacements['\u1E39']=Replacements['\u1E3B']=Replacements['\u1E3D']=Replacements['\u24DB']=Replacements['\u2C61']=Replacements['\uA747']=Replacements['\uA749']=Replacements['\uA781']=Replacements['\uFF4C']="l";
            Replacements['\u01C7']="LJ";
            Replacements['\u1EFA']="LL";
            Replacements['\u01C8']="Lj";
            Replacements['\u24A7']="(l)";
            Replacements['\u01C9']="lj";
            Replacements['\u1EFB']="ll";
            Replacements['\u02AA']="ls";
            Replacements['\u02AB']="lz";
            Replacements['\u019C']=Replacements['\u1D0D']=Replacements['\u1E3E']=Replacements['\u1E40']=Replacements['\u1E42']=Replacements['\u24C2']=Replacements['\u2C6E']=Replacements['\uA7FD']=Replacements['\uA7FF']=Replacements['\uFF2D']="M";
            Replacements['\u026F']=Replacements['\u0270']=Replacements['\u0271']=Replacements['\u1D6F']=Replacements['\u1D86']=Replacements['\u1E3F']=Replacements['\u1E41']=Replacements['\u1E43']=Replacements['\u24DC']=Replacements['\uFF4D']="m";
            Replacements['\u24A8']="(m)";
            Replacements['\u00D1']=Replacements['\u0143']=Replacements['\u0145']=Replacements['\u0147']=Replacements['\u014A']=Replacements['\u019D']=Replacements['\u01F8']=Replacements['\u0220']=Replacements['\u0274']=Replacements['\u1D0E']=Replacements['\u1E44']=Replacements['\u1E46']=Replacements['\u1E48']=Replacements['\u1E4A']=Replacements['\u24C3']=Replacements['\uFF2E']="N";
            Replacements['\u00F1']=Replacements['\u0144']=Replacements['\u0146']=Replacements['\u0148']=Replacements['\u0149']=Replacements['\u014B']=Replacements['\u019E']=Replacements['\u01F9']=Replacements['\u0235']=Replacements['\u0272']=Replacements['\u0273']=Replacements['\u1D70']=Replacements['\u1D87']=Replacements['\u1E45']=Replacements['\u1E47']=Replacements['\u1E49']=Replacements['\u1E4B']=Replacements['\u207F']=Replacements['\u24DD']=Replacements['\uFF4E']="n";
            Replacements['\u01CA']="NJ";
            Replacements['\u01CB']="Nj";
            Replacements['\u24A9']="(n)";
            Replacements['\u01CC']="nj";
            Replacements['\u00D2']=Replacements['\u00D3']=Replacements['\u00D4']=Replacements['\u00D5']=Replacements['\u00D6']=Replacements['\u00D8']=Replacements['\u014C']=Replacements['\u014E']=Replacements['\u0150']=Replacements['\u0186']=Replacements['\u019F']=Replacements['\u01A0']=Replacements['\u01D1']=Replacements['\u01EA']=Replacements['\u01EC']=Replacements['\u01FE']=Replacements['\u020C']=Replacements['\u020E']=Replacements['\u022A']=Replacements['\u022C']=Replacements['\u022E']=Replacements['\u0230']=Replacements['\u1D0F']=Replacements['\u1D10']=Replacements['\u1E4C']=Replacements['\u1E4E']=Replacements['\u1E50']=Replacements['\u1E52']=Replacements['\u1ECC']=Replacements['\u1ECE']=Replacements['\u1ED0']=Replacements['\u1ED2']=Replacements['\u1ED4']=Replacements['\u1ED6']=Replacements['\u1ED8']=Replacements['\u1EDA']=Replacements['\u1EDC']=Replacements['\u1EDE']=Replacements['\u1EE0']=Replacements['\u1EE2']=Replacements['\u24C4']=Replacements['\uA74A']=Replacements['\uA74C']=Replacements['\uFF2F']="O";
            Replacements['\u00F2']=Replacements['\u00F3']=Replacements['\u00F4']=Replacements['\u00F5']=Replacements['\u00F6']=Replacements['\u00F8']=Replacements['\u014D']=Replacements['\u014F']=Replacements['\u0151']=Replacements['\u01A1']=Replacements['\u01D2']=Replacements['\u01EB']=Replacements['\u01ED']=Replacements['\u01FF']=Replacements['\u020D']=Replacements['\u020F']=Replacements['\u022B']=Replacements['\u022D']=Replacements['\u022F']=Replacements['\u0231']=Replacements['\u0254']=Replacements['\u0275']=Replacements['\u1D16']=Replacements['\u1D17']=Replacements['\u1D97']=Replacements['\u1E4D']=Replacements['\u1E4F']=Replacements['\u1E51']=Replacements['\u1E53']=Replacements['\u1ECD']=Replacements['\u1ECF']=Replacements['\u1ED1']=Replacements['\u1ED3']=Replacements['\u1ED5']=Replacements['\u1ED7']=Replacements['\u1ED9']=Replacements['\u1EDB']=Replacements['\u1EDD']=Replacements['\u1EDF']=Replacements['\u1EE1']=Replacements['\u1EE3']=Replacements['\u2092']=Replacements['\u24DE']=Replacements['\u2C7A']=Replacements['\uA74B']=Replacements['\uA74D']=Replacements['\uFF4F']="o";
            Replacements['\u0152']=Replacements['\u0276']="OE";
            Replacements['\uA74E']="OO";
            Replacements['\u0222']=Replacements['\u1D15']="OU";
            Replacements['\u24AA']="(o)";
            Replacements['\u0153']=Replacements['\u1D14']="oe";
            Replacements['\uA74F']="oo";
            Replacements['\u0223']="ou";
            Replacements['\u01A4']=Replacements['\u1D18']=Replacements['\u1E54']=Replacements['\u1E56']=Replacements['\u24C5']=Replacements['\u2C63']=Replacements['\uA750']=Replacements['\uA752']=Replacements['\uA754']=Replacements['\uFF30']="P";
            Replacements['\u01A5']=Replacements['\u1D71']=Replacements['\u1D7D']=Replacements['\u1D88']=Replacements['\u1E55']=Replacements['\u1E57']=Replacements['\u24DF']=Replacements['\uA751']=Replacements['\uA753']=Replacements['\uA755']=Replacements['\uA7FC']=Replacements['\uFF50']="p";
            Replacements['\u24AB']="(p)";
            Replacements['\u024A']=Replacements['\u24C6']=Replacements['\uA756']=Replacements['\uA758']=Replacements['\uFF31']="Q";
            Replacements['\u0138']=Replacements['\u024B']=Replacements['\u02A0']=Replacements['\u24E0']=Replacements['\uA757']=Replacements['\uA759']=Replacements['\uFF51']="q";
            Replacements['\u24AC']="(q)";
            Replacements['\u0239']="qp";
            Replacements['\u0154']=Replacements['\u0156']=Replacements['\u0158']=Replacements['\u0210']=Replacements['\u0212']=Replacements['\u024C']=Replacements['\u0280']=Replacements['\u0281']=Replacements['\u1D19']=Replacements['\u1D1A']=Replacements['\u1E58']=Replacements['\u1E5A']=Replacements['\u1E5C']=Replacements['\u1E5E']=Replacements['\u24C7']=Replacements['\u2C64']=Replacements['\uA75A']=Replacements['\uA782']=Replacements['\uFF32']="R";
            Replacements['\u0155']=Replacements['\u0157']=Replacements['\u0159']=Replacements['\u0211']=Replacements['\u0213']=Replacements['\u024D']=Replacements['\u027C']=Replacements['\u027D']=Replacements['\u027E']=Replacements['\u027F']=Replacements['\u1D63']=Replacements['\u1D72']=Replacements['\u1D73']=Replacements['\u1D89']=Replacements['\u1E59']=Replacements['\u1E5B']=Replacements['\u1E5D']=Replacements['\u1E5F']=Replacements['\u24E1']=Replacements['\uA75B']=Replacements['\uA783']=Replacements['\uFF52']="r";
            Replacements['\u24AD']="(r)";
            Replacements['\u015A']=Replacements['\u015C']=Replacements['\u015E']=Replacements['\u0160']=Replacements['\u0218']=Replacements['\u1E60']=Replacements['\u1E62']=Replacements['\u1E64']=Replacements['\u1E66']=Replacements['\u1E68']=Replacements['\u24C8']=Replacements['\uA731']=Replacements['\uA785']=Replacements['\uFF33']="S";
            Replacements['\u015B']=Replacements['\u015D']=Replacements['\u015F']=Replacements['\u0161']=Replacements['\u017F']=Replacements['\u0219']=Replacements['\u023F']=Replacements['\u0282']=Replacements['\u1D74']=Replacements['\u1D8A']=Replacements['\u1E61']=Replacements['\u1E63']=Replacements['\u1E65']=Replacements['\u1E67']=Replacements['\u1E69']=Replacements['\u1E9C']=Replacements['\u1E9D']=Replacements['\u24E2']=Replacements['\uA784']=Replacements['\uFF53']="s";
            Replacements['\u1E9E']="SS";
            Replacements['\u24AE']="(s)";
            Replacements['\u00DF']="ss";
            Replacements['\uFB06']="st";
            Replacements['\u0162']=Replacements['\u0164']=Replacements['\u0166']=Replacements['\u01AC']=Replacements['\u01AE']=Replacements['\u021A']=Replacements['\u023E']=Replacements['\u1D1B']=Replacements['\u1E6A']=Replacements['\u1E6C']=Replacements['\u1E6E']=Replacements['\u1E70']=Replacements['\u24C9']=Replacements['\uA786']=Replacements['\uFF34']="T";
            Replacements['\u0163']=Replacements['\u0165']=Replacements['\u0167']=Replacements['\u01AB']=Replacements['\u01AD']=Replacements['\u021B']=Replacements['\u0236']=Replacements['\u0287']=Replacements['\u0288']=Replacements['\u1D75']=Replacements['\u1E6B']=Replacements['\u1E6D']=Replacements['\u1E6F']=Replacements['\u1E71']=Replacements['\u1E97']=Replacements['\u24E3']=Replacements['\u2C66']=Replacements['\uFF54']="t";
            Replacements['\u00DE']=Replacements['\uA766']="TH";
            Replacements['\uA728']="TZ";
            Replacements['\u24AF']="(t)";
            Replacements['\u02A8']="tc";
            Replacements['\u00FE']=Replacements['\u1D7A']=Replacements['\uA767']="th";
            Replacements['\u02A6']="ts";
            Replacements['\uA729']="tz";
            Replacements['\u00D9']=Replacements['\u00DA']=Replacements['\u00DB']=Replacements['\u00DC']=Replacements['\u0168']=Replacements['\u016A']=Replacements['\u016C']=Replacements['\u016E']=Replacements['\u0170']=Replacements['\u0172']=Replacements['\u01AF']=Replacements['\u01D3']=Replacements['\u01D5']=Replacements['\u01D7']=Replacements['\u01D9']=Replacements['\u01DB']=Replacements['\u0214']=Replacements['\u0216']=Replacements['\u0244']=Replacements['\u1D1C']=Replacements['\u1D7E']=Replacements['\u1E72']=Replacements['\u1E74']=Replacements['\u1E76']=Replacements['\u1E78']=Replacements['\u1E7A']=Replacements['\u1EE4']=Replacements['\u1EE6']=Replacements['\u1EE8']=Replacements['\u1EEA']=Replacements['\u1EEC']=Replacements['\u1EEE']=Replacements['\u1EF0']=Replacements['\u24CA']=Replacements['\uFF35']="U";
            Replacements['\u00F9']=Replacements['\u00FA']=Replacements['\u00FB']=Replacements['\u00FC']=Replacements['\u0169']=Replacements['\u016B']=Replacements['\u016D']=Replacements['\u016F']=Replacements['\u0171']=Replacements['\u0173']=Replacements['\u01B0']=Replacements['\u01D4']=Replacements['\u01D6']=Replacements['\u01D8']=Replacements['\u01DA']=Replacements['\u01DC']=Replacements['\u0215']=Replacements['\u0217']=Replacements['\u0289']=Replacements['\u1D64']=Replacements['\u1D99']=Replacements['\u1E73']=Replacements['\u1E75']=Replacements['\u1E77']=Replacements['\u1E79']=Replacements['\u1E7B']=Replacements['\u1EE5']=Replacements['\u1EE7']=Replacements['\u1EE9']=Replacements['\u1EEB']=Replacements['\u1EED']=Replacements['\u1EEF']=Replacements['\u1EF1']=Replacements['\u24E4']=Replacements['\uFF55']="u";
            Replacements['\u24B0']="(u)";
            Replacements['\u1D6B']="ue";
            Replacements['\u01B2']=Replacements['\u0245']=Replacements['\u1D20']=Replacements['\u1E7C']=Replacements['\u1E7E']=Replacements['\u1EFC']=Replacements['\u24CB']=Replacements['\uA75E']=Replacements['\uA768']=Replacements['\uFF36']="V";
            Replacements['\u028B']=Replacements['\u028C']=Replacements['\u1D65']=Replacements['\u1D8C']=Replacements['\u1E7D']=Replacements['\u1E7F']=Replacements['\u24E5']=Replacements['\u2C71']=Replacements['\u2C74']=Replacements['\uA75F']=Replacements['\uFF56']="v";
            Replacements['\uA760']="VY";
            Replacements['\u24B1']="(v)";
            Replacements['\uA761']="vy";
            Replacements['\u0174']=Replacements['\u01F7']=Replacements['\u1D21']=Replacements['\u1E80']=Replacements['\u1E82']=Replacements['\u1E84']=Replacements['\u1E86']=Replacements['\u1E88']=Replacements['\u24CC']=Replacements['\u2C72']=Replacements['\uFF37']="W";
            Replacements['\u0175']=Replacements['\u01BF']=Replacements['\u028D']=Replacements['\u1E81']=Replacements['\u1E83']=Replacements['\u1E85']=Replacements['\u1E87']=Replacements['\u1E89']=Replacements['\u1E98']=Replacements['\u24E6']=Replacements['\u2C73']=Replacements['\uFF57']="w";
            Replacements['\u24B2']="(w)";
            Replacements['\u1E8A']=Replacements['\u1E8C']=Replacements['\u24CD']=Replacements['\uFF38']="X";
            Replacements['\u1D8D']=Replacements['\u1E8B']=Replacements['\u1E8D']=Replacements['\u2093']=Replacements['\u24E7']=Replacements['\uFF58']="x";
            Replacements['\u24B3']="(x)";
            Replacements['\u00DD']=Replacements['\u0176']=Replacements['\u0178']=Replacements['\u01B3']=Replacements['\u0232']=Replacements['\u024E']=Replacements['\u028F']=Replacements['\u1E8E']=Replacements['\u1EF2']=Replacements['\u1EF4']=Replacements['\u1EF6']=Replacements['\u1EF8']=Replacements['\u1EFE']=Replacements['\u24CE']=Replacements['\uFF39']="Y";
            Replacements['\u00FD']=Replacements['\u00FF']=Replacements['\u0177']=Replacements['\u01B4']=Replacements['\u0233']=Replacements['\u024F']=Replacements['\u028E']=Replacements['\u1E8F']=Replacements['\u1E99']=Replacements['\u1EF3']=Replacements['\u1EF5']=Replacements['\u1EF7']=Replacements['\u1EF9']=Replacements['\u1EFF']=Replacements['\u24E8']=Replacements['\uFF59']="y";
            Replacements['\u24B4']="(y)";
            Replacements['\u0179']=Replacements['\u017B']=Replacements['\u017D']=Replacements['\u01B5']=Replacements['\u021C']=Replacements['\u0224']=Replacements['\u1D22']=Replacements['\u1E90']=Replacements['\u1E92']=Replacements['\u1E94']=Replacements['\u24CF']=Replacements['\u2C6B']=Replacements['\uA762']=Replacements['\uFF3A']="Z";
            Replacements['\u017A']=Replacements['\u017C']=Replacements['\u017E']=Replacements['\u01B6']=Replacements['\u021D']=Replacements['\u0225']=Replacements['\u0240']=Replacements['\u0290']=Replacements['\u0291']=Replacements['\u1D76']=Replacements['\u1D8E']=Replacements['\u1E91']=Replacements['\u1E93']=Replacements['\u1E95']=Replacements['\u24E9']=Replacements['\u2C6C']=Replacements['\uA763']=Replacements['\uFF5A']="z";
            Replacements['\u24B5']="(z)";
            Replacements['\u2070']=Replacements['\u2080']=Replacements['\u24EA']=Replacements['\u24FF']=Replacements['\uFF10']="0";
            Replacements['\u00B9']=Replacements['\u2081']=Replacements['\u2460']=Replacements['\u24F5']=Replacements['\u2776']=Replacements['\u2780']=Replacements['\u278A']=Replacements['\uFF11']="1";
            Replacements['\u2488']="1.";
            Replacements['\u2474']="(1)";
            Replacements['\u00B2']=Replacements['\u2082']=Replacements['\u2461']=Replacements['\u24F6']=Replacements['\u2777']=Replacements['\u2781']=Replacements['\u278B']=Replacements['\uFF12']="2";
            Replacements['\u2489']="2.";
            Replacements['\u2475']="(2)";
            Replacements['\u00B3']=Replacements['\u2083']=Replacements['\u2462']=Replacements['\u24F7']=Replacements['\u2778']=Replacements['\u2782']=Replacements['\u278C']=Replacements['\uFF13']="3";
            Replacements['\u248A']="3.";
            Replacements['\u2476']="(3)";
            Replacements['\u2074']=Replacements['\u2084']=Replacements['\u2463']=Replacements['\u24F8']=Replacements['\u2779']=Replacements['\u2783']=Replacements['\u278D']=Replacements['\uFF14']="4";
            Replacements['\u248B']="4.";
            Replacements['\u2477']="(4)";
            Replacements['\u2075']=Replacements['\u2085']=Replacements['\u2464']=Replacements['\u24F9']=Replacements['\u277A']=Replacements['\u2784']=Replacements['\u278E']=Replacements['\uFF15']="5";
            Replacements['\u248C']="5.";
            Replacements['\u2478']="(5)";
            Replacements['\u2076']=Replacements['\u2086']=Replacements['\u2465']=Replacements['\u24FA']=Replacements['\u277B']=Replacements['\u2785']=Replacements['\u278F']=Replacements['\uFF16']="6";
            Replacements['\u248D']="6.";
            Replacements['\u2479']="(6)";
            Replacements['\u2077']=Replacements['\u2087']=Replacements['\u2466']=Replacements['\u24FB']=Replacements['\u277C']=Replacements['\u2786']=Replacements['\u2790']=Replacements['\uFF17']="7";
            Replacements['\u248E']="7.";
            Replacements['\u247A']="(7)";
            Replacements['\u2078']=Replacements['\u2088']=Replacements['\u2467']=Replacements['\u24FC']=Replacements['\u277D']=Replacements['\u2787']=Replacements['\u2791']=Replacements['\uFF18']="8";
            Replacements['\u248F']="8.";
            Replacements['\u247B']="(8)";
            Replacements['\u2079']=Replacements['\u2089']=Replacements['\u2468']=Replacements['\u24FD']=Replacements['\u277E']=Replacements['\u2788']=Replacements['\u2792']=Replacements['\uFF19']="9";
            Replacements['\u2490']="9.";
            Replacements['\u247C']="(9)";
            Replacements['\u2469']=Replacements['\u24FE']=Replacements['\u277F']=Replacements['\u2789']=Replacements['\u2793']="10";
            Replacements['\u2491']="10.";
            Replacements['\u247D']="(10)";
            Replacements['\u246A']=Replacements['\u24EB']="11";
            Replacements['\u2492']="11.";
            Replacements['\u247E']="(11)";
            Replacements['\u246B']=Replacements['\u24EC']="12";
            Replacements['\u2493']="12.";
            Replacements['\u247F']="(12)";
            Replacements['\u246C']=Replacements['\u24ED']="13";
            Replacements['\u2494']="13.";
            Replacements['\u2480']="(13)";
            Replacements['\u246D']=Replacements['\u24EE']="14";
            Replacements['\u2495']="14.";
            Replacements['\u2481']="(14)";
            Replacements['\u246E']=Replacements['\u24EF']="15";
            Replacements['\u2496']="15.";
            Replacements['\u2482']="(15)";
            Replacements['\u246F']=Replacements['\u24F0']="16";
            Replacements['\u2497']="16.";
            Replacements['\u2483']="(16)";
            Replacements['\u2470']=Replacements['\u24F1']="17";
            Replacements['\u2498']="17.";
            Replacements['\u2484']="(17)";
            Replacements['\u2471']=Replacements['\u24F2']="18";
            Replacements['\u2499']="18.";
            Replacements['\u2485']="(18)";
            Replacements['\u2472']=Replacements['\u24F3']="19";
            Replacements['\u249A']="19.";
            Replacements['\u2486']="(19)";
            Replacements['\u2473']=Replacements['\u24F4']="20";
            Replacements['\u249B']="20.";
            Replacements['\u2487']="(20)";
            Replacements['\u00AB']=Replacements['\u00BB']=Replacements['\u201C']=Replacements['\u201D']=Replacements['\u201E']=Replacements['\u2033']=Replacements['\u2036']=Replacements['\u275D']=Replacements['\u275E']=Replacements['\u276E']=Replacements['\u276F']=Replacements['\uFF02']="\"";
            Replacements['\u2018']=Replacements['\u2019']=Replacements['\u201A']=Replacements['\u201B']=Replacements['\u2032']=Replacements['\u2035']=Replacements['\u2039']=Replacements['\u203A']=Replacements['\u275B']=Replacements['\u275C']=Replacements['\uFF07']="\'";
            Replacements['\u2010']=Replacements['\u2011']=Replacements['\u2012']=Replacements['\u2013']=Replacements['\u2014']=Replacements['\u207B']=Replacements['\u208B']=Replacements['\uFF0D']="-";
            Replacements['\u2045']=Replacements['\u2772']=Replacements['\uFF3B']="[";
            Replacements['\u2046']=Replacements['\u2773']=Replacements['\uFF3D']="]";
            Replacements['\u207D']=Replacements['\u208D']=Replacements['\u2768']=Replacements['\u276A']=Replacements['\uFF08']="(";
            Replacements['\u2E28']="((";
            Replacements['\u207E']=Replacements['\u208E']=Replacements['\u2769']=Replacements['\u276B']=Replacements['\uFF09']=")";
            Replacements['\u2E29']="))";
            Replacements['\u276C']=Replacements['\u2770']=Replacements['\uFF1C']="<";
            Replacements['\u276D']=Replacements['\u2771']=Replacements['\uFF1E']=">";
            Replacements['\u2774']=Replacements['\uFF5B']="{";
            Replacements['\u2775']=Replacements['\uFF5D']="}";
            Replacements['\u207A']=Replacements['\u208A']=Replacements['\uFF0B']="+";
            Replacements['\u207C']=Replacements['\u208C']=Replacements['\uFF1D']="=";
            Replacements['\uFF01']="!";
            Replacements['\u203C']="!!";
            Replacements['\u2049']="!?";
            Replacements['\uFF03']="#";
            Replacements['\uFF04']="$";
            Replacements['\u2052']=Replacements['\uFF05']="%";
            Replacements['\uFF06']="&";
            Replacements['\u204E']=Replacements['\uFF0A']="*";
            Replacements['\uFF0C']=",";
            Replacements['\uFF0E']=".";
            Replacements['\u2044']=Replacements['\uFF0F']="/";
            Replacements['\uFF1A']="]=";
            Replacements['\u204F']=Replacements['\uFF1B']=";";
            Replacements['\uFF1F']="?";
            Replacements['\u2047']="??";
            Replacements['\u2048']="?!";
            Replacements['\uFF20']="@";
            Replacements['\uFF3C']="\\";
            Replacements['\u2038']=Replacements['\uFF3E']="^";
            Replacements['\uFF3F']="_";
            Replacements['\u2053']=Replacements['\uFF5E']="~";

        }
    }





