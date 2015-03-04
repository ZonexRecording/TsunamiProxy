var express = require('express');
var router = express.Router();

var serverCollection = new Array();
function addserver(serverdata) {
  updated = false;
  for(var i = 0; i < serverCollection.length; i++)
    if (serverCollection[i].name == serverdata.name) {
      serverCollection[i] = serverdata;
      updated = true;
      break;
    }
  if (!updated)
    serverCollection.push(serverdata);
}

var time1 = new Date().getTime() / 1000;
var time2 = new Date().getTime() / 1000 - 200;
var time3 = new Date().getTime() / 1000 - 500;

/* GET home page. */
router.get('/', function(req, res, next) {
  res.render('index', {
    title: 'Express',
    time: new Date().getTime() / 1000,
    servers: serverCollection
  });
});


router.put('/', function (req, res, next) {
  var jdata = '';
  var data;
  var ipAddress = req.socket.remoteAddress;
  req.on('data', function (chunk) {
    if (chunk != undefined)
      jdata += chunk;
  });
  req.on('end', function (chunk) {
    if(chunk != undefined)
      jdata += chunk;
    data = JSON.parse(jdata);

    if (data != undefined && data.name != undefined) {
      data.ip = ipAddress;
      data.time = new Date().getTime() / 1000;
      addserver(data);
    }

    res.end(ipAddress);
  });
});

module.exports = router;
