﻿/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
/// <reference path="../../../jquery-bbq/jquery.ba-bbq.js" />
(function (window) {
    mini.namespace("EDI.Entity.Index");
    var self = EDI.Entity.Index;
    self.loadData = loadData;
    self.prifix = "EDI_Entity_Index_";
    self.help = { appSystemCode: "Anycmd", areaCode: "EDI", resourceCode: "Entity", functionCode: "Index" };
    helper.helperSplitterInOne(self);
    window.onFilterChanged = onFilterChanged;
    self.gridReload = function () {
        grid.reload();
    };
    self.edit = function (record) {
        var records = grid.getSelecteds();
        if (records && records.length != 1) {
            mini.showTips({
                content: "请选中一行",
                state: "warning",
                x: "center",
                y: "top",
                timeout: 3000
            });
            return;
        }
        entityTabs1.activeTab(entityTabs1.getTab("editTab"));
    }
    self.add = function () {
        var newRow = grid.getRow(0);
        if (treeOrganization && !currentNode) {
            mini.alert("请选择要添加到的组织结构");
            return;
        }
        if (!newRow || !newRow.isNewRow) {
            grid.deselectAll();
            newRow = { isNewRow: true, Id: "-1" };
            if (treeOrganization) {
                newRow.ZZJGM = currentNode.Name;
                newRow.OrganizationCode = currentNode.Code;
            }
            grid.addRow(newRow, 0);
            grid.select(newRow);
        }
    };

    var gridParams;
    var currentNode;
    var filterData = {};
    var filterBoxs = {};
    var fregment = $.deparam.fragment();
    var queryString = $.deparam.querystring();
    var ontologyCode = fregment.ontologyCode || queryString.ontologyCode;
    var ontologyID = fregment.ontologyID || queryString.ontologyID;
    var isArchive = fregment.isArchive || queryString.isArchive;
    var archiveID = fregment.archiveID || queryString.archiveID;

    var tabConfigs = {
        entityTab: {
        },
        distributeCommandTab: {
            url: bootPATH + "../EDI/Command/DistributeMessage?entityTypeCode=DistributeMessage&ontologyCode=" + ontologyCode,
            namespace: "Command.DistributeMessage"
        },
        distributeFailingCommandTab: {
            url: bootPATH + "../EDI/Command/DistributeFailingMessage?entityTypeCode=DistributeFailingMessage&ontologyCode=" + ontologyCode,
            namespace: "Command.DistributeFailingMessage"
        },
        distributedCommandTab: {
            url: bootPATH + "../EDI/Command/DistributedMessage?entityTypeCode=DistributedMessage&ontologyCode=" + ontologyCode,
            namespace: "Command.DistributedMessage"
        },
        transmitCommandTab: {
            url: bootPATH + "../EDI/Command/TransmitCommand?entityTypeCode=TransmitCommand&ontologyCode=" + ontologyCode,
            namespace: "Command.TransmitCommand"
        },
        transmitedCommandTab: {
            url: bootPATH + "../EDI/Command/TransmitedCommand?entityTypeCode=TransmitedCommand&ontologyCode=" + ontologyCode,
            namespace: "Command.TransmitedCommand"
        },
        executedCommandTab: {
            url: bootPATH + "../EDI/Command/HandledCommand?entityTypeCode=HandledCommand&ontologyCode=" + ontologyCode,
            namespace: "Command.HandledCommand"
        },
        executeFailingCommandTab: {
            url: bootPATH + "../EDI/Command/HandleFailingCommand?entityTypeCode=HandleFailingCommand&ontologyCode=" + ontologyCode,
            namespace: "Command.HandleFailingCommand"
        },
        receivedCommandTab: {
            url: bootPATH + "../EDI/Command/ReceivedMessage?entityTypeCode=ReceivedMessage&ontologyCode=" + ontologyCode,
            namespace: "Command.ReceivedMessage"
        },
        unacceptedMessageTab: {
            url: bootPATH + "../EDI/Command/UnacceptedMessage?entityTypeCode=UnacceptedMessage&ontologyCode=" + ontologyCode,
            namespace: "Command.UnacceptedMessage"
        },
        localEventTab: {
            url: bootPATH + "../EDI/Command/LocalEvent?entityTypeCode=LocalEvent&ontologyCode=" + ontologyCode,
            namespace: "Command.LocalEvent"
        },
        clientEventTab: {
            url: bootPATH + "../EDI/Command/ClientEvent?entityTypeCode=ClientEvent&ontologyCode=" + ontologyCode,
            namespace: "Command.ClientEvent"
        }
    };
    var entityTabConfigs = {
        infoTab: {
            url: bootPATH + "../EDI/Entity/Details?ontologyCode=" + ontologyCode,
            params: [{ "pName": 'id', "pValue": "Id" }],
            namespace: "Entity.Details"
        },
        editTab: {
            url: bootPATH + "../EDI/Entity/Edit?ontologyCode=" + ontologyCode,
            params: [{ "pName": 'id', "pValue": "Id" }, { "pName": 'organizationCode', "pValue": "OrganizationCode" }],
            namespace: "Entity.Edit"
        },
        nodeTab: {
            url: bootPATH + "../EDI/Node/EntityNodes?ontologyCode=" + ontologyCode,
            params: [{ "pName": 'entityID', "pValue": "Id" }],
            namespace: "Entity.EntityNodes"
        },
        executedCommandTab: {
            url: bootPATH + "../EDI/Command/EntityHandledCommands?ontologyCode=" + ontologyCode,
            params: [{ "pName": "entityID", "pValue": "Id" }],
            namespace: "Command.EntityHandledCommands"
        },
        executeFailingCommandTab: {
            url: bootPATH + "../EDI/Command/EntityHandleFailingCommands?ontologyCode=" + ontologyCode,
            params: [{ "pName": "entityID", "pValue": "Id" }],
            namespace: "Command.EntityHandleFailingCommands"
        },
        receivedCommandTab: {
            url: bootPATH + "../EDI/Command/EntityReceivedMessages?ontologyCode=" + ontologyCode,
            params: [{ "pName": "entityID", "pValue": "Id" }],
            namespace: "Command.EntityReceivedMessages"
        },
        distributeCommandTab: {
            url: bootPATH + "../EDI/Command/EntityDistributeMessages?ontologyCode=" + ontologyCode,
            params: [{ "pName": "entityID", "pValue": "Id" }],
            namespace: "Command.EntityDistributeMessages"
        },
        distributedCommandTab: {
            url: bootPATH + "../EDI/Command/EntityDistributedMessages?ontologyCode=" + ontologyCode,
            params: [{ "pName": "entityID", "pValue": "Id" }],
            namespace: "Command.EntityDistributedMessages"
        },
        distributeFailingCommandTab: {
            url: bootPATH + "../EDI/Command/EntityDistributeFailingMessages?ontologyCode=" + ontologyCode,
            params: [{ "pName": "entityID", "pValue": "Id" }],
            namespace: "Command.EntityDistributeFailingMessages"
        },
        localEventTab: {
            url: bootPATH + "../EDI/Command/EntityLocalEvents?ontologyCode=" + ontologyCode,
            params: [{ "pName": "entityID", "pValue": "Id" }],
            namespace: "Command.EntityLocalEvents"
        },
        clientEventTab: {
            url: bootPATH + "../EDI/Command/EntityClientEvents?ontologyCode=" + ontologyCode,
            params: [{ "pName": "entityID", "pValue": "Id" }],
            namespace: "Command.EntityClientEvents"
        }
    };

    mini.parse();

    var btnImportExcel = mini.get(self.prifix + "btnImportExcel");
    var btnExportExcel = mini.get(self.prifix + "btnExportExcel");
    if (btnImportExcel) {
        if (window.top.topShowTab) {
            btnImportExcel.on("click", function () {
                var tab = {};
                tab.text = "数据导入" + ontologyCode;
                tab.url = bootPATH + '../EDI/Entity/Import?ontologyCode=' + ontologyCode;
                window.top.topShowTab(tab);
            });
        }
        else {
            btnImportExcel.setHref(bootPATH + '../EDI/Entity/Import?ontologyCode=' + ontologyCode);
            btnImportExcel.setTarget("_blank");
        }
    }
    var Export_win1 = mini.get(self.prifix + "Export_win1");

    if (btnExportExcel) {
        btnExportExcel.on("click", function () {
            Export_win1.show();
        });
    }
    var lbtnArchive = mini.get(self.prifix + "lbtnArchive");
    if (lbtnArchive) {
        if (window.top.topShowTab) {
            lbtnArchive.on("click", function () {
                var tab = {};
                tab.text = lbtnArchive.text;
                tab.url = bootPATH + '../EDI/Archive/Index?ontologyCode=' + ontologyCode + "&ontologyID=" + ontologyID;
                window.top.topShowTab(tab);
            });
        }
        else {
            lbtnArchive.setHref(bootPATH + '../EDI/Archive/Index?ontologyCode=' + ontologyCode + "&ontologyID=" + ontologyID);
        }
    }
    var cblExportElement = mini.get(self.prifix + "cblExportElement");
    $("." + self.prifix + "btnExportCancel").click(function () {
        Export_win1.hide();
    });
    var cblImportElement = mini.get(self.prifix + "cblImportElement");
    $("." + self.prifix + "btnExportAll").click(function () {
        gridParams.elements = cblExportElement.getValue();
        gridParams.exportType = "allPage";
        exportExcel();
    });
    $("." + self.prifix + "btnExportCurrentPage").click(function () {
        gridParams.elements = cblExportElement.getValue();
        gridParams.exportType = "currentPage";
        exportExcel();
    });
    $("." + self.prifix + "btnDownloadTemplate").click(function () {
        gridParams.elements = cblImportElement.getValue();
        gridParams.exportType = "temp";
        exportExcel();
    });
    var limit = mini.get(self.prifix + "limit");
    var btnSearchOrganization = mini.get(self.prifix + "btnSearchOrganization");
    if (btnSearchOrganization) {
        btnSearchOrganization.on("click", searchOrganization);
    }
    var btnSearchClear = mini.get(self.prifix + "btnSearchClear");
    if (btnSearchClear) {
        btnSearchClear.on("click", clearSearch);
    }
    var keyOrganization = mini.get(self.prifix + "keyOrganization");
    if (keyOrganization) {
        keyOrganization.on("enter", searchOrganization);
    }
    var treeOrganization = mini.get(self.prifix + "treeOrganization");
    if (treeOrganization) {
        treeOrganization.on("nodeselect", onOrganizationNodeSelect);
        treeOrganization.on("beforeload", onOrganizationTreeBeforeload);
    }
    var chkbIncludedescendants = mini.get(self.prifix + "chkbIncludedescendants");
    if (chkbIncludedescendants) {
        chkbIncludedescendants.on("checkedchanged", onCheckedChanged);
    }

    var tabs1 = mini.get("tabs1");
    tabs1.on("tabload", ontabload);
    tabs1.on("activechanged", onactivechanged);

    var entityTabs1 = mini.get(self.prifix + "entityTabs1");
    var grid = mini.get(self.prifix + "datagrid1");
    grid.on("beforeload", function (e) {
        if (!isArchive) {
            e.sender.__total = 0;
        }
        gridParams = e.data;
    });
    grid.on("drawcell", helper.ondrawcell(self, "EDI.Entity.Index"));
    grid.on("load", helper.onGridLoad);
    loadData();

    function openImportView() {
        var win = mini.open({
            title: '导入',
            url: bootPATH + '../EDI/Entity/ImportView?ontologyCode=' + ontologyCode,
            showModal: true,
            width: 800,
            height: 600
        });

        win.showAtPos('center', 'middle');
    }

    function loadData() {
        if (isArchive && archiveID != getParams().archiveID) {
            archiveID = getParams().archiveID;
            search();
        }
        else if (treeOrganization) {
            currentNode = treeOrganization.getRootNode();
            if (!currentNode.Id) {
                currentNode = treeOrganization.getChildNodes(currentNode)[0];
            }
            if (currentNode.Id) {
                treeOrganization.selectNode(currentNode);
            }
        }
        else {
            search();
        }
    }

    function getParams() {
        data = {};
        if (isArchive) {
            if (self.params && self.params.archiveID) {
                data.archiveID = self.params.archiveID;
            }
            else {
                var fragment = $.deparam.fragment();
                var querystring = $.deparam.querystring();
                data.archiveID = fragment.archiveID || querystring.archiveID;
            }
        }
        return data;
    }

    function searchOrganization() {
        var k = keyOrganization.getValue().trim();
        if (k == "") {
            treeOrganization.clearFilter();
            $("#" + self.prifix + "msg").hide()
        } else {
            k = k.toLowerCase();
            var anyIsTrue = false;
            treeOrganization.filter(function (node) {
                var name = node.Name ? node.Name.toLowerCase() : "";
                if (!node.expanded && !node.isLeaf && !node.IsCategory) {
                    return false;
                }
                if (name.indexOf(k) != -1) {
                    anyIsTrue = true;
                    return true;
                }
            });
            if (anyIsTrue) {
                $("#" + self.prifix + "msg").hide()
            }
            else {
                $("#" + self.prifix + "msg").show();
            }
        }
        treeOrganization.expandAll();
    }

    function onCheckedChanged(e) {
        search();
    }

    helper.index.allInOne(
        null,
        grid,
        bootPATH + "../EDI/Entity/Edit?ontologyCode=" + ontologyCode,
        bootPATH + "../EDI/Entity/Edit?ontologyCode=" + ontologyCode,
        bootPATH + "../EDI/Entity/DeleteEntity?ontologyCode=" + ontologyCode,
        self);
    helper.index.tabInOne(grid, entityTabs1, entityTabConfigs, self);

    function onOrganizationNodeSelect(e) {
        var tree = e.sender;
        var node = e.node;
        var isLeaf = e.isLeaf;
        currentNode = node;
        var text = "";
        if (node.Code) {
            text = "组织结构码：" + node.Code;
        }
        $("#" + self.prifix + "spanOrgCode").text(text);
        filterData.pageIndex = 0;
        loadTabData("refresh", tabs1.getTab(tabs1.activeIndex));
    }

    function onOrganizationTreeBeforeload(e) {
        var tree = e.sender;
        var node = e.node;
        var params = e.params;

        params.parentID = node.Id;
    }

    function onFilterChanged(e) {
        var textbox = e.sender;
        var value = textbox.getValue();
        var name = textbox.name;
        var oldValue = filterData[name];
        filterBoxs[name] = textbox;
        filterData[name] = value;
        if (!textbox.onEnter) {
            textbox.on("enter", onKeyEnter);
        }
        if ((!oldValue && value) || (oldValue && oldValue != value)) {
            search();
        }
    }

    function search() {
        if (treeOrganization && !currentNode.Id) {
            return;
        }
        var data = { translate: true };
        if (isArchive) {
            data.archiveID = getParams().archiveID;
        }
        var filterArray = [];
        for (var k in filterData) {
            if (filterData[k]) {
                filterArray.push({ field: k, value: filterData[k] });
            }
        }
        data.filters = JSON.stringify(filterArray);
        if (currentNode) {
            data["organizationID"] = currentNode.Id;
            data["organizationCode"] = currentNode.Code;
            if (chkbIncludedescendants.getValue() == "1") {
                if (currentNode) {
                    if (currentNode.isLeaf) {
                        data["includedescendants"] = false;
                    }
                    else {
                        data["includedescendants"] = true;
                    }
                }
            }
            else {
                data["includedescendants"] = false;
            }
        }
        grid.__total = 0;
        if (!grid.sortField) {
            grid.sortBy("CreateOn", "desc");
        }
        grid.load(data);
    }

    function clearSearch() {
        for (var key in filterBoxs) {
            if (filterBoxs[key].setValue) {
                filterBoxs[key].setValue("");
            }
        }
        filterData = {};
        search();
    }

    function onKeyEnter(e) {
        search();
    }

    function loadTabData(refresh, tab) {
        var tabName = tab.name;
        var tabConfig = tabConfigs[tabName];
        if (!tabConfig) {
            mini.alert("意外的tabName:" + tabName);
        }
        if (tabName == "entityTab") {
            if (treeOrganization) {
                if (tab.organizationCode != currentNode.Code) {
                    tab.organizationCode = currentNode.Code;
                    search();
                }
                return;
            }
        }
        if (refresh || !tabConfig['initialized']) {
            var tabBody = $(tabs1.getTabBodyEl(tab));
            var isInner = tabBody.hasClass("inner");
            var params = {};
            if (currentNode) {
                params.organizationCode = currentNode.Code;
            }
            if (isInner) {
                mini.namespace(tabConfig.namespace);
                var module = eval(tabConfig.namespace);
                module.params = params;
                if (tabConfig["isLoaded"]) {
                    module.loadData();
                }
                else {
                    params.isInner = true;
                    tabBody.load(tabConfig.url, params, function () {
                        module.loadData();
                    });
                    tabConfig["isLoaded"] = true;
                }
            }
            else {
                if (tabConfig['iframe']) {
                    var iframe = tabConfig['iframe'];
                    if (tabConfig.namespace) {
                        var module = iframe.contentWindow.eval(tabConfig.namespace);
                        if (module) {
                            module.params = params;
                            module.loadData();
                        }
                        else {
                            mini.alert("未找到命名空间" + tabConfig.namespace);
                        }
                    }
                    else {
                        mini.alert("未指定命名空间" + tabConfig.namespace);
                    }
                }
                else {
                    var href = $.param.fragment(tabConfig.url, params);
                    tabs1.loadTab(href, tab, function () {
                        var module = tabs1.getTabIFrameEl(tab).contentWindow.eval(tabConfig.namespace);
                        if (module) {
                            module.params = params;
                            module.loadData();
                        }
                    });
                }
            }

            tabConfig['initialized'] = true;
        }

        if (refresh) {
            for (var key in tabConfigs) {
                tabConfigs[key]['initialized'] = false;
            }
            tabConfig['initialized'] = true;
        }
    }

    function ontabload(e) {
        var tabs = e.sender;
        var tab = e.tab;
        var tabName = tab.name;
        var iframe = tabs.getTabIFrameEl(e.tab);
        var tabConfig = tabConfigs[tabName];
        if (tabConfig) {
            tabConfig['iframe'] = iframe;
        }
    }

    function onactivechanged(e) {
        loadTabData(null, e.tab);
    }
    function exportExcel() {
        var submitfrm = document.createElement("form");
        submitfrm.action = "Export";
        submitfrm.method = "post";
        submitfrm.target = "_blank";
        document.body.appendChild(submitfrm);

        if (gridParams) {
            gridParams.ontologyCode = ontologyCode;
            gridParams.limit = limit.getValue();
            for (var p in gridParams) {
                var input = mini.append(submitfrm, "<input type='hidden' name='" + p + "'>");
                var v = gridParams[p];
                if (typeof v != "string") v = mini.encode(v);
                input.value = v;
            }
        }

        submitfrm.submit();
        setTimeout(function () {
            submitfrm.parentNode.removeChild(submitfrm);
            Export_win1.hide();
        }, 1000);
    }
})(window);