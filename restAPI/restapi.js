var http = require("http");
var express = require('express'); // szerver, amit használsz, külön metódusok a get, post, put és delete-hez
var app = express(); // szerver (példány)
var mysql = require('mysql');
var bodyParser = require('body-parser');
var url = require('url'); // pl ha valami ilyen: /valami/:id -> id parseol-ja

var connection = mysql.createConnection({
    host     : 'localhost',
    user     : 'root',
    password : '',
    database : 'reservations'
});

connection.connect(function(err) {
    if (err) throw err
    console.log('Connected successfully!')
})

app.use( bodyParser.json() );
app.use(bodyParser.urlencoded({
    extended: true
}));

var server = app.listen(3000,  "127.0.0.1", function () {
    var host = server.address().address
    var port = server.address().port
    console.log("Server started at URI: http://%s:%s", host, port)
});

app.get('/login', function (req, res) {
    var quer = url.parse(req.url, true).query;
    var api_req = http.request({
            hostname: '127.0.0.1',
            path: '/restAPI/users_index.php?username='+quer.username+'&password='+quer.password,
            method: 'GET'
        },
        api_res => {
            api_res.on('data', d => {
                var data = JSON.parse(d);
                res.json(data);
            })
        })

    api_req.on('error', error => {
        console.log(error); throw error;
    })
    api_req.end()
});
app.get('/reservation', function (req, res) {
    var api_req = http.request({
            hostname: '127.0.0.1',
            path: '/restAPI/reservation_index.php',
            method: 'GET'
        },
        api_res => {
            api_res.on('data', d => {
                var data = JSON.parse(d);
                res.json(data);
            })
        })
    api_req.on('error', error => {
        console.log(error); throw error;
    })
    api_req.end()
});

var sql = "select * from users;";
var query = connection.query(sql, function (err, results) {
    if (err) throw err;
// results a query eredménye
})