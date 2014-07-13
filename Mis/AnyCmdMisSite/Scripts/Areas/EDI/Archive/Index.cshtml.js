/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window, $) {
    mini.namespace("EDI.Archive.Index");
    var self = EDI.Archive.Index;
    self.prifix = "EDI_Archive_Index_";
    self.search = search;
    self.gridReload = function () {
        grid.reload();
    };
    self.help = { appSystemCode: "Anycmd", areaCode: "EDI", resourceCode: "Archive", functionCode: "Index" };
    helper.helperSplitterInOne(self);
    mini.namespace("Archive.Edit");
    var edit = Archive.Edit;
    edit.SaveData = SaveData;
    edit.prifix = "EDI_Archive_Index_Edit_";
    var faceInitialized = false;
    var ontologyCode = $.deparam.fragment().ontologyCode || $.deparam.querystring().ontologyCode;
    var ontologyID = $.deparam.fragment().ontologyID || $.deparam.querystring().ontologyID;

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../EDI/Archive/Details",
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "Archive.Details"
        },
        dataTab: {
            url: bootPATH + "../EDI/Entity/Index?ontologyCode=" + ontologyCode + "&isArchive=true",
            params: [{ "pName": 'archiveID', "pValue": "Id" }],
            namespace: "EDI.Entity.Index"
        }
    };
    self.filters = {
        Title: {
            type: 'string',
            comparison: 'like'
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
    grid.on("drawcell", helper.ondrawcell(self, "EDI.Archive.Index"));
    grid.on("load", helper.onGridLoad);
    grid.sortBy("ArchiveOn", "desc");

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
        grid.load(data);
    }

    helper.index.allInOne(
        edit,
        grid,
        bootPATH + "../EDI/Archive/Edit",
        bootPATH + "../EDI/Archive/Edit",
        bootPATH + "../EDI/Archive/Delete",
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    function SaveData() {
        var messageid;
        var m = $(form.el);

        var data = m.serialize();
        form.validate();
        if (form.isValid() == false) return;

        var id = m.find("input[name='Id']").val();
        var url = bootPATH + "../EDI/Archive/Create?ontologyCode=" + ontologyCode + "&ontologyID=" + ontologyID;
        if (id) {
            url = bootPATH + "../EDI/Archive/Update";
        }
        else {
            messageid = mini.loading("请等待...", "归档中");
        }

        $.post(url, data, function (result) {
            helper.response(result, function () {
                edit.CloseWindow("save");
            });
            if (messageid) {
                mini.hideMessageBox(messageid);
            }
        }, "json");
    }

    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../EDI/Archive/Create?ontologyCode=" + ontologyCode + "&ontologyID=" + ontologyID,
        bootPATH + "../EDI/Archive/Update",
        bootPATH + "../EDI/Archive/Get",
        form, edit);
})(window, jQuery);