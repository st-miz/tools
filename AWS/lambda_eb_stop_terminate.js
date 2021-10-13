/**
 * ElasticBeansTalkを停止するfunction
 */
// 停止させるEBの名前
const EB_ENVIRONMENT_NAMES = [
    // 終了する順番も多少大事なのであまり順番を入れ替えない事
    "xxxxx"
];

var AWS = require('aws-sdk');
AWS.config.region = 'us-west-1';
AWS.config.apiVersions = {
  elasticbeanstalk: '2010-12-01',
};
var elasticbeanstalk = new AWS.ElasticBeanstalk();

exports.handler = (event, context, callback) => {
    for (var i = 0, len = EB_ENVIRONMENT_NAMES.length; i < len; i++) {
        ebTerminate(EB_ENVIRONMENT_NAMES[i], null);
    }
    context.done(null, 'EB TerminateEnvironment done.');
};

// EB 環境の終了をする
function ebTerminate(evName, callback){
    var params = {
        //EnvironmentId: e-xxxxxid
        EnvironmentName: evName
    };
    elasticbeanstalk.terminateEnvironment(params, function(err, data) {
    if (err) console.log(err, err.stack); // an error occurred
    else     console.log(data);           // successful response
    /*
    data = {
    AbortableOperationInProgress: false,
    ApplicationName: "my-app",
    CNAME: "my-env.elasticbeanstalk.com",
    DateCreated: <Date Representation>,
    DateUpdated: <Date Representation>,
    EndpointURL: "awseb-e-f-AWSEBLoa-xxxxxxxxxxx.us-west-2.elb.amazonaws.com",
    EnvironmentId: "e-fh2eravpns",
    EnvironmentName: "my-env",
    Health: "Grey",
    SolutionStackName: "64bit Amazon Linux 2015.03 v2.0.0 running Tomcat 8 Java 8",
    Status: "Terminating",
    Tier: {
    Name: "WebServer",
    Type: "Standard",
    Version: " "
    }
    }
    */
    });
    callback();
}