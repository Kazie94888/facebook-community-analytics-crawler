var abp = abp || {};
$(function () {
    var l = abp.localization.getResource("LookOn");
	var merchantService = window.lookOn.controllers.merchants.merchant;
	
	
        var lastNpIdId = '';
        var lastNpDisplayNameId = '';

        var _lookupModal = new abp.ModalManager({
            viewUrl: abp.appPath + "Shared/LookupModal",
            scriptUrl: "/Pages/Shared/lookupModal.js",
            modalClass: "navigationPropertyLookup"
        });

        $('.lookupCleanButton').on('click', '', function () {
            $(this).parent().parent().find('input').val('');
        });

        _lookupModal.onClose(function () {
            var modal = $(_lookupModal.getModal());
            $('#' + lastNpIdId).val(modal.find('#CurrentLookupId').val());
            $('#' + lastNpDisplayNameId).val(modal.find('#CurrentLookupDisplayName').val());
        });
	    $('#AppUserFilterLookupOpenButton').on('click', '', function () {
        lastNpDisplayNameId = 'AppUser_Filter_Email';
        lastNpIdId = 'AppUserIdFilter';
        _lookupModal.open({
            currentId: $('#AppUserIdFilter').val(),
            currentDisplayName: $('#AppUser_Filter_Email').val(),
            serviceMethod: function () {
                            return window.lookOn.controllers.merchants.merchant.getAppUserLookup;
                            
            }
        });
    });
    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Merchants/CreateModal",
        scriptUrl: "/Pages/Merchants/createModal.js",
        modalClass: "merchantCreate"
    });

	var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Merchants/EditModal",
        scriptUrl: "/Pages/Merchants/editModal.js",
        modalClass: "merchantEdit"
    });

	var getFilter = function() {
        return {
            filterText: $("#FilterText").val(),
            name: $("#NameFilter").val(),
			phone: $("#PhoneFilter").val(),
			address: $("#AddressFilter").val(),
			email: $("#EmailFilter").val(),
			fax: $("#FaxFilter").val(),
			appUserId: $("#AppUserIdFilter").val(),			categoryId: $("#CategoryIdFilter").val()
        };
    };

    var dataTable = $("#MerchantsTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        order: [[1, "asc"]],
        ajax: abp.libs.datatables.createAjax(merchantService.getList, getFilter),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('LookOn.Merchants.Edit'),
                                action: function (data) {
                                    editModal.open({
                                     id: data.record.merchant.id
                                     });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('LookOn.Merchants.Delete'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    merchantService.delete(data.record.merchant.id)
                                        .then(function () {
                                            abp.notify.info(l("SuccessfullyDeleted"));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                }
            },
			{ data: "merchant.name" },
			{ data: "merchant.phone" },
			{ data: "merchant.address" },
			{ data: "merchant.email" },
			{ data: "merchant.fax" },
            {
                data: "appUser.email",
                defaultContent : "", 
                orderable: false
            },
            {
                data: "category.name",
                defaultContent : "", 
                orderable: false
            }
        ]
    }));

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $("#NewMerchantButton").click(function (e) {
        e.preventDefault();
        createModal.open();
    });

	$("#SearchForm").submit(function (e) {
        e.preventDefault();
        dataTable.ajax.reload();
    });

    $('#AdvancedFilterSectionToggler').on('click', function (e) {
        $('#AdvancedFilterSection').toggle();
    });

    $('#AdvancedFilterSection').on('keypress', function (e) {
        if (e.which === 13) {
            dataTable.ajax.reload();
        }
    });

    $('#AdvancedFilterSection select').change(function() {
        dataTable.ajax.reload();
    });
});
