using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace main
{
    public class project
    {
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
            List<President> presidents = Start();
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
            return Xmot.Substring(Xmot.IndexOf('\'') < 0 ? 0 : Xmot.IndexOf('\'')).Replace(":","").Replace("?","").Replace("'","").Replace(",", "").Replace(";", "").Replace(" ", "").Replace(".", "").Replace("=", "").Replace("-", "").Replace("_", "").Replace("ç","").ToLower();
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
    }
}
