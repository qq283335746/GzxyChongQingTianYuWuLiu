
var uploadifyFun = {
    SmsPhoneUpload: function (clientId) {
        $("#" + clientId + "").uploadify({
            'swf': '../../Scripts/plugins/uploadify/scripts/uploadify.swf',
            'uploader': '../../Handlers/HandlerUpload.ashx',
            'width': 80,
            'height': 26,
            'auto': false,
            'multi': true,
            'buttonText': '选择文件',
            'fileTypeDesc': '支持的文件格式：',
            'fileTypeExts': '*.xls; *.xlsx;',
            'onUploadError': function (file, errorCode, errorMsg, errorString) {
                $.messager.alert('系統提示', '文件上传失败', 'info');
            },
            'onUploadSuccess': function (file, data, response) {
                $("#txtaImportData").text(data);
            },
            'onQueueComplete': function (queueData) {
            }
        })
    }
}