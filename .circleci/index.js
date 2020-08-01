var io = require("socket.io-client")("https://server.stekovaya.repl.co");
var fs = require("fs");
var i = 0;
io.on("connect",()=>{
  var ds = fs.readdirSync("sample/console");
  var fc = (function(){
    i++;
    var x = ds[i - 1];
    if(x == "likerogue.stk" || x == "montecarlo.stk" || x == "pi.stk" || x == "fibonacci.stk" || x == "yourname.stk") return;
    console.log("Running " + x + "...");
    io.emit("run",fs.readFileSync("sample/console/" + ds[i - 1]) + "",d=>{
      if(d.s == 0){
        console.log("---success");
      }else{
        console.log("---fail");
        console.log(d.r);
        process.exit(1);
      }
      console.log(d.r);
      if(i == ds.length){
        process.exit(0);
      }else{
        fc();
      }
    });
  });
  fc();
});
