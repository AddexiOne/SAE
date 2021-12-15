using System;
using System.IO;
using System.Collections.Generic;


class test{
	static void Main(){
		Dictionary<string, int> Dico = new Dictionary<string, int>();
	Dico.Add("Alexandre", 2);
	Dico.Add("Tellement",1);
	Dico.Add("magnifique",1);
	Dico.Add("Alexandree",2);
	racines(Dico);
	}

	static void radical(){
		string mot = "magnifique";
		for(int num1=1; num1<=3; num1++){
		string path = "../../static/hintsfiles/etape" +num+".txt";
		if(File.Exists(path)){
			Console.WriteLine($"{path} exists");
			StreamReader sr = new StreamReader(path);
			int mL = mot.Length;
			List<string> VCs = new List<string>();
			string vc ="";
			string c= "";
			int k=0;
			bool test= false; 
			if(estVoyelle(mot[0])){
				k =0;
			}
			else{
				c += mot[0];
				k =1;
			}
			for(int i=k+1; i<mL; i++){
				vc += mot[i-1];
				int m = i;
				while(test==false){
					test = estVoyelle(mot[m]);
					if(!test){
					vc += mot[m];
					m++;
					}
				}
				i=m;
				test=false;
				VCs.Add(vc);
				vc ="";
			}
			Console.WriteLine(c);
			foreach(string s in VCs){
				Console.WriteLine(s);
			}
		}
		else{
			Console.WriteLine($"{path} does not exists");
		}
	}
	
	public static bool estVoyelle(char c){
		List<char> voy = new List<char>(){'a','e','i','o','u','y'};
			foreach(char v in voy){
				if(v == c) return true;
			}
			return false;
	}


        public static void racines(Dictionary<string, int> init){
            string path = "../../static/hintsfiles/step1.txt";
            Dictionary<string, int> res = new Dictionary<string, int>();
            bool test;
            Dictionary<string, string> terminaison = new Dictionary<string, string>();
	    List<string> terL = new List<string>();
            if(File.Exists(path)){
                //Open the connection to the file
                StreamReader sr = new StreamReader(path);
                string line;
                string endOfWord;
                string replacement;
                while((line = sr.ReadLine())!= null){
                    endOfWord = line.Split(' ')[1];
                    replacement = line.Split(' ')[2];
		    terL.Add(endOfWord);
                    if(!terminaison.ContainsKey(endOfWord)){
                        terminaison.Add(endOfWord, replacement);
                    }
                }
                   
		//We read the dictionary init, and we delete the suffix if there is one
		foreach(KeyValuePair<string, int> kvp in init){
			bool test1 = false;
			for(int i=0; i<terL.Count && !test1; i++){
				if(kvp.Key.Length > terL[i].Length){
					string termKey = kvp.Key.Substring(kvp.Key.Length-terL[i].Length);
					if(termKey == terL[i]){
						string res1 = "";
						if(terminaison[terL[i]] == "epsilon"){
							res1 += kvp.Key.Substring(0, kvp.Key.Length-terL[i].Length);
						}
						else{
							res1 += kvp.Key.Substring(0, kvp.Key.Length-terL[i].Length);
							res1 += terminaison[terL[i]];
						}
						if(res.ContainsKey(res1)){
							res[res1] += kvp.Value;
						}
						else{
							res.Add(res1, kvp.Value);
							Console.WriteLine($"{res[res1]}, {res1}");
						}
						test1 = true;

					}
				}
			}	
		}
                    
            }
            else System.Console.WriteLine("nope");
        }
	return res;
}
