
var MenusFun = {
    Init: function () {
        MenusFun.LeftMenuHover();
        MenusFun.SelectCurrent();
        //        MenusFun.Hover();
    },
    Hover: function () {
        $(".nav a").hover(function () {
            $(this).addClass("hover").siblings().removeClass("hover");
        }, function () {
            $(this).removeClass("hover")
        })
    },
    LeftMenuHover: function () {
        $(".side-menu li").hover(function () {
            $(this).addClass("current").siblings().removeClass("current");
        }, function () {
            $(this).removeClass("current")
        })
    },
    SelectCurrent: function () {
        var currMenu = $("#SitePaths>span:last");
        if (currMenu != undefined) {
            $(".side-menu li").filter(":contains('" + currMenu.text() + "')").addClass("current").siblings().removeClass("current");
        }
    }
};

var UserMenus = {
    Init: function () {
        //UserMenus.TreeLoad();
    },
    TreeLoad: function () {
        var t = $("#menuTree");
        $.ajax({
            url: "/ScriptServices/UsersService.asmx/GetTreeJsonForMenu",
            type: "post",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            success: function (json) {
                var jsonData = (new Function("", "return " + json.d))();
                t.tree({
                    data: jsonData,
                    formatter: function (node) {
                        if (node.id.length > 0) {
                            return "<a href=\"" + node.id + "\">" + node.text + "</a>";
                        }
                        return node.text;
                    },
                    animate: true
                })
                UserMenus.SelectCurrent();
                t.children().children("div:first").hide();
                UserMenus.AddMenuClass();
            }
        });
    },
    SelectCurrent: function () {
        var currMenu = $("#SitePaths>span:last").text();
        $("#menuTree").find("a").each(function () {
            if ($(this).text() == currMenu) {
                $(this).parent().parent().addClass("current");
            }
        })
    },
    AddMenuClass: function () {
        var className = "order,customer,product,pay,message,report";
        var arr = className.split(",");
        var menus = $("#menuTree").find("a:not(:first)");
        menus.each(function (index, item) {
            var currClass = arr[index];
            $(this).addClass(currClass);
        })
    }
};

var AdminMenus = {
    Init: function () {
        //AdminMenus.InitAccordion();
        //AdminMenus.InitLayout();
        AdminMenus.TreeLoad();
        //AdminMenus.InitTabs();
    },
    TreeLoad: function () {
        var t = $("#eastTree");
        $.ajax({
            url: "/ScriptServices/AdminService.asmx/GetTreeJsonForMenu",
            type: "post",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            success: function (json) {
                var jsonData = (new Function("", "return " + json.d))();
                t.tree({
                    data: jsonData,
                    formatter: function (node) {
                        if (node.id.length > 0) {
                            return "<a href=\"" + node.id + "\">" + node.text + "</a>";
                        }
                        return node.text;
                    },
                    animate: true
                })
                //AdminMenus.OnCurrExpand();
            }
        });
    }
    
};