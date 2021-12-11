using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

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
                GenerateFile(this.dico, pathDictionnary);
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
                        string motNormalise = Normalise(mot);
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
                System.Console.WriteLine($"Fichier {path} écrit !");
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

        static void Main(string[] args)
        {
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
        }


        public static string Normalise(string Xmot)
        {
            return Xmot.Replace(",", "").Replace(";", "").Replace(" ", "").Replace(".", "").Replace("=", "").Replace("-", "").Replace("\"", "");
        }
    }
}