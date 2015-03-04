function timeSpan(count) {
  var sec = count % 60;
  var min = (count - sec) / 60;
  var sec1 = sec % 10;
  var sec10 = (sec - sec1) / 10;

  var result = min + ':' + sec10;
  result += sec1;
  return result;
};

$(function () {
  var count = 0;
  var timetag = $("#timetag");

  setInterval(function () {
    count++;
    var sec = count % 60;
    timetag.html(timeSpan(count));
  }, 1000);
});
