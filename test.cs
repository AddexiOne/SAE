using System;
using System.IO;
using System.Collections.Generic;
class X {
	const string PATHRES = "../../../web/html/results/";
	static void Main(){
	/*
		To write the different files we must first create it then we complete them
	*/
		string[] fpath = Directory.GetDirectories("results/");
		foreach(string b in fpath){
			string pathres = PATHRES + b.Split('/')[b.Split('/').Length-1];
			System.Console.WriteLine(pathres);
			if(!Directory.Exists(pathres)){
				Directory.CreateDirectory(pathres);
				System.Console.WriteLine("cree");
			}
			foreach(string d in Directory.GetDirectories(b)){
				//Directory CLEAN/RAW
				if(d.Split('/')[d.Split('/').Length-1]=="RAW"){
					foreach(string sd in Directory.GetFiles(d)){
						string file =pathres + '/' + (sd.Split('/')[sd.Split('/').Length-1]).Split('.')[0] + ".html";
						System.Console.WriteLine("b:"+b);
						StreamReader sr = new StreamReader("../../../web/html/results/squelet.html");
						string resultatFinal = "";
						string line;
						while((line = sr.ReadLine()) != null){
							resultatFinal += line;
							string classe = "word-cloud";
							string linkTag = "nav-link";
							if(contient(line, linkTag)){
								resultatFinal += writeLinks(b, PATHRES, fpath);
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

	public static string writeLinks(string link, string webFile, string[] files){
		string res = "<h2>Navigation</h2>\n";

		//Creation of the UL :
		foreach(string president in files){
			string pres = president.Split('/')[president.Split('/').Length-1];
			res += "<ul class=\"president\">" + pres;
			// System.Console.WriteLine(pres);
			foreach(string fichier in Directory.GetFiles(webFile +pres)){
				res += "<li class=\"link\"><a href=\"../" +pres+ "/" + (fichier.Split('/')[fichier.Split('/').Length-1]).Split('.')[0]  +".html\">"+(fichier.Split('/')[fichier.Split('/').Length-1]).Split('.')[0]+"</a></li>\n";
			}
			res+= "</ul>";
		}
		// System.Console.WriteLine(res);
		return res;
	}
	public static int premierOcc(string line, char c){
		bool test =false;
		int res =0;
		for(int i=0; i<line.Length && !test; i++){
			if(line[i] == c){
				res =i;
				test= true;
			}
		}
		return res+1;
	}
	public static bool contient(string line, string contained){
		bool test = false;
		for(int i=0; i<line.Length-(contained.Length-1) && !test; i++){
			if(line.Substring(i, contained.Length) == contained) test = true;
		}
		return test;
	}
}
