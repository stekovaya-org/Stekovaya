var io = require("socket.io-client")("https://server.stekovaya.repl.co");
var fs = require("fs");
io.on("connect",()=>{
  fs.readdirSync("sample/console").forEach(x=>{
    if(x == "likerogue.stk" || x == "montecarlo.stk" || x == "pi.stk" || x == "fibonacci.stk" || x == "yourname.stk") return;
    console.log("Running " + x + "...");
    io.emit("run",fs.readFileSync("sample/console/" + x) + "",d=>{
      if(d.s == 0){
        console.log("---success");
      }else{
        console.log("---fail");
        console.log(d.r);
        process.exit(1);
      }
      console.log(d.r);
      return;
    });
  });
  process.exit(0);
});
