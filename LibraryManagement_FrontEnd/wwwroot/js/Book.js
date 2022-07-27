var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "http://localhost:5215/api/BooksApi/GetAllBooks",
            "dataSrc": ""
        },
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "author", "width": "15%" },
            { "data": "publisher", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                             <div class=" btn-group" role="group">
                    <a href="/Books/UpsertBook?id=${data}" class="btn btn-primary mx-1"><i class="bi bi-pencil-square"></i> &nbsp; Edit</a>
                    <a onClick=Delete('/Books/DeleteBook?id=${data}') class="btn btn-danger mx-1"><i class="bi bi-trash"></i> &nbsp; Delete</a>
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