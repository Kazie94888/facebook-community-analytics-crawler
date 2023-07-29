$(function () {
    var l = abp.localization.getResource("LookOn");
	var userInfoService = window.lookOn.controllers.userInfos.userInfo;
	
	
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
        lastNpDisplayNameId = 'AppUser_Filter_UserName';
        lastNpIdId = 'AppUserIdFilter';
        _lookupModal.open({
            currentId: $('#AppUserIdFilter').val(),
            currentDisplayName: $('#AppUser_Filter_UserName').val(),
            serviceMethod: function () {
                            return window.lookOn.controllers.userInfos.userInfo.getAppUserLookup;
                            
            }
        });
    });
    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "UserInfos/CreateModal",
        scriptUrl: "/Pages/UserInfos/createModal.js",
        modalClass: "userInfoCreate"
    });

	var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "UserInfos/EditModal",
        scriptUrl: "/Pages/UserInfos/editModal.js",
        modalClass: "userInfoEdit"
    });

	var getFilter = function() {
        return {
            filterText: $("#FilterText").val(),
            identificationNumber: $("#IdentificationNumberFilter").val(),
			appUserId: $("#AppUserIdFilter").val()
        };
    };

    var dataTable = $("#UserInfosTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        order: [[1, "asc"]],
        ajax: abp.libs.datatables.createAjax(userInfoService.getList, getFilter),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('LookOn.UserInfos.Edit'),
                                action: function (data) {
                                    editModal.open({
                                     id: data.record.userInfo.id
                                     });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('LookOn.UserInfos.Delete'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    userInfoService.delete(data.record.userInfo.id)
                                        .then(function () {
                                            abp.notify.info(l("SuccessfullyDeleted"));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                }
            },
			{ data: "userInfo.identificationNumber" },
            {
                data: "appUser.userName",
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

    $("#NewUserInfoButton").click(function (e) {
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
