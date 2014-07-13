/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window, $) {
    mini.namespace("AC.RDatabase.Index");
    var self = AC.RDatabase.Index;
    self.prifix = "AC_RDatabase_Index_";
    self.search = search;
    self.gridReload = function () {
        grid.reload();
    };
    self.help = { appSystemCode: "Anycmd", areaCode: "AC", resourceCode: "RDatabase", functionCode: "Index" };
    helper.helperSplitterInOne(self);
    mini.namespace("RDatabase.Edit");
    var edit = RDatabase.Edit;
    edit.prifix = "AC_RDatabase_Index_Edit_";
    var faceInitialized = false;

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../AC/RDatabase/Details",
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "RDatabase.Details"
        },
        tableTab: {
            url: bootPATH + "../AC/RDatabase/Tables",
            params: [{ "pName": 'databaseID', "pValue": "Id" }],
            namespace: "AC.RDatabase.Tables"
        },
        viewTab: {
            url: bootPATH + "../AC/RDatabase/Views",
            params: [{ "pName": 'databaseID', "pValue": "Id" }],
            namespace: "AC.RDatabase.Views"
        },
        tableSpaceTab: {
            url: bootPATH + "../AC/RDatabase/TableSpaces",
            params: [{ "pName": 'databaseID', "pValue": "Id" }],
            namespace: "AC.RDatabase.TableSpaces"
        }
    };
    self.filters = {
        DataSource: {
            type: 'string',
            comparison: 'like'
        },
        CatalogName: {
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
    grid.on("drawcell", helper.ondrawcell(self, "AC.RDatabase.Index"));
    grid.on("load", helper.onGridLoad);
    grid.sortBy("CatalogName", "asc");

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
        bootPATH + "../AC/RDatabase/Edit",
        bootPATH + "../AC/RDatabase/Edit",
        bootPATH + "../AC/RDatabase/DeleteDatabase",
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../AC/RDatabase/Create",
        bootPATH + "../AC/RDatabase/Update",
        bootPATH + "../AC/RDatabase/Get",
        form, edit);
})(window, jQuery);