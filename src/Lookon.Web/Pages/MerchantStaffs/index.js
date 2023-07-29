$(function () {
    var l = abp.localization.getResource("LookOn");
	var merchantUserService = window.lookOn.controllers.merchantUsers.merchantUser;
	
	
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
                            return window.lookOn.controllers.merchantUsers.merchantUser.getMerchantLookup;
                            
            }
        });
    });
    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantStaffs/CreateModal",
        scriptUrl: "/Pages/MerchantStaffs/createModal.js",
        modalClass: "merchantUserCreate"
    });

	var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantStaffs/EditModal",
        scriptUrl: "/Pages/MerchantStaffs/editModal.js",
        modalClass: "merchantUserEdit"
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
			appUserId: $("#AppUserIdFilter").val(),			merchantId: $("#MerchantIdFilter").val()
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
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('LookOn.MerchantUsers.Edit'),
                                action: function (data) {
                                    editModal.open({
                                     id: data.record.merchantUser.id
                                     });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('LookOn.MerchantUsers.Delete'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    merchantUserService.delete(data.record.merchantUser.id)
                                        .then(function () {
                                            abp.notify.info(l("SuccessfullyDeleted"));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                }
            },
            {
                data: "appUser.email",
                defaultContent : "", 
                orderable: false
            },
            {
                data: "merchant.name",
                defaultContent : "", 
                orderable: false
            },
            {
                data: "merchantUser.isActive",
                render: function (isActive) {
                    return ShowBooleanLabel(isActive);
                },
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

    $("#NewMerchantUserButton").click(function (e) {
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
