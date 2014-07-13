/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window, $) {
    mini.namespace("AC.Role.Index");
    var self = AC.Role.Index;
    self.prifix = "AC_Role_Index_";
    self.sortUrl = bootPATH + "../AC/Role/UpdateSortCode";
    self.add = add;
    self.search = search;
    self.gridReload = function () {
        grid.reload();
    };
    self.help = { appSystemCode: "Anycmd", areaCode: "AC", resourceCode: "Role", functionCode: "Index" };
    helper.helperSplitterInOne(self);
    mini.namespace("Role.Edit");
    var edit = Role.Edit;
    edit.SetData = SetData;
    edit.prifix = "AC_Role_Index_Edit_";
    var faceInitialized = false;

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../AC/Role/Details",
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "Role.Details"
        },
        permissionTab: {
            url: bootPATH + "../AC/Role/Permissions",
            params: [{ "pName": 'roleID', "pValue": "Id" }],
            namespace: "Role.Permissions"
        },
        menuTab: {
            url: bootPATH + "../AC/Role/Menus",
            params: [{ "pName": 'roleID', "pValue": "Id" }],
            namespace: "Role.Menus"
        },
        accountTab: {
            url: bootPATH + "../AC/Role/Accounts",
            params: [{ "pName": 'roleID', "pValue": "Id" }],
            namespace: "Role.Accounts"
        },
        groupTab: {
            url: bootPATH + "../AC/Role/Groups",
            params: [{ "pName": 'roleID', "pValue": "Id" }],
            namespace: "Role.Groups"
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
    grid.on("drawcell", ondrawcell);
    var helperDrawcell = helper.ondrawcell(self, "AC.Role.Index");
    grid.on("load", helper.onGridLoad);
    grid.sortBy("SortCode", "asc");

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

    function add() {
        var data = { action: "new" };
        data.CategoryID = "";
        var categoryID = self.filters.CategoryID.value;
        if (categoryID) {
            data.CategoryID = categoryID;
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
    function SetData(data) {
        //跨页面传递的数据对象，克隆后才可以安全使用
        data = mini.clone(data);
        if (data.action == "edit") {
            $.ajax({
                url: bootPATH + "../AC/Role/Get",
                data: { id: data.id },
                cache: false,
                success: function (result) {
                    helper.response(result, function () {
                        form.setData(result);
                        form.validate();
                        if (result.Icon) {
                            $("#msg").html("<img style='margin-top:3px;' src='" + bootPATH + "../Content/icons/16x16/" + result.Icon + "' alt='图标' />");
                        } else {
                            $("#msg").html("");
                        }
                    });
                }
            });
        }
        else if (data.action == "new") {
            form.setData(data);
        }
        if (!faceInitialized) {
            $.getJSON(bootPATH + "../Home/GetIcons", null, function (result) {
                $("#message_face").jqfaceedit({ txtAreaObj: $("#msg"), containerObj: $(mini.get('faceWindow').getBodyEl()), emotions: result, top: 25, left: -27, width: 658, height: 420 });
            });
            faceInitialized = true;
        }
    }

    function ondrawcell(e) {
        var field = e.field;
        var value = e.value;
        var record = e.record;
        if (field) {
            switch (field) {
                case "Name":
                    if (record.Icon) {
                        e.cellHtml = "<img src='" + bootPATH + "../Content/icons/16x16/" + record.Icon + "'></img>" + value;
                    }
                    else {
                        e.cellHtml = value;
                    }
                    break;
            }
        }
        helperDrawcell(e);
    }
    helper.index.allInOne(
        edit,
        grid,
        bootPATH + "../AC/Role/Edit",
        bootPATH + "../AC/Role/Edit",
        bootPATH + "../AC/Role/Delete",
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../AC/Role/Create",
        bootPATH + "../AC/Role/Update",
        bootPATH + "../AC/Role/Get",
        form, edit);
})(window, jQuery);