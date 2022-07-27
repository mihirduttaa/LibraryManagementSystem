var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblDataLendBookUser').DataTable({
        "ajax": {
            "url": "http://localhost:5211/LendBookUser/GetAllLendRequestByUser",
            "dataSrc": ""
        },
        "columns": [
            { "data": "bookName", "width": "15%" },
            { "data": "customerUserName", "width": "15%" },
            { "data": "status", "width": "15%" },
            {
                "data": "lendDate",
                "render": function (data) {
                    return `${data.substring(0,10)}`
                }
                , "width": "15%" },
            //{
            //    "data": {"id":"id","status":"status"},
            //    "render": function (data) {
            //        return `
            //                <div class="text-center">
            //                  <button ${data.status=='Accepted'?"":"disabled"} href="/Books/UpsertBook?id=${data.id}" class="btn btn-primary mx-1">Pay Now</button>
            //                </div>
            //                `
            //    },
            //    "width": "15%"
            //}
        ]
    });
}

