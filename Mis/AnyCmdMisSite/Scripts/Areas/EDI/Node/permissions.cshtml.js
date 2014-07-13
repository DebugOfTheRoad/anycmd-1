/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
// 接口：edit、remove
(function (window) {
    mini.namespace("Node.Permissions");
    var self = Node.Permissions;
    self.prifix = "EDI_Node_Permissions_";
    self.loadData = loadData;
    var nodeID = $.deparam.fragment().nodeID || $.deparam.querystring().nodeID || '';

    var ontologyID;
    var contentWindowWith = 240;
    var contentWindowHeight = 100;
    var currentRecord;
    var nodeInitialized = false;
    var nodeIframe;

    var elementFilters = {
        Name: {
            type: 'string',
            comparison: 'like'
        },
        Code: {
            type: 'string',
            comparison: 'like'
        },
        IsEnabled: {
            type: 'numeric',
            comparison: 'eq'
        }
    };

    mini.parse();

    for (var k in elementFilters) {
        var id = k + "Filter";
        if (self.prifix) {
            id = self.prifix + id;
        }
        var filterBox = mini.get(id);
        elementFilters[k].filterBox = filterBox;
        filterBox.on("valuechanged", helper.index.onFilterChanged(elementFilters, search));
    }
    var btnSearchClear = mini.get(self.prifix + "btnSearchClear");
    btnSearchClear.on("click", function () {
        helper.index.clearSearch(elementFilters, search);
    });

    var btnSaveAction = mini.get(self.prifix + "btnSaveAction");
    btnSaveAction.on("click", saveNodeAction);
    var btnSaveElementAction = mini.get(self.prifix + "btnSaveElementAction");
    btnSaveElementAction.on("click", saveNodeElementAction);
    var dgOntology = mini.get(self.prifix + "dgOntology");
    dgOntology.on("drawcell", ondrawcell);
    dgOntology.on("load", helper.onGridLoad);
    dgOntology.on("selectionchanged", function (e) {
        if (!dgAction.sortField) {
            dgAction.sortBy("Code", "asc");
        }
        var record = dgOntology.getSelected();
        ontologyID = record.Id;
        dgAction.load({ nodeID: getParams().nodeID, ontologyID: ontologyID });
        search();
        $("#" + self.prifix + "ontologyName").text(record.Name);
    });
    dgOntology.load({ nodeID: getParams().nodeID });
    dgOntology.sortBy("SortCode", "asc");
    var dgAction = mini.get(self.prifix + "dgAction");
    dgAction.on("drawcell", ondrawcell);
    var dgElementAction = mini.get(self.prifix + "dgElementAction");
    dgElementAction.on("drawcell", ondrawcell);
    var dgElement = mini.get(self.prifix + "dgElement");
    dgElement.on("selectionchanged", function (e) {
        var data = getParams();
        var record = dgElement.getSelected();
        var elementID = record.Id;
        if (!dgElementAction.sortField) {
            dgElementAction.sortBy("Name", "asc");
        }
        dgElementAction.load({ nodeID: data.nodeID, elementID: elementID });
        $("#" + self.prifix + "elementName").text(record.Name);
    });
    dgElement.on("drawcell", ondrawcell);
    dgElement.on("load", helper.onGridLoad);

    function loadData() {
        search();
        if (!dgAction.sortField) {
            dgAction.sortBy("Name", "asc");
        }
        dgAction.load({ nodeID: getParams().nodeID, ontologyID: ontologyID });
    }

    function getParams() {
        if (self.params && self.params.nodeID) {
            return self.params;
        }
        else {
            return { nodeID: $.deparam.fragment().nodeID || $.deparam.querystring().nodeID };
        }
    }

    function search() {
        if (ontologyID) {
            var data = getParams();
            data.ontologyID = ontologyID;
            var filterArray = [];
            for (var k in elementFilters) {
                var filter = elementFilters[k];
                var field = k;
                if (filter.field) {
                    field = filter.field;
                }
                if (filter.value) {
                    filterArray.push({ field: field, type: filter.type, comparison: filter.comparison, value: filter.value });
                }
            }
            data.filters = JSON.stringify(filterArray);
            if (!dgElement.sortField) {
                dgElement.sortBy("SortCode", "asc");
            }
            dgElement.load(data);
            dgElementAction.clearRows();
        }
    }

    function saveNodeAction() {
        var data = dgAction.getChanges();
        var json = mini.encode(data);

        dgAction.loading("保存中，请稍后......");
        $.ajax({
            url: bootPATH + "../EDI/Node/AddOrUpdateNodeActions?nodeID=" + nodeID,
            data: { data: json },
            type: "post",
            success: function (result) {
                dgAction.reload();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                mini.alert(jqXHR.responseText);
            }
        });
    }

    function saveNodeElementAction() {
        var data = dgElementAction.getChanges();
        var json = mini.encode(data);

        dgElementAction.loading("保存中，请稍后......");
        $.ajax({
            url: bootPATH + "../EDI/Node/AddOrUpdateNodeElementActions?nodeID=" + nodeID,
            data: { data: json },
            type: "post",
            success: function (result) {
                dgElementAction.reload();
            },
            error: function (jqXHR, textStatus, errorThrown) {
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
                case "OntologyName":
                    var url = bootPATH + "../EDI/Ontology/Details?isTooltip=true&id=" + record.OntologyID
                    e.cellHtml = "<a href='" + url + "' onclick='helper.cellTooltip(this);return false;' rel='" + url + "'>" + value + "</a>";
                    break;
                case "ActionIsAllow":
                case "ActionIsAudit":
                case "ActionIsPersist":
                case "IsEnabled":
                    if (value == "正常" || value == "1" || value == true) {
                        e.cellHtml = "<span class='icon-enabled width16px'></span>";
                    } else if (value == "禁用" || value == "0" || value == false) {
                        e.cellHtml = "<span class='icon-disabled width16px'></span>";
                    } break;
                case "Name":
                    if (e.sender == dgElement) {
                        var url = bootPATH + "../EDI/Element/Details?isTooltip=true&id=" + record.Id
                        e.cellHtml = "<a href='" + url + "' onclick='helper.cellTooltip(this);return false;' rel='" + url + "'>" + value + "</a>";
                    }
                    break;
                case "Icon":
                    if (value) {
                        e.cellHtml = "<img src='" + bootPATH + "../Content/icons/16x16/" + value + "'></img>";
                    }
                    break;
                case "IsAudit":
                    if (e.sender == dgOntology || e.sender == dgElement) {
                        if (value == "正常" || value == "1" || value == true) {
                            e.cellHtml = "<span class='icon-enabled width16px'></span>";
                        } else if (value == "禁用" || value == "0" || value == false) {
                            e.cellHtml = "<span class='icon-disabled width16px'></span>";
                        }
                    }
                    break;
            }
        }
    }
})(window);