/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../jquery-tmpl/jquery.tmpl.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
/// <reference path="../../../miniui/miniui.js" />
(function (window) {
    // TODO:考虑从url上获取controller是否合适)
    var href = trimSlash(window.location.href);
    href = href.substring(0, href.lastIndexOf("/"));
    var explicitController = $.deparam.querystring().controller || $.deparam.fragment().controller;
    var explicitentityTypeCode = $.deparam.querystring().entityTypeCode || $.deparam.fragment().entityTypeCode;
    var controller = explicitController;
    var entityTypeCode = explicitentityTypeCode;
    var implicitController;

    if (!controller) {
        implicitController = href.substring(href.lastIndexOf('/') + 1)
        controller = implicitController;
    }
    if (!entityTypeCode) {
        entityTypeCode = implicitController;
        if (!entityTypeCode) {
            alert("出错了，解析不出entityTypeCode");
        }
    }
    
    var s = entityTypeCode + ".Details";
    var divID = entityTypeCode + "_Details";
    var templateID = entityTypeCode + "_DetailsTemplate";
    var $divID = "#" + divID;
    var $templateID = "#" + templateID;
    mini.namespace(s);
    var self = eval(s);
    $().ready(function () {
        var area = $($divID).attr("area");
        var url = bootPATH + "../" + area + "/" + controller + "/GetInfo";
        self.loadData = function () {
            helper.requesting();
            $.getJSON(
                url,
                getParams(),
                function (result) {
                    helper.response(result, function () {
                        $($divID).empty();
                        if (result) {
                            $($templateID).tmpl(result).appendTo($divID);
                        }
                    });
                    helper.responsed();
                });
        };
        if (location.pathname.toLowerCase() == "/" + area.toLowerCase() + "/" + controller.toLowerCase() + "/details") {
            var params = getParams();
            if (params && params.id) {
                self.loadData();
            }
        }
    });

    function getParams() {
        if (self.params && self.params.id) {
            return self.params;
        }
        else {
            return { id: $.deparam.fragment().id || $.deparam.querystring().id };
        }
    }

    function trimSlash(href) {
        var index = href.lastIndexOf("?");
        if (index > 0) {
            href = href.substring(0, index);
        }
        index = href.lastIndexOf('/');
        if (index == href.length) {
            href = href.substring(0, href.length - 1);
            trimSlash(href);
        }
        return href;
    }
})(window);