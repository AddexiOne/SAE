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
        static void Main(string[] args)
        {
            List<President> presidents = Start();
        }

        public static Dictionary<string, int> racines(Dictionary<string, int> init){
            string path = "../../static/hintsfiles/step1.txt";
            Dictionary<string, int> res = new Dictionary<string, int>();
            bool test;
            Dictionary<string, string> terminaison = new Dictionary<string, string>();
            if(File.Exists(path)){
                // Compare the size of the radical compared to the word we are looking at
                //Open the connection to the file
                StreamReader sr = new StreamReader(path);
                string line;
                string endOfWord;
                string replacement;
                while((line = sr.ReadLine())!= null){
                    endOfWord = line.Split(' ')[1];
                    replacement = line.Split(' ')[2];
                    if(!terminaison.ContainsKey(endOfWord)){
                        terminaison.Add(endOfWord, replacement);
                    }
                }
                foreach(KeyValuePair<string, int> kvp in init){
                    //Delete de s at the end of the word if there is one
                    test = false; 
                    string winit = kvp.Key;
                    if(winit.Length >0){
                        if(winit[winit.Length-1]=='s'){
                            winit = Reverse(Reverse(winit).Substring(1));
                        }
                    }
                    
                    foreach(KeyValuePair<string, string> kvpF in terminaison.OrderBy(key => key.Key)){
                        string wout = kvpF.Key;
                        //if winit is bigger thant wout
                        if(winit.Length > wout.Length){
                            if(winit.Substring(winit.Length-wout.Length)==wout){
                                string final = Reverse(Reverse(kvp.Key).Substring(wout.Length));
                                if(kvpF.Value == "epsilon"){
                                    if(res.ContainsKey(final)){
                                        res[final] += init[kvp.Key];
                                    }
                                    else{
                                        res.Add(final, kvp.Value);
                                    }
                                    break;
                                }
                                else{
                                    final += kvpF.Value;
                                    if(res.ContainsKey(final)){
                                        res[final] += init[kvp.Key];
                                    }
                                    else{
                                        res.Add(final, kvp.Value);
                                    }
                                    break;
                                }

                            }
                        }
                    }
                    
                }
                
            }
            else System.Console.WriteLine("nope");
            return res;
        }

        public static string Reverse(string s){
            string res = "";
            if(s.Length>0){
            for(int i=s.Length-1; i>=0; i--){
                res += s[i];
            }
            }
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
            return Xmot.Replace(",", "").Replace(";", "").Replace(" ", "").Replace(".", "").Replace("=", "").Replace("-", "").Replace("\"", "");
        }
    }
}
