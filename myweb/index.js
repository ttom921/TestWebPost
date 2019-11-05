//基本宣告使用
var express = require('express');
var app = express();
var multer = require('multer');
var bodyParser = require('body-parser');
app.use(bodyParser.json());

//檔案的儲存位置
var Storage = multer.diskStorage({
    destination: function (req, file, callback) {
        callback(null, "./Images");
    },
    filename: function (req, file, callback) {
        callback(null, file.fieldname + "_" + Date.now() + "_" + file.originalname);
    }
});
var upload = multer({ storage: Storage }).array("imgUploader", 3); //Field name and max count 

//網頁相關
app.get("/", function (req, res) {
    res.sendFile(__dirname + "/index.html");
});
//上傳api
app.post("/FMS_WS/Services/API/FileUpload", function (req, res) {
    upload(req, res, function (err) {
        //console.log(req.headers.filename);
        if (err) {
            return res.end("Something went wrong!");
        }
        return res.end("File uploaded sucessfully!.");
    });
});
//上傳api
// app.post("/api/Upload", function (req, res) {
//     upload(req, res, function (err) {
//         if (err) {
//             return res.end("Something went wrong!");
//         }
//         return res.end("File uploaded sucessfully!.");
//     });
// });


app.listen(3000, function () {
    console.log('Example app listening on port 3000!');
});