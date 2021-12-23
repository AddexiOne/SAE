using System;
using System.Collections.Generic;
 class X{
    public static void Main(string[] args){
        string radical = "magnific";
        int regle = 2;
        List<string> VCs = new List<string>();
            string suiteVC = "";
            int k=0;
            while(!estVoyelle(radical[k])) k++;
            for(int i=k; i<radical.Length; i++){
                
                if(estVoyelle(radical[i])){
                    VCs.Add(suiteVC);
                    suiteVC = radical[i] + "";
                }
                else{
                    suiteVC += radical[i];
                }
        }
        VCs.Add(suiteVC);
        if(VCs.Count > regle) System.Console.WriteLine("yes");
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
}