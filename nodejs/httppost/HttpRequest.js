var request = require('request');

//次をコール
//httpPost(task.options, task.errorResultCallback, next);

function httpPost(options, resultCallback) {
    options.timer = new Date(); //処理速度計測用
    request.post( options, function(error, response, body) {
        //node error
        if(error) {
            errorMsg("error:" + error, options);
            resultCallback(false, body, error, response);
            return;
        }

        let logBuf  = "-----------------\n";
        // logBuf += "response.statusCode : " + response.statusCode + "\n";
        // logBuf += "body" + body + "\n";

        logBuf += "> uri: " + options.uri + "\n";

        //http status code
        if(response.statusCode != 200) {
            errorMsg("error response.statusCode != 200", options);
            resultCallback(false, body, error, response);
            return;
        }

        //body check 
        if(!body) {
            errorMsg("error body is null", options);
            resultCallback(false, body, error, response);
            return;
        }

        logBuf += "Result : Success\n";
        //logBuf += "Time: " + time + "\n";
        logBuf += "Time: " + (new Date() - options.timer) + " ms \n";
        logBuf += "Request : " + JSON.stringify(options.body) + "\n";
        logBuf += "Response.StatusCode : " + response.statusCode + "\n";
        logBuf += "> Response\n";
        logBuf += JSON.stringify(body,null,"   ");
        logBuf += "\n-----------------\n";
        console.log(logBuf);
        //success
        resultCallback(true, body, error, response);
    }); //end of reques.post
}

function errorMsg( msg, object ) {
    console.log( "************\nFailed\n" + msg + "\n************");
}

exports.httpPost = httpPost;