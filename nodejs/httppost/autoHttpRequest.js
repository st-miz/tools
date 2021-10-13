//modules
const httpRequest = require('./HttpRequest');
const fs = require('fs');

//入力パラメータ
// 1)URLを設定する(default値)
// ただしjson fileにuriが記載してあればそちらを採用する
//const uri = 'https://www6.mobilecall.jp/api/all/all/3.0/svc/';
//const uri = 'http://www-test.mobilecall.jp/api/all/all/3.0/svc/';
const uri = 'http://localhost:8080/all/3.0/svc/xxxxxxxx';
// 2)対象のrequest内容が記載されたコンテンツ用ファイル名称(送信したいjsonファイルの数だけ定義)
// const targetRequestFile = ["./request_json/checkarea.json", "./request_json/PriceEstimate.json"];
const targetRequestFile = ["./request_json/CheckWideArea.json"];

// 3)httpヘッダや必須に近いリクエスト関連の項目をあげる
const headers = {
  "Accept":"Application/json",
  "Content-Type": "application/json",
  "charset":"utf-8",
  "User-Agent": "request_test / test 1.0.0"
};

//実際に送信する
function httpPost(requestData){
    let targetUri = (requestData.uri) ? requestData.uri : uri;
    let options = {
        uri: targetUri,
        json: true,
        headers: headers,
        timeout: 60 * 1000,
        body: requestData.body
    };
    httpRequest.httpPost( options, function(isSuccess, body, error, response) {
        //console.log(JSON.stringify(body, null, "  "));
    });
}

//fileからリクエストデータを作成する
const createRequestData = function (jsonContents){
    let request = {};
    let hasBody = jsonContents.body;
    if (hasBody) {
        request.body = jsonContents.body;
    } else {
        request.body = {};//empty!
    }

    if (jsonContents.uri){
        request.uri = jsonContents.uri;
    } else {
        request.uri = uri;
    }
    return request;
}

//jsonfile中身(コンテンツ)を読み取る
const readContentsFile = function (filePath, callback){
    fs.stat(filePath, (error, stats) => {
        if (error) {
            console.log('file erro..path=' + filePath + ", " + error);
        } else {
            //console.log('file exists..path=' + filePath);
            fs.readFile(filePath, 'utf8', (err, data) => {
                if (err) {
                    throw err;
                }
                let req = callback(JSON.parse(data));
                httpPost(req);
            }); // end of fs.readFile
        }
    }); //end of fs.stat
}
// main start!
// fileを読み取って指定先へpost送信する
targetRequestFile.forEach(f => 
    readContentsFile(f, createRequestData)
);