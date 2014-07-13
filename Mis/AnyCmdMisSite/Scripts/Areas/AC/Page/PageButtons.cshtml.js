/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../helper.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    mini.namespace("Page.PageButtons");
    var self = Page.PageButtons;
    self.prifix = "AC_Page_PageButtons_";
    self.loadData = loadData;
    var oldPageID;

    mini.parse();

    var rbIsAssigned = mini.get(self.prifix + "rbIsAssigned");
    rbIsAssigned.on("valuechanged", loadData);
    var btnSave = mini.get(self.prifix + "btnSave");
    if (btnSave) {
        btnSave.on("click", saveData);
    }
    var btnSearch = mini.get(self.prifix + "btnSearch");
    btnSearch.on("click", loadData);
    var key = mini.get(self.prifix + "key");
    key.on("enter", loadData);
    var grid = mini.get(self.prifix + "datagrid1");
    grid.on("drawcell", ondrawcell);
    grid.on("load", helper.onGridLoad);
    grid.on("cellbeginedit", cellbeginedit);

    function loadData() {
        var data = getParams();
        if (!grid.sortField) {
            grid.sortBy("SortCode", "asc");
        }
        grid.load(data);
    }

    function cellbeginedit(e) {
        var editor = e.editor;
        if (editor && e.field == "FunctionID") {
            var data = getParams();
            if (!oldPageID || oldPageID != data.pageID || !editor.data || editor.data.length == 0) {
                oldPageID = data.pageID;
                var url = bootPATH + "../AC/Function/GetManagedFunctions?appSystemID=" + data.appSystemID + "&pageController=" + data.controller;
                editor.load(url);
            }
        }
    }

    function getParams() {
        var data = { key: key.getValue(), isAssigned: rbIsAssigned.getValue() };
        if (self.params && self.params.pageID) {
            data.pageID = self.params.pageID;
            data.appSystemID = self.params.appSystemID;
            data.controller = self.params.controller;
        }
        else {
            var fragment = $.deparam.fragment();
            var querystring = $.deparam.querystring();
            data.pageID = fragment.pageID || querystring.pageID;
            data.appSystemID = fragment.appSystemID || querystring.appSystemID;
            data.controller = fragment.controller || querystring.controller;
        }
        return data;
    }

    function saveData() {
        var data = grid.getChanges();
        var json = mini.encode(data);

        grid.loading("保存中，请稍后......");
        $.ajax({
            url: bootPATH + "../AC/Page/AddOrRemoveButtons",
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
                case "FunctionID":
                    e.cellHtml = record.FunctionName;
                    break;
                case "ButtonIsEnabled":
                    if (value == "正常" || value == "1") {
                        e.cellHtml = "<span class='icon-enabled width16px'></span>";
                    } else if (value == "禁用" || value == "0") {
                        e.cellHtml = "<span class='icon-disabled width16px'></span>";
                    } break;
                case "Icon":
                    if (value) {
                        e.cellHtml = "<img src='" + bootPATH + "../Content/icons/16x16/" + value + "'></img>";
                    }
                    break;
            }
        }
    }
})(window);