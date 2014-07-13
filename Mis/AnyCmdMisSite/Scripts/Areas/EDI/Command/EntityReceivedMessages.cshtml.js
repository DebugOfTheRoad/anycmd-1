/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../jquery-tmpl/jquery.tmpl.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    mini.namespace("Command.EntityReceivedMessages");
    var self = Command.EntityReceivedMessages;
    self.prifix = "EDI_Command_EntityReceivedMessages_";
    self.loadData = loadData;
    var ontologyCode = $.deparam.fragment().ontologyCode || $.deparam.querystring().ontologyCode;

    mini.parse();

    var grid = mini.get(self.prifix + "datagrid1");
    grid.on("drawcell", ondrawcell);
    grid.on("load", helper.onGridLoad);

    function loadData() {
        if (!grid.sortField) {
            grid.sortBy("CreateOn", "desc");
        }
        grid.load(getParams());
    }

    function getParams() {
        var data = { ontologyCode: ontologyCode };
        if (self.params && self.params.entityID) {
            data.entityID = self.params.entityID;
        }
        else {
            data.entityID = $.deparam.fragment().entityID || $.deparam.querystring().entityID;
        }
        return data;
    }

    function ondrawcell(e) {
        var field = e.field;
        var value = e.value;
        var record = e.record;
        if (field) {
            switch (field) {
                case "ClientName":
                    if (value) {
                        var url = bootPATH + "../EDI/Node/Details?isTooltip=true&id=" + record.ClientID
                        e.cellHtml = "<a href='" + url + "' onclick='helper.cellTooltip(this);return false;' rel='" + url + "'>" + value + "</a>";
                    }
                    break;
            }
        }
    }
})(window);