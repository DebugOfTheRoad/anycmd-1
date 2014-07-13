/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../jquery-tmpl/jquery.tmpl.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
/// <reference path="../../../miniui/miniui.js" />
(function (window) {
    mini.namespace("Help.Details");
    var self = Help.Details;

    var divID = "Help_Details";
    var templateID = "Help_DetailsTemplate";
    var $templateID = "#" + templateID;
    var $divID = "#" + divID;

    self.loadData = loadData;

    $().ready(function () {
        if (location.pathname.toLowerCase() == "/AC/help/details") {
            var params = getParams();
            if (params && (params.id || params.code)) {
                self.loadData();
            }
        }
    });

    function loadData() {
        helper.requesting();
        $.getJSON(
            bootPATH + "../AC/Help/GetInfo",
            getParams(),
            function (result) {
                helper.response(result, function () {
                    $($divID).empty();
                    $($templateID).tmpl(result).appendTo($divID);
                    $("#content").html(result.Content);
                });
                helper.responsed();
            });
    }

    function getParams() {
        if (self.params && self.params.id) {
            return self.params;
        }
        else {
            var fragment = $.deparam.fragment();
            var querystring = $.deparam.querystring();
            return { id: fragment.id || querystring.id };
        }
    }
})(window);