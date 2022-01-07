using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace main
{
    public class project
    {
        public const string PATHRES = "../../../web/html/results/";
        public const string PATHDISCOURS = "./../../static/";
        public const string EXTENSIONTXT = ".txt";
        public const string relativePathResultByPresidentULT = "./results/";
        public const string EXTENSIONCSV = ".csv";
        static Random rnd = new Random();

        public struct Discour
        { 
            public string pathSpeech { get; }
            // Path from the main.cs to the actual location of the raw discour (.txt)
            public string pathDictionnaryRaw { get; }
            // Path from the main.cs to the location of the raw version of the <Discour>
            public string pathDictionnaryClean { get; }
            // Path from the main.cs to the location of the cleaned version of the <Discour>
            public string pathHtmlFile {get; set;}
            // Path from the main.cs to the location of the HTML's result file
            public string pathHtmlFileR {get; set;}
            // Path from the index.html to the result/result's file
            public List<Mot> listMot { get; set; }
            // The collections that contains the List<Mot> that we at the end use to write the HTML
            public int annee { get; }
            // year of the <Discour>
            public string HtmlContent {get; set;}
            // string storing the HTML content of the <Discour>
            public Discour(string president, int annee) :this()
            {
                /*
                    Constructor that instanciate and asign the <Discour>
                    Uses all kind of functions to create the final result and asign each variable of <Discour>
                    The presence of this constructor really makes the whole project very straightforward,
                    ie : generating the files, the results automatically, while we create a <Discour> makes a very small main and reserve the memory at the very start of the project
                */
                this.annee = annee;
                this.pathSpeech = PATHDISCOURS + president + "/" + annee + EXTENSIONTXT;
                this.pathDictionnaryRaw = relativePathResultByPresidentULT + president + "/" + "RAW" + "/" + annee + EXTENSIONCSV;
                this.pathDictionnaryClean = relativePathResultByPresidentULT + president + "/" + "CLEAN" + "/" + annee + EXTENSIONCSV;
                this.listMot = GenerateList(this.pathSpeech);
                generateFileRaw(racines(supprimeVide(this.listMot)), this.pathDictionnaryRaw);
                generatefileClean(racines(supprimeVide(this.listMot)), this.pathDictionnaryClean);
                this.HtmlContent = "";
                this.pathHtmlFile = "../../../web/html/results/v3c/";
                this.pathHtmlFileR = "../";
            }
            public static List<Mot> GenerateList(string path)
            {
                /*
                    This function creates a simple List of <Mot> with the content of the <Discour>'s .txt file
                */
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
                return supprimeVide(liste);
            }
            public static void generateFileRaw(List<Mot> XListDiscours, string path)
            {
                /*
                    This method creates a file in the argument #2 (path) file, the result contains Raw words (non-modified/formated)
                */
                string toBeWritten = "";
                foreach (Mot kvp in XListDiscours)
                {
                    // Write the normalized word, followed by a ',' then its number of occurences, making the result file a .csv file
                    if (kvp.clean != "")
                    {
                        toBeWritten += $"{NormaliseRaw(kvp.raw.ToLower())},{kvp.nbOcc}\n";
                    }
                }
                File.WriteAllText(path, toBeWritten);
            }
            public static void generatefileClean(List<Mot> XListDiscours, string path)
            {
                /*
                    This method creates a file in the argument #2 (path) file, the result contains clean words (modified/formated)
                */
                string toBeWritten = "";
                foreach (Mot kvp in XListDiscours)
                {
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
            public string namePresident {get; set;}
            public List<Discour> listSpeeches {get; set;}
            public string pathResultsDirectory {get; set;}
            public President(string nom) :this()
            {
                this.namePresident = nom;
                this.listSpeeches = new List<Discour>();
                this.pathResultsDirectory = "../../../web/html/results/v3c/" + this.namePresident + "/"; 
                Directory.CreateDirectory("../../../web/html/results/v3c/" + this.namePresident);
                Directory.CreateDirectory(this.pathResultsDirectory);
            }
        }
        public struct Terminaison
        {
            public string suffixe { get; set; }
            // suffixe is the string that we'll compare to other words
            public string remplacement { get; set; }
            // string that'll tells us what to do when we have to modify a <Mot>
            public int regle { get; set; }
            // number of occurences of a List<string> VCs, to tell if a <Mot> that have been cleaned is valid
            public Terminaison(int regle, string suff, string rempl)
            {
                /*
                    Constructor that instanciate and asign values to <terminaison>'s variables
                */
                this.regle = regle;
                this.suffixe = suff;
                this.remplacement = rempl;
            }
        }
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
                this.raw = NormaliseRaw(raw);
                this.nbOcc = nbOcc;
            }
        }
        public static List<President> Start()
        {
            /*
                Foreach presidents in the listPresidents, I create a new <President> that will contain each of it's informations, discours and so on.
            */
            List<President> presidents = new List<President>();
            List<string> listPresidents = new List<string>() { "GISCARDDESTAING", "MITTERAND", "CHIRAC", "SARKOZY", "HOLLANDE", "MACRON" };

            // Integer that'll count the years
            int annee = 1974;
            // Browse the List to find the .txt file, located in the static repertory
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
                        // Creation of a new Discours with the constructor (izi life :))
                        Discour d = new Discour(s, annee);
                        president.listSpeeches.Add(d);
                    }
                    annee++;
                }
                presidents.Add(president);
            }
            // return the collection of <President> which contains its respective collection of <Discour>
            return presidents;
        }
        static void Main(string[] args)
        {
            /*
                Main Uses Start() BuildHtmlFiles and Modify_CSS() to first create the collection, then use the collection to create a readable file in html
                And the the result in HTML is formated graphically
            */
            System.Console.WriteLine("Traitement des textes");
            List<President> listPresidents = Start();
            System.Console.WriteLine("Génération des fichiers HTML");
            BuildFillHtmlFile(listPresidents);
            System.Console.WriteLine("modification du fichier CSS");
            Modify_CSS();
            Console.Clear();
            System.Console.WriteLine("DONE");
        }
        public static List<Mot> racines(List<Mot> init)
        {
            /*
                Racines is the fonction that start the processing of each <Mot> by Going 3 times into the racinesCalled() so that we dont have any unecessary character in every word
                With that we'll be able to really compare each radical and telle its true number of occurences
            */
            List<Mot> res = Copy(init);
            for (int i = 1; i <= 3; i++)
            {
                res = racinesCalled(res, i);
            }
            return res;
        }
        public static List<Terminaison> generateListTerminaison(string path)
        {
            /*
                This fonction read a whole file given in argument and add every information of the file to a List of Terminaison
            */
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
        public static List<Mot> racinesCalled(List<Mot> init, int nu)
        {
            /*
                With a specified number that'll define which file we are looking at, we browse evry Mot in the collection in argument #1.
                With the collection of terminaisons generated by generateListTerminaison, we can now look at every <Mot> and compare its terminaison to the file's <Terminaison>
                If the terminaison mathces, we modify the word with the terminaiuson's replacement and then check if the said radical is valid with "isValidRadical"
                We then return a Sorted Collection that doesn't contains any duplicates with "Finalise" 
            */
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
            /*
                Finalise is a fonction that takes care of any duplicates in the collection List<mot> in argument and returns a collection without duplicate
            */
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
            /*
                From a List of <Mot> not sorted, we, with the tri a insertion's way, sort the list in argument to finaly return a sorted List<Mot>
            */
            List<Mot> alea_tri = new List<Mot>();
            List<Mot> alea = new List<Mot>();
            int i;

            alea = Copy(init);
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
        public static bool isValidRadical(string radical, int rule)
        {
            /*
                By looking at the pure radical of the word in argument and its rule, we can tell if a radical is considered valid or not.
                First we split the word in seperates strings named VC (VoyelleConsonne), if the word contains more than <rule> VCs, the radical is valid.
            */
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
            /*
                The fonction takes a list of <Mot> and check if each <Mot> is considered as an empty_word, 
                if it is, we remove it from the collection.
                We then return the collection without empty_words
            */
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
            /*
                Test if the argument is a voyelle
            */
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
            /*
                Clean the argument so that we have only letters of the alphabet in the result's string
            */
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
        public static string NormaliseRaw(string Xmot){
            /*
                Clean the argument so that we have a claen word without any disturbent character
            */
            string res = "";
            foreach(char c in Xmot){
                if(c == '.' || c == ',' || c == '-' || c == '_') res += "";
                else if (c == '\'') res = "";
                else if (c == '\"') res += "";
                else res += c;
            }
            return res.ToLower();
        }
        public static bool Majuscule(char c)
        {
            // Test if the argument is a Majuscule
            bool res = false;
            if ((int)c >= (int)'A' && (int)c <= (int)'Z') res = true;
            return res;
        }
        public static bool Minuscule(char c)
        {
            // Test if the argument (char) is a minuscule
            bool res = false;
            if ((int)c >= (int)'a' && (int)c <= (int)'z') res = true;
            return res;
        }
        public static List<Mot> Copy(List<Mot> init1)
        {
            // foreach element of the argument's list, we add it into antother list.
            List<Mot> res = new List<Mot>();
            foreach (Mot kvp in init1)
            {
                res.Add(new Mot(kvp.raw, kvp.clean, kvp.nbOcc));
            }
            return res;
        }
        public static void BuildFillHtmlFile(List<President> init){
            /*
                This method read a skeleton HTML file that has been created and filled manually.
                While reading the skeleton file, we check if the current line contains the triggering classes, 
                If yes, we then browse all <President> and their <Discour> to add the read lines, and then we add generated html, wheter its the links or the word Cloud
                At the end, we then complete the HTML file by adding the rest of the skeleton lines.
            */
            const string PATHSKELETON = "../../../web/html/results/skeleton.html";
            StreamReader sr = new StreamReader(PATHSKELETON);
            // We read the skeleton file waiting for the triggering classes
            string line;
            string coreHtml = "";
            while((line = sr.ReadLine()) != null){
                coreHtml += line + '\n';
                /*
                    If a triggering class is found, we simply add the related content to the <Discour>'s file.
                */
                if(contient(line, "nav-link")){
                    for(int i=0; i<init.Count; i++){
                        for(int j=0; j<init[i].listSpeeches.Count; j++){
                            Discour temp = new Discour(init[i].namePresident, init[i].listSpeeches[j].annee);
                            temp.HtmlContent += init[i].listSpeeches[j].HtmlContent + coreHtml;
                            temp.HtmlContent += writeLinks(init);
                            init[i].listSpeeches[j] = temp;
                        }
                    }
                    coreHtml = "";
                }
                else if(contient(line, "word-cloud")){
                    for(int i=0; i<init.Count; i++){
                        for(int j=0; j<init[i].listSpeeches.Count; j++){
                            Discour temp = new Discour(init[i].namePresident, init[i].listSpeeches[j].annee);
                            temp.HtmlContent += init[i].listSpeeches[j].HtmlContent + coreHtml;
                            temp.HtmlContent += writeWordCloud(init[i].listSpeeches[j].listMot);
                            init[i].listSpeeches[j] = temp;
                        }
                    }
                    coreHtml = "";
                }
                else if(contient(line, "name-date")){
                    for(int i=0; i<init.Count; i++){
                        for(int j=0; j<init[i].listSpeeches.Count; j++){
                            Discour temp = new Discour(init[i].namePresident, init[i].listSpeeches[j].annee);
                            temp.HtmlContent += init[i].listSpeeches[j].HtmlContent + coreHtml;
                            temp.HtmlContent += writeNameDate(init[i], init[i].listSpeeches[j]);
                            init[i].listSpeeches[j] = temp;
                        }
                    }
                    coreHtml = "";
                }
            }
            // We now add the build string to the <Discour> and then write the File
            // coreHtml is added at the end so that we have to end of the skeleton file written in the html result file
            for(int i=0; i<init.Count; i++){
                for(int j=0; j<init[i].listSpeeches.Count; j++){
                    Discour temp = new Discour(init[i].namePresident, init[i].listSpeeches[j].annee);
                    temp.HtmlContent += init[i].listSpeeches[j].HtmlContent + coreHtml;
                    init[i].listSpeeches[j] = temp;
                    File.WriteAllText(init[i].listSpeeches[j].pathHtmlFile+init[i].namePresident+"/"+init[i].listSpeeches[j].annee+".html", init[i].listSpeeches[j].HtmlContent);
                }
            }
        }
        public static string writeNameDate(President p, Discour d){
            // This fonction write the name of the president and the year so that we have some king of title before the word-cloud
            return "\t\t\t\t\t" +p.namePresident + " " + d.annee + ":\n";
        }
        public static string writeWordCloud(List<Mot> listMotinit){
            /*
                This fonction take as argument the List of <Mot> and pick the first 15 words
                We insert them into a list as <string> ( We add the RAW version of the word)
                we then insert the <div> containing the word, shuffling the 15 elements of the ListRaw
                We the return the div composed of the 15 words
            */
            const string TABDIV = "\n\t\t\t\t\t";
            string res = "";
            List<string> listRaw = new List<string>();
            for(int i=0; i<15; i++){
                listRaw.Add(listMotinit[i].raw);
            }
            for(int i=listRaw.Count; i>0; i--){
                string motTBW = listRaw[rnd.Next(0,i)];
                listRaw.Remove(motTBW);
                res += TABDIV + "<div class=\"cloud\" id=\"" + i + "\">" + motTBW + "</div>";
            }
            return res + '\n';
        }
        public static string writeLinks(List<President> init)
        {   
            /*
            This fonction uses a List of <President> that contains the List of <Discour> and for each <Discour> we get its HTML result file's path.
            Foreach President and each discours, we then add the path to the file (relative to the index.html file) inside their right tag.
            Each line is put into a string that will be returned.
        */
            const string ACCEUIL = "\t\t\t\t<a href=\"../../../index.html\" class=\"index\"><h2>Acceuil</h2></a>";
            const string TABLI = "\n\t\t\t\t\t";
            const string TABUL = "\n\t\t\t\t";
            const string TABH3 = TABUL;

            string res = ACCEUIL;
            // Browe the list of <President>
            foreach (President president in init)
            {
                // We now write The president's name and create its ul list (that we will fill later)
                res += TABH3 + "<h3>-"+ president.namePresident + "</h3>";
                res += TABUL + "<ul class=\"president\">";
                // Browse every President's <Discour>
                foreach(Discour discours in president.listSpeeches){
                    // Add the link to the discours inside the returned string variable
                    res += TABLI + "<li class=\"link\"><a href=\"" + discours.pathHtmlFileR +president.namePresident+ '/' + discours.annee + ".html" + "\">" + discours.annee + "</a></li>";
                }
                // End of the president's discours so we close its list (ul)
                res += TABUL + "</ul>";
            }
            return res + '\n';
        }
        public static bool contient(string line, string contained){
            /*
                This fonction look at each string of the contained length.
                We start at index 0, we add the length of the supposed contained string.
                We then compare the resultant string to the contained string.
                If it is the same, we then return true
                if not, we keep looking at the line until it is ended
            */
            bool test = false;
            if(line.Length >0){
                for (int i = 0; i < line.Length - (contained.Length - 1) && !test; i++){
                    if (line.Substring(i, contained.Length) == contained) test = true;
                }
            }
            return test;
        }
        public static void Modify_CSS(){
            /*
                This fonction generate a random display mode of the words in the word-cloud
                To do so, we build a string that we'll add to a css file.
            */

            const string PATHCSSFILE = "../../../web/src/css/word_cloud.css";
            const string HEADER = ".word-cloud > :nth-child(";
            const string ENDHEADER = "){\n";
            const string FOOTER = "\n}\n";
            string res = "";
            // ListeRemain contains all the words we have to give a css style
            List<int> listeRemaining = new List<int>();
            // ListRota is a list that contains the 3 rules of rotation that the word can take
            List<string> listRota = new List<string>(){"\n\ttransform: rotate(45deg);", "\n\ttransform: rotateZ(90deg);", ""};
            for(int j=0; j<=16; j++){
                listeRemaining.Add(j);
            }
            for(int i=16; i>=0; i--){
                int randomPicker = rnd.Next(1,16);
                int randomized = listeRemaining[i%randomPicker];
                listeRemaining.Remove(randomized);
                res += HEADER + (randomized) + ENDHEADER;
                res += listRota[rnd.Next(0,3)];
                res += "\n\twidth:" + rnd.Next(17, 33) + "%;";
                res += "\n\tflex: 1 1 0;";
                res += "\n\tfont-size:" + (300-(i*3.5)) + "%;";
                res += "\n\tcolor: rgb(102," + rnd.Next(0,256) + ",102);" + FOOTER;
            }
            File.WriteAllText(PATHCSSFILE, res);
        }
    }
}