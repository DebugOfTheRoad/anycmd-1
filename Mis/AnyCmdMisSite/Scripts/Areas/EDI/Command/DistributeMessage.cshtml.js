﻿/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
(function (window) {
    mini.namespace("Command.DistributeMessage");
    var self = Command.DistributeMessage;
    self.prifix = "EDI_Command_DistributeMessage_";
    self.search = search;
    var ontologyCode = $.deparam.fragment().ontologyCode || $.deparam.querystring().ontologyCode;
    self.gridReload = function () {
        grid.reload();
    };
    self.loadData = loadData;
    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../EDI/Command/Details?entityTypeCode=DistributeMessage",
            entityTypeCode: 'DistributeMessage',
            controller: "Command",
            params: [
                { "pName": 'commandType', "pValue": "Distribute" },
                { "pName": 'id', "pValue": "Id" },
                { "pName": 'ontologyCode', "pValue": "Ontology" }],
            namespace: "DistributeMessage.Details"
        }
    };

    mini.parse();

    var actionCode = mini.get(self.prifix + "actionCode");
    var nodeID = mini.get(self.prifix + "nodeID");
    actionCode.on("valuechanged", search);
    if (nodeID) {
        nodeID.on("valuechanged", search);
    }
    var tabs1 = mini.get(self.prifix + "tabs1");
    var grid = mini.get(self.prifix + "datagrid1");
    grid.on("drawcell", ondrawcell);
    var helperDrawcell = helper.ondrawcell(self, "Command.DistributeMessage");
    grid.on("load", helper.onGridLoad);
    grid.load({ ontologyCode: ontologyCode });
    grid.sortBy("CreateOn", "desc");

    helper.index.allInOne(
        null,
        grid,
        bootPATH + "../EDI/Command/Edit?ontologyCode=" + ontologyCode,
        bootPATH + "../EDI/Command/Edit?ontologyCode=" + ontologyCode,
        bootPATH + "../EDI/Command/DeleteDistributeMessage?ontologyCode=" + ontologyCode,
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    function search() {
        var data = {
            ontologyCode: ontologyCode,
            actionCode: actionCode.getValue(),
            organizationCode: getParams().organizationCode
        };
        data.nodeID = nodeID.getValue();
        grid.load(data, function () {
            var record = grid.getSelected();
            if (!record) {
                tabs1.hide();
            }
        });
    }

    function loadData() {
        search();
    }

    function getParams() {
        if (self.params && self.params.organizationCode) {
            return self.params;
        }
        else {
            return { organizationCode: $.deparam.fragment().organizationCode || $.deparam.querystring().organizationCode }
        }
    }

    function onKeyEnter(e) {
        search();
    }

    function ondrawcell(e) {
        var field = e.field;
        var value = e.value;
        var record = e.record;
        if (field) {
            switch (field) {
                case "ClientName":
                    var url = bootPATH + "../EDI/Node/Details?isTooltip=true&id=" + record.ClientID
                    e.cellHtml = "<a href='" + url + "' onclick='helper.cellTooltip(this);return false;' rel='" + url + "'>" + value + "</a>";
                    break;
            }
        }
        helperDrawcell(e);
    }
})(window);