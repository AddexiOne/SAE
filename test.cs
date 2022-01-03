using System;
using System.IO;
using System.Collections.Generic;
class X {
	const string PATHRES = "web/html/results/";
	static void Main(){
	/*
		To write the different files we must first create it then we complete them
	*/
		string[] fpath = Directory.GetDirectories("prog/code/v3c/results/");
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
						System.Console.WriteLine("File Created");
						StreamReader sr = new StreamReader("web/html/results/squelet.html");
						string resultatFinal = "";
						string line;
						while((line = sr.ReadLine()) != null){
							resultatFinal += line;
							string classe = "word-cloud";
							int pOcc = premierOcc(line, '\"');
							if(pOcc != 0 && line.Length >= pOcc+classe.Length){
								if(line.Substring(pOcc, classe.Length) == classe){
									// System.Console.WriteLine(classe);
									// System.Console.WriteLine(sd);
									StreamReader srRes = new StreamReader(sd);
									string l2;
									for(int compt = 0; compt<=9; compt++){
										l2 = srRes.ReadLine();
										string motTBW = l2.Split(',')[0];
										resultatFinal += "<div class=\"cloud\">" + motTBW + "</div>\n"; 
									}
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
}
