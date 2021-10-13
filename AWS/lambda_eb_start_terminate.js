/**
 * ElasticBeansTalkを再構築するfunction
 */
// 起動させるEB
const EB_ENVIRONMENT_NAMES = [
    "xxxxxx"
];

var AWS = require('aws-sdk');
AWS.config.region = 'us-west-1';
AWS.config.apiVersions = {
  elasticbeanstalk: '2010-12-01',
};
exports.handler = (event, context, callback) => {
    for (var i = 0, len = EB_ENVIRONMENT_NAMES.length; i < len; i++) {
        ebRebuild(context, EB_ENVIRONMENT_NAMES[i], function(name){
            console.log(name + ". end..");
        });
    }
//    context.done(null, 'done!!');
};

// EB 環境の終了をする
function ebRebuild(context, ebName, callback){
    var params = {
        //EnvironmentId: e-xxxxxid
        EnvironmentName: ebName
    };
    var elasticbeanstalk = new AWS.ElasticBeanstalk();
    elasticbeanstalk.rebuildEnvironment(params, function(err, data) {
        if (err) console.log(err, err.stack); // an error occurred
        else     console.log(data);           // successful response
    });
    callback(ebName);
}
