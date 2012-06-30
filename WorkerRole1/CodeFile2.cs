using System;
using System.Collections.Generic;


class NameSplitter()
{

    List<string> salutations = new List<string>() {
    "hon.","judge","dr","doctor","miss","fr","misses","mr","mister","mrs","ms","sir",
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
    "prof","professor","freiherr","Dame","bishop","baroness","baron","count",
    "countess","duke","dutchess","graf","gräfin","margrave","margravine","earl","marquis"};

    List<string> preficies = new List<string>() {
    "abu","bon","ben","bin","da","dal","de","del","della","der","de","di","dí","du","e","ibn",
    "la","le","pietro","san","st","ste","ter","van","vel","von","vere","vanden"};

     List<string> suffices = new List<string>() { 
     "esq","esquire","jr","sr","i","ii","iii","iv","v","clu","chfc","cfp","md","phd","cpa",
     "senior","Junior","apr","rph","pe","ma","dmd","cme","dds"};

    List<string> conjunctions = new List<string>() {"&", "and", "et", "e", "und", "y"};

}


 
 Sr.
 Sir
 Saint
 Prof.
 Pharmd
 Ph.D., Nd
 Ph.d.
 Ph.d
 Nd
 Md., Ph.D.
 Md.
 M.D., Ph.D.
 M.D.
 M. D.
 Jr., Ph.d.
 Jr., M.D.
 Jr.
 J.
 IV
 III, Ph.D.
 III
 II
 Freiherr von
 Esq.
 Duchess of
 Dr.
 Dame
 D.
 Cardinal
 Bishop of Hippo
 Baroness
 baron de
 Baron


 

])
CAPITALIZATION_EXCEPTIONS = (
    ("ii" ,"II"),
    ("iii","III"),
    ("iv" ,"IV"),
    ("md" ,"M.D."),
    ("phd","Ph.D."),
)
CONJUNCTIONS = set([])

(NA)

(ADP)
(AFT)
(ART)
(COM)
(CON)
(COP)
(COR)
(CRT)
(DST)
(EDT)
(FRW)
(ILT)
(INT)
(NRT)
(PHT)
(PRD)
(RTL)
(TRN)
(ADP)
(AFT)
(ART)
(COM)
(CON)
(COP)
(COR)
(CRT)
(DST)
(EDT)
(FRW)
(ILT)
(INT)
(NRT)
(PHT)
(PRD)
(RTL)
(TRN)
