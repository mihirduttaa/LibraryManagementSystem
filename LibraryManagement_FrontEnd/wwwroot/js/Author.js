var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblDataAuthor').DataTable({
        "ajax": {
            "url": "http://localhost:5215/api/AuthorsApi/GetAllAuthors",
            "dataSrc": ""
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "country", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                             <div class=" btn-group" role="group">
                    <a href="/Authors/Details?id=${data}" class="btn btn-success mx-1"> Details</a>
                    <a href="/Authors/UpsertAuthor?id=${data}" class="btn btn-primary mx-1"><i class="bi bi-pencil-square"></i> &nbsp; Edit</a>
                    <a onClick=Delete('/Authors/DeleteAuthor?id=${data}') class="btn btn-danger mx-1"><i class="bi bi-trash"></i> &nbsp; Delete</a>
                </div>
                            `
                },
                "width": "15%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        Swal.fire(
                            'Deleted!',
                            'Your file has been deleted.',
                            'success'
                        )
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: 'Could not Delete File!',

                        });
                    }
                }
            })


        }
    })
}