$(function () {
    var l = abp.localization.getResource("LookOn");
    var merchantSyncInfoService = window.lookOn.controllers.merchantSyncInfos.merchantSyncInfo;

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
        lastNpDisplayNameId = 'Merchant_Name';
        lastNpIdId = 'MerchantIdFilter';
        _lookupModal.open({
            currentId: $('#MerchantIdFilter').val(),
            currentDisplayName: $('#Merchant_Name').val(),
            serviceMethod: function () {
                return window.lookOn.controllers.merchantSyncInfos.merchantSyncInfo.getMerchantLookup;

            }
        });
    });
    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantSyncInfos/CreateModal",
        scriptUrl: "/Pages/MerchantSyncInfos/createModal.js",
        modalClass: "merchantSyncInfoCreate"
    });

    var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantSyncInfos/EditModal",
        scriptUrl: "/Pages/MerchantSyncInfos/editModal.js",
        modalClass: "merchantSyncInfoEdit"
    });

    var getFilter = function () {
        return {
            filterText: $("#FilterText").val(),
            merchantEmail: $("#MerchantEmailFilter").val(),
            merchantId: $("#MerchantIdFilter").val()
        };
    };

    var dataTable = $("#MerchantSyncInfosTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        order: [],
        ajax: abp.libs.datatables.createAjax(merchantSyncInfoService.getList, getFilter),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("ForceSyncOrders"),
                                visible: abp.auth.isGranted('LookOn.MerchantSyncInfos.Edit'),
                                confirmMessage: function () {
                                    return l("ForceSyncOrdersConfirmationMessage");
                                },
                                action: function (data) {
                                    merchantSyncInfoService.forceSyncOrders(data.record.merchantSyncInfo.id)
                                        .then(function () {
                                            abp.notify.info(l("SuccessfullyForceSyncOrders"));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                }
            },
            {data: "merchant.name", orderable: false},
            {
                data: "merchantSyncInfo.page1SyncStatus", orderable: false,
                render: function (page1SyncStatus) {
                    if (page1SyncStatus === undefined ||
                        page1SyncStatus === null) {
                        return "";
                    }

                    var localizationKey = "Enum:MerchantSyncStatus:" + page1SyncStatus;
                    var localized = l(localizationKey);

                    if (localized === localizationKey) {
                        abp.log.warn("No localization found for " + localizationKey);
                        return "";
                    }

                    return localized;
                }
            },
            {
                data: "merchantSyncInfo.page2SyncStatus", orderable: false,
                render: function (page1SyncStatus) {
                    if (page1SyncStatus === undefined ||
                        page1SyncStatus === null) {
                        return "";
                    }

                    var localizationKey = "Enum:MerchantSyncStatus:" + page1SyncStatus;
                    var localized = l(localizationKey);

                    if (localized === localizationKey) {
                        abp.log.warn("No localization found for " + localizationKey);
                        return "";
                    }

                    return localized;
                }
            },
            {
                data: "merchantSyncInfo.page3SyncStatus", orderable: false,
                render: function (page1SyncStatus) {
                    if (page1SyncStatus === undefined ||
                        page1SyncStatus === null) {
                        return "";
                    }

                    var localizationKey = "Enum:MerchantSyncStatus:" + page1SyncStatus;
                    var localized = l(localizationKey);

                    if (localized === localizationKey) {
                        abp.log.warn("No localization found for " + localizationKey);
                        return "";
                    }

                    return localized;
                }
            },
            {
                data: "merchantSyncInfo.ecomScan", orderable: false,
                render: function (ecomScan) {
                    if (!ecomScan) {
                        return "";
                    }

                    var statusKey = "Enum:MerchantJobStatus:" + ecomScan.rawOrderSyncStatus;
                    var status = l(statusKey);

                    if (!!ecomScan.lastRawOrderSyncedAt) {
                        var date = Date.parse(ecomScan.lastRawOrderSyncedAt);
                        status += " </br>" + (new Date(date)).toLocaleDateString(abp.localization.currentCulture.name);
                    }

                    return status;
                }
            },
            {
                data: "merchantSyncInfo.ecomScan", orderable: false, render: function (ecomScan) {
                    if (!ecomScan) {
                        return "";
                    }

                    var statusKey = "Enum:MerchantJobStatus:" + ecomScan.cleanOrderSyncStatus;
                    var status = l(statusKey);

                    if (!!ecomScan.lastRawOrderSyncedAt) {
                        var date = Date.parse(ecomScan.lastRawOrderSyncedAt);
                        status += " </br>" + (new Date(date)).toLocaleDateString(abp.localization.currentCulture.name);
                    }

                    return status;
                }
            },
        ]
    }));

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $("#NewMerchantSyncInfoButton").click(function (e) {
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

    $('#AdvancedFilterSection select').change(function () {
        dataTable.ajax.reload();
    });
});
