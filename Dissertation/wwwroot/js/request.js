const cancelRequestButton = document.getElementById("cancelRequestButton");
const requestStart = document.getElementById("requestStart");
const requestSendButton = document.getElementById("requestSendButton");

if (requestSendButton) {
    requestSendButton.addEventListener("click", function (event) {
        var startDate = document.getElementById("requestStart").value;
        var endDate = document.getElementById("requestEnd").value;
        var formData = new FormData();

        formData.append('id', itemId);
        formData.append('requestStart', startDate);
        formData.append('requestEnd', endDate);

        $.ajax({
            type: "POST",
            url: "/Member/Request/SendRequest",
            data: formData,
            contentType: false,
            processData: false,
            success: function () {
                requestForm = document.getElementById("requestForm");

                while (requestForm.firstChild) {
                    requestForm.removeChild(requestForm.firstChild);
                }

                successText = document.createElement("p");
                successText.className = "text-success";
                successText.textContent = "Request sent successfully";
                requestForm.appendChild(successText);
            },
            error: function () {
                alert("Error sending request.");
            }
        });
    });
}

if (cancelRequestButton) {
    cancelRequestButton.addEventListener("click", function (event) {
        var formData = new FormData();

        formData.append('id', itemId);

        $.ajax({
            type: "POST",
            url: "/Member/Request/CancelRequest",
            data: formData,
            contentType: false,
            processData: false,
            success: function () {
                requestForm = document.getElementById("requestForm");

                while (requestForm.firstChild) {
                    requestForm.removeChild(requestForm.firstChild);
                }

                successText = document.createElement("p");
                successText.className = "text-success";
                successText.textContent = "Request cancelled successfully";
                requestForm.appendChild(successText);
            },
            error: function () {
                alert("Error cancelling request.");
            }
        });
    });
}

function setRequestDefault() {
    // Sets the default start date
    const requestStart = document.getElementById("requestStart");
    var minDate = new Date();
    minDate.setDate(minDate.getDate() + 1);
    requestStart.min = formatDate(minDate);

    var maxDate = new Date();
    maxDate.setDate(maxDate.getDate() + 7);
    requestStart.max = formatDate(maxDate);
}

if (requestStart) {
    requestStart.addEventListener("change", function () {
        const requestEnd = document.getElementById("requestEnd");
        requestEnd.value = requestStart.value;

        const startDate = requestStart.value;
        const [year, month, day] = startDate.split('-').map(Number);

        const minDate = new Date(year, month - 1, day);
        minDate.setDate(minDate.getDate() + 1);
        requestEnd.min = formatDate(minDate);

        const maxDate = new Date(year, month - 1, day);
        maxDate.setDate(maxDate.getDate() + itemMaxDays);
        requestEnd.max = formatDate(maxDate);
    });
}

function formatDate(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}

if (requestStart) {
    setRequestDefault();
}