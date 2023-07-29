$(function () {
    var l = abp.localization.getResource("LookOn");
    var merchantSubscriptionService = window.lookOn.controllers.merchantSubscriptions.merchantSubscription;


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
                return window.lookOn.controllers.merchantSubscriptions.merchantSubscription.getMerchantLookup;

            }
        });
    });
    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantSubscriptions/CreateModal",
        scriptUrl: "/Pages/MerchantSubscriptions/createModal.js",
        modalClass: "merchantSubscriptionCreate"
    });

    var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantSubscriptions/EditModal",
        scriptUrl: "/Pages/MerchantSubscriptions/editModal.js",
        modalClass: "merchantSubscriptionEdit"
    });
    
    var upgradeModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantSubscriptions/UpgradeModal",
        scriptUrl: "/Pages/MerchantSubscriptions/upgradeModal.js",
        modalClass: "merchantSubscriptionUpgrade"
    });
    var updateStatusModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantSubscriptions/UpdateStatusModal",
        scriptUrl: "/Pages/MerchantSubscriptions/updateStatusModal.js",
        modalClass: "merchantSubscriptionUpdateStatus"
    });

    var getFilter = function () {
        return {
            filterText: $("#FilterText").val(),
            startDateTimeMin: $("#StartDateTimeFilterMin").data().datepicker.getFormattedDate('yyyy-mm-dd'),
            startDateTimeMax: $("#StartDateTimeFilterMax").data().datepicker.getFormattedDate('yyyy-mm-dd'),
            endDateTimeMin: $("#EndDateTimeFilterMin").data().datepicker.getFormattedDate('yyyy-mm-dd'),
            endDateTimeMax: $("#EndDateTimeFilterMax").data().datepicker.getFormattedDate('yyyy-mm-dd'),
            subscriptionType: $("#SubscriptionTypeFilter").val(),
            subscriptionStatus: $("#SubscriptionStatusFilter").val(),
            notificationDateMin: $("#NotificationDateFilterMin").data().datepicker.getFormattedDate('yyyy-mm-dd'),
            notificationDateMax: $("#NotificationDateFilterMax").data().datepicker.getFormattedDate('yyyy-mm-dd'),
            notificationSent: (function () {
                var value = $("#NotificationSentFilter").val();
                if (value === undefined || value === null || value === '') {
                    return '';
                }
                return value === 'true';
            })(),
            notificationSentAtMin: $("#NotificationSentAtFilterMin").data().datepicker.getFormattedDate('yyyy-mm-dd'),
            notificationSentAtMax: $("#NotificationSentAtFilterMax").data().datepicker.getFormattedDate('yyyy-mm-dd'),
            merchantId: $("#MerchantIdFilter").val()
        };
    };

    var dataTable = $("#MerchantSubscriptionsTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        scrollCollapse: false,
        order: [[4, "asc"]],
        ajax: abp.libs.datatables.createAjax(merchantSubscriptionService.getList, getFilter),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('LookOn.MerchantSubscriptions.Edit'),
                                action: function (data) {
                                    editModal.open({
                                        id: data.record.merchantSubscription.id
                                    });
                                }
                            },
                            {
                                text: l("Subscription.Admin.UpdateStatus"),
                                visible: abp.auth.isGranted('LookOn.MerchantSubscriptions.Edit'),
                                action: function (data) {
                                    updateStatusModal.open({
                                        id: data.record.merchantSubscription.id
                                    });
                                }
                            },
                            {
                                text: l("Subscription.Admin.ExtendSubscription"),
                                visible: abp.auth.isGranted('LookOn.MerchantSubscriptions.Edit'),
                                confirmMessage: function () {
                                    return l("Subscription.Admin.ExtendSubscriptionConfirmationMessage");
                                },
                                action: function (data) {
                                    console.log(new Date().toJSON())
                                    console.log(new Date())
                                    merchantSubscriptionService.setSubscription({
                                        merchantId: data.record.merchant.id,
                                        subscriptionType: data.record.merchantSubscription.subscriptionType,
                                        from: new Date().toJSON()
                                    })
                                        .then(function () {
                                            abp.notify.info(l("Subscription.Admin.ExtendSubscriptionSuccessfully"));
                                            dataTable.ajax.reload();
                                        });
                                }
                            },
                            {
                                text: l("Subscription.Admin.UpgradeSubscription"),
                                visible: abp.auth.isGranted('LookOn.MerchantSubscriptions.Edit'),
                                action: function (data) {
                                    upgradeModal.open({
                                        id: data.record.merchantSubscription.id
                                    });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('LookOn.MerchantSubscriptions.Delete'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    merchantSubscriptionService.delete(data.record.merchantSubscription.id)
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
                data: "merchant.name",
                defaultContent: "",
                orderable: false
            },
            {
                data: "merchantSubscription.startDateTime",
                render: function (startDateTime) {
                    if (!startDateTime) {
                        return "";
                    }

                    var date = Date.parse(startDateTime);
                    return (new Date(date)).toLocaleDateString(abp.localization.currentCulture.name);
                }
            },
            {
                data: "merchantSubscription.endDateTime",
                render: function (endDateTime) {
                    if (!endDateTime) {
                        return "";
                    }

                    var date = Date.parse(endDateTime);
                    return (new Date(date)).toLocaleDateString(abp.localization.currentCulture.name);
                }
            },
            {
                data: "merchantSubscription.subscriptionType",
                render: function (subscriptionType) {
                    if (subscriptionType === undefined ||
                        subscriptionType === null) {
                        return "";
                    }

                    var localizationKey = "Enum:SubscriptionType:" + subscriptionType;
                    var localized = l(localizationKey);

                    if (localized === localizationKey) {
                        abp.log.warn("No localization found for " + localizationKey);
                        return "";
                    }

                    return localized;
                }
            },
            {
                data: "merchantSubscription.subscriptionStatus",
                render: function (subscriptionStatus) {
                    if (subscriptionStatus === undefined ||
                        subscriptionStatus === null) {
                        return "";
                    }

                    var localizationKey = "Enum:SubscriptionStatus:" + subscriptionStatus;
                    var localized = l(localizationKey);

                    if (localized === localizationKey) {
                        abp.log.warn("No localization found for " + localizationKey);
                        return "";
                    }

                    return localized;
                }
            },
            {
                data: "merchantSubscription.notificationDate",
                render: function (notificationDate) {
                    if (!notificationDate) {
                        return "";
                    }

                    var date = Date.parse(notificationDate);
                    return (new Date(date)).toLocaleDateString(abp.localization.currentCulture.name);
                }
            },
            {
                data: "merchantSubscription.notificationSent",
                render: function (notificationSent) {
                    return notificationSent ? l("Yes") : l("No");
                }
            },
            {
                data: "merchantSubscription.notificationSentAt",
                render: function (notificationSentAt) {
                    if (!notificationSentAt) {
                        return "";
                    }

                    var date = Date.parse(notificationSentAt);
                    return (new Date(date)).toLocaleDateString(abp.localization.currentCulture.name);
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

    upgradeModal.onResult(function () {
        dataTable.ajax.reload();
    });
    
    updateStatusModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $("#NewMerchantSubscriptionButton").click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    $("#SearchForm").submit(function (e) {
        e.preventDefault();
        dataTable.ajax.reload();
    });

    $('#AdvancedFilterSectionToggler').on('click', function (e) {
        $('#AdvancedFilterSection').toggle();
        abp.ui.unblock();
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
