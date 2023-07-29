$(function () {
    var l = abp.localization.getResource("LookOn");
    var merchantSocialCommunityService = window.lookOn.controllers.merchantSocialCommunities.merchantSocialCommunity;

    // var getFilter = function() {
    //     return {
    //         filterText: $("#FilterText").val(),
    //         merchantEmail: $("#MerchantEmailFilter").val(),
    //         merchantId: $("#MerchantIdFilter").val()
    //     };
    // };

    var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MerchantSocialCommunities/EditSocialCommunityModal",
        scriptUrl: "/Pages/MerchantSocialCommunities/editSocialCommunityModal.js",
        modalClass: "editSocialCommunityModal"
    });

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

    var getFilter = function() {
        return {
            filterText: $("#FilterText").val(),
            merchantId: $("#MerchantIdFilter").val(),
            hasCommunityId: (function () {
                var value = $("#HasCommunityIdFilter").val();
                if (value === undefined || value === null || value === '') {
                    return '';
                }
                return value === 'true';
            })()
        };
    };

    var dataTable = $("#MerchantSocialCommunitiesTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        order: [],
        ajax: abp.libs.datatables.createAjax(merchantSocialCommunityService.getMerchantSocialCommunities, getFilter),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('LookOn.MerchantSyncInfos.Edit'),
                                action: function (data) {
                                    editModal.open({
                                        merchantId: data.record.merchantId,
                                        socialCommunityName: data.record.socialCommunityName
                                    });
                                }
                            }
                        ]
                }
            },
            {
                data: "merchantName",
                defaultContent: "",
                orderable: false
            },
            {
                data: "socialCommunityName",
                defaultContent: "",
                orderable: false
            },
            {
                data: "socialCommunityId",
                defaultContent: "",
                orderable: false
            },
            {
                data: "url",
                defaultContent: "",
                orderable: false,
                render: function (url) {
                    return '<a href="' + url + '" target="_blank">' + url + '</a>';
                }
            },
            {
                data: "communityType",
                render: function (communityType) {
                    if (communityType === undefined ||
                        communityType === null) {
                        return "";
                    }
                    var localizationKey = "Enum:SocialCommunityType:" + communityType;
                    var localized = l(localizationKey);

                    if (localized === localizationKey) {
                        abp.log.warn("No localization found for " + localizationKey);
                        return "";
                    }

                    return localized;
                },
                orderable: false
            },
            {
                data: "verificationStatus",
                render: function (verificationStatus) {
                    if (verificationStatus === undefined ||
                        verificationStatus === null) {
                        return "";
                    }
                    var localizationKey = "Enum:SocialCommunityVerificationStatus:" + verificationStatus;
                    var localized = l(localizationKey);

                    if (localized === localizationKey) {
                        abp.log.warn("No localization found for " + localizationKey);
                        return "";
                    }

                    return localized;
                },
                orderable: false
            },
            {
                data: "verificationReason",
                render: function (verificationStatus) {
                    return verificationStatus;
                },
                orderable: false
            },
            {
                data: "verifiedAt",
                render: function (verifiedAt) {
                    if (!verifiedAt) {
                        return "";
                    }
                    var date = Date.parse(verifiedAt);
                    return (new Date(date)).toLocaleDateString(abp.localization.currentCulture.name);
                },
                orderable: false
            }
        ]
    }));
    editModal.onResult(function () {
        dataTable.ajax.reload();
    });
});
