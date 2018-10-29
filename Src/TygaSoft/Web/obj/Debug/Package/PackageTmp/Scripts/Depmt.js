var depmtFun = {
    BindCbt: function (clientId) {
        var cbt = $("#" + clientId + "");
        var t = cbt.combotree('tree');

        $.ajax({
            url: "/ScriptServices/UsersService.asmx/GetJsonForDepmt",
            type: "post",
            data: "{}",
            async: true,
            contentType: "application/json; charset=utf-8",
            success: function (json) {
                var jsonData = (new Function("", "return " + json.d))();
                t.tree({
                    data: jsonData,
                    animate: true
                })
                depmtFun.OnCurrExpand(t);
            }
        });
    },
    OnCurrExpand: function (t) {
        var root = t.tree('getRoot');
        if (root) {
            var childNodes = t.tree('getChildren', root.target);
            if (childNodes && (childNodes.length > 0)) {
                var cnLen = childNodes.length;
                for (var i = 0; i < cnLen; i++) {
                    t.tree('collapse', childNodes[i].target);
                }
            }
        }
        var currNode = t.tree('find', $("#hCurrExpandNode").val());
        if (currNode) {
            currFun.OnExpand(t, currNode);
        }
    },
    OnExpand: function (t, node) {
        if (node) {
            t.tree('expand', node.target);
            var pNode = t.tree('getParent', node.target);
            if (pNode) {
                currFun.OnExpand(t, pNode);
            }
        }
    }
}