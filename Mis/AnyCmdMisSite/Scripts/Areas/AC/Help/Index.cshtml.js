/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    mini.namespace("AC.Help.Index");
    var self = AC.Help.Index;
    self.prifix = "AC_Help_Index_";
    self.sortUrl = bootPATH + "../AC/Help/UpdateSortCode";
    self.help = { appSystemCode: "Anycmd", areaCode: "AC", resourceCode: "Help", functionCode: "Index" };
    helper.helperSplitterInOne(self);
    self.loadData = search;
    self.search = search;
    self.edit = edit;
    self.add = add;

    self.gridReload = function () {
        grid.reload();
    };
    mini.namespace("Help.Edit");
    var editPage = Help.Edit;
    editPage.prifix = "AC_Help_Index_Edit_";
    editPage.SaveData = SaveData;
    editPage.SetData = SetData;

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../AC/Help/Details",
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "Help.Details"
        },
        operationLogTab: {
            url: bootPATH + "../AC/OperationLog/Index",
            params: [{ "pName": 'targetID', "pValue": "Id" }],
            namespace: "AC.OperationLog.Index"
        }
    };
    var editor;
    var restoreHeight;

    $().ready(function () {
        KindEditor.ready(function (K) {
            editor = K.create("#" + editPage.prifix + 'contentEditor', {
                allowFileManager: true,
                width: "100%",
                uploadJson: bootPATH + '../Scripts/kindeditor/asp.net/upload_json.ashx',
                fileManagerJson: bootPATH + '../Scripts/kindeditor/asp.net/file_manager_json.ashx',
                afterCreate: function () {
                    restoreHeight = getEditorAutoHeight();
                    this.edit.setHeight(restoreHeight);
                }
            });
        });
    });

    mini.parse();
    var win = mini.get(editPage.prifix + "win1");
    var form = new mini.Form(editPage.prifix + "form1");
    win.on("resize", function (e) {
        editor.edit.setHeight(getEditorAutoHeight());
    });
    win.on("buttonclick", function (e) {
        if (e.name == "max") {
            if (e.source.state == "restore") {
                //放大
                editor.edit.setHeight((document.body.clientHeight - 270) + "px");
            } else {
                //还原
                editor.edit.setHeight(restoreHeight);
            }
        }
    });

    function getEditorAutoHeight() {
        var winHeight = parseInt(win.height);
        return (winHeight - 270) + "px";
    }

    var hdContent = mini.get(editPage.prifix + "hdContent");
    var key = mini.get(self.prifix + "key");
    key.on("enter", search);
    var currentDicRecord;

    var tabs1 = mini.get(self.prifix + "tabs1");
    var grid = mini.get(self.prifix + "datagrid1");
    grid.on("drawcell", helper.ondrawcell(self, "AC.Help.Index"));
    grid.on("load", helper.onGridLoad);
    grid.sortBy("SortCode", "asc");

    function onNodeSelect(e) {
        var node = e.node;
        grid.load({ categoryID: node.Id });
    }

    function add() {
        var data = { action: "new", oldWidth: parseInt(win.width.replace("px", "")), oldHeight: parseInt(editor.edit.height.replace("px", "")) };
        if (win) {
            win.setTitle("添加");
            win.setIconCls("icon-add");
            win.show();
            editPage.SetData(data, editor);
        }
    }

    function edit() {
        var row = grid.getSelected();
        if (row) {
            var data = { id: row.Id, action: "edit", oldWidth: parseInt(win.width.replace("px", "")), oldHeight: parseInt(editor.edit.height.replace("px", "")) };
            if (win) {
                win.setTitle("编辑");
                win.setIconCls("icon-edit");
                editPage.SetData(data, editor);
                win.show();
            }
        } else {
            mini.alert("请选中一条记录");
        }
    }

    function search() {
        var data = { key: key.getValue() };
        grid.load(data, function () {
            var record = grid.getSelected();
            if (!record) {
                tabs1.hide();
            }
        });
    }

    function SetData(data, editor) {
        //跨页面传递的数据对象，克隆后才可以安全使用
        data = mini.clone(data);
        editor = editor;
        editor.html("");
        if (data.action == "edit") {
            $.ajax({
                url: bootPATH + "../AC/Help/Get?id=" + data.id,
                cache: false,
                success: function (result) {
                    helper.response(result, function () {
                        form.setData(result);
                        editor.html(hdContent.getValue());
                        form.validate();
                    });
                }
            });
        }
        else if (data.action == "new") {
            data["DicID"] = "";
            data["OntologyID"] = $.deparam.fragment().ontologyID;
            form.setData(data);
        }
    }

    function SaveData() {
        hdContent.setValue(editor.html());
        var data = $("#" + editPage.prifix + "form1").serialize();
        var id = $("#" + editPage.prifix + "form1" + " input[name='Id']").val();
        var url = "../Help/Create";
        if (id) {
            url = "../Help/Update";
        }
        form.validate();
        if (form.isValid() == false) return;

        $.post(url, data, function (result) {
            helper.response(result, function () {
                editPage.CloseWindow("save");
            });
        }, "json");
    }
    helper.index.allInOne(
        editPage,
        grid,
        bootPATH + "../AC/Help/Edit",
        bootPATH + "../AC/Help/Edit",
        bootPATH + "../AC/Help/Delete",
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../AC/Help/Create",
        bootPATH + "../AC/Help/Update",
        bootPATH + "../AC/Help/Get",
        form, editPage);
})(window);