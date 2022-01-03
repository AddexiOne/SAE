using System;
using System.IO;

class X {
	static void Main(){
		string path = "/Users/turing/Desktop/SAE/prog/code/v3c/results/HOLLANDE/RAW/2016.csv";
		StreamReader sr = new StreamReader(path);
		if(File.Exists(path)){
			string line;
			for(int i=0; i<20; i++){
				line = sr.ReadLine();
				System.Console.WriteLine(line.Split(",")[0]);
			}
		}	
	}
}
