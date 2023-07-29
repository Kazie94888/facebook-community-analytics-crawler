$(function () {
    var l = abp.localization.getResource("LookOn");
	var merchantStoreService = window.lookOn.controllers.merchantStores.merchantStore;
	
	
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
	    $('#MerchantFilterLookupOpenButton').on('click', '', function () {
        lastNpDisplayNameId = 'Merchant_Filter_Name';
        lastNpIdId = 'MerchantIdFilter';
        _lookupModal.open({
            currentId: $('#MerchantIdFilter').val(),
            currentDisplayName: $('#Merchant_Filter_Name').val(),
            serviceMethod: function () {
                            return window.lookOn.controllers.merchantStores.merchantStore.getMerchantLookup;
                            
            }
        });
    });
    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantStores/CreateModal",
        scriptUrl: "/Pages/MerchantStores/createModal.js",
        modalClass: "merchantStoreCreate"
    });

	var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantStores/EditModal",
        scriptUrl: "/Pages/MerchantStores/editModal.js",
        modalClass: "merchantStoreEdit"
    });

	var getFilter = function() {
        return {
            filterText: $("#FilterText").val(),
            name: $("#NameFilter").val(),
			code: $("#CodeFilter").val(),
            active: (function () {
                var value = $("#ActiveFilter").val();
                if (value === undefined || value === null || value === '') {
                    return '';
                }
                return value === 'true';
            })(),
			merchantId: $("#MerchantIdFilter").val(),			platformId: $("#PlatformIdFilter").val()
        };
    };

    var dataTable = $("#MerchantStoresTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        order: [[1, "asc"]],
        ajax: abp.libs.datatables.createAjax(merchantStoreService.getList, getFilter),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('LookOn.MerchantStores.Edit'),
                                action: function (data) {
                                    editModal.open({
                                     id: data.record.merchantStore.id
                                     });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('LookOn.MerchantStores.Delete'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    merchantStoreService.delete(data.record.merchantStore.id)
                                        .then(function () {
                                            abp.notify.info(l("SuccessfullyDeleted"));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                }
            },
			{ data: "merchantStore.name" },
			{ data: "merchantStore.code" },
            {
                data: "merchantStore.active",
                render: function (active) {
                    return active ? l("Yes") : l("No");
                }
            },
            {
                data: "merchant.name",
                defaultContent : "", 
                orderable: false
            },
            {
                data: "platform.name",
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

    $("#NewMerchantStoreButton").click(function (e) {
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
