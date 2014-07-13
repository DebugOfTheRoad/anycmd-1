/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    mini.namespace("AC.Menu.Index");
    var self = AC.Menu.Index;
    self.prifix = "AC_Menu_Index_";
    self.help = { appSystemCode: "Anycmd", areaCode: "AC", resourceCode: "Menu", functionCode: "Index" };
    helper.helperSplitterInOne(self);
    self.add = add;
    self.edit = edit;
    mini.namespace("Menu.Edit");
    var edit = Menu.Edit;
    edit.prifix = "AC_Menu_Index_Edit_";
    edit.SaveData = SaveData;
    edit.SetData = SetData;
    var faceInitialized = false;

    window.addNewNode = addNewNode;
    window.updateCurrentNodeName = updateCurrentNodeName;
    window.updateChildNodeName = updateChildNodeName;
    window.removeChildNode = removeChildNode;

    var currentNode;
    var contentWindowWith = 480;
    var contentWindowHeight = 220;
    var gridChildrenInitialized = false;
    var detailsInitialized = false;
    var childreniframe;
    var infoiframe;
    var roleInitialized = false;
    var roleiframe;

    mini.parse();

    var btnRemove = mini.get(self.prifix + "btnRemove");
    if (btnRemove) {
        btnRemove.on("click", remove);
    }
    var btnAdd = mini.get(self.prifix + "btnAdd");
    if (btnAdd) {
        btnAdd.on("click", add);
    }
    var btnEdit = mini.get(self.prifix + "btnEdit");
    if (btnEdit) {
        btnEdit.on("click", function () {
            if (currentNode && currentNode.Id) {
                var id;
                if (currentNode.Id) {
                    id = currentNode.Id;
                }
                else {
                    id = currentNode;
                }
                var data = { action: "edit", id: id };
                win.setTitle("编辑");
                win.setIconCls("icon-edit");
                win.show();
                edit.SetData(data);
            }
            else {
                mini.alert("请选择一个节点");
            }
        });
    }

    var win = mini.get(edit.prifix+"win1");
    var form = new mini.Form(edit.prifix+"form1");

    var tree1 = mini.get(self.prifix + "tree1");
    tree1.on("nodeclick", onNodeClick);
    tree1.on("beforeload", onTreeBeforeload);
    var tabs1 = mini.get(self.prifix + "tabs1");
    tabs1.hide();
    tabs1.on("tabload", ontabload);
    tabs1.on("activechanged", onactivechanged);

    function onNodeClick(e) {
        var tree = e.sender;
        var node = e.node;
        var isLeaf = e.isLeaf;
        currentNode = node;
        $("#"+self.prifix+"tabs1").show();
        loadTabData("refresh", tabs1.getTab(tabs1.activeIndex));
    }

    function onTreeBeforeload(e) {
        var tree = e.sender;
        var node = e.node;
        var params = e.params;

        params.parentID = node.Id;
    }

    function haveSelectedNode() {
        if (!currentNode) {
            mini.alert("请先选择一个节点");
            return false;
        }
        return true;
    }

    function add() {
        if (haveSelectedNode()) {
            var data = { action: "new", ParentID: currentNode.Id };
            win.setTitle("添加");
            win.setIconCls("icon-add");
            win.show();
            edit.SetData(data);
        }
    }

    function remove() {
        if (!currentNode || !currentNode.Id) {
            mini.alert("请选择一个叶节点");
            return;
        }
        if (!currentNode.isLeaf) {
            mini.alert("不能删除父节点")
            return;
        }
        if (haveSelectedNode()) {
            mini.confirm("确定删除记录？", "确定？", function (action) {
                if (action == "ok") {
                    $.post(bootPATH + "../AC/Menu/Delete", { id: currentNode.Id }, function (result) {
                        helper.response(result, function () {
                            tree1.removeNode(currentNode);
                        });
                    }, "json");
                }
            });
        }
    }

    function addNewNode(newNode) {
        if (currentNode.isLeaf || currentNode.expanded) {
            tree1.addNode(newNode, 0, currentNode);
        }
    }
    function updateCurrentNodeName(newName) {
        if (currentNode) {
            tree1.updateNode(currentNode, { Name: newName });
            if (tabs1.activeIndex == 0) {
                var infoTab = tabs1.getTab("infoTab");
                loadTabData("refresh", infoTab);
            }
        }
    }
    function updateChildNodeName(id, newName) {
        if (currentNode.expanded) {
            var nodes = tree1.getChildNodes(currentNode);
            var node;
            if (nodes) {
                for (var i = 0; i < nodes.length; i++) {
                    if (nodes[i].Id == id) {
                        node = nodes[i];
                        break;
                    }
                }
            }
            if (node) {
                tree1.updateNode(node, { Name: newName });
            }
        }
    }
    function removeChildNode(id) {
        if (currentNode.expanded) {
            var nodes = tree1.getChildNodes(currentNode);
            var node;
            if (nodes) {
                for (var i = 0; i < nodes.length; i++) {
                    if (nodes[i].Id == id) {
                        node = nodes[i];
                        break;
                    }
                }
            }
            if (node) {
                tree1.removeNode(node);
            }
        }
    }
    /***********tabs***********/
    function onactivechanged(e) {
        loadTabData(null, e.tab);
    }

    function loadTabData(refresh, tab) {
        var tabName = tab.name;
        switch (tabName) {
            // 详细信息          
            case "infoTab":
                if (refresh || !detailsInitialized) {
                    if (currentNode) {
                        var data = { id: currentNode.Id };
                        if (infoiframe) {
                            infoiframe.contentWindow.Menu.Details.params = data;
                            infoiframe.contentWindow.Menu.Details.loadData();
                        }
                        else {
                            var href = bootPATH + "../AC/Menu/Details";
                            tabs1.loadTab(href, tab, function () {
                                var contentWindow = tabs1.getTabIFrameEl(tab).contentWindow;
                                contentWindow.Menu.Details.params = data;
                                contentWindow.Menu.Details.loadData();
                            });
                        }
                    }

                    detailsInitialized = true;
                } break;
                // 子菜单          
            case "childrenTab":
                if (refresh || !gridChildrenInitialized) {
                    var data = { };
                    if (currentNode) {
                        data.parentID = currentNode.Id;
                        data.parentCode = currentNode.Code;
                    }
                    var href = $.param.fragment(bootPATH + "../AC/Menu/Children", data);
                    if (childreniframe) {
                        childreniframe.contentWindow.location.href = href;
                        childreniframe.contentWindow.Menu.Children.loadData();
                    }
                    else {
                        tabs1.loadTab(href, tab);
                    }

                    gridChildrenInitialized = true;
                } break;
                // 角色          
            case "roleTab":
                if (refresh || !roleInitialized) {
                    var data = {};
                    if (currentNode) {
                        data.menuID = currentNode.Id;
                    }
                    var href = $.param.fragment(bootPATH + "../AC/Menu/Roles", data);
                    if (roleiframe) {
                        roleiframe.contentWindow.location.href = href;
                        roleiframe.contentWindow.Menu.Roles.loadData();
                    }
                    else {
                        tabs1.loadTab(href, tab);
                    }

                    roleInitialized = true;
                } break;
            default:
                alert("意外的tab名:" + tabName);
                break;
        }

        if (refresh) {
            if (tab.name != "infoTab") {
                detailsInitialized = false;
            }
            if (tab.name != "childrenTab") {
                gridChildrenInitialized = false;
            }
            if (tab.name != "roleTab") {
                roleInitialized = false;
            }
        }
    }

    function ontabload(e) {
        var tabs = e.sender;
        var tab = e.tab;
        var tabName = tab.name;
        var iframe = tabs.getTabIFrameEl(e.tab);
        switch (tabName) {
            case "infoTab":
                infoiframe = iframe;
                break;
            case "childrenTab":
                childreniframe = iframe;
                break;
            case "roleTab":
                roleiframe = iframe;
                break;
            default:
                alert("意外的tab名:" + tabName);
                break;
        }
    }

    function SaveData() {
        var parentID = $.deparam.fragment().parentID;
        if (parentID) {
            $("#" + edit.prifix + "form1 input[name='ParentID']").val(parentID);
        }
        var data = $("#" + edit.prifix + "form1").serialize();
        var id = $("#" + edit.prifix + "form1 input[name='Id']").val();
        var url = bootPATH + "../AC/Menu/Create";
        if (id) {
            url = bootPATH + "../AC/Menu/Update";
        }
        form.validate();
        if (form.isValid() == false) return;

        $.post(url, data, function (result) {
            helper.response(result, function () {
                var name = $("#" + edit.prifix + "form1 input[name='Name']").val();
                if (id) {
                    if (window.updateCurrentNodeName) {
                        window.updateCurrentNodeName(name);
                    }
                    if (window.parent.updateChildNodeName) {
                        window.parent.updateChildNodeName(id, name);
                    }
                }
                else {
                    if (window.addNewNode) {
                        window.addNewNode({ Id: result.id, Name: name, isLeaf: true });
                    }
                    if (window.parent.addNewNode) {
                        window.parent.addNewNode({ Id: result.id, Name: name, isLeaf: true });
                    }
                }
                edit.CloseWindow("save");
            });
        }, "json");
    };

    function SetData(data) {
        //跨页面传递的数据对象，克隆后才可以安全使用
        data = mini.clone(data);
        if (data.action == "edit") {
            $.ajax({
                url: bootPATH + "../AC/Menu/Get",
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
        bootPATH + "../AC/Menu/Create",
        bootPATH + "../AC/Menu/Update",
        bootPATH + "../AC/Menu/Get",
        form, edit);
})(window);