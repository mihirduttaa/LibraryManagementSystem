var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblDataLendBook').DataTable({
        "ajax": {
            "url": "http://localhost:5215/api/LendBookApi/GetAllLendBookRequests",
            "dataSrc": ""
        },
        "columns": [
            { "data": "bookName", "width": "15%","class":"text-center" },
            { "data": "customerUserName", "width": "15%", "class": "text-center"  },
            { "data": "status", "width": "15%", "class": "text-center" },
            { "data": "lendDate",
                "render": function (data) {
                    return `${data.substring(0, 10)}`
                }
                , "width": "15%", "class": "text-center" },
            { "data": "expectedReturnDate", 
                "render": function (data) {
                    return `${data.substring(0, 10)}`
                }
                , "width": "15%", "class": "text-center" },
            {
                "data": {"id":"id","status":"status"},
                "render": function (data) {
                    return `
                             <div class=" btn-group" role="group">
                    <a ${data.status == "Pending" ? "" : "hidden"} onClick=AcceptLendRequest('/LendBook/AcceptLendBookStatus?id=${data.id}') class="btn btn-success mx-1"><i class="bi bi-trash"></i> &nbsp; Accept</a>
                    <a ${data.status == "Pending" ? "" : "hidden"} onClick=RejectLendRequest('/LendBook/RejectLendBookStatus?id=${data.id}') class="btn btn-danger mx-1"><i class="bi bi-trash"></i> &nbsp; Reject</a>
                  <a ${data.status == "Accepted" ? "" :"hidden"} href="/Lendbook/ReturnBook?id=${data.id}" class="btn btn-primary mx-1"><i class="bi bi-pencil-square"></i> &nbsp; Return Book</a>
                </div>
                            `
                },
                "width": "15%", "class": "text-center"
            }
        ]
    });
}

function AcceptLendRequest(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, Accept it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'POST',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        Swal.fire(
                            'Accepted!',
                            'Lend Request has been Accepted.',
                            'success'
                        )
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: data.message,

                        });
                    }
                }
            })


        }
    })
}

function RejectLendRequest(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, Reject it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'POST',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        Swal.fire(
                            'Rejected!',
                            'Lend Request has been Rejected.',
                            'success'
                        )
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: data.message,

                        });
                    }
                }
            })


        }
    })
}