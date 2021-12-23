using System;
using System.IO;
using System.Collections.Generic;


class test
{
    static void Main()
    {
        // Dictionary<string, int> Dico = new Dictionary<string, int>();
        // Dico.Add("alexandre", 2);
        // Dico.Add("tellement", 1);
        // Dico.Add("magnifique", 1);
        // Dico.Add("finition", 2);
        // Dictionary<string, int> d1 = racines1(Dico, 1);
        // Dictionary<string, int> d2 = racines1(d1, 2);
        // Dictionary<string, int> d3 = racines1(d2,3);
        // foreach (KeyValuePair<string, int> kvp in d3)
        // {
        //     Console.WriteLine(kvp.Key + "  " + kvp.Value);
        // }
        radical();
    }

    static void radical()
    {
        string mot = "magnific";
        
        
            string path = "../../static/hintsfiles/etape1.txt";
            if (File.Exists(path))
            {
                Console.WriteLine($"{path} exists");
                StreamReader sr = new StreamReader(path);
                int mL = mot.Length;
                List<string> VCs = new List<string>();
                string vc = "";
                string c = "";
                int k = 0;
                bool test = false;
                if (estVoyelle(mot[0]))
                {
                    k = 0;
                }
                else
                {
                    c += mot[0];
                    k = 1;
                }
                for (int i = k+1; i < mL; i++)
                {
                    vc += mot[i-1];
                    k = i;
                    while (test == false && i<7)
                    {
                        test = estVoyelle(mot[k]);
                        if (!test)
                        {
                            vc += mot[k];
                            k++;
                        }
                    }
                    System.Console.WriteLine($"k:{k}");
                    i =k;
                    test = false;
                    VCs.Add(vc);
                    vc = "";
                }
                VCs[VCs.Count-1] += mot[mot.Length-1];
                
                int m = VCs.Count;
                System.Console.WriteLine(m);
            }
            else
            {
                Console.WriteLine($"{path} does not exists");
            }
        
    }

    public static bool estVoyelle(char c)
    {
        List<char> voy = new List<char>() { 'a', 'e', 'i', 'o', 'u', 'y' };
        foreach (char v in voy)
        {
            if (v == c) return true;
        }
        return false;
    }

    public static Dictionary<string, int> Copy(Dictionary<string, int> init1)
    {
        Dictionary<string, int> res = new Dictionary<string, int>();
        foreach (KeyValuePair<string, int> kvp in init1)
        {
            res.Add(kvp.Key, kvp.Value);
        }
        return res;
    }


    public struct remplacement{
        public string remplacement;
        public string terminaison;
        public int regle;
    }

    public static Dictionary<string, int> racines1(Dictionary<string, int> init, int nu)
    {
        Dictionary<string, int> init1 = Copy(init);
        string path = "../../static/hintsfiles/step" + nu + ".txt";
        Dictionary<string, int> res = new Dictionary<string, int>();
        bool test;
        res = Copy(init1);
        if (File.Exists(path))
        {
            Dictionary<string, string> terminaison = new Dictionary<string, string>();
            List<string> terL = new List<string>();
            //Open the connection to the file
            StreamReader sr = new StreamReader(path);
            string line;
            string endOfWord;
            string replacement;
            while ((line = sr.ReadLine()) != null)
            {
                var 
                endOfWord = line.Split(' ')[1];
                replacement = line.Split(' ')[2];
                terL.Add(endOfWord);
                if (!terminaison.ContainsKey(endOfWord))
                {
                    terminaison.Add(endOfWord, replacement);
                }
            }

            //We read the dictionary init1, and we delete the suffix if there is one
            foreach (KeyValuePair<string, int> kvp in init1)
            {
                bool test1 = false;

                for (int i = 0; i < terL.Count && !test1; i++)
                {
                    bool modif = false;
                    if (kvp.Key.Length > terL[i].Length)
                    {
                        string termKey = kvp.Key.Substring(kvp.Key.Length - terL[i].Length);
                        if (termKey == terL[i])
                        {
                            res.Remove(kvp.Key);
                            string res1 = "";
                            if (terminaison[terL[i]] == "epsilon")
                            {
                                res1 += kvp.Key.Substring(0, kvp.Key.Length - terL[i].Length);
                            }
                            else
                            {
                                res1 += kvp.Key.Substring(0, kvp.Key.Length - terL[i].Length);
                                res1 += terminaison[terL[i]];
                            }
                            if(radical_Is_Valid(res1)){
                                    if (res.ContainsKey(res1))
                                {
                                    res[res1] += kvp.Value;
                                }
                                else
                                {
                                    res.Add(res1, kvp.Value);
                                    // Console.WriteLine($"{res[res1]}, {res1}");
                                }
                            }
                        }

                    }
                }
            }
        }
        return res;
    }


}



