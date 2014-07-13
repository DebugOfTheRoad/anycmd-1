/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
(function (window) {
	mini.namespace("EDI.InfoDicItem.Index");
	var self = EDI.InfoDicItem.Index;
	self.prifix = "EDI_InfoDicItem_Index_";
	self.sortUrl = bootPATH + "../EDI/InfoDicItem/UpdateSortCode";
	self.help = { appSystemCode: "Anycmd", areaCode: "EDI", resourceCode: "InfoDicItem", functionCode: "Index" };
	helper.helperSplitterInOne(self);
	self.gridReload = function () {
		grid.reload();
	};
	self.search = search;
	self.loadData = loadData;
	mini.namespace("InfoDicItem.Edit");
	var edit = InfoDicItem.Edit;
	edit.prifix = "EDI_InfoDicItem_Index_Edit_";

	var tabConfigs = {
		infoTab: {
		    url: bootPATH + "../EDI/InfoDicItem/Details",
		    entityTypeCode: 'InfoDicItem',
		    controller: 'InfoDicItem',
			params: [{ "pName": 'id', "pValue": "Id" }],
			namespace: "InfoDicItem.Details"
		},
		operationLogTab: {
		    url: bootPATH + "../AC/OperationLog/Index",
		    params: [{ "pName": 'targetID', "pValue": "Id" }],
		    namespace: "AC.OperationLog.Index"
		}
	};
	self.filters = {
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
	var dicFilters = {
		DicName: {
			field: 'Name',
			type: 'string',
			comparison: 'like'
		},
		DicCode: {
			field: 'Code',
			type: 'string',
			comparison: 'like'
		}
	};

	mini.parse();

	var win = mini.get(edit.prifix + "win1");
	var form;
	if (win) {
	    form = new mini.Form(edit.prifix + "form1");
	}

	var tabs1 = mini.get(self.prifix + "tabs1");
	var grid = mini.get(self.prifix + "dgDicItem");
	grid.on("drawcell", helper.ondrawcell(self, "EDI.InfoDicItem.Index"));
	grid.on("load", helper.onGridLoad);

	helper.index.allInOne(
		edit,
		grid,
		bootPATH + "../EDI/InfoDicItem/Edit",
		bootPATH + "../EDI/InfoDicItem/Edit",
		bootPATH + "../EDI/InfoDicItem/Delete",
		self);
	helper.index.tabInOne(grid, tabs1, tabConfigs, self);

	function loadData() {
	    search();
	}

	function getParams() {
	    data = {};
	    if (self.params && self.params.infoDicID) {
	        data.infoDicID = self.params.infoDicID;
	    }
	    else {
	        data.infoDicID = $.deparam.fragment().infoDicID || $.deparam.querystring().infoDicID
	    }
	    return data;
	}

	function search() {
		var data = {};
		data.dicID = getParams().infoDicID;
		var filterArray = [];
		for (var k in self.filters) {
			var filter = self.filters[k];
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
		    grid.sortBy("SortCode", "asc");
		}
		grid.load(data);
	}

	function dgDicOndrawcell(e) {
		var field = e.field;
		var value = e.value;
		var columnName = e.column.name;
	};

	helper.edit.allInOne(
		self,
		win,
		{
		    url: bootPATH + "../EDI/InfoDicItem/Create",
		    params: [{
		        "pName": 'InfoDicID', "getValue": function () {
		            return getParams().infoDicID;
		        }}]
		},
		bootPATH + "../EDI/InfoDicItem/Update",
		bootPATH + "../EDI/InfoDicItem/Get",
		form, edit);
})(window);