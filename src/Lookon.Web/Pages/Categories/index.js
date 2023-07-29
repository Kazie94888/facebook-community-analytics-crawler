$(function () {
    var l = abp.localization.getResource("LookOn");
	var categoryService = window.lookOn.controllers.categories.category;
	
	
	
    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Categories/CreateModal",
        scriptUrl: "/Pages/Categories/createModal.js",
        modalClass: "categoryCreate"
    });

	var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Categories/EditModal",
        scriptUrl: "/Pages/Categories/editModal.js",
        modalClass: "categoryEdit"
    });

	var getFilter = function() {
        return {
            filterText: $("#FilterText").val(),
            name: $("#NameFilter").val(),
			code: $("#CodeFilter").val(),
			description: $("#DescriptionFilter").val(),
			orderMin: $("#OrderFilterMin").val(),
			orderMax: $("#OrderFilterMax").val()
        };
    };

    var dataTable = $("#CategoriesTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        order: [[1, "asc"]],
        ajax: abp.libs.datatables.createAjax(categoryService.getList, getFilter),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('LookOn.Categories.Edit'),
                                action: function (data) {
                                    editModal.open({
                                     id: data.record.id
                                     });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('LookOn.Categories.Delete'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    categoryService.delete(data.record.id)
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
			{ data: "code" },
			{ data: "description" },
			{ data: "order" }
        ]
    }));

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $("#NewCategoryButton").click(function (e) {
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
