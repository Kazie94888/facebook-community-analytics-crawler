$(function () {
    var l = abp.localization.getResource("LookOn");
	var merchantUserService = window.lookOn.controllers.merchantUsers.merchantUser;
	
    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantStaffs/CreateModal",
        scriptUrl: "/Pages/MerchantStaffs/createModal.js",
        modalClass: "merchantUserCreate"
    });
    
	var getFilter = function() {
        return {
            filterText: $("#FilterText").val(),
            isActive: (function () {
                var value = $("#IsActiveFilter").val();
                if (value === undefined || value === null || value === '') {
                    return '';
                }
                return value === 'true';
            })(),
            merchantId: $("#MerchantIdFilter").val()
        };
    };

    var dataTable = $("#MerchantUsersTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        order: [],
        ajax: abp.libs.datatables.createAjax(merchantUserService.getList, getFilter),
        columnDefs: [
            {
                data: "appUser.email",
                defaultContent : "", 
                orderable: false
            }
        ]
    }));

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });
    
    $("#NewMerchantUserButton").click(function (e) {
        e.preventDefault();
        createModal.open();
    });

	$("#SearchForm").submit(function (e) {
        e.preventDefault();
        dataTable.ajax.reload();
    });

});
