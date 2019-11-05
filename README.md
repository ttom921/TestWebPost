# TestWebPost
因為公司有需要來測試一下，有關上傳檔案的測試，所以來測試一下，會來架設一台server和使用C#來上傳檔案。

在安裝Express之前來建立一各資料夾

```
mkdir myweb
cd myweb
```

## 再來建立一個 package.js 檔案

```
npm init
```

阿就一直按 Enter 就好了！
安裝 Express

```
npm install express --save
```

在 `myapp` 目錄中，建立 `index.js` 檔案

我是vscode來執行此程式

因為要處理有關網頁的東西所以來安裝一些額的套件。

```
npm install multer
```

 這個套件處理有關 multipart/form-data 

```
npm install body-parser
```

```javascript
//基本宣告使用
var express = require('express');
var app = express();
var multer = require('multer');
var bodyParser = require('body-parser');
var app = Express();
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
app.post("/api/Upload", function (req, res) {
    upload(req, res, function (err) {
        if (err) {
            return res.end("Something went wrong!");
        }
        return res.end("File uploaded sucessfully!.");
    });
});


app.listen(3000, function () {
    console.log('Example app listening on port 3000!');
});
```

在下面建立`indexl.html`

```html
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>Upload images to server using Node JS</title>
    <script src="Scripts/jquery-3.1.1.min.js"></script>
    <script src="Scripts/jquery.form.min.js"></script>
    <script>
        $(document).ready(function () {
            var options = {
                beforeSubmit: showRequest,  // pre-submit callback
                success: showResponse  // post-submit callback
            };

            // bind to the form's submit event
            $('#frmUploader').submit(function () {
                $(this).ajaxSubmit(options);
                // always return false to prevent standard browser submit and page navigation
                return false;
            });
        });

        // pre-submit callback
        function showRequest(formData, jqForm, options) {
            alert('Uploading is starting.');
            return true;
        }

        // post-submit callback
        function showResponse(responseText, statusText, xhr, $form) {
            alert('status: ' + statusText + '\n\nresponseText: \n' + responseText );
        }
    </script>
</head>
<body>
    <form id="frmUploader" enctype="multipart/form-data" action="api/Upload/" method="post">
        <input type="file" name="imgUploader" multiple />
        <input type="submit" name="submit" id="btnSubmit" value="Upload" />
    </form>
</body>
</html>
```

在下面要建立資料夾`Scripts`可以來放jquery

## Wireshark相關

要選擇**Npcap Loopback Adapter**，在過濾器上輸入`tcp.port == 3000`

查看封包的head，是先選擇有Post如`POST /FMS_WS/Services/API/FileUpload/ HTTP/1.1  (image/jpeg)`在下面有`Hypertext Transfer Protocol`下面可以看到















## 參考資料



[透過 HttpWebRequest 模擬Client上傳檔案](http://no2don.blogspot.com/2012/11/caspnet-httpwebrequest-client.html)

[WireShark学习之抓取和分析HTTP数据包](https://www.centos.bz/2017/10/wireshark%E5%AD%A6%E4%B9%A0%E4%B9%8B%E6%8A%93%E5%8F%96%E5%92%8C%E5%88%86%E6%9E%90http%E6%95%B0%E6%8D%AE%E5%8C%85/)