﻿/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    mini.namespace("AC.ResourceType.Index");
    var self = AC.ResourceType.Index;
    self.prifix = "AC_ResourceType_Index_";
    self.sortUrl = bootPATH + "../AC/ResourceType/UpdateSortCode";
    self.add = add;
    self.loadData = loadData;
    self.search = search;
    self.gridReload = function () {
        grid.reload();
    };
    mini.namespace("ResourceType.Edit");
    var edit = ResourceType.Edit;
    edit.prifix = "AC_ResourceType_Index_Edit_";
    edit.SetData = SetData;
    var faceInitialized = false;

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../AC/ResourceType/Details",
            entityTypeCode: 'ResourceType',
            controller: 'ResourceType',
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "ResourceType.Details"
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
        Code: {
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
    grid.on("drawcell", ondrawcell);
    var helperDrawcell = helper.ondrawcell(self, "AC.ResourceType.Index");
    grid.on("load", helper.onGridLoad);
    grid.sortBy("SortCode", "asc");

    function add() {
        var data = { action: "new" };
        grid.loading("使劲加载中...");
        helper.index.winReady(edit, function (win) {
            win.setTitle("添加");
            win.setIconCls("icon-add");
            win.show();
            edit.SetData(data);
            grid.unmask();
        });
    };

    function loadData() {
        search();
    }

    function getParams() {
        data = {};
        if (self.params && self.params.appSystemID) {
            data.appSystemID = self.params.appSystemID;
        }
        else {
            data.appSystemID = $.deparam.fragment().appSystemID || $.deparam.querystring().appSystemID
        }
        return data;
    }

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
        if (!grid.sortField) {
            grid.sortBy("SortCode", "asc");
        }
        grid.load(data);
    }

    function ondrawcell(e) {
        var field = e.field;
        var value = e.value;
        var record = e.record;
        if (field) {
            switch (field) {
                case "Icon":
                    if (value) {
                        e.cellHtml = "<img src='" + bootPATH + "../Content/icons/16x16/" + value + "'></img>";
                    }
                    break;
            }
        }
        helperDrawcell(e);
    }

    function SetData(data) {
        //跨页面传递的数据对象，克隆后才可以安全使用
        data = mini.clone(data);
        if (data.action == "edit") {
            $.ajax({
                url: bootPATH + "../AC/ResourceType/Get",
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
            data.TypeCode = "db";
            form.setData(data);
        }
        if (!faceInitialized) {
            $.getJSON(bootPATH + "../Home/GetIcons", null, function (result) {
                $("#message_face").jqfaceedit({ txtAreaObj: $("#msg"), containerObj: $(mini.get('faceWindow').getBodyEl()), emotions: result, top: 25, left: -27, width: 658, height: 420 });
            });
            faceInitialized = true;
        }
    }

    helper.index.allInOne(
        edit,
        grid,
        bootPATH + "../AC/ResourceType/Edit",
        bootPATH + "../AC/ResourceType/Edit",
        bootPATH + "../AC/ResourceType/Delete",
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../AC/ResourceType/Create",
        bootPATH + "../AC/ResourceType/Update",
        bootPATH + "../AC/ResourceType/Get",
        form, edit);
})(window);