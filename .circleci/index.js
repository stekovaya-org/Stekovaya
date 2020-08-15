var fs = require("fs");
var i = -1;
var dl = JSON.parse(fs.readFileSync(".circleci/dening.json") + "");
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
  var d = {};
  try{
    d.r = require("child_process").execSync("mono stkvy.exe sample/console/" + x + " 2>&1") + "";
    d.s = 0;
  }catch(err){
    d.r = err.stdout + "";
    d.s = 1;
  }
  if(d.s == 1){
    console.log("-----\x1b[1mfail\x1b[m-----");
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
}
fc();
