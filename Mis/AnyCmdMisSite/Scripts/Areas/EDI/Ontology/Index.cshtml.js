/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
// 接口：edit、remove
(function (window) {
    mini.namespace("EDI.Ontology.Index");
    var self = EDI.Ontology.Index;
    self.prifix = "EDI_Ontology_Index_";
    self.sortUrl = bootPATH + "../EDI/Ontology/UpdateSortCode";
    self.help = { appSystemCode: "Anycmd", areaCode: "EDI", resourceCode: "Ontology", functionCode: "Index" };
    helper.helperSplitterInOne(self);
    self.gridReload = function () {
        grid.reload();
    };
    mini.namespace("Ontology.Edit");
    var edit = Ontology.Edit;
    edit.prifix = "EDI_Ontology_Index_Edit_";
    edit.SetData = SetData;
    var faceInitialized = false;

    var tabConfigs = {
        infoTab: {
            url: bootPATH + "../EDI/Ontology/Details",
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "Ontology.Details"
        },
        infoGroupTab: {
            url: bootPATH + "../EDI/Ontology/InfoGroups",
            params: [{ "pName": 'ontologyID', "pValue": "Id" }],
            namespace: "Ontology.InfoGroups"
        },
        actionTab: {
            url: bootPATH + "../EDI/Ontology/Actions",
            params: [{ "pName": 'ontologyID', "pValue": "Id" }],
            namespace: "Ontology.Actions"
        },
        elementTab: {
            url: bootPATH + "../EDI/Ontology/Elements",
            params: [{ "pName": 'ontologyID', "pValue": "Id" }],
            namespace: "Ontology.Elements"
        },
        nodeCareTab: {
            url: bootPATH + "../EDI/Node/OntologyNodeCares",
            params: [{ "pName": 'ontologyID', "pValue": "Id" }],
            namespace: "Ontology.NodeCares"
        },
        organizationTabPage: {
            url: bootPATH + "../EDI/Ontology/Organizations",
            params: [{ "pName": 'ontologyID', "pValue": "Id" }, { "pName": "isEntityOrganized", "pValue": "IsOrganizationalEntity" }],
            namespace: "Ontology.Organizations"
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
    
    var btnSave = mini.get(self.prifix + "btnSave");
    if (btnSave) {
        btnSave.on("click", saveData);
    }
    var tabs1 = mini.get(self.prifix + "tabs1");
    var grid = mini.get(self.prifix + "datagrid1");
    grid.on("drawcell", ondrawcell);
    var helperDrawcell = helper.ondrawcell(self, "EDI.Ontology.Index");
    grid.on("load", helper.onGridLoad);
    grid.load();
    grid.sortBy("SortCode", "asc");

    helper.index.allInOne(
        edit,
        grid,
        bootPATH + "../EDI/Ontology/Edit",
        bootPATH + "../EDI/Ontology/Edit",
        bootPATH + "../EDI/Ontology/Delete",
        self);
    helper.index.tabInOne(grid, tabs1, tabConfigs, self);

    function saveData() {
        var data = grid.getChanges();
        var json = mini.encode(data);

        grid.loading("保存中，请稍后......");
        $.ajax({
            url: bootPATH + "../EDI/Ontology/UpdateOntologies",
            data: { data: json },
            type: "post",
            success: function (result) {
                helper.response(result, function () {
                    grid.reload();
                }, function () {
                    grid.unmask();
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                grid.unmask();
                mini.alert(jqXHR.responseText);
            }
        });
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
                url: bootPATH + "../EDI/Ontology/Get",
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
        bootPATH + "../EDI/Ontology/Create",
        bootPATH + "../EDI/Ontology/Update",
        bootPATH + "../EDI/Ontology/Get",
        form, edit);
})(window);