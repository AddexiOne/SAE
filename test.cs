using System;
 class X{
	public static void Main(string[] args){
		char c = 'a';
		if(!Majuscule(c) && !Minuscule(c)) System.Console.WriteLine("pas une lettre");
		else if (Majuscule(c)) System.Console.WriteLine(" Majuscule lettre");
		else if (Minuscule(c)) System.Console.WriteLine("minuscule");
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
            if ((int)c > (int)'a' && (int)c < (int)'z') res = true;
            return res;
        }
}