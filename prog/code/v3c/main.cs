using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace main
{
    public class project
    {
        const string PATHRES = "../../../web/html/results/";
        public const string PATHDISCOURS = "./../../static/";
        public const string EXTENSIONTXT = ".txt";
        public const string relativePathResultByPresidentULT = "./results/";
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
                this.pathDictionnaryRaw = relativePathResultByPresidentULT + president + "/" + "RAW" + "/" + annee + EXTENSIONCSV;
                this.pathDictionnaryClean = relativePathResultByPresidentULT + president + "/" + "CLEAN" + "/" + annee + EXTENSIONCSV;
                this.dico = GenerateList(this.pathSpeech);
                GenerateFileR(racines(supprimeVide(this.dico)), this.pathDictionnaryRaw);
                GenerateFileC(racines(supprimeVide(this.dico)), this.pathDictionnaryClean);
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
                        Mot m = new Mot(mot, Normalise(mot), 1);
                        if (!liste.Contains(m))
                        {
                            liste.Add(m);
                        }
                        else
                        {
                            Mot rm = new Mot(m.raw, m.clean, m.nbOcc + 1);
                            int indexx = liste.IndexOf(m);
                            if (indexx > 0)
                            {
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
                        toBeWritten += $"{NormaliseClean(kvp.raw.ToLower())},{kvp.nbOcc}\n";
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
                        toBeWritten += $"{Normalise(kvp.clean)},{kvp.nbOcc}\n";
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
        }
        public struct Terminaison
        {
            public string suffixe { get; set; }
            public string remplacement { get; set; }
            public int regle { get; set; }
            public Terminaison(int regle, string suff, string rempl)
            {
                this.regle = regle;
                this.suffixe = suff;
                this.remplacement = rempl;
            }
        }
        public struct Mot
        {
            public string clean { get; set; }
            public string raw { get; set; }
            public int nbOcc { get; set; }
            public Mot(string raw, string clean, int nbOcc)
            {
                this.clean = clean;
                this.raw = raw;
                this.nbOcc = nbOcc;
            }
        }
        public static void Start()
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
                        if (!Directory.Exists(relativePathResultByPresidentULT + s))
                        {
                            Directory.CreateDirectory(relativePathResultByPresidentULT + s);
                        }
                        if (!Directory.Exists(relativePathResultByPresidentULT + s + "RAW"))
                        {
                            Directory.CreateDirectory(relativePathResultByPresidentULT + s + "/RAW");
                            Directory.CreateDirectory(relativePathResultByPresidentULT + s + "/CLEAN");
                        }
                        Discour d = new Discour(s, annee);
                        president.listSpeeches.Add(d);
                    }
                    annee++;
                }
                presidents.Add(president);
            }
        }
        static void Main(string[] args)
        {
            System.Console.WriteLine("Traitement des textes");
            // Start();
            System.Console.WriteLine("Génération / Modification des fichiers HTML");
            GenerateHTMLFiles();
            System.Console.WriteLine("modification du fichier CSS");
            Modify_CSS();
        }
        public static List<Mot> racines(List<Mot> init)
        {
            List<Mot> res = Copy(init);
            for (int i = 1; i <= 3; i++)
            {
                res = racinesPath(res, i);
            }
            return res;
        }
        public static List<Terminaison> generateListTerminaison(string path)
        {
            List<Terminaison> result = new List<Terminaison>();
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path);
                string line;
                /*
               We read the whole file and we keep all the suffix in a List containing "struct<Terminsaison> t"
               */
                while ((line = sr.ReadLine()) != null)
                {
                    Terminaison t = new Terminaison(int.Parse(line.Split(' ')[0]), line.Split(' ')[1], line.Split(' ')[2]);
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
                            if (index > 0)
                            {
                                res.RemoveAt(index);
                            }
                            // declaration of the string that'll contain the radical
                            string radical = "";
                            if (terminaisons[i].remplacement == "epsilon")
                            {
                                radical += kvp.clean.Substring(0, kvp.clean.Length - terminaisons[i].suffixe.Length);
                            }
                            else
                            {
                                radical += kvp.clean.Substring(0, kvp.clean.Length - terminaisons[i].suffixe.Length);
                                radical += terminaisons[i].remplacement;
                            }
                            Mot m = new Mot(kvp.raw, radical, kvp.nbOcc);
                            // Now we verify that the radical obtained is valid with its rule
                            if (isValidRadical(radical, terminaisons[i].regle))
                            {
                                // Now with the supposed right radical, we add it into the dictionary that contains every radical
                                // If the word is already contained, we add the current radical's number of occurences to the dictionnary's number of occurences
                                if (res.Contains(m))
                                {
                                    Mot rm = new Mot(m.raw, m.clean, kvp.nbOcc + res[res.IndexOf(m)].nbOcc);
                                    int indexx = res.IndexOf(m);
                                    if (indexx > 0)
                                    {
                                        res.RemoveAt(indexx);
                                        res.Add(rm);
                                    }
                                }
                                // Else we add the radical to the dictionnary
                                else
                                {

                                    res.Add(new Mot(m.raw, m.clean, 1));
                                }
                            }
                            else
                            {
                                int indexx = res.IndexOf(kvp);
                                if (indexx > 0)
                                {
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
        public static List<Mot> Finalise(List<Mot> init)
        {
            // We declare the result Dictionary at this state so we can keep an eye on what we are working on
            Dictionary<string, Mot> tempD = new Dictionary<string, Mot>();
            List<Mot> result = new List<Mot>();
            // We have a list of <Mot> that does not handle duplicates. Indeed, the structure of the struct can't define and compare 2 similar cleaned word as their number of appearances and raw version isnt the same
            // So now we create a dictionnary containing all words without duplicates by adding them to a Dictionary and giving them the right nimber of appearnces and so on
            // So we now browe the List looking for new words and duplicates
            foreach (Mot word in init)
            {
                // If the result contains the cleaned word, we add the occurences
                if (tempD.ContainsKey(word.clean))
                {
                    Mot temp = new Mot(word.raw, word.clean, word.nbOcc + tempD[word.clean].nbOcc);
                    tempD[word.clean] = temp;
                }
                else
                {
                    tempD.Add(word.clean, new Mot(word.raw, word.clean, word.nbOcc));
                }
            }
            foreach (KeyValuePair<string, Mot> kvp in tempD)
            {
                result.Add(kvp.Value);
            }
            return result;
        }
        public static List<Mot> Sort(List<Mot> init)
        {
            List<Mot> alea_tri = new List<Mot>();
            List<Mot> alea = new List<Mot>();
            int i;

            alea = Clone(init);
            alea_tri.Add(alea[0]);
            alea.RemoveAt(0);

            while (alea.Count > 0)
            {
                i = 0;
                while ((i < alea_tri.Count) && alea[0].nbOcc > alea_tri[i].nbOcc)
                {
                    i++;
                }

                alea_tri.Insert(i, alea[0]);
                alea.RemoveAt(0);
            }
            List<Mot> res = new List<Mot>();
            foreach (Mot m in alea_tri)
            {
                res.Insert(0, m);
            }
            return res;
        }
        public static List<Mot> Clone(List<Mot> init)
        {
            List<Mot> res = new List<Mot>();
            foreach (Mot m in init)
            {
                res.Add(m);
            }
            return res;
        }
        public static bool isValidRadical(string radical, int rule)
        {
            List<string> VCs = new List<string>();
            bool res = true;
            bool voyellePres = false;
            string suiteVC = "";
            for (int i = 0; i < radical.Length; i++)
            {
                if (estVoyelle(radical[i]))
                {
                    VCs.Add(suiteVC);
                    suiteVC = radical[i] + "";
                    voyellePres = true;
                }
                else
                {
                    suiteVC += radical[i];
                }
            }
            if (voyellePres)
            {
                VCs.Add(suiteVC);
            }
            else
            {
                res = false;
            }
            if (VCs.Count < rule) res = false;
            return res;
        }
        public static List<Mot> supprimeVide(List<Mot> init)
        {
            string path = "../../static/hintsfiles/mot_vide.txt";
            List<string> listWords = new List<string>();
            List<Mot> res = Copy(init);
            StreamReader sr = new StreamReader(path);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                listWords.Add(line);
            }

            foreach (string word in listWords)
            {
                foreach (Mot kvp in init)
                {
                    if (word == kvp.clean)
                    {
                        int index = res.IndexOf(kvp);
                        if (index > 0)
                        {
                            res.RemoveAt(index);
                        }
                    }
                }
            }
            return res;
        }
        public static bool estVoyelle(char c)
        {
            List<char> voyelles = new List<char>() { 'a', 'e', 'i', 'o', 'u', 'y' };
            bool test = false;
            for (int i = 0; i < voyelles.Count && test == false; i++)
            {
                if (voyelles[i] == c)
                {
                    test = true;
                }
            }
            return test;
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
        public static string NormaliseClean(string Xmot){
            string res = "";
            foreach(char c in Xmot){
                if(c == '.') res += "";
                else if (c == '\"') res += "";
                else res += c;
            }
            return res;
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
        public static string Replace(char c)
        {
            return "";
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

        public static void GenerateHTMLFiles()
        {
            Random rnd = new Random();
            string[] tabResultsHTML = Directory.GetDirectories("results/");
            foreach (string fileResultHTML in tabResultsHTML)
            {
                string relativePathResultByPresident = PATHRES + fileResultHTML.Split('/')[fileResultHTML.Split('/').Length - 1];
                if (!Directory.Exists(relativePathResultByPresident))
                {
                    Directory.CreateDirectory(relativePathResultByPresident);
                    // System.Console.WriteLine("cree");
                }
                foreach (string tabPresidentsHTML in Directory.GetDirectories(fileResultHTML))
                {
                    //Directory CLEAN/RAW
                    if (tabPresidentsHTML.Split('/')[tabPresidentsHTML.Split('/').Length - 1] == "RAW")
                    {
                        foreach (string fileResultByP in Directory.GetFiles(tabPresidentsHTML))
                        {
                            string file = relativePathResultByPresident + '/' + (fileResultByP.Split('/')[fileResultByP.Split('/').Length - 1]).Split('.')[0] + ".html";
                            StreamReader sr = new StreamReader("../../../web/html/results/squelet.html");
                            string resultatFinal = "";
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                resultatFinal += line;
                                string classe = "word-cloud";
                                string linkTag = "nav-link";
                                string infosD = "name-date";
                                if(line.Length > 0){
                                if(contient(line, linkTag))
                                {
                                    resultatFinal += writeLinks(tabResultsHTML);
                                }
                                if(contient(line, classe))
                                {
                                    StreamReader srRes = new StreamReader(fileResultByP);
                                    string l2;
                                    List<string> listShuffled = new List<string>();
                                    for (int compt = 0; compt <= 15; compt++)
                                    {
                                        l2 = srRes.ReadLine();
                                        listShuffled.Add(l2.Split(',')[0]);
                                    }
                                    for(int m = 15; m>=0; m--){
                                        int randomPicker = rnd.Next(1,16);
                                        string motTBW = listShuffled[m%randomPicker];
                                        listShuffled.RemoveAt(m%randomPicker);
                                        resultatFinal += "<div class=\"cloud\" id=\"" + (15-m) + "\">" + motTBW + "</div>\n";
                                    }
                                }
                                if (contient(line, infosD))
                                {
                                    resultatFinal += " > " + relativePathResultByPresident.Split('/')[relativePathResultByPresident.Split('/').Length - 1] + ' ' + (fileResultByP.Split('/')[fileResultByP.Split('/').Length - 1]).Split('.')[0] + ':';
                                }
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
        public static string writeLinks(string[] files)
        {   string res ="";
            res += "<a href=\"../../index.html\" class=\"index\"><h2>Acceuil</h2></a>\n";

            //Creation of the UL :
            foreach (string president in files)
            {
                string pres = president.Split('/')[president.Split('/').Length - 1];
                res += "<ul class=\"president\"><h2>-" + pres + "</h2>";
                // System.Console.WriteLine(pres);
                string[] fichier = Directory.GetFiles("results/" + pres + "/CLEAN");

                fichier = Trie(fichier);
                for (int i = 0; i < fichier.Length; i++)
                {
                    res += "<li class=\"link\"><a href=\"../" + pres + "/" + (fichier[i].Split('/')[fichier[i].Split('/').Length - 1]).Split('.')[0] + ".html\">" + (fichier[i].Split('/')[fichier[i].Split('/').Length - 1]).Split('.')[0] + "</a></li>\n";
                }
                res += "</ul>";

            }
            // System.Console.WriteLine(res);
            return res;
        }
        public static bool contient(string line, string contained)
        {
            bool test = false;
            for (int i = 0; i < line.Length - (contained.Length - 1) && !test; i++)
            {
                if (line.Substring(i, contained.Length) == contained) test = true;
            }
            return test;
        }

        public static string[] Trie(string[] init)
        {
            List<int> temp = new List<int>();
            foreach (string s in init)
            {
                temp.Add(int.Parse(s.Split('/')[s.Split('/').Length - 1].Split('.')[0]));
            }
            //Tri
            List<int> res = new List<int>();
            int i;

            res.Add(temp[0]);
            temp.RemoveAt(0);

            while (temp.Count > 0)
            {
                i = 0;
                while ((i < res.Count) && temp[0] > res[i])
                {
                    i++;
                }

                res.Insert(i, temp[0]);
                temp.RemoveAt(0);
            }
            for (int k = 0; k < temp.Count; k++)
            {
                init[k] = temp[temp.Count - k] + ".html";
            }
            return init;
        }
        public static void Modify_CSS(){
            const string PATHCSSFILE = "../../../web/src/css/word_cloud.css";
            const string HEADER = ".word-cloud > :nth-child(";
            const string ENDHEADER = "){\n";
            const string FOOTER = "\n}\n";
            Random rnd = new Random();
            string res = "";
            List<int> listeRemaining = new List<int>();
            for(int j=0; j<=16; j++){
                listeRemaining.Add(j);
            }
            for(int i=16; i>=0; i--){
                int randomPicker = rnd.Next(1,16);
                int randomized = listeRemaining[i%randomPicker];
                listeRemaining.Remove(randomized);
                res += HEADER + randomized + ENDHEADER;
                res += "\n\twidth:" + rnd.Next(17, 33) + "%;";
                res += "\n\tbackground-color: rgb(102," + rnd.Next(0,256) + ",102);";
                res += "\n\tcolor: black;" + FOOTER;
            }
            File.WriteAllText(PATHCSSFILE, res);
        }
    }
}
