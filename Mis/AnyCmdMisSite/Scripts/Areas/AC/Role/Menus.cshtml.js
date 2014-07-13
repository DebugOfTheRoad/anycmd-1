/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window, $) {
    mini.namespace("Role.Menus");
    var self = Role.Menus;
    self.prifix = "AC_Role_Menus_";
    self.loadData = loadData;

    var loaded = false;
    var addMenuIDs = [];
    var removeMenuIDs = [];

    mini.parse();

    var btnRefresh = mini.get(self.prifix + "btnRefresh");
    if (btnRefresh) {
        btnRefresh.on("click", function () {
            loadData();
        });
    }
    var tree1 = mini.get(self.prifix + "tree1");
    tree1.on("nodecheck", onNodeCheck);
    tree1.on("drawnode", onDrawNode);

    function loadData() {
        loadTree();
    }

    function loadTree() {
        var data = getParams();
        helper.requesting();
        if (!tree1.url) {
            tree1.url = bootPATH + "../AC/Menu/GetNodesByRoleID";
        }
        $.get(bootPATH + "../AC/Menu/GetNodesByRoleID",
            data,
            function (result) {
                tree1.loadList(result, "MenuID", "ParentID");
                helper.responsed();
            });
    }

    function getParams() {
        data = {};
        if (self.params && self.params.roleID) {
            data.roleID = self.params.roleID;
        }
        else {
            data.roleID = $.deparam.fragment().roleID || $.deparam.querystring().roleID
        }
        return data;
    }

    function onNodeCheck(e) {
        var tree = e.sender;
        var node = e.node;
        var isLeaf = e.isLeaf;
        if (!e.checked) {
            tree.bubbleParent(node, function (n) {
                if (n.MenuID && n.checked) {
                    if (addMenuIDs.indexOf(n.MenuID) < 0) {
                        addMenuIDs.push(n.MenuID);
                    }
                    if (removeMenuIDs.indexOf(n.MenuID) >= 0) {
                        removeMenuIDs.remove(n.MenuID);
                    }
                }
            }, self);
            tree.cascadeChild(node, function (n) {
                if (n.MenuID && n.checked) {
                    if (addMenuIDs.indexOf(n.MenuID) < 0) {
                        addMenuIDs.push(n.MenuID);
                    }
                    if (removeMenuIDs.indexOf(n.MenuID) >= 0) {
                        removeMenuIDs.remove(n.MenuID);
                    }
                }
            }, self);
        }
        else {
            tree.bubbleParent(node, function (n) {
                if (n.MenuID && !n.checked) {
                    if (removeMenuIDs.indexOf(n.MenuID) < 0) {
                        removeMenuIDs.push(n.MenuID);
                    }
                    if (addMenuIDs.indexOf(n.MenuID) >= 0) {
                        addMenuIDs.remove(n.MenuID);
                    }
                }
            }, self);
            tree.cascadeChild(node, function (n) {
                if (n.MenuID && n.checked) {
                    if (removeMenuIDs.indexOf(n.MenuID) < 0) {
                        removeMenuIDs.push(n.MenuID);
                    }
                    if (addMenuIDs.indexOf(n.MenuID) >= 0) {
                        addMenuIDs.remove(n.MenuID);
                    }
                }
            }, self);
        }
        saveData();
    }
    function saveData() {
        if (addMenuIDs.length > 0 || removeMenuIDs.length > 0) {
            var data = {
                roleID: getParams().roleID,
                addMenuIDs: addMenuIDs.join(","),
                removeMenuIDs: removeMenuIDs.join(",")
            };
            $.post(bootPATH + "../AC/Role/AddOrRemoveMenus",
                data,
                function (result) {
                    helper.response(result, function () {
                        addMenuIDs.clear();
                        removeMenuIDs.clear();
                    });
                }, "json");
        }
    }
    function onDrawNode(e) {
        var tree = e.sender;
        var node = e.node;
        if (!node.Id) {
            e.showCheckBox = false;
        }
    }
    Array.prototype.remove = function (b) {
        var a = this.indexOf(b);
        if (a >= 0) {
            this.splice(a, 1);
            return true;
        }
        return false;
    };
})(window, jQuery);