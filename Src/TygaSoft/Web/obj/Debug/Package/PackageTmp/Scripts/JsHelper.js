
var jsHelper = {
    OpenWaiting: function () {
        $("#dlgWaiting").dialog('open');
    },
    CloseWaiting: function () {
        $("#dlgWaiting").dialog('close');
    },
    BindCbb: function (cbbId, myDataForClientId) {
        var currData = $("#" + myDataForClientId + "").html();
        if (currData != undefined && currData.length > 0) {
            var json = eval("(" + currData + ")");
            $("#" + cbbId + "").combobox({
                data: json
            });
        }
    },
    FloatTryParse: function (vl) {
        var regex = /^(\d+)|(\d+)\.?(\d+)$/;
        if (!regex.test(vl)) {
            return false;
        }
        return true;
    },
    StartProgressbar: function () {
        var value = $('#pgsBar').progressbar('getValue');
        if (value < 90) {
            value += Math.floor(Math.random() * 10);
            $('#pgsBar').progressbar('setValue', value);
            setTimeout(arguments.callee, 300);
        }
        else if (value >= 90 && value < 99) {
            value += 1;
            $('#pgsBar').progressbar('setValue', value);
            setTimeout(arguments.callee, 300);
        }
    },
    EndProgressbar: function () {
        var value = $('#pgsBar').progressbar('getValue');
        if (value < 100) {
            value += Math.floor(Math.random() * 10);
            $('#pgsBar').progressbar('setValue', value);
            setTimeout(arguments.callee, 300);
        }
    },
    OnProgressbarChange: function (value) {
        if (value >= 100) {
            alert(value);
            $("#dlgPgsbar").dialog('close');
        }
    },
    OnDlgClose: function () {
        $("#dlgPgsbar").dialog('close');
    },
    IsLogin: function (url) {

        var myDataForUserInfo = $("#myDataForUserInfo");
        if (myDataForUserInfo == undefined) return false;
        if (myDataForUserInfo.html() == undefined) return false;
        var json = eval("(" + myDataForUserInfo.html() + ")");
        $.map(json, function (item) {
            if (item.UserIsLogin != "1") {
                jsHelper.HiddenValue = url;
                jsHelper.OnLogin("open");
            }
            else {
                if (url != "") {
                    window.location = url;
                }
            }
        })
    },
    OnLogin: function (v) {
        if (v == "open") {
            $('#dlgLogin').dialog('open');
            return false;
        }
        var isValid = $('#dlgLogin').form('validate');
        if (!isValid) return false;

        var userName = $("#txtUserName").val();
        var psw = $("#txtPsw").val();
        var vc = $("#txtVc").val();

        $.ajax({
            url: "/ScriptServices/SharesService.asmx/Login",
            type: "post",
            contentType: "application/json; charset=utf-8",
            data: '{userName:"' + userName + '",psw:"' + psw + '",vc:"' + vc + '"}',
            contentType: "application/json; charset=utf-8",
            beforeSend: function () {
                $("#dlgWaiting").dialog('open');
            },
            complete: function () {
                $("#dlgWaiting").dialog('close');
            },
            success: function (data) {
                var msg = data.d;
                if (msg == "1") {
                    if (jsHelper.HiddenValue != "") {
                        window.location = jsHelper.HiddenValue;
                    }

                    $('#dlgLogin').dialog('close');
                }
                else {
                    $.messager.alert('系统提示', msg, 'info');
                }
            }
        });
    },
    HiddenValue: ''
}