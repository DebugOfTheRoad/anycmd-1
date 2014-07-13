/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window, $) {
    mini.namespace("EDI.Batch.Index");
    var self = EDI.Batch.Index;
    self.prifix = "EDI_Batch_Index_";
    self.search = search;
    self.gridReload = function () {
        grid.reload();
    };
    self.help = { appSystemCode: "Anycmd", areaCode: "EDI", resourceCode: "Batch", functionCode: "Index" };
    helper.helperSplitterInOne(self);
    mini.namespace("Batch.Edit");
    var edit = Batch.Edit;
    edit.SaveData = SaveData;
    edit.prifix = "EDI_Batch_Index_Edit_";
    var faceInitialized = false;

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../EDI/Batch/Details",
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "Batch.Details"
        }
    };
    self.filters = {
        Title: {
            type: 'string',
            comparison: 'like'
        },
        Type: {
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
    grid.on("drawcell", helper.ondrawcell(self, "EDI.Batch.Index"));
    grid.on("load", helper.onGridLoad);
    grid.sortBy("CreateOn", "desc");

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
        bootPATH + "../EDI/Batch/Edit",
        bootPATH + "../EDI/Batch/Edit",
        bootPATH + "../EDI/Batch/Delete",
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    function SaveData() {
        var messageid;
        var m = $(form.el);
        
        var data = m.serialize();
        form.validate();
        if (form.isValid() == false) return;

        var id = m.find("input[name='Id']").val();
        var url = bootPATH + "../EDI/Batch/Create";
        if (id) {
            url = bootPATH + "../EDI/Batch/Update";
        }
        else {
            messageid = mini.loading("请等待...", "处理中");
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
        bootPATH + "../EDI/Batch/Create",
        bootPATH + "../EDI/Batch/Update",
        bootPATH + "../EDI/Batch/Get",
        form, edit);
})(window, jQuery);