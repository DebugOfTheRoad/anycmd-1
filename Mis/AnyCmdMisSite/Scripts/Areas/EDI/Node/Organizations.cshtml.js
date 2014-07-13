/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    mini.namespace("Node.Organizations");
    var self = Node.Organizations;
    self.prifix = "EDI_Node_Organizations_";
    self.loadData = loadData;
    mini.parse();

    var loaded = false;
    var addOrganizationIDs = [];
    var removeOrganizationIDs = [];
    var currentOntology;

    var dgOntology = mini.get(self.prifix + "dgOntology");
    dgOntology.on("drawcell", ondrawcell);
    dgOntology.on("load", helper.onGridLoad);
    dgOntology.on("selectionchanged", function (e) {
        var data = getParams();
        addOrganizationIDs.clear();
        removeOrganizationIDs.clear();
        var record = dgOntology.getSelected();
        currentOntology = record;
        if (!record.IsOrganizationalEntity) {
            mini.alert("选中的本体不是组织结构型本体");
        }
        else {
            loadOrganizationTreeNode(record.Id, data.nodeID);
        }
        $("#" + self.prifix + "ontologyName").text(record.Name);
    });
    var treeOrganization = mini.get(self.prifix + "treeOrganization");
    treeOrganization.on("beforeload", onTreeBeforeload);
    treeOrganization.on("nodecheck", onNodeCheck);
    treeOrganization.on("cellclick", function (e) {
        if (e.field == "IsAudit") {
            if (!e.node.checked) {
                return false;
            }
            $.post(bootPATH + "../EDI/Node/UpdateOrganization",
                { id: e.node.Id, nodeID: getParams().nodeID, ontologyID: currentOntology.Id, isAudit: e.node.IsAudit },
                function (result) {
                    helper.response(result, function () {
                    });
                }, "json");
        }
    });
    function loadData() {
        dgOntology.load({ nodeID: getParams().nodeID });
        if (!dgOntology.sortField) {
            dgOntology.sortBy("SortCode", "asc");
        }
    }

    function onTreeBeforeload(e) {
        var tree = e.sender;    //树控件
        var node = e.node;      //当前节点
        var params = e.params;  //参数对象
        var data = getParams();
        params.parentID = node.Id;
        params.ontologyID = currentOntology.Id;
        params.nodeID = data.nodeID;
    }

    function loadOrganizationTreeNode(ontologyID, nodeID) {
        $.get(bootPATH + "../EDI/Node/GetOrganizationNodesByParentID"
            , { ontologyID: ontologyID, nodeID: nodeID }
            , function (result) {
                treeOrganization.loadList(result, "Id", "ParentID");
            });
    }

    function getParams() {
        if (self.params && self.params.nodeID) {
            return self.params;
        }
        else {
            return {
                nodeID: $.deparam.fragment().nodeID || $.deparam.querystring().nodeID
            }
        }
    }
    function saveData() {
        if (addOrganizationIDs.length > 0 || removeOrganizationIDs.length > 0) {
            var data = {
                ontologyID: currentOntology.Id,
                nodeID: getParams().nodeID,
                addOrganizationIDs: addOrganizationIDs.join(","),
                removeOrganizationIDs: removeOrganizationIDs.join(",")
            };
            $.post(bootPATH + "../EDI/Node/AddOrRemoveOrganizations",
                data,
                function (result) {
                    helper.response(result, function () {
                        addOrganizationIDs.clear();
                        removeOrganizationIDs.clear();
                    });
                }, "json");
        }
    }
    function onNodeCheck(e) {
        var tree = e.sender;
        var node = e.node;
        var isLeaf = e.isLeaf;
        if (!e.checked) {
            tree.bubbleParent(node, function (n) {
                if (n.Id && n.checked) {
                    if (addOrganizationIDs.indexOf(n.Id) < 0) {
                        addOrganizationIDs.push(n.Id);
                    }
                    if (removeOrganizationIDs.indexOf(n.Id) >= 0) {
                        removeOrganizationIDs.remove(n.Id);
                    }
                }
            }, self);
            tree.cascadeChild(node, function (n) {
                if (n.Id && n.checked) {
                    if (addOrganizationIDs.indexOf(n.Id) < 0) {
                        addOrganizationIDs.push(n.Id);
                    }
                    if (removeOrganizationIDs.indexOf(n.Id) >= 0) {
                        removeOrganizationIDs.remove(n.Id);
                    }
                }
            }, self);
        }
        else {
            tree.bubbleParent(node, function (n) {
                if (n.Id && !n.checked) {
                    if (removeOrganizationIDs.indexOf(n.Id) < 0) {
                        removeOrganizationIDs.push(n.Id);
                    }
                    if (addOrganizationIDs.indexOf(n.Id) >= 0) {
                        addOrganizationIDs.remove(n.Id);
                    }
                }
            }, self);
            tree.cascadeChild(node, function (n) {
                if (n.Id && !n.checked) {
                    if (removeOrganizationIDs.indexOf(n.Id) < 0) {
                        removeOrganizationIDs.push(n.Id);
                    }
                    if (addOrganizationIDs.indexOf(n.Id) >= 0) {
                        addOrganizationIDs.remove(n.Id);
                    }
                }
            }, self);
        }
        saveData();
    }

    function ondrawcell(e) {
        var field = e.field;
        var value = e.value;
        var record = e.record;
        if (field) {
            switch (field) {
                case "ActionIsAllowed":
                case "ActionIsAudit":
                    if (value == "正常" || value == "1" || value == "是" || value == "true") {
                        e.cellHtml = "<span class='icon-enabled width16px'></span>";
                    } else if (value == "禁用" || value == "0" || value == "否" || value == "false") {
                        e.cellHtml = "<span class='icon-disabled width16px'></span>";
                    } break;
                case "IsEnabled":
                case "IsAudit":
                    if (e.sender == dgOntology) {
                        if (value == "正常" || value == "1" || value == "是" || value == "true") {
                            e.cellHtml = "<span class='icon-enabled width16px'></span>";
                        } else if (value == "禁用" || value == "0" || value == "否" || value == "false") {
                            e.cellHtml = "<span class='icon-disabled width16px'></span>";
                        } 
                    }
                    break;
            }
        }
    }
})(window);