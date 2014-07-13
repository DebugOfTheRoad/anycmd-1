/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    mini.namespace("AC.Group.Index");
    var self = AC.Group.Index;
    self.prifix = "AC_Group_Index_";
    self.sortUrl = bootPATH + "../AC/Group/UpdateSortCode";
    self.help = { appSystemCode: "Anycmd", areaCode: "AC", resourceCode: "Group", functionCode: "Index" };
    helper.helperSplitterInOne(self);
    self.add = add;
    self.search = search;
    self.gridReload = function () {
        grid.reload();
    };
    mini.namespace("Group.Edit");
    var edit = Group.Edit;
    edit.prifix = "AC_Group_Index_Edit_";

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../AC/Group/Details",
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "Group.Details"
        },
        roleTab: {
            url: bootPATH + "../AC/Group/Roles",
            params: [{ "pName": 'groupID', "pValue": "Id" }],
            namespace: "Group.Roles"
        },
        accountTab: {
            url: bootPATH + "../AC/Group/Accounts",
            params: [{ "pName": 'groupID', "pValue": "Id" }],
            namespace: "Group.Accounts"
        },
        operationLogTab: {
            url: bootPATH + "../AC/OperationLog/Index",
            params: [{ "pName": 'targetID', "pValue": "Id" }],
            namespace: "AC.OperationLog.Index"
        }
    };
    self.filters = {
        Name: {
            type: 'string',
            comparison: 'like'
        },
        IsEnabled: {
            type: 'numeric',
            comparison: 'eq'
        },
        CategoryCode: {
            type: 'string',
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
    grid.on("drawcell", helper.ondrawcell(self, "AC.Group.Index"));
    grid.on("load", helper.onGridLoad);
    search();

    function search() {
        var data = getParams();
        if (!grid.sortField) {
            grid.sortBy("SortCode", "asc");
        }
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

    function getParams() {
        var data = {};
        return data;
    }

    function add() {
        var data = { action: "new", TypeCode: 'AC' };
        data.CategoryCode = "";
        var categoryCode = self.filters.CategoryCode.value;
        if (categoryCode) {
            data.CategoryCode = categoryCode;
        }
        grid.loading("使劲加载中...");
        helper.index.winReady(edit, function (win) {
            win.setTitle("添加");
            win.setIconCls("icon-add");
            win.show();
            edit.SetData(data);
            grid.unmask();
        });
    };

    helper.index.allInOne(
        edit,
        grid,
        bootPATH + "../AC/Group/Edit",
        bootPATH + "../AC/Group/Edit",
        bootPATH + "../AC/Group/Delete",
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../AC/Group/Create",
        bootPATH + "../AC/Group/Update",
        bootPATH + "../AC/Group/Get",
        form, edit);
})(window);