
var shareNav = {
    Init: function () {
        shareNav.navHover();
        shareNav.navCurr();
    },
    navHover: function () {
        $("#nav a").hover(function () {
            $(this).addClass("hover").siblings().removeClass("hover");
        }, function () {
            $(this).removeClass("hover");
        })
    },
    navCurr: function () {
        var sps = $("#SitePaths>span");
        var s = "首页";
        if (sps.length > 2) {
            s = sps.eq(2).text();
        }
        var currNav = $("#nav").find("a").filter(":contains('" + s + "')");
        currNav.addClass("curr").siblings().removeClass("curr");
    },
    getWest: function () {
        //左边菜单导航
        $.ajax({
            url: "/ScriptServices/SharesService.asmx/GetSiteHelper",
            type: "post",
            contentType: "application/json; charset=utf-8",
            success: function (html) {
                $("#menuNav").html(html.d);
                $("#menuNav").accordion({
                    fit: true,
                    border: false
                });
                var hoverA = $("#menuNav").find("a[class=hover]");
                $(".layout-panel-center>.panel-header>.panel-title").text(hoverA.parent().prev().find("[class=panel-title]").text() + ">" + hoverA.text());
            }
        });
    }
}