$(function () {
    var l = abp.localization.getResource("LookOn");
	var platformService = window.lookOn.controllers.platforms.platform;
	
	
	
    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Platforms/CreateModal",
        scriptUrl: "/Pages/Platforms/createModal.js",
        modalClass: "platformCreate"
    });

	var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Platforms/EditModal",
        scriptUrl: "/Pages/Platforms/editModal.js",
        modalClass: "platformEdit"
    });

	var getFilter = function() {
        return {
            filterText: $("#FilterText").val(),
            name: $("#NameFilter").val(),
			description: $("#DescriptionFilter").val(),
			url: $("#UrlFilter").val(),
			logoUrl: $("#LogoUrlFilter").val()
        };
    };

    var dataTable = $("#PlatformsTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        order: [[1, "asc"]],
        ajax: abp.libs.datatables.createAjax(platformService.getList, getFilter),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('LookOn.Platforms.Edit'),
                                action: function (data) {
                                    editModal.open({
                                     id: data.record.id
                                     });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('LookOn.Platforms.Delete'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    platformService.delete(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l("SuccessfullyDeleted"));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                }
            },
			{ data: "name" },
			{ data: "description" },
			{ data: "url" },
			{ data: "logoUrl" }
        ]
    }));

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $("#NewPlatformButton").click(function (e) {
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
