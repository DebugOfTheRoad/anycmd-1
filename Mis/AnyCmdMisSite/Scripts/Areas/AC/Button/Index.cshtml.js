﻿/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
(function (window) {
    mini.namespace("AC.Button.Index");
    var self = AC.Button.Index;
    self.prifix = "AC_Button_Index_";
    self.sortUrl = bootPATH + "../AC/Button/UpdateSortCode";
    self.help = { appSystemCode: "Anycmd", areaCode: "AC", resourceCode: "Button", functionCode: "Index" };
    helper.helperSplitterInOne(self);
    self.gridReload = function () {
        grid.reload();
    };
    mini.namespace("Button.Edit");
    var edit = Button.Edit;
    edit.prifix = "AC_Button_Index_Edit_";
    edit.SetData = SetData;
    var faceInitialized = false;

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../AC/Button/Details",
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "Button.Details"
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

    var btnEdit = mini.get(self.prifix + "btnEdit");
    var btnDelete = mini.get(self.prifix + "btnRemove");
    var editPermission = btnEdit && btnEdit.enabled;
    var deletePermission = btnDelete && btnDelete.enabled;

    var tabs1 = mini.get(self.prifix + "tabs1");
    var grid = mini.get(self.prifix + "datagrid1");
    grid.on("drawcell", ondrawcell);
    var helperDrawcell = helper.ondrawcell(self, "AC.Button.Index");
    grid.on("load", helper.onGridLoad);
    grid.load();
    grid.sortBy("SortCode", "asc");

    helper.index.allInOne(
        edit,
        grid,
        bootPATH + "../AC/Button/Edit",
        bootPATH + "../AC/Button/Edit",
        bootPATH + "../AC/Button/Delete",
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

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
                url: bootPATH + "../AC/Button/Get",
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

    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../AC/Button/Create",
        bootPATH + "../AC/Button/Update",
        bootPATH + "../AC/Button/Get",
        form, edit);
})(window);
