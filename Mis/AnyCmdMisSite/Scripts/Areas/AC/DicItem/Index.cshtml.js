/// <reference path="../../../jquery-1.8.3.intellisense.js" />
/// <reference path="../../../miniui/miniui.js" />
(function (window) {
	mini.namespace("AC.DicItem.Index");
	var self = AC.DicItem.Index;
	self.prifix = "AC_DicItem_Index_";
	self.sortUrl = bootPATH + "../AC/DicItem/UpdateSortCode";
	self.help = { appSystemCode: "Anycmd", areaCode: "AC", resourceCode: "DicItem", functionCode: "Index" };
	helper.helperSplitterInOne(self);
	self.add = add;
	self.search = search;

	self.gridReload = function () {
		grid.reload();
	};
	mini.namespace("DicItem.Edit");
	var edit = DicItem.Edit;
	edit.prifix = "AC_DicItem_Index_Edit_";

	var currentDicRecord;
	var tabConfigs = {
		infoTab: {
			url: bootPATH + "../AC/DicItem/Details",
			params: [{ "pName": 'id', "pValue": "Id" }],
			namespace: "DicItem.Details"
		},
		operationLogTab: {
		    url: bootPATH + "../AC/OperationLog/Index",
		    params: [{ "pName": 'targetID', "pValue": "Id" }],
		    namespace: "AC.OperationLog.Index"
		}
	};

	mini.parse();

	var win = mini.get(edit.prifix + "win1");
	var form;
	if (win) {
	    form = new mini.Form(edit.prifix + "form1");
	}

	var key = mini.get(self.prifix + "key");
	key.on("enter", search);
	var tabs1 = mini.get(self.prifix + "tabs1");
	var grid = mini.get(self.prifix + "dgDicItem");
	var dgDic = mini.get(self.prifix + "dgDic");
	dgDic.on("load", helper.onGridLoad);
	dgDic.on("selectionchanged", function (e) {
		currentDicRecord = dgDic.getSelected();
		grid.load({ dicID: currentDicRecord.Id });
		grid.setPageIndex(0);
		grid.sortBy("SortCode", "asc");
	});
	dgDic.load();
	dgDic.sortBy("SortCode", "asc");
	grid.on("drawcell", helper.ondrawcell(self, "AC.DicItem.Index"));
	grid.on("load", helper.onGridLoad);
	
	helper.index.allInOne(
		edit,
		grid,
		bootPATH + "../AC/DicItem/Edit",
		bootPATH + "../AC/DicItem/Edit",
		bootPATH + "../AC/DicItem/Delete",
		self);
	helper.index.tabInOne(grid, tabs1, tabConfigs, self);

	function add() {
		var data = { action: "new" };
		if (win) {
			win.setTitle("添加");
			win.setIconCls("icon-add");
			win.show();
			if (currentDicRecord && currentDicRecord.Id) {
				data.DicID = currentDicRecord.Id;
			}
			edit.SetData(data);
		}
	}

	function search() {
		var data = { key: key.getValue() };
		if (currentDicRecord && currentDicRecord.Id) {
			data.dicID = currentDicRecord.Id;
		}
		grid.load(data);
	}

	helper.edit.allInOne(
		self,
		win,
		bootPATH + "../AC/DicItem/Create",
		bootPATH + "../AC/DicItem/Update",
		bootPATH + "../AC/DicItem/Get",
		form, edit);
})(window);