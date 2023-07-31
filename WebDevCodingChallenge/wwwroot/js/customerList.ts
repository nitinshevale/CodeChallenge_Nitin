
function GenerateTable(customers) {

    //Create a HTML Table element.
    var table = document.createElement("table");
    table.setAttribute('class', 'customerTable');
    table.border = "2";
    var columnCount : number = customers[0].length;
    var row = table.insertRow(-1);

    for (var i = 0; i < columnCount + 2; i++) {
        var headerCell: HTMLElement = document.createElement("th");
        if (i < columnCount) {
            headerCell.innerHTML = customers[0][i];
        }
        else {
            headerCell.innerHTML = "";
        }
        row.appendChild(headerCell);
    }

    for (var i = 1; i < customers.length; i++) {
        row = table.insertRow(-1);
        for (var j = 0; j < columnCount + 2; j++) {
            if (j < columnCount) {
                var cell = row.insertCell(-1);
                var entry: HTMLElement = document.createElement("a");
                entry.innerHTML = customers[i][j];
                if (j == 3)
                    entry.setAttribute('href', "mailto:" + customers[i][j]);

                cell.appendChild(entry);
            }
            else if (j < columnCount + 1) {
                var cell = row.insertCell(-1);
                var button: HTMLElement = document.createElement("button");
                button.setAttribute("type", "button");
                button.setAttribute("id", "Edit_Button");
                button.setAttribute("customer_id", customers[i][0]);
                button.setAttribute("class", "btn btn-primary");
                button.setAttribute("type", "submit");
                button.innerHTML = "Edit";
                cell.appendChild(button);
            }
            else {
                var cell = row.insertCell(-1);
                var button: HTMLElement = document.createElement("button");
                button.setAttribute("type", "button");
                button.setAttribute("id", "Delete_Button");
                button.setAttribute("customer_id", customers[i][0]);
                button.setAttribute("class", "btn btn-primary");
                button.setAttribute("type", "submit");
                button.setAttribute("data-bs-toggle", "modal");
                button.setAttribute("data-bs-target", "deleteModal");
                button.innerHTML = "Delete";
                cell.appendChild(button);
            }
        }
    }

    var customerTable = document.getElementById("customerTable");
    customerTable.innerHTML = "";
    customerTable.appendChild(table);
}

function HandleAlert() {
    var searchParams = new URLSearchParams(window.location.search);
    $("#bootstrap_alert").hide();
    if ((document.location.href).toString().includes("updated")) {
        $("#bootstrap_alert").show();
        $("#bootstrap_alert").html("Table Successfully Updated!");

        setTimeout(
            function () {
                document.location.href = String(document.location.href).replace("?updated", "");
            }, 3000);
    }
}

$(document).ready(function () {
    //Handle all Button Clicks
    $(".btn-primary").click(function () {
        //Handle Customer Edit / Delete functions
        if (this.getAttribute("customer_id") != null) {
            var customer_id: string = this.getAttribute("customer_id");

            if (this.id == "Delete_Button") {
                //@ts-ignore
                $('#deleteModal').modal({ show: true });
                $('#deleteModalTitle').html("Delete Customer " + customer_id);
                $('button#Modal_Delete_Button').attr("customer_id", customer_id);
            }
            else if (this.id == "Edit_Button") {
                try {
                    window.location.href = "/Customer?id=" + customer_id;
                }
                catch (e) {
                    alert("Couldn't Edit Customer. An Error has occurred. | " + e);
                }
            }
            else if (this.id == "Modal_Delete_Button") {
                try {
                    $.ajax({
                        type: "DELETE",
                        crossDomain: true,
                        url: "/CustomerList/Delete/" + customer_id,
                        cache: false,
                        success: function (data) {
                            console.log("Deleteing Customer " + customer_id);
                            location.reload();
                        },
                        error: function (request, error) {
                            alert("Couldn't Delete Customer. An Error has occurred. | " + error);
                        }
                    });
                }
                catch (e) {
                    alert("Couldn't Delete Customer. An Error has occurred. | " + e);
                }
            }
        }

        //Handle everything else
        else {
            if (this.id == "Create_Customer") {
                window.location.href = "/Customer/";
            }
            else if (this.id == "Reset_Table") {
                $("#" + this.id).prop("disabled", true);
                $("#" + this.id).html("Resetting...");
                $.ajax({
                    type: "GET",
                    crossDomain: true,
                    url: "/CustomerList/ResetTable/",
                    cache: false,
                    success: function (data) {
                        location.reload();
                    },
                    error: function (request, error) {
                        alert("Couldn't Reset Table. An Error has occurred. | " + error);
                    }
                });
            }
        }

    });
});

$(window).on('load', function () {
    HandleAlert();
});