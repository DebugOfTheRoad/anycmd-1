﻿/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    var currentNode;
    var filters = {
        LoginName: {
            type: 'string',
            comparison: 'like'
        },
        IsEnabled: {
            type: 'numeric',
            comparison: 'eq'
        },
        ContractorCode: {
            type: 'string',
            comparison: 'like'
        },
        ContractorName: {
            type: 'string',
            comparison: 'like'
        }
    };

    mini.parse();

    $().ready(function () {
        $(".Select_btnSelectOk").click(selectOk);
        $(".Select_btnSelectOkClose").click(function () {
            if (selectOk()) {
                hideWin();
            }
            return false;
        });
        $(".Select_btnSelectCancel").click(function () {
            hideWin();
        });
    });

    for (var k in filters) {
        var id = k + "Filter";
        var filterBox = mini.get(id);
        filters[k].filterBox = filterBox;
        filterBox.on("valuechanged", helper.index.onFilterChanged(filters, search));
    }
    var btnSearchClear = mini.get("Select_btnSearchClear");
    btnSearchClear.on("click", function () {
        helper.index.clearSearch(filters, search);
    });

    var treeOrganization = mini.get("Select_treeOrganization");
    treeOrganization.on("nodeselect", onOrganizationNodeSelect);
    treeOrganization.on("beforenodeselect", function (e) {
        var tree = e.sender;
        var node = e.node;
        if (node.IsCategory) {
            e.cancel = true;
            if (node.expanded) {
                tree.collapseNode(node);
            }
            else {
                tree.expandNode(node);
            }
        }
    });
    treeOrganization.on("beforeload", onOrganizationTreeBeforeload);
    var chkbIncludedescendants = mini.get("Select_chkbIncludedescendants");
    chkbIncludedescendants.on("checkedchanged", onCheckedChanged);
    var grid = mini.get("Select_dgSelectUser");
    grid.on("drawcell", helper.ondrawcell(window));
    grid.on("load", helper.onGridLoad);

    function onOrganizationNodeSelect(e) {
        var tree = e.sender;
        var node = e.node;
        var isLeaf = e.isLeaf;
        currentNode = node;
        search();
    }

    function onOrganizationTreeBeforeload(e) {
        var tree = e.sender;
        var node = e.node;
        var params = e.params;

        params.parentID = node.Id;
    }

    function onCheckedChanged(e) {
        search();
    }

    function selectOk() {
        var rows = grid.getSelecteds();
        var s = "";
        var n = "";
        for (var i = 0, l = rows.length; i < l; i++) {
            var row = rows[i];
            s += row.Id;
            n += row.LoginName;
            if (i != l - 1) {
                s += ",";
                n += ",";
            }
        }
        if (s) {
            if (window.onSelectOk) {
                window.onSelectOk({ accountIDs: s, accountNames: n });
            }
            return true;
        }
        else {
            mini.alert("没有选中用户");
            return false;
        }
    }

    function hideWin() {
        var win = mini.get("Select_win1");
        win.hide();
    }

    function search() {
        var data = { };
        if (currentNode && currentNode.Id) {
            data.organizationID = currentNode.Id;
            data.organizationCode = currentNode.Code;
        }
        if (chkbIncludedescendants.getValue() == "1") {
            data.includedescendants = true;
        }
        else {
            data.includedescendants = false;
        }
        var filterArray = [];
        for (var k in filters) {
            var filter = filters[k];
            var field = k;
            if (filter.field) {
                field = filter.field;
            }
            if (filter.value) {
                filterArray.push({ field: field, type: filter.type, comparison: filter.comparison, value: filter.value });
            }
        }
        data.filters = JSON.stringify(filterArray);
        if (!grid.sortField) {
            grid.sortBy("CreateOn", "desc");
        }
        grid.load(data);
    }
})(window);