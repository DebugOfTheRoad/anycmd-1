/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
// 接口：edit、remove
(function (window) {
    mini.namespace("AC.AppSystem.Index");
    var self = AC.AppSystem.Index;
    self.prifix = "AC_AppSystem_Index_";
    self.sortUrl = bootPATH + "../AC/AppSystem/UpdateSortCode";
    self.help = { appSystemCode: "Anycmd", areaCode: "AC", resourceCode: "AppSystem", functionCode: "Index" };
    helper.helperSplitterInOne(self);
    self.gridReload = function () {
        grid.reload();
    };
    mini.namespace("AppSystem.Edit");
    var edit = AppSystem.Edit;
    edit.prifix = "AC_AppSystem_Index_Edit_";

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../AC/AppSystem/Details",
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "AppSystem.Details"
        },
        resourceTab: {
            url: bootPATH + "../AC/ResourceType/Index",
            params: [{ "pName": 'appSystemCode', "pValue": "Code" }, { "pName": 'appSystemID', "pValue": "Id" }],
            namespace: "AC.ResourceType.Index"
        },
        functionTab: {
            url: bootPATH + "../AC/Function/Index",
            params: [{ "pName": 'appSystemCode', "pValue": "Code" }, { "pName": 'appSystemID', "pValue": "Id" }],
            namespace: "AC.Function.Index"
        },
        pageTab: {
            url: bootPATH + "../AC/Page/Index",
            params: [{ "pName": 'appSystemCode', "pValue": "Code" }, { "pName": 'appSystemID', "pValue": "Id" }],
            namespace: "AC.Page.Index"
        },
        operationLogTab: {
            url: bootPATH + "../AC/OperationLog/Index",
            params: [{ "pName": 'targetID', "pValue": "Id" }],
            namespace: "AC.OperationLog.Index"
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
    grid.on("drawcell", helper.ondrawcell(self, "AC.AppSystem.Index"));
    grid.on("load", helper.onGridLoad);
    grid.load();
    grid.sortBy("SortCode", "asc");

    helper.index.allInOne(
        edit,
        grid,
        bootPATH + "../AC/AppSystem/Edit",
        bootPATH + "../AC/AppSystem/Edit",
        bootPATH + "../AC/AppSystem/Delete",
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../AC/AppSystem/Create",
        bootPATH + "../AC/AppSystem/Update",
        bootPATH + "../AC/AppSystem/Get",
        form, edit);
})(window);