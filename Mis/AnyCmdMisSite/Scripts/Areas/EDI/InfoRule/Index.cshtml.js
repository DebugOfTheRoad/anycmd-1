﻿/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
(function (window) {
    mini.namespace("EDI.InfoRule.Index");
    var self = EDI.InfoRule.Index;
    self.prifix = "EDI_InfoRule_Index_";
    self.help = { appSystemCode: "Anycmd", areaCode: "EDI", resourceCode: "InfoRule", functionCode: "Index" };
    helper.helperSplitterInOne(self);
    self.gridReload = function () {
        grid.reload();
    };
    self.search = search;
    mini.namespace("InfoRule.Edit");
    var edit = InfoRule.Edit;
    edit.prifix = "EDI_InfoRule_Index_Edit_";

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../EDI/InfoRule/Details",
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "InfoRule.Details"
        },
        operationLogTab: {
            url: bootPATH + "../AC/OperationLog/Index",
            params: [{ "pName": 'targetID', "pValue": "Id" }],
            namespace: "AC.OperationLog.Index"
        }
    };
    self.filters = {
        Title: {
            type: 'string',
            comparison: 'like'
        },
        Name: {
            type: 'string',
            comparison: 'like'
        },
        IsEnabled: {
            type: 'numeric',
            comparison: 'eq'
        }
    };

    mini.parse();

    var win = mini.get(edit.prifix + "win1");
    var form;
    if (win) {
        form = new mini.Form(edit.prifix + "form1");
    }

    var tabs1 = mini.get(self.prifix + "tabs1");
    var grid = mini.get(self.prifix + "datagrid1");
    var helperDrawcell = helper.ondrawcell(self, "EDI.InfoRule.Index");
    grid.on("drawcell", ondrawcell);
    grid.on("load", helper.onGridLoad);
    grid.load();
    grid.sortBy("CreateOn", "desc");

    helper.index.allInOne(
        edit,
        grid,
        bootPATH + "../EDI/InfoRule/Edit",
        bootPATH + "../EDI/InfoRule/Edit",
        bootPATH + "../EDI/InfoRule/DeleteInfoRule",
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    function search() {
        var data = {};
        var filterArray = [];
        for (var k in self.filters) {
            var filter = self.filters[k];
            if (filter.value) {
                filterArray.push({ field: k, type: filter.type, comparison: filter.comparison, value: filter.value });
            }
        }
        data.filters = JSON.stringify(filterArray);
        grid.load(data, function () {
            var record = grid.getSelected();
            if (!record) {
                tabs1.hide();
            }
        });
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
                case "IsGlobal":
                    if (value == true || value == "1" || value == "是") {
                        e.cellHtml = "<span class='icon-enabled width16px'></span>";
                    } else if (value == false || value == "0" || value == "否") {
                        e.cellHtml = "<span class='icon-disabled width16px'></span>";
                    } break;
            }
        }
        helperDrawcell(e);
    }

    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../EDI/InfoRule/Create",
        bootPATH + "../EDI/InfoRule/Update",
        bootPATH + "../EDI/InfoRule/Get",
        form, edit);
})(window);