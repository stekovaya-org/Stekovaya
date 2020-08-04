using System;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.IO;
using System.Drawing;

namespace SEPL {
  public class SEPL {
    public class UnderflowException : Exception {
      public UnderflowException(string msg) : base(msg){
      }
    }
    public class UndefinedException : Exception {
      public UndefinedException(string msg) : base(msg){
      }
    }
    public class CalculationException : Exception {
      public CalculationException(string msg) : base(msg){
      }
    }
    public class ReadonlyException : Exception {
      public ReadonlyException(string msg) : base(msg){
      }
    }
    public class Ms {
      public static decimal Gcd(decimal m,decimal n){
        if(m < n){
          var tmp = m;
          m = n;
          n = tmp;
        }
        while(true){
          var r = m % n;
          if (r == 0) return n;
          m = n;
          n = r;
        }
      }
      public static decimal Lcm(decimal m,decimal n){
        return (m * n) / Gcd(m,n);
      }
      public static bool Including(Hashtable h,string hn){
        return ((System.Collections.Generic.List<string>)h["_INCED"]).Contains("Include." + hn);
      }
      public static string Unic(Match m){
        return Char.ConvertFromUtf32(Convert.ToInt32(m.Groups[1].Value,16));
      }
      public static string FormatText(string txt){
        string res = txt;
        Hashtable convtbl = new Hashtable{
          ["\\ESC"] = "\x1B",
          ["\\NUL"] = "\x00",
          ["\\BEL"] = "\x07",
          ["\\CRT"] = "\x0D",
          ["\\LFD"] = "\x0A",
          ["\\ATR"] = "\x1B[39m",
          ["\\ABR"] = "\x1B[49m",
          ["\\BLD"] = "\x1B[1m",
          ["\\DIM"] = "\x1B[2m",
          ["\\ITL"] = "\x1B[3m",
          ["\\UDL"] = "\x1B[4m",
          ["\\REV"] = "\x1B[7m",
          ["\\SME"] = "\x1B[8m",
          ["\\DIS"] = "\x1B[m"
        };
        for(int ans = 0; ans <= 7; ans++){
          string ca = ans.ToString();
          res = res.Replace("\\ACT#" + ca,"\x1B[" + (30 + ans).ToString() + "m");
          res = res.Replace("\\ACB#" + ca,"\x1B[" + (40 + ans).ToString() + "m");
        }
        for(int ans = 255; ans != -1; ans--){
          string ca = ans.ToString();
          res = res.Replace("\\ATE#" + ca,"\x1B[38;5;" + ca + "m");
          res = res.Replace("\\ABE#" + ca,"\x1B[48;5;" + ca + "m");
        }
        foreach(string kys in convtbl.Keys){
          res = res.Replace(kys,convtbl[kys].ToString());
        }
        return Regex.Replace(Regex.Replace(res,@"\\UNF([0-9A-Fa-f]{4})",Unic),@"\\UNT([0-9A-Fa-f]{2})",Unic);
      }
      public static bool Check(Stack stak,int hm){
        return stak.Count >= hm;
      }
      public static bool IsNum(string[] nn){
        bool b = true;
        try{
          foreach(string x in nn) decimal.Parse(x);
        }catch{
          b = false;
        }
        return b;
      }
      public static bool IsNum(string nm){
        bool b = true;
        try{
          decimal.Parse(nm);
        }catch{
          b = false;
        }
        return b;
      }
      public static bool IsUint(string nm){
        bool b = true;
        try{
          uint.Parse(nm);
        }catch{
          b = false;
        }
        return b;
      }
      public static bool IsDouble(string nm){
        bool b = true;
        try{
          double.Parse(nm);
        }catch{
          b = false;
        }
        return b;
      }
      public static bool IsRegex(string nm){
        bool b = true;
        try{
          new Regex(nm);
        }catch{
          b = false;
        }
        return b;
      }
      public static void Underflow(Stack stak,int hm,string[] com,int line,int pos){
        if(!Check(stak,hm)){
          int z = 0;
          string slc = string.Join(" ",com.Select(x =>{
            z++;
            return (z - 1 == pos ? ">>> " + x + " <<<" : x);
          }));
          throw new UnderflowException($"\r\n:{line+1}: Stack underflow\r\n{slc}");
        }
      }
      public class Libs {
        public class Stdin {
        }
        public class File {
        }
        public class Error {
        }
        public static Stack UsingThese = new Stack();
        public static Hashtable Coms = new Hashtable{
          ["Include.Util.Stdin"] = new string[]{"INP"},
          ["Include.Util.File"] = new string[]{"WRT","RED","DEL","EXS"},
          ["Include.Util.Error"] = new string[]{"ERR","WRN"}
        };
      }
      public static string[] Include(string name,string[] t,int i,int j){
        //string comn = name.Split(".")[2];
        string[] coma = name.Split(new char[]{'.'});
        if(!(coma.Length >= 3)){
          Console.WriteLine("\x1B[1m\x1B[33mwarn: \x1B[mPackage \"" + name + "\" unrecognized");
          return new string[0]{};
        }
        if(typeof(Libs).GetMember(coma[2]).Length == 0 || (coma[2] == "Stdin" ? !(coma[1] == "Util" && coma[0] == "Include") : (coma[2] == "File" ? !(coma[1] == "Util" && coma[0] == "Include") : (coma[2] == "Error" ? !(coma[1] == "Util" && coma[0] == "Include") : false)))){
          int z = 0;
          string slc = string.Join(" ",t.Select(x => {
            z++;
            return (z - 1 == j ? ">>> " + x + " <<<" : x);
          }));
          throw new UndefinedException($"\r\n:{i+1}: Unknown package\r\n{slc}");
        }
        Libs.UsingThese.Push(name);
        return (string[])Libs.Coms[name];
      }
    }
    public class Runner {
      public static void Run(string code,bool ld = false){
        Bitmap bmp = new Bitmap(100,100);
        int wrn = 0;
        int lineis = 0;
        int posis = 0;
        string[] linect = {};
       // File.WriteAllText("a.exe","");
        AssemblyName asmName = new AssemblyName(){ Name = "a" };
        AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName,AssemblyBuilderAccess.Save);
        ModuleBuilder modBuilder = asmBuilder.DefineDynamicModule("a.exe","a.exe",true);
        TypeBuilder typeBuilder = modBuilder.DefineType("program",TypeAttributes.Class);
        MethodBuilder metBuilder = typeBuilder.DefineMethod("Main",MethodAttributes.Public | MethodAttributes.Static,typeof(void),Type.EmptyTypes);
        ILGenerator ilMain = metBuilder.GetILGenerator();
        MethodInfo mwrite = Type.GetType("System.Console").GetMethod("WriteLine",new Type[]{typeof(string)});
        /*
        ilMain.Emit(OpCodes.Ldstr,"Hi.");
        ilMain.EmitCall(OpCodes.Call,Type.GetType("System.Console").GetMethod("WriteLine",new Type[]{typeof(string)}),null);
        ilMain.Emit(OpCodes.Ret);
        typeBuilder.CreateType();
        asmBuilder.SetEntryPoint(metBuilder);
        asmBuilder.Save("a.exe");
        if(File.Exists("a.exe.mdb")) File.Delete("a.exe.mdb");
        */
        string[] p = Regex.Split(code.Replace("\r",""),"\n");
        Random Rnd = new Random();
        bool op = false;
        bool fp = false;
        bool ib = false;
        bool it = false;
        bool frl = false;
        bool eme = false;
        int fll = 0;
        int cou = 0;
        string[] coul = {};
        string str = "";
        Stack stk = new Stack();
        Hashtable hash = new Hashtable();
        Hashtable lab = new Hashtable();
        Hashtable fnc = new Hashtable();
        var avl = new System.Collections.Generic.List<string>();
        hash.Add("_F00",0);
        hash.Add("_F01",1);
        hash.Add("_EMPTY","");
        hash.Add("_FALSE",0);
        hash.Add("_TRUE",1);
        hash.Add("_INCED",new System.Collections.Generic.List<string>());
        try{
        for(int i = 0; i < p.Length; i++){
          lineis = i;
          string[] t = Regex.Split(p[i]," ");
          linect = t;
          ib = false;
          it = false;
          for(int j = 0; j < t.Length; j++){
            posis = j;
            string[] q = t;
            if(eme && q[j] != "RME") q[j] = "EMP";
            if(ib){
              if(!it) q[j] = "EMP";
            }
            if(op && q[j] != "END"){
              string ph = q[j];
              foreach(string key in hash.Keys){
                Regex regx = new Regex("{" + key + "}");
                ph = regx.Replace(ph,hash[key].ToString());
              }
              str+=(fp ? " " : "") + ph;
              q[j] = "EMP";
              fp = true;
            }
            if(q[j] == "DEF"){
              Ms.Underflow(stk,2,t,i,j);
              string c = stk.Pop().ToString();
              string n = stk.Pop().ToString();
              ilMain.Emit(OpCodes.Stloc,cou);
              Array.Resize(ref coul,coul.Length + 1);
              coul[coul.Length - 1] = n;
              cou++;
              if(hash.ContainsKey(n)){
                if(Regex.Replace(n,@"^_[^\n]+$","") == ""){
                  int slcn = 0;
                  string slc = string.Join(" ",t.Select(x=>{
                    slcn++;
                    return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                  }));
                  throw new ReadonlyException($"\r\n:{i+1}: Cant change readonly variable’s value\r\n{slc}");
                }else{
                  hash[n] = c;
                }
              }else{
                hash.Add(n,c);
              }
            }else if(q[j] == "RMS"){
              eme = true;
            }else if(q[j] == "RME"){
              eme = false;
            }else if(q[j] == "DAT"){
              DateTime dtn = DateTime.Now;
              stk.Push(dtn.ToLocalTime());
            }else if(q[j] == "UTC"){
              DateTime dtn = DateTime.UtcNow;
              stk.Push(dtn.ToUniversalTime());
            }else if(q[j] == "SPL"){
              Ms.Underflow(stk,3,t,i,j);
              string h = stk.Pop().ToString();
              string v = stk.Pop().ToString();
              string c = stk.Pop().ToString();
              if(hash.ContainsKey(h)){
                if(Regex.Replace(h,@"^_[^\n]+$","") == ""){
                  int slcn = 0;
                  string slc = string.Join(" ",t.Select(x=>{
                    slcn++;
                    return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                  }));
                  throw new ReadonlyException($"\r\n:{i+1}: Cant change readonly variable’s value\r\n{slc}");
                }
                if(!Ms.IsRegex(v)){
                  int slcn = 0;
								  string slct = string.Join(" ",t.Select(x=>{
									  slcn++;
									  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
								  }));
								  throw new UndefinedException($"\r\n:{i+1}: \"{v}\" isnt regex\r\n{slct}");
                }
                hash[h] = Regex.Split(c,v).ToList();
              }else{
                if(!Ms.IsRegex(v)){
                  int slcn = 0;
								  string slct = string.Join(" ",t.Select(x=>{
									  slcn++;
									  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
								  }));
								  throw new UndefinedException($"\r\n:{i+1}: \"{v}\" isnt regex\r\n{slct}");
                }
                hash.Add(h,Regex.Split(c,v).ToList());
              }
            }else if(q[j] == "EAR"){
              Ms.Underflow(stk,3,t,i,j);
              string h = stk.Pop().ToString();
							string c = stk.Pop().ToString();
							string v = stk.Pop().ToString();
							if(hash.ContainsKey(v)){
							  var vl = new System.Collections.Generic.List<string>();
							  try{
                vl = (System.Collections.Generic.List<string>)hash[v];
							  }catch{
                  int slcn = 0;
								  string slct = string.Join(" ",t.Select(x=>{
									  slcn++;
									  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
								  }));
								  throw new UndefinedException($"\r\n:{i+1}: \"{v}\" isnt array\r\n{slct}");
                }
                if(!Ms.IsUint(c)){
                  int slcn = 0;
                  string slc = string.Join(" ",t.Select(x=>{
                    slcn++;
                    return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                  }));
                  throw new CalculationException($"\r\n:{i+1}: Index must be a number\r\n{slc}");
                }else{
                  if(Regex.Replace(v,@"^_[^\n]+$","") == ""){
                    int slcn = 0;
                    string slc = string.Join(" ",t.Select(x=>{
                      slcn++;
                      return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                    }));
                    throw new ReadonlyException($"\r\n:{i+1}: Cant change readonly variable’s value\r\n{slc}");
                  }
                  vl[int.Parse(c)] = h;
                  hash[v] = vl;
                }
              }else{
								int slcn = 0;
								string slct = string.Join(" ",t.Select(x=>{
									slcn++;
									return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
								}));
                throw new UndefinedException($"\r\n:{i+1}: Undefined array\r\n{slct}");
              }
            }else if(q[j] == "RND"){
              stk.Push(Convert.ToDecimal(Rnd.NextDouble()));
            }else if(q[j] == "FLR"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!Ms.IsNum(h)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc floor (given a string)\r\n{slc}");
							}else{
								stk.Push(Math.Floor(Decimal.Parse(h)));
							}
            }else if(q[j] == "ROD"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!Ms.IsNum(h)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc round (given a string)\r\n{slc}");
							}else{
								stk.Push(Math.Round(Decimal.Parse(h)));
							}
            }else if(q[j] == "CEL"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!Ms.IsNum(h)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc ceil (given a string)\r\n{slc}");
							}else{
								stk.Push(Math.Ceiling(Decimal.Parse(h)));
							}
            }else if(q[j] == "TRC"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!Ms.IsNum(h)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc trunc (given a string)\r\n{slc}");
							}else{
								stk.Push(Math.Truncate(Decimal.Parse(h)));
							}
            }else if(q[j] == "ABS"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!Ms.IsNum(h)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc ABS (given a string)\r\n{slc}");
							}else{
								stk.Push(Math.Abs(Decimal.Parse(h)));
							}
            }else if(q[j] == "SIN"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!Ms.IsNum(h)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc sin (given a string)\r\n{slc}");
							}else{
								stk.Push(Math.Sin(Double.Parse(h)));
							}
            }else if(q[j] == "COS"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!Ms.IsNum(h)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc cos (given a string)\r\n{slc}");
							}else{
								stk.Push(Math.Cos(Double.Parse(h)));
							}
            }else if(q[j] == "TAN"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!Ms.IsNum(h)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc tan (given a string)\r\n{slc}");
							}else{
								stk.Push(Math.Tan(Double.Parse(h)));
							}
            }else if(q[j] == "ASN"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!Ms.IsNum(h)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc asin (given a string)\r\n{slc}");
							}else{
								stk.Push(Math.Asin(Double.Parse(h)));
							}
            }else if(q[j] == "DRW"){
              Ms.Underflow(stk,5,t,i,j);
              string[] h = {stk.Pop().ToString(),stk.Pop().ToString(),stk.Pop().ToString(),stk.Pop().ToString(),stk.Pop().ToString()};
              h = h.Reverse().ToArray();
              if(!Ms.IsNum(h)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant plot given strings\r\n{slc}");
							}else{
							  int[] nh = h.Select(nhe=>Convert.ToInt32(nhe.ToString())).ToArray();
							  if(nh[0] >= bmp.Width){
							    int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant plot on given int\r\n{slc}");
							  }
							  if(nh[1] >= bmp.Height){
							    int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant plot on given int\r\n{slc}");
							  }
							  bmp.SetPixel(nh[0],nh[1],Color.FromArgb(nh[2],nh[3],nh[4]));
							}
            }else if(q[j] == "SVE"){
              Ms.Underflow(stk,1,t,i,j);
              bmp.Save(stk.Pop().ToString());
            }else if(q[j] == "ACS"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!Ms.IsNum(h)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc acos (given a string)\r\n{slc}");
							}else{
								stk.Push(Math.Acos(Double.Parse(h)));
							}
            }else if(q[j] == "ATN"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!Ms.IsNum(h)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc atan (given a string)\r\n{slc}");
							}else{
								stk.Push(Math.Atan(Double.Parse(h)));
							}
            }else if(q[j] == "WTE" && Ms.Including(hash,"Util.Error")){
              Ms.Underflow(stk,1,t,i,j);
              Console.Error.WriteLine(stk.Pop().ToString());
            }else if(q[j] == "NWE" && Ms.Including(hash,"Util.Error")){
              Ms.Underflow(stk,1,t,i,j);
              Console.Error.Write(stk.Pop().ToString());
            }else if(q[j] == "DNWE" && Ms.Including(hash,"Util.Error")){
              Ms.Underflow(stk,1,t,i,j);
              Console.Error.Write(stk.Peek().ToString());
            }else if(q[j] == "DWTE" && Ms.Including(hash,"Util.Error")){
              Ms.Underflow(stk,1,t,i,j);
              Console.Error.WriteLine(stk.Peek().ToString());
            }else if(q[j] == "BEL"){
              Console.Beep();
            }else if(q[j] == "CLS"){
              Console.Clear();
            }else if(q[j] == "ERR" && Ms.Including(hash,"Util.Error")){
              Ms.Underflow(stk,1,t,i,j);
              int slcn = 0;
              string h = stk.Pop().ToString();
              string slct = string.Join(" ",t.Select(x=>{
                slcn++;
                return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
              }));
              throw new Exception($"\r\n:INTERNAL-{i+1}: {h}\r\n{slct}");
            }else if(q[j] == "WRN" && Ms.Including(hash,"Util.Error")){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              Console.WriteLine($"\x1B[m\x1B[33m(INTERNAL-{i+1}:INTERNAL-{j+1})warn: \x1B[m{h}");
              wrn++;
            }else if(q[j] == "SCO"){
              Ms.Underflow(stk,2,t,i,j);
              string hf = stk.Pop().ToString();
              string hs = stk.Pop().ToString();
              stk.Push(Ms.FormatText(hs + hf));
            }else if(q[j] == "INC"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              int cfi = 0;
              if(!(ld && h == "Include.Util.Stdin")){
                foreach(string cavl in Ms.Include(h,t,i,j)){
                  avl.Add(cavl);
                  cfi++;
                }
                var pht = (System.Collections.Generic.List<string>)hash["_INCED"];
                pht.Add(h);
                hash["_INCED"] = pht;
              }
              if(cfi == 0) wrn++;
            }else if(q[j] == "BRK"){
              frl = false;
            }else if(q[j] == "STK"){
              Console.WriteLine($"STACK<{stk.Count}> " + string.Join(" ",stk.ToArray().Reverse().Select(x => x.ToString())));
            }else if(q[j] == "FOR"){
              if(!frl) hash["_F00"] = 0;
              if(!frl) hash["_F01"] = 1;
              frl = true;
              fll = i;
            }else if(q[j] == "EFOR"){
              if(frl){
                hash["_F00"] = decimal.Parse(hash["_F00"].ToString()) + 1;
                hash["_F01"] = decimal.Parse(hash["_F01"].ToString()) + 1;
                i = fll - 1;
                break;
              }
            }else if(q[j] == "THN"){
              Ms.Underflow(stk,1,t,i,j);
              ib = true;
              string h = stk.Pop().ToString();
              if(h != "0" && h != ""){
                it = true;
              }
            }else if(q[j] == "POP"){
              Ms.Underflow(stk,1,t,i,j);
              stk.Pop();
            }else if(q[j] == "SLP"){
              Ms.Underflow(stk,1,t,i,j);
							string time = stk.Pop().ToString();
							if(!Ms.IsNum(time)){
								int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant sleep given a string\r\n{slc}");
							}else{
								Thread.Sleep(Int32.Parse(time));
							}
            }else if(q[j] == "REM"){
              ib = true;
              it = false;
            }else if(q[j] == "TOB"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(h == "0" || h == ""){
                stk.Push(0);
              }else{
                stk.Push(1);
              }
            }else if(q[j] == "NEQ"){
              Ms.Underflow(stk,2,t,i,j);
              if(stk.Pop().ToString() != stk.Pop().ToString()){
                stk.Push(1);
              }else{
                stk.Push(0);
              }
            }else if(q[j] == "NOT"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(h == "0" || h == ""){
                stk.Push(1);
              }else{
                stk.Push(0);
              }
            }else if(q[j] == "LSS"){
              Ms.Underflow(stk,2,t,i,j);
              string[] qu = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(qu)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
								throw new CalculationException($"\r\n:{i+1}: Cant compare strings.\r\n{slc}");
              }
              decimal[] da = qu.Select(x=>decimal.Parse(x)).ToArray();
              if(da[1] < da[0]){
                stk.Push(1);
              }else{
                stk.Push(0);
              }
            }else if(q[j] == "LEQ"){
              Ms.Underflow(stk,2,t,i,j);
              string[] qu = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(qu)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
								throw new CalculationException($"\r\n:{i+1}: Cant compare strings.\r\n{slc}");
              }
              decimal[] da = qu.Select(x=>decimal.Parse(x)).ToArray();
              if(da[1] <= da[0]){
                stk.Push(1);
              }else{
                stk.Push(0);
              }
            }else if(q[j] == "GTR"){
              Ms.Underflow(stk,2,t,i,j);
              string[] qu = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(qu)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
								throw new CalculationException($"\r\n:{i+1}: Cant compare strings.\r\n{slc}");
              }
              decimal[] da = qu.Select(x=>decimal.Parse(x)).ToArray();
              if(da[1] > da[0]){
                stk.Push(1);
              }else{
                stk.Push(0);
              }
            }else if(q[j] == "GEQ"){
              Ms.Underflow(stk,2,t,i,j);
              string[] qu = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(qu)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
								throw new CalculationException($"\r\n:{i+1}: Cant compare strings.\r\n{slc}");
              }
              decimal[] da = qu.Select(x=>decimal.Parse(x)).ToArray();
              if(da[1] >= da[0]){
                stk.Push(1);
              }else{
                stk.Push(0);
              }
            }else if(q[j] == "EQU"){
              Ms.Underflow(stk,2,t,i,j);
              if(stk.Pop().ToString() == stk.Pop().ToString()){
                stk.Push(1);
              }else{
                stk.Push(0);
              }
            }else if(q[j] == "STR"){
              str = "";
              op = true;
              fp = false;
            }else if(q[j] == "END"){
              op = false;
              fp = false;
              ilMain.Emit(OpCodes.Ldstr,Ms.FormatText(str));
              stk.Push(Ms.FormatText(str));
            }else if(q[j] == "MOD"){
              Ms.Underflow(stk,2,t,i,j);
              string[] qu = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(qu)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc string % number / number % string\r\n{slc}");
              }else{
                stk.Push(decimal.Parse(qu[1]) % decimal.Parse(qu[0]));
							}
            }else if(q[j] == "ADD"){
              Ms.Underflow(stk,2,t,i,j);
              string[] qu = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(qu)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc string + number / number + string\r\n{slc}");
              }else{
                stk.Push(decimal.Parse(qu[0]) + decimal.Parse(qu[1]));
							}
            }else if(q[j] == "SUB"){
              Ms.Underflow(stk,2,t,i,j);
              string[] qu = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(qu)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc string - number / number - string\r\n{slc}");
              }else{
                stk.Push(decimal.Parse(qu[1]) - decimal.Parse(qu[0]));
							}
            }else if(q[j] == "MUL"){
              Ms.Underflow(stk,2,t,i,j);
              string[] qu = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(qu)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc string * number / number * string\r\n{slc}");
              }else{
                stk.Push(decimal.Parse(qu[0]) * decimal.Parse(qu[1]));
							}
            }else if(q[j] == "DIV"){
              Ms.Underflow(stk,2,t,i,j);
              string[] qu = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(qu)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc string ÷ number / number ÷ string\r\n{slc}");
              }else{
                stk.Push(decimal.Parse(qu[1]) / decimal.Parse(qu[0]));
							}
            }else if(q[j] == "POW"){
              Ms.Underflow(stk,2,t,i,j);
              string[] qu = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(qu)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc string ^ number / number ^ string\r\n{slc}");
              }else{
                stk.Push(decimal.Parse(Math.Pow(double.Parse(qu[1]),double.Parse(qu[0])).ToString()));
							}
            }else if(q[j] == "DMSG"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Peek().ToString();
              Console.WriteLine(h);
              ilMain.Emit(OpCodes.Dup);
              ilMain.EmitCall(OpCodes.Call,mwrite,null);
            }else if(q[j] == "DNMG"){
              Ms.Underflow(stk,1,t,i,j);
              Console.Write(stk.Peek().ToString());
              ilMain.Emit(OpCodes.Dup);
              ilMain.EmitCall(OpCodes.Call,mwrite,null);
            }else if(q[j] == "EMP"){
            }else if(q[j] == "ZET"){
              Ms.Underflow(stk,1,t,i,j);
              string[] h = {stk.Pop().ToString()};
              if(!Ms.IsNum(h)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc ζ(string)\r\n{slc}");
              }else{
                double zeta = 0;
                for(double z = 1; z < 10001; z++){
                  zeta = zeta + (1 / Math.Pow(z,double.Parse(h[0])));
                }
                stk.Push(decimal.Parse(zeta.ToString()));
              }
            }else if(q[j] == "GCD"){
              Ms.Underflow(stk,2,t,i,j);
              string[] h = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(h)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc GCD(string,string) / GCD(string,number) / GCD(number,string)\r\n{slc}");
              }else{
                stk.Push(Ms.Gcd(decimal.Parse(h[0]),decimal.Parse(h[1])));
              }
            }else if(q[j] == "LCM"){
              Ms.Underflow(stk,2,t,i,j);
              string[] h = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(h)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc LCM(string,string) / LCM(string,number) / LCM(number,string)\r\n{slc}");
              }else{
                stk.Push(Ms.Lcm(decimal.Parse(h[0]),decimal.Parse(h[1])));
              }
            }else if(q[j] == "ROT"){
              Ms.Underflow(stk,2,t,i,j);
              string[] h = {stk.Pop().ToString(),stk.Pop().ToString()};
              if(!Ms.IsNum(h)){
                int slcn = 0;
                string slc = string.Join(" ",t.Select(x=>{
                  slcn++;
                  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                }));
                throw new CalculationException($"\r\n:{i+1}: Cant calc string√number / number√string\r\n{slc}");
              }
              stk.Push(decimal.Parse(Math.Pow(double.Parse(h[0]),1 / double.Parse(h[1])).ToString()));
            }else if(q[j] == "MSG"){
              Ms.Underflow(stk,1,t,i,j);
              Console.WriteLine(stk.Pop().ToString());
              ilMain.EmitCall(OpCodes.Call,mwrite,null);
            }else if(q[j] == "NMG"){
              Ms.Underflow(stk,1,t,i,j);
              Console.Write(stk.Pop().ToString());
              ilMain.EmitCall(OpCodes.Call,mwrite,null);
            }else if(q[j] == "EXT"){
              Environment.Exit(0);
            }else if(q[j] == "ERX"){
              Environment.Exit(1);
            }else if(q[j] == "DUP"){
              Ms.Underflow(stk,1,t,i,j);
              stk.Push(stk.Peek().ToString());
            }else if(q[j] == "JBL"){
              Ms.Underflow(stk,1,t,i,j);
              i = int.Parse(stk.Pop().ToString()) - 2;
            }else if(q[j] == "JBP"){
              Ms.Underflow(stk,1,t,i,j);
              j = int.Parse(stk.Pop().ToString()) - 2;
            }else if(q[j] == "LAD"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(lab.ContainsKey(h)){
                lab[h] = i;
              }else{
                lab.Add(h,i);
              }
            }else if(q[j] == "LAA"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!lab.ContainsKey(h)){
                Console.WriteLine("\x1B[1m\x1B[33m(" + (i + 1) + ":" + (j + 1) + ")warn:\x1B[m Label \"" + h + "\" not defined");
                wrn++;
              }else{
                i = int.Parse(lab[h].ToString()) - 1;
              }
            }else if(q[j] == "LEN"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(hash.ContainsKey(h)){
                if(hash[h].GetType() == typeof(System.Collections.Generic.List<string>)){
                  stk.Push(((System.Collections.Generic.List<string>)hash[h]).Count);
                }else{
                  stk.Push(hash[h].ToString().Length);
                }
              }else{
								int slcn = 0;
								string slct = string.Join(" ",t.Select(x=>{
									slcn++;
									return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
								}));
                throw new UndefinedException($"\r\n:{i+1}: Undefined array\r\n{slct}");
              }
            }else if(q[j] == "SLEN"){
              Ms.Underflow(stk,1,t,i,j);
              string v = stk.Pop().ToString();
              stk.Push(v.ToString().Length);
            }else if(q[j] == "APSH"){
              Ms.Underflow(stk,2,t,i,j);
              string h = stk.Pop().ToString();
							string v = stk.Pop().ToString();
							if(hash.ContainsKey(h)){
							  var vl = new System.Collections.Generic.List<string>();
							  try{
                vl = (System.Collections.Generic.List<string>)hash[h];
							  }catch{
                  int slcn = 0;
								  string slct = string.Join(" ",t.Select(x=>{
									  slcn++;
									  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
								  }));
								  throw new UndefinedException($"\r\n:{i+1}: \"{h}\" isnt array\r\n{slct}");
                }
                if(Regex.Replace(h,@"^_[^\n]+$","") == ""){
                  int slcn = 0;
                  string slc = string.Join(" ",t.Select(x=>{
                    slcn++;
                    return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                  }));
                  throw new ReadonlyException($"\r\n:{i+1}: Cant change readonly variable’s value\r\n{slc}");
                }
                vl.Add(v);
                hash[h] = vl;
              }else{
								int slcn = 0;
								string slct = string.Join(" ",t.Select(x=>{
									slcn++;
									return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
								}));
                throw new UndefinedException($"\r\n:{i+1}: Undefined array\r\n{slct}");
              }
            }else if(q[j] == "APOP"){
							Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(hash.ContainsKey(h)){
                var vl = new System.Collections.Generic.List<string>();
                try{
                vl = (System.Collections.Generic.List<string>)hash[h];
                }catch{
                  int slcn = 0;
								  string slct = string.Join(" ",t.Select(x=>{
									  slcn++;
									  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
								  }));
								  throw new UndefinedException($"\r\n:{i+1}: \"{h}\" isnt array\r\n{slct}");
                }
                if(Regex.Replace(h,@"^_[^\n]+$","") == ""){
                  int slcn = 0;
                  string slc = string.Join(" ",t.Select(x=>{
                    slcn++;
                    return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                  }));
                  throw new ReadonlyException($"\r\n:{i+1}: Cant change readonly variable’s value\r\n{slc}");
                }
                string hn = vl[vl.Count - 1].ToString();
                vl.RemoveAt(vl.Count - 1);
                hash[h] = vl;
                stk.Push(hn);
              }else{
                int slcn = 0;
								string slct = string.Join(" ",t.Select(x=>{
									slcn++;
									return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
								}));
                throw new UndefinedException($"\r\n:{i+1}: Undefined array\r\n{slct}");
              }
            }else if(q[j] == "ETF"){
							Ms.Underflow(stk,2,t,i,j);
              string hs = stk.Pop().ToString();
              string h = stk.Pop().ToString();
              if(hash.ContainsKey(h)){
                bool isthisnum = true;
                try{
                  uint.Parse(hs);
                }catch{
                  isthisnum = false;
                }
                if(!isthisnum){
                  int slcn = 0;
								  string slct = string.Join(" ",t.Select(x=>{
									  slcn++;
									  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
								  }));
								  throw new UndefinedException($"\r\n:{i+1}: Index must be a positive int\r\n{slct}");
                }
                var vl = new System.Collections.Generic.List<string>();
                try{
                vl = (System.Collections.Generic.List<string>)hash[h];
                }catch{
                  int slcn = 0;
								  string slct = string.Join(" ",t.Select(x=>{
									  slcn++;
									  return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
								  }));
								  throw new UndefinedException($"\r\n:{i+1}: \"{h}\" isnt array\r\n{slct}");
                }
                string hn = vl[int.Parse(hs)].ToString();
                stk.Push(hn);
              }else{
                int slcn = 0;
								string slct = string.Join(" ",t.Select(x=>{
									slcn++;
									return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
								}));
                throw new UndefinedException($"\r\n:{i+1}: Undefined array\r\n{slct}");
              }
            }else if(q[j] == "ARR"){
              string h = stk.Pop().ToString();
              int cnt = int.Parse(stk.Pop().ToString());
              Ms.Underflow(stk,cnt,t,i,j);
              System.Collections.Generic.List<string> arrl = new System.Collections.Generic.List<string>();
              int cn = 0;
              while(cn < cnt){
                arrl.Add(stk.Pop().ToString());
                cn++;
              }
              arrl.Reverse();
              if(hash.ContainsKey(h)){
                if(Regex.Replace(h,@"^_[^\n]+$","") == ""){
                  int slcn = 0;
                  string slc = string.Join(" ",t.Select(x=>{
                    slcn++;
                    return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                  }));
                  throw new ReadonlyException($"\r\n:{i+1}: Cant change readonly variable’s value\r\n{slc}");
                }
                hash[h] = arrl;
              }else{
                hash.Add(h,arrl);
              }
            }else if(q[j] == "WRT" && ((System.Collections.Generic.List<string>)hash["_INCED"]).Contains("Include.Util.File")){
              Ms.Underflow(stk,2,t,i,j);
              string hs = stk.Pop().ToString();
              string hf = stk.Pop().ToString();
              File.WriteAllText(hf,hs);
            }else if(q[j] == "RED" &&((System.Collections.Generic.List<string>)hash["_INCED"]).Contains("Include.Util.File")){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!File.Exists(h)){
                Console.WriteLine("\x1B[1m\x1B[33m(" + (i + 1) + ":" + (j + 1) + ")warn: \x1B[mFile doesnt exist \"" + h + "\"");
                wrn++;
                stk.Push("\x00");
              }else{
                stk.Push(File.ReadAllText(h));
              }
            }else if(q[j] == "DEL" && ((System.Collections.Generic.List<string>)hash["_INCED"]).Contains("Include.Util.File")){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(!File.Exists(h)){
                Console.WriteLine("\x1B[1m\x1B[33m(" + (i + 1) + ":" + (j + 1) + ")warn: \x1B[mFile doesnt exist \"" + h + "\"");
                wrn++;
              }else{
                File.Delete(h);
              }
            }else if(q[j] == "EXS" && ((System.Collections.Generic.List<string>)hash["_INCED"]).Contains("Include.Util.File")){
              Ms.Underflow(stk,1,t,i,j);
              stk.Push(File.Exists(stk.Pop().ToString()) ? 1 : 0);
            }else if(q[j] == "INP" && ((System.Collections.Generic.List<string>)hash["_INCED"]).Contains("Include.Util.Stdin")){
              Ms.Underflow(stk,1,t,i,j);
              Console.Write(stk.Pop().ToString());
              stk.Push(Console.ReadLine());
            }else if(q[j] == "FNC"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(fnc.ContainsKey(h)){
                fnc[h] = i;
              }else{
                fnc.Add(h,i);
              }
              if(i == p.Length - 1){
                goto END;
              }else{
                break;
              }
            }else if(q[j] == "CLL"){
              Ms.Underflow(stk,1,t,i,j);
              string h = stk.Pop().ToString();
              if(fnc.ContainsKey(h)){
                int nq = 0;
                while(p[(int)fnc[h]].Split(" ")[nq] != "FNC"){
                nq++;
                }
                j = nq;
                i = (int)fnc[h] - 1;
              }else{
                int slcn = 0;
                string slct = string.Join(" ",t.Select(x=>{
                    slcn++;
                    return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                  }));
                throw new UndefinedException($"\r\n:{i+1}: Undefined function\r\n{slct}");
              }
            }else{
              if(hash.ContainsKey(q[j])){
                if(hash[q[j]].GetType() == typeof(System.Collections.Generic.List<string>)){
                  var f = (System.Collections.Generic.List<string>)hash[q[j]];
                  stk.Push("[" + String.Join(",",f.ToArray().Select(x => x.ToString())) + "]");
                }else{
                  stk.Push(Ms.FormatText(hash[q[j]].ToString()));
                }
                int nl = 0;
                int ol = 0;
                foreach(string qs in coul){
                  if(qs == q[j]) nl = ol;
                  ol++;
                }
                ilMain.Emit(OpCodes.Ldloc,nl);
              }else{
                if(Regex.Replace(q[j],@"'[^\n]+'|""[^\n]+""","") == ""){
                  stk.Push(Ms.FormatText(Regex.Replace(q[j],@"'([^\n]+)'|""([^\n]+)""","$1$2")));
                  ilMain.Emit(OpCodes.Ldstr,Regex.Replace(q[j],@"'([^\n]+)'|""([^\n]+)""","$1$2"));
                }else if(Regex.Replace(q[j],@"[0-9-\.]+","") == ""){
                  if(Regex.Replace(q[j],@"-*\.[0-9]+","") == ""){
                    stk.Push(Regex.Replace(q[j],@"\.[0-9]+","0$0"));
                    ilMain.Emit(OpCodes.Ldstr,Regex.Replace(q[j],@"\.[0-9]+","0$0"));
                  }else{
                    ilMain.Emit(OpCodes.Ldstr,q[j]);
                    stk.Push(q[j]);
                  }
                }else{
                  if(hash.ContainsKey(q[j].Split("-")[0]) && hash[q[j].Split("-")[0]].GetType() == typeof(System.Collections.Generic.List<string>)){
                    stk.Push(Ms.FormatText(((System.Collections.Generic.List<string>)hash[q[j].Split("-")[0]])[int.Parse(q[j].Split("-")[1])]));
                  }else{
                    int slcn = 0;
                    string slct = string.Join(" ",t.Select(x=>{
                      slcn++;
                      return (slcn - 1 == j ? ">>> " + x + " <<<" : x);
                    }));
                  throw new UndefinedException($"\r\n:{i+1}: Undefined word\r\n{slct}");
                  }
                }
              }
            }
          }
          END:
          ;
        }
      }catch(Exception err){
        Exception fer = err;
        string ern = err.GetType().ToString().Split(".")[err.GetType().ToString().Split(".").Length - 1];
        if(ern == "OverflowException" || ern == "FormatException"){
          int slcn = 0;
          string slct = string.Join(" ",linect.Select(x=>{
            slcn++;
            return (slcn - 1 == posis ? ">>> " + x + " <<<" : x);
                    }));
          fer = new Exception($"\r\n:{lineis+1}: Overflowed or underflowed\r\n{slct}");
        }
        if(ern == "ArgumentOutOfRangeException"){
          int slcn = 0;
          string slct = string.Join(" ",linect.Select(x=>{
            slcn++;
            return (slcn - 1 == posis ? ">>> " + x + " <<<" : x);
                    }));
          fer = new Exception($"\r\n:{lineis+1}: Index was out of range\r\n{slct}");
        }
        throw fer;
      }
        ilMain.Emit(OpCodes.Ret);
        typeBuilder.CreateType();
        asmBuilder.SetEntryPoint(metBuilder);
       // asmBuilder.Save("a.exe");
        if(File.Exists("a.exe.mdb")) File.Delete("a.exe.mdb");
        //Console.WriteLine("\r\n" + wrn.ToString() + " warns");
      }
    }
  }
}