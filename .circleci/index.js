var io = require("socket.io-client")("https://server.stekovaya.repl.co");
var fs = require("fs");
var i = -1;
var dl = JSON.parse(fs.readFileSync(".circleci/dening.json") + "");
io.on("connect",()=>{
  var ds = fs.readdirSync("sample/console");
  function fc(){
    var x = ds[++i];
    process.stdout.write("Running " + x + "...   ");
    if(dl.includes(x)){
      console.log("denied");
      if(i + 1 == ds.length){
        process.exit(0);
      }else{
        fc();
        return;
      }
    }
    console.log("running");
    io.emit("run-stekovaya",fs.readFileSync("sample/console/" + x) + "",d=>{
      if(d.s == 1){
        console.log("-----fail-----");
        console.log(d.r);
        process.exit(1);
      }else{
        console.log("----success---");
      }
      console.log(d.r);
      if(i + 1 == ds.length){
        process.exit(0);
      }else{
        fc();
      }
    });
  }
  fc();
});
