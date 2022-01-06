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
         public struct Mot
        {
            public string clean { get; set; }
            // version of the word that'll be modified, cleaned.
            public string raw { get; set; }
            // raw version of the word, no formating or so ever
            public int nbOcc { get; set; }
            // number of appearences of a said <Mot>
            public Mot(string raw, string clean, int nbOcc)
            {
                /*
                    Constructor that instanciate and asign values to the <Mot>'s variables
                */
                this.clean = clean;
                this.raw = Normalise(raw);
                this.nbOcc = nbOcc;
            }
        }
        public struct Discour
        {
            public string pathSpeech { get; }
            public string pathDictionnary { get; }
            public Dictionary<string, int> dico { get; set; }
            public int annee { get; }
            public Discour(string president, int annee)
            {
                this.annee = annee;
                this.pathSpeech = PATHDISCOURS + president + "/" + annee + EXTENSIONTXT;
                this.pathDictionnary = PATHRESULT + president + "/" + annee + EXTENSIONCSV;
                this.dico = GenerateDictionary(this.pathSpeech);
                GenerateFile(racines(supprimeVide(this.dico)), pathDictionnary);
            }

            public override string ToString()
            {
                return this.pathSpeech.Split('/')[pathSpeech.Split('/').Length - 1];
            }

            public static Dictionary<string, int> GenerateDictionary(string path)
            {
                StreamReader sr = new StreamReader(path);
                Dictionary<string, int> dico = new Dictionary<string, int>();
                string ligne;
                while ((ligne = sr.ReadLine()) != null)
                {
                    foreach (string mot in ligne.Split(' '))
                    {
                        string motNormalise = Normalise(mot).ToLower();
                        if (!dico.ContainsKey(motNormalise))
                        {
                            dico.Add(motNormalise, 1);
                        }
                        else
                        {
                            dico[motNormalise] += 1;
                        }
                    }
                }
                return dico;
            }
            public static void GenerateFile(Dictionary<string, int> XdicoDiscours, string path)
            {
                // Création de l'objet StreamWriter pour écrire dans les fichiers:
                string toBeWritten = "";
                foreach (KeyValuePair<string, int> kvp in XdicoDiscours)
                {
                    // Ecriture de la clef suivie de sa valeur séparé par une virgule
                    if (kvp.Key != "")
                    {
                        toBeWritten += $"{kvp.Key.ToLower()},{kvp.Value}\n";
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
                        Discour d = new Discour(s, annee);
                        president.listSpeeches.Add(d);
                    }
                    annee++;
                }
                presidents.Add(president);
            }
            return presidents;
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
        static void Main(string[] args)
        {
            List<President> presidents = Start();
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
        public static Dictionary<string, int> racines(Dictionary<string,int> init){
            Dictionary<string,int> res = Copy(init);
            for(int i=1; i<=3; i++){
                res = racinesPath(res, i);
            }
            return res;
        }
        public static Dictionary<string, int> racinesPath(Dictionary<string, int> init, int nu)
        {
            string path = "../../static/hintsfiles/step" + nu + ".txt";
            Dictionary<string, int> res = new Dictionary<string, int>();
            res = Copy(init);
            List<Terminaison> terminaisons = generateListTerminaison(path);
         
            //We read the dictionary init, and we delete the suffix if it matches with one contained in <terminaisons>
            foreach (KeyValuePair<string, int> kvp in init)
            {
                bool wordModified = false;
                // We browse the whole list containing the terminaison
                // If the word has not been modified we keep browsing the List
                for (int i = 0; i < terminaisons.Count && !wordModified; i++)
                {
                    // If the length of the current word is greater than the current terminaison's suffixe
                    if (kvp.Key.Length > terminaisons[i].suffixe.Length)
                    {
                        // termKey = end of current Key word
                        string termKey = kvp.Key.Substring(kvp.Key.Length - terminaisons[i].suffixe.Length);
                        if (termKey == terminaisons[i].suffixe)
                        {
                            res.Remove(kvp.Key);
                            // declaration of the string that'll contain the radical
                            string radical = "";
                            if (terminaisons[i].remplacement == "epsilon"){
                                radical += kvp.Key.Substring(0, kvp.Key.Length - terminaisons[i].suffixe.Length);
                            }
                            else{
                                radical += kvp.Key.Substring(0, kvp.Key.Length - terminaisons[i].suffixe.Length);
                                radical += terminaisons[i].remplacement;
                            }
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
        public static Dictionary<string, int> Copy(Dictionary<string, int> init){
            Dictionary<string, int> res = new Dictionary<string, int>();
            foreach(KeyValuePair<string, int> kvp in init){
                res.Add(kvp.Key, kvp.Value);
            }
            return res;
        }
        public static Dictionary<string, int> supprimeVide(Dictionary<string, int> init){
            string path = "../../static/hintsfiles/mot_vide.txt";
            List<string> listWords = new List<string>();
            Dictionary<string, int> res = Copy(init);
            StreamReader sr = new StreamReader(path);
            string line;
            while((line=sr.ReadLine())!=null){
                listWords.Add(line);
            }

            foreach(string word in listWords){
                foreach(KeyValuePair<string, int> kvp in init){
                    if(word == kvp.Key){
                        res.Remove(kvp.Key);
                    }
                }
            }
            return res;
        }
        public static string Normalise(string Xmot)
        {
            string temp = Xmot;
            string res = "";
            foreach (char c in temp)
            {
                if(c == '\'') res = "";
                else if (Majuscule(c)) res += c;
                else if (Minuscule(c)) res += c;
                else res += "";
            }
            return res.ToLower();
        }
        public static bool Majuscule(char c)
        {
            bool res = false;
            if ((int)c >= (int)'A' && (int)c <= (int)'Z') res = true;
            return res;
        }
        public static bool Minuscule(char c)
        {
            bool res = false;
            if ((int)c >= (int)'a' && (int)c <= (int)'z') res = true;
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
