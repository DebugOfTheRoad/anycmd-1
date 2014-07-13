/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window, $) {
    mini.namespace("AC.RDatabase.Tables");
    var self = AC.RDatabase.Tables;
    self.prifix = "AC_RDatabase_Tables_";
    self.loadData = loadData;
    self.search = search;
    self.gridReload = function () {
        grid.reload();
    };
    mini.namespace("AC.RDatabase.Tables.TableEdit");
    var edit = AC.RDatabase.Tables.TableEdit;
    edit.SetData = SetData;
    edit.prifix = "AC_RDatabase_Tables_TableEdit_"

    var tabConfigs = {
        columnTab: {
            url: bootPATH + "../AC/RDatabase/TableColumns",
            params: [
                { "pName": 'tableID', "pValue": "Id" },
                { "pName": 'databaseID', "pValue": "DatabaseID" },
                { "pName": 'schemaName', "pValue": "SchemaName" },
                { "pName": 'tableName', "pValue": "Name" }],
            namespace: "AC.RDatabase.TableColumns"
        }
    };
    self.filters = {
        Description: {
            type: 'string',
            comparison: 'like'
        },
        SchemaName: {
            type: 'string',
            comparison: 'like'
        },
        Name: {
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
    grid.on("drawcell", helper.ondrawcell(self, "AC.RDatabase.Tables"));
    grid.on("load", helper.onGridLoad);

    helper.index.allInOne(
        edit,
        grid,
        null,
        bootPATH + "../AC/RDatabase/Edit",
        null,
        self);

    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    function loadData() {
        search();
    }

    function getParams() {
        var data = { };
        if (self.params && self.params.databaseID) {
            data.databaseID = self.params.databaseID;
        }
        else {
            data.databaseID = $.deparam.fragment().databaseID || $.deparam.querystring().databaseID;
        }
        return data;
    }

    function search() {
        var data = getParams();
        var filterArray = [];
        for (var k in self.filters) {
            var filter = self.filters[k];
            if (filter.value) {
                filterArray.push({ field: k, type: filter.type, comparison: filter.comparison, value: filter.value });
            }
        }
        data.filters = JSON.stringify(filterArray);
        if (!grid.sortField) {
            grid.sortBy("Id", "asc");
        }
        grid.load(data, function () {
            var record = grid.getSelected();
            if (!record) {
                tabs1.hide();
            }
        });
    }
    function SetData(data) {
        //跨页面传递的数据对象，克隆后才可以安全使用
        data = mini.clone(data);
        if (data.action == "edit") {
            $.ajax({
                url: bootPATH + "../AC/RDatabase/GetTable",
                data: { id: data.id, databaseID: getParams().databaseID },
                cache: false,
                success: function (result) {
                    helper.response(result, function () {
                        form.setData(result);
                        form.validate();
                    });
                }
            });
        }
        else if (data.action == "new") {
            form.setData(data);
        }
    }
    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../AC/RDatabase/UpdateTable",
        bootPATH + "../AC/RDatabase/UpdateTable",
        bootPATH + "../AC/RDatabase/GetTable",
        form, edit);
})(window, jQuery);