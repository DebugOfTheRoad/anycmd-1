/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window, $) {
    mini.namespace("AC.RDatabase.TableSpaces");
    var self = AC.RDatabase.TableSpaces;
    self.prifix = "AC_RDatabase_TableSpaces_";
    self.loadData = loadData;
    self.search = search;

    mini.parse();

    var grid = mini.get(self.prifix + "datagrid1");
    grid.on("drawcell", helper.ondrawcell(self, "AC.RDatabase.TableSpaces"));
    grid.on("load", helper.onGridLoad);

    helper.index.allInOne(
        null,
        grid,
        null,
        null,
        null,
        self);

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
        if (!grid.sortField) {
            grid.sortBy("Name", "asc");
        }
        grid.load(data);
    }
})(window, jQuery);