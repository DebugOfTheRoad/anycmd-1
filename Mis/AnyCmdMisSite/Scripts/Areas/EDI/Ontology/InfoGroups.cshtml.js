/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    mini.namespace("Ontology.InfoGroups");
    var self = Ontology.InfoGroups;
    self.prifix = "EDI_Ontology_InfoGroups_";
    self.loadData = loadData;
    self.gridReload = function () {
        grid.reload();
    };
    mini.namespace("Ontology.InfoGroups.Edit");
    var edit = Ontology.InfoGroups.Edit;
    edit.prifix = "EDI_Ontology_InfoGroups_Edit_";
    edit.SetData = SetData;

    mini.parse();

    var win = mini.get(edit.prifix + "win1");
    var form;
    if (win) {
        form = new mini.Form(edit.prifix + "form1");
    }

    var grid = mini.get(self.prifix + "datagrid1");
    grid.on("drawcell", helper.ondrawcell(self, "Ontology.InfoGroups"));
    grid.on("load", helper.onGridLoad);

    helper.index.allInOne(
        edit,
        grid,
        bootPATH + "../EDI/Ontology/EditInfoGroup",
        bootPATH + "../EDI/Ontology/EditInfoGroup",
        bootPATH + "../EDI/Ontology/DeleteInfoGroup",
        self);

    function loadData() {
        if (!grid.sortField) {
            grid.sortBy("SortCode", "asc");
        }
        grid.load(getParams());
    }

    function getParams() {
        if (self.params && self.params.ontologyID) {
            return self.params;
        }
        else {
            return { ontologyID: $.deparam.fragment().ontologyID || $.deparam.querystring().ontologyID }
        }
    }

    function SetData(data) {
        //跨页面传递的数据对象，克隆后才可以安全使用
        data = mini.clone(data);
        if (data.action == "edit") {
            $.ajax({
                url: bootPATH + "../EDI/Ontology/GetInfoGroup?id=" + data.id,
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
            data["OntologyID"] = getParams().ontologyID;
            form.setData(data);
        }
    }

    helper.edit.allInOne(
        self,
        win,
        bootPATH + "../EDI/Ontology/AddInfoGroup",
        bootPATH + "../EDI/Ontology/UpdateInfoGroup",
        bootPATH + "../EDI/Ontology/GetInfoGroup",
        form, edit);
})(window);