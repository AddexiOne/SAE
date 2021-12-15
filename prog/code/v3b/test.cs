using System;
using System.IO;
using System.Collections.Generic;


class test{
	static void Main(){
		string mot = "magnifique";
		string path = "../../static/hintsfiles/etape1.txt";
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
}
