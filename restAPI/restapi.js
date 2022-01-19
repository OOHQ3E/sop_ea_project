var http = require("http");
var express = require('express');
var app = express();
var mysql = require('mysql');
var bodyParser = require('body-parser');
var url = require('url');
const swaggerUI = require("swagger-ui-express");
const swaggerDocument = require('./openapi.json');
const swaggerJsDoc = require("swagger-jsdoc");

const options = {
    definition: {
        openapi: "3.0.0",
        info: {
            title: "OpenAPI for Reservation table",
            version: "1.0.0",
            description: "This is an OpenAPI for Reservation table - a project for Service Oriented Programming Lecture",
        },
        servers: [
            {
                url: `http://127.0.0.1:1234`,
            },
        ],
    },
    apis: ["./*.js"],
};
const specs = swaggerJsDoc(options);
app.get("/openapi.json", (req, res) => res.json(swaggerDocument))
app.use("/docs", swaggerUI.serve, swaggerUI.setup(swaggerDocument));

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

var server = app.listen(1234,  "127.0.0.1", function () {
    var host = server.address().address
    var port = server.address().port
    console.log("Server started at URI: http://%s:%s", host, port)
});

app.get('/login', function (req, res) {
    var quer = url.parse(req.url, true).query;
    if (!quer.username || !quer.password){
        //console.log("Username and password is required!")
        res.end("Username and password is required!")
        return;
    }
    else{
        var api_req = http.request({
                hostname: '127.0.0.1',
                path: '/restAPI/userLogin.php?username='+quer.username+'&password='+quer.password,
                method: 'GET'
            },
            api_res => {
                api_res.on('data', d => {
                    var data = JSON.parse(d);
                    if (data == false){
                        //console.log("There is no such profile with the given data! Please try again!")
                        res.end("There is no such profile with the given data! Please try again!")
                    }
                    else{
                        res.json(data);
                    }
                })
            })

        api_req.on('error', error => {
            console.log(error); throw error;
        })
        api_req.end()
    }

});
app.get('/reservation', function (req, res) {
    var quer = url.parse(req.url,true).query;
    if (!quer.username  || !quer.password){
        //console.log("No username or password added");
        res.end("No username or password added")
    }
    else{
        var sql = "select count(name) as count from users where name='"+quer.username+"' and password='"+quer.password+"'";
        var query = connection.query(sql, function (err, results) {
            if (err) throw err;
            if (results[0].count > 0){
                var api_req = http.request({
                        hostname: '127.0.0.1',
                        path: '/restAPI/reservation.php',
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
            }
            else{
                res.end("You must be logged in to use this function!");
            }
        })
    }

});
app.post('/reservation',function (req,res){
    var quer = req.body;
    if (!quer.reservator  || !quer.password){
        //console.log("No username or password added");
        res.end("No username or password added")
    }
    else{
        var sql = "select count(name) as count from users where name='"+quer.reservator+"' and password='"+quer.password+"'";
        var query = connection.query(sql, function (err, results) {
            if (err) throw err;
            if (results[0].count > 0){
                var sql = "select count(reservedBy) as count from reservation where seatRow="+(quer.rownum)+" and seatColumn="+(quer.columnnum)+"";
                var query = connection.query(sql, function (err, results) {
                    if (err) throw err;
                    if(results[0].count == 0){
                        var sql = "insert into reservation(reservedBy,seatRow,seatColumn) values('"+quer.reservator+"',"+quer.rownum+","+quer.columnnum+")";
                        var query = connection.query(sql, function (err, results) {
                            if (err) throw err;
                            else{
                                res.end("Reservation for "+quer.reservator+" was successful!");
                            }
                        })
                    }
                    else{
                        res.end("This seat (col: "+(quer.columnnum+1)+",row: "+(quer.rownum+1)+") is already taken, please choose a different one, if possible! - updating table")
                    }
                })
            }
            else{
                res.end("You must be logged in to use this function!");
            }
        })
    }
})
app.delete('/reservation',function (req,res){
var params = url.parse(req.url,true).query;
    if (!params.username  || !params.password){
        //console.log("No username or password added");
        res.end("No username or password added")
    }
    else{
        var sql = "select count(name) as count from users where name='"+params.username+"' and password='"+params.password+"' and Admin=1";
        var query = connection.query(sql, function (err, results) {
            if (err) throw err;
            if (results[0].count > 0){
                var sql;
                if(!params.reservedby){
                    sql = "delete from reservation where id="+params.id+"";
                }
                else{
                    sql = "delete from reservation where reservedBy='"+params.reservedby+"'";
                }
                var query = connection.query(sql, function (err, results) {
                    if (err) throw err;
                    else{
                        res.end("Successful deletion!");
                    }
                })
            }
            else{
                res.end("You must be logged in AND be an admin to use this function!");
            }
        })
    }
})

app.put('/reservation',function (req,res){
    var quer = req.body;
    if (!quer.username  || !quer.password){
        //console.log("No username or password added");
        res.end("No username or password added")
    }
    else{
        var sql = "select count(name) as count from users where name='"+quer.username+"' and password='"+quer.password+"' and Admin=1";
        var query = connection.query(sql, function (err, results) {
            if (err) throw err;
            if (results[0].count > 0){
                if (isNaN(quer.seatrow)){
                    return res.end("Invalid input! (must be number)")
                }
                if ( isNaN(quer.seatcolumn)){
                    return res.end("Invalid input! (must be number)")
                }
                if (quer.seatrow > 20){
                    return res.end("The given row is bigger than 20!")
                }
                if (quer.seatcolumn > 20){
                    return res.end("The given column is bigger than 20!")
                }
                if (quer.seatrow <= 0){
                    return res.end("The given row is smaller than 1!")
                }
                if (quer.seatcolumn <= 0){
                    return res.end("The given column is smaller than 1!")
                }

                var sql = "select count(reservedBy) as count from reservation where seatRow="+(quer.seatrow-1)+" and seatColumn="+(quer.seatcolumn-1)+"";
                var query = connection.query(sql, function (err, results) {
                    if (err) throw err;
                    if(results[0].count == 0){
                        var sql = "update reservation set seatRow ="+(quer.seatrow-1)+", seatColumn="+(quer.seatcolumn-1)+" where id="+quer.id+"";
                        var query = connection.query(sql, function (err, results) {
                            if (err) throw err;
                            else{
                                res.end("Successful edit!");
                            }
                        })
                    }
                    else{
                    res.end("This seat is already taken, cannot modify to it!")
                    }
                })
            }
            else{
                res.end("You must be logged in AND be an admin to use this function!");
            }
        })
    }
})
