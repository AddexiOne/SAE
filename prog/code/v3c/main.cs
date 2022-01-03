using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace main
{
    public class project
    {
        const string PATHRES = "../../../web/html/results/";
        public const string PATHDISCOURS = "./../../static/";
        public const string EXTENSIONTXT = ".txt";
        public const string PATHRESULT = "./results/";
        public const string EXTENSIONCSV = ".csv";
        public struct Discour
        {
            public string pathSpeech { get; }
            public string pathDictionnaryRaw { get; }
            public string pathDictionnaryClean { get; }
            public List<Mot> dico { get; set; }
            public int annee { get; }
            public Discour(string president, int annee)
            {
                this.annee = annee;
                this.pathSpeech = PATHDISCOURS + president + "/" + annee + EXTENSIONTXT;
                this.pathDictionnaryRaw = PATHRESULT + president + "/" + "RAW" + "/" + annee + EXTENSIONCSV;
                this.pathDictionnaryClean = PATHRESULT + president +"/" +  "CLEAN" + "/" + annee + EXTENSIONCSV;
                this.dico = GenerateList(this.pathSpeech);
                GenerateFileR(racines(supprimeVide(this.dico)), this.pathDictionnaryRaw);
                GenerateFileC(racines(supprimeVide(this.dico)), this.pathDictionnaryClean);
            }

            public override string ToString()
            {
                return this.pathSpeech.Split('/')[pathSpeech.Split('/').Length - 1];
            }

            public static List<Mot> GenerateList(string path)
            {
                StreamReader sr = new StreamReader(path);
                List<Mot> liste = new List<Mot>();
                string ligne;
                while ((ligne = sr.ReadLine()) != null)
                {
                    foreach (string mot in ligne.Split(' '))
                    {
                        string motNormalise = Normalise(mot).ToLower();
                        Mot m = new Mot(mot, motNormalise, 1);
                        if (!liste.Contains(m))
                        {
                            liste.Add(m);
                        }
                        else
                        {
                            Mot rm = new Mot(m.raw, m.clean, m.nbOcc+1);
                            int indexx = liste.IndexOf(m);
                            if(indexx > 0){
                                 liste.RemoveAt(indexx);
                            }
                            liste.Add(rm);
                        }
                    }
                }
                return liste;
            }
            public static void GenerateFileR(List<Mot> XListDiscours, string path)
            {
                // Création de l'objet StreamWriter pour écrire dans les fichiers:
                string toBeWritten = "";
                foreach (Mot kvp in XListDiscours)
                {
                    // Ecriture de la clef suivie de sa valeur séparé par une virgule
                    if (kvp.clean != "")
                    {
                        toBeWritten += $"{Normalise(kvp.raw.ToLower())},{kvp.nbOcc}\n";
                    }
                }
                File.WriteAllText(path, toBeWritten);
            }
            public static void GenerateFileC(List<Mot> XListDiscours, string path)
            {
                // Création de l'objet StreamWriter pour écrire dans les fichiers:
                string toBeWritten = "";
                foreach (Mot kvp in XListDiscours)
                {
                    // Ecriture de la clef suivie de sa valeur séparé par une virgule
                    if (kvp.clean != "")
                    {
                        toBeWritten += $"{Normalise(kvp.clean.ToLower())},{kvp.nbOcc}\n";
                    }
                }
                File.WriteAllText(path, toBeWritten);
            }
        }
        public struct President
        {
            public string namePresident;
            public List<Discour> listSpeeches;

            public President(string nom)
            {
                this.namePresident = nom;
                this.listSpeeches = new List<Discour>();
            }

            public override string ToString()
            {
                string res = $"{namePresident} a fait :\n";
                foreach (Discour d in listSpeeches)
                {
                    res += d.ToString() + "\n";
                }
                return res;
            }
        }
        public struct Terminaison{
            public string suffixe {get;set;}
            public string remplacement{get;set;}
            public int regle {get;set;}
            public Terminaison(int regle, string suff, string rempl){
                this.regle = regle;
                this.suffixe =suff;
                this.remplacement = rempl;
            }
        }
        public struct Mot{
            public string clean {get; set;}
            public string raw {get; set;}
            public int nbOcc {get;set;}
            public Mot(string raw, string clean, int nbOcc){
                this.clean = clean;
                this.raw = raw;
                this.nbOcc = nbOcc;
            }

        }
        public static List<President> Start(){
            /*
                Liste de presidents : Pour chaque president connu dans listPresidents,
                je créé un president, et je lui ajoute tout ses discours
            */
            List<President> presidents = new List<President>();
            List<string> listPresidents = new List<string>() { "GISCARDDESTAING", "MITTERAND", "CHIRAC", "SARKOZY", "HOLLANDE", "MACRON" };

            // Création d'une variable comptant les années
            int annee = 1974;
            //Parcours de la liste des présidents afin de chercher les textes présents dans le répertoire static
            foreach (string s in listPresidents)
            {
                var president = new President(s);
                for (int i = 0; i <= Directory.GetFiles(PATHDISCOURS + s + '/').Length; i++)
                {
                    if (File.Exists(PATHDISCOURS + s + '/' + annee + EXTENSIONTXT))
                    {
                        if(!Directory.Exists(PATHRESULT+s)){
                            Directory.CreateDirectory(PATHRESULT+s);
                        }
                        if(!Directory.Exists(PATHRESULT+s+"RAW")){
                            Directory.CreateDirectory(PATHRESULT+s+"/RAW");
                            Directory.CreateDirectory(PATHRESULT+s+"/CLEAN");
                        }
                        Discour d = new Discour(s, annee);
                        president.listSpeeches.Add(d);
                    }
                    annee++;
                }
                presidents.Add(president);
            }
            return presidents;
        } 
        static void Main(string[] args)
        {
            // List<President> presidents = Start();
            GenerateHTMLFiles();
        }
        public static List<Mot> racines(List<Mot> init){
            List<Mot> res = Copy(init);
            for(int i=1; i<=3; i++){
                res = racinesPath(res, i);
            }
            return res;
        }
        public static List<Terminaison> generateListTerminaison(string path){
            List<Terminaison> result = new List<Terminaison>();        
            if(File.Exists(path)){
                StreamReader sr = new StreamReader(path);
                string line;
                 /*
                We read the whole file and we keep all the suffix in a List containing "struct<Terminsaison> t"
                */
                while ((line = sr.ReadLine()) != null)
                {
                    Terminaison t = new Terminaison(int.Parse(line.Split(' ')[0]),line.Split(' ')[1], line.Split(' ')[2]);
                    result.Add(t);
                }
            }
            return result;
        }
        public static List<Mot> racinesPath(List<Mot> init, int nu)
        {
            string path = "../../static/hintsfiles/step" + nu + ".txt";
            List<Mot> res = new List<Mot>();
            res = Copy(init);
            List<Terminaison> terminaisons = generateListTerminaison(path);
         
            //We read the dictionary init, and we delete the suffix if it matches with one contained in <terminaisons>
            foreach (Mot kvp in init)
            {
                bool wordModified = false;
                // We browse the whole list containing the terminaison
                // If the word has not been modified we keep browsing the List
                for (int i = 0; i < terminaisons.Count && !wordModified; i++)
                {
                    // If the length of the current word is greater than the current terminaison's suffixe
                    if (kvp.clean.Length > terminaisons[i].suffixe.Length)
                    {
                        // termKey = end of current Key word
                        string termKey = kvp.clean.Substring(kvp.clean.Length - terminaisons[i].suffixe.Length);
                        if (termKey == terminaisons[i].suffixe)
                        {
                            int index = res.IndexOf(kvp);
                            if(index > 0){
                                res.RemoveAt(index);
                            }
                            // declaration of the string that'll contain the radical
                            string radical = "";
                            if (terminaisons[i].remplacement == "epsilon"){
                                radical += kvp.clean.Substring(0, kvp.clean.Length - terminaisons[i].suffixe.Length);
                            }
                            else{
                                radical += kvp.clean.Substring(0, kvp.clean.Length - terminaisons[i].suffixe.Length);
                                radical += terminaisons[i].remplacement;
                            }
                            Mot m = new Mot(kvp.raw, radical, kvp.nbOcc);
                            // Now we verify that the radical obtained is valid with its rule
                            if(isValidRadical(radical, terminaisons[i].regle)){
                                // Now with the supposed right radical, we add it into the dictionary that contains every radical
                                // If the word is already contained, we add the current radical's number of occurences to the dictionnary's number of occurences
                                if(res.Contains(m)){
                                    Mot rm = new Mot(m.raw, m.clean, kvp.nbOcc+res[res.IndexOf(m)].nbOcc);
                                    int indexx = res.IndexOf(m);
                                    if(indexx > 0){
                                        res.RemoveAt(indexx);
                                        res.Add(rm);
                                    }
                                }
                                // Else we add the radical to the dictionnary
                                else{
                                    
                                    res.Add(new Mot(m.raw, m.clean, 1));
                                }  
                            }
                            else{
                                int indexx = res.IndexOf(kvp);
                                if(indexx > 0){
                                    res.RemoveAt(indexx);
                                }
                            }
                            // We make sure we dont modify again the word with this boolean that tells the program not to everproceed in this for loop with this special word
                            wordModified = true;
                        }
                    }
                }
            }
            return Sort(Finalise(res));
        }
        public static List<Mot> Finalise(List<Mot> init){
            // We declare the result Dictionary at this state so we can keep an eye on what we are working on
            Dictionary<string, Mot> tempD = new Dictionary<string, Mot>();
            List<Mot> result = new List<Mot>();
            // We have a list of <Mot> that does not handle duplicates. Indeed, the structure of the struct can't define and compare 2 similar cleaned word as their number of appearances and raw version isnt the same
            // So now we create a dictionnary containing all words without duplicates by adding them to a Dictionary and giving them the right nimber of appearnces and so on
            // So we now browe the List looking for new words and duplicates
            foreach(Mot word in init){
                // If the result contains the cleaned word, we add the occurences
                if(tempD.ContainsKey(word.clean)){
                    Mot temp = new Mot(word.raw, word.clean, word.nbOcc + tempD[word.clean].nbOcc);
                    tempD[word.clean] = temp;
                }
                else{
                    tempD.Add(word.clean, new Mot(word.raw, word.clean, word.nbOcc));
                }
            }
            foreach(KeyValuePair<string, Mot> kvp in tempD){
                result.Add(kvp.Value);
            }
            return result;
        }

        public static List<Mot> Sort(List<Mot> init){
            List<Mot> alea_tri =new List<Mot>();
            List<Mot> alea= new List<Mot>();
            int i;

            alea=Clone(init);
            alea_tri.Add(alea[0]);
            alea.RemoveAt(0);

            while(alea.Count >0)
            {
                i=0;
                while( (i<alea_tri.Count) && alea[0].nbOcc>alea_tri[i].nbOcc)
                {
                    i++;
                }

            alea_tri.Insert(i, alea[0]);
            alea.RemoveAt(0);
            }
            List<Mot> res = new List<Mot>();
            foreach(Mot m in alea_tri){
                res.Insert(0, m);
            }
            return res;
        }

        public static List<Mot> Clone (List<Mot> init){
            List<Mot> res = new List<Mot>();
            foreach(Mot m in init)
            {
                res.Add(m);
            }
            return res;
        }
        public static bool isValidRadical(string radical, int rule){
            List<string> VCs = new List<string>();
            bool res = true;
            bool voyellePres = false;
            string suiteVC = "";
            for(int i=0; i<radical.Length; i++){
                if(estVoyelle(radical[i])){
                    VCs.Add(suiteVC);
                    suiteVC = radical[i] + "";
                    voyellePres = true;
                }
                else{
                    suiteVC += radical[i];
                }
            }
            if(voyellePres){
                VCs.Add(suiteVC);    
            }
            else{
                res = false;
            }
            if(VCs.Count < rule) res = false;
            return res;
        }
        public static List<Mot> supprimeVide(List<Mot> init){
            string path = "../../static/hintsfiles/mot_vide.txt";
            List<string> listWords = new List<string>();
            List<Mot> res = Copy(init);
            StreamReader sr = new StreamReader(path);
            string line;
            while((line=sr.ReadLine())!=null){
                listWords.Add(line);
            }

            foreach(string word in listWords){
                foreach(Mot kvp in init){
                    if(word == kvp.clean){
                        int index = res.IndexOf(kvp);
                        if(index > 0){
                            res.RemoveAt(index);
                        }
                    }
                }
            }
            return res;
        }
        public static bool estVoyelle(char c){
            List<char> voyelles = new List<char>(){'a','e','i','o','u','y'};
            bool test = false;
            for(int i=0; i<voyelles.Count && test==false; i++){
                if(voyelles[i]==c){
                    test = true;
                }
            }
            return test;
        }
        public static string Normalise(string Xmot)
        {
            return Xmot.Substring(Xmot.IndexOf('\'') < 0 ? 0 : Xmot.IndexOf('\'')).Replace(":","").Replace("?","").Replace("'","").Replace(",", "").Replace(";", "").Replace(" ", "").Replace(".", "").Replace("=", "").Replace("-", "").Replace("_", "").Replace("ç","").Replace("«", "").Replace("»", "").ToLower();
        }
        public static List<Mot> Copy(List<Mot> init1)
        {
            List<Mot> res = new List<Mot>();
            foreach (Mot kvp in init1)
            {
                res.Add(new Mot(kvp.raw, kvp.clean, kvp.nbOcc));
            }
            return res;
        }

        public static void GenerateHTMLFiles(){
            string[] fpath = Directory.GetDirectories("results/");
		    foreach(string b in fpath){
                string pathres = PATHRES + b.Split('/')[b.Split('/').Length-1];
                // System.Console.WriteLine(pathres);
                if(!Directory.Exists(pathres)){
                    Directory.CreateDirectory(pathres);
                    // System.Console.WriteLine("cree");
                }
                foreach(string d in Directory.GetDirectories(b)){
                    //Directory CLEAN/RAW
                    if(d.Split('/')[d.Split('/').Length-1]=="RAW"){
                        foreach(string sd in Directory.GetFiles(d)){
                            string file =pathres + '/' + (sd.Split('/')[sd.Split('/').Length-1]).Split('.')[0] + ".html";
                            // System.Console.WriteLine("b:"+b);
                            StreamReader sr = new StreamReader("../../../web/html/results/squelet.html");
                            string resultatFinal = "";
                            string line;
                            while((line = sr.ReadLine()) != null){
                                resultatFinal += line;
                                string classe = "word-cloud";
                                string linkTag = "nav-link";
                                string infosD = "name-date";
                                if(contient(line, linkTag)){
                                    resultatFinal += writeLinks(PATHRES, fpath);
                                }
                                if(contient(line, classe)){
                                    StreamReader srRes = new StreamReader(sd);
                                    string l2;
                                    for(int compt = 0; compt<=10; compt++){
                                        l2 = srRes.ReadLine();
                                        string motTBW = l2.Split(',')[0];
                                        resultatFinal += "<div class=\"cloud\" id=\""+compt+"\">" + motTBW + "</div>\n"; 
                                    }						
                                }
                                if(contient(line, infosD)){
                                    resultatFinal += pathres.Split('/')[pathres.Split('/').Length-1] + ' ' + (sd.Split('/')[sd.Split('/').Length-1]).Split('.')[0]; 
                                }
                                resultatFinal += "\n";
                                // if(classe == line.Substring(premierOcc(line, '\"'), classe.Length))
                                // System.Console.WriteLine(line);
                            }
                            File.WriteAllText(file, resultatFinal);
                        }
                    }
			    }
            }
        }
        public static string writeLinks(string webFile, string[] files){
            string res = "<h2>Navigation</h2>\n";
            res += "<a href=\"../../index.html\">Acceuil</a>\n";

            //Creation of the UL :
            foreach(string president in files){
                string pres = president.Split('/')[president.Split('/').Length-1];
                res += "<ul class=\"president\">" + pres;
                // System.Console.WriteLine(pres);
                string[] fichier = Directory.GetFiles("results/"+pres + "/CLEAN");
                if(fichier.Length > 2){
                fichier = Trie(fichier);
                for(int i=0; i<fichier.Length; i++){
                    res += "<li class=\"link\"><a href=\"../" +pres+ "/" + (fichier[i].Split('/')[fichier[i].Split('/').Length-1]).Split('.')[0]  +".html\">"+(fichier[i].Split('/')[fichier[i].Split('/').Length-1]).Split('.')[0]+"</a></li>\n";
                }
                res+= "</ul>";
                }
            }
            // System.Console.WriteLine(res);
		    return res;
	    }
        public static bool contient(string line, string contained){
            bool test = false;
            for(int i=0; i<line.Length-(contained.Length-1) && !test; i++){
                if(line.Substring(i, contained.Length) == contained) test = true;
            }
            return test;
	    }

        public static string[] Trie(string[] init){
            System.Console.WriteLine("ok");
            List<int> temp = new List<int>();
            foreach(string s in init){
                System.Console.WriteLine("2ok");
                temp.Add(int.Parse(s.Split('/')[s.Split('/').Length-1].Split('.')[0]));
                System.Console.WriteLine("3ok");
            }
            //Tri
            int n = temp.Count-1;
            for(int i=2; i<=n; i++){
                int v = temp[i];
                int j=i;
                while(temp[j-1]>v){
                    temp[j] = temp[j-1];
                    j=j-1;
                }
                temp[j] = v;
            }
            foreach(int m in temp) System.Console.WriteLine(m);
            for(int k=0; k<temp.Count; k++){
                init[k] = temp[temp.Count-k] + ".html";
            }
            return init;
        }
    }
}
