/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window, $) {
    mini.namespace("AC.RDatabase.ViewDefinition");
    var self = AC.RDatabase.ViewDefinition;
    self.prifix = "AC_RDatabase_ViewDefinition_";
    self.loadData = loadData;

    function loadData() {
        var data = getParams();
        $("#" + self.prifix + "content").load(bootPATH + "../AC/RDatabase/GetViewDefinition?databaseID=" + data.databaseID + "&viewID=" + data.viewID);
    }

    function getParams() {
        var data = {};
        if (self.params && self.params.databaseID) {
            data.databaseID = self.params.databaseID;
            data.viewID = self.params.viewID;
        }
        else {
            data.databaseID = $.deparam.fragment().databaseID || $.deparam.querystring().databaseID;
            data.viewID = $.deparam.fragment().viewID || $.deparam.querystring().viewID;
        }
        return data;
    }
})(window, jQuery);