document.getElementById("requestSendButton").addEventListener("click", function (event) {
    var dateTime = document.getElementById("requestDate").value;
    var formData = new FormData();

    formData.append('id', itemId);
    formData.append('requestTime', dateTime);

    $.ajax({
        type: "POST",
        url: "/Member/UserRequest/SendRequest",
        data: formData,
        contentType: false,
        processData: false,
        success: function () {
            requestForm = document.getElementById("reviewForm");

            while (requestForm.firstChild) {
                reviewForm.removeChild(reviewForm.firstChild);
            }

            successText = document.createElement("p");
            successText.className = "text-success";
            successText.textContent = "Request sent successfully";
            reviewForm.appendChild(successText);
        },
        error: function () {
            alert("Error sending request.");
        }
    });
});

document.getElementById("cancelRequestButton").addEventListener("click", function (event) {
    var formData = new FormData();

    formData.append('id', itemId);

    $.ajax({
        type: "POST",
        url: "/Member/UserRequest/CancelRequest",
        data: formData,
        contentType: false,
        processData: false,
        success: function () {
            requestForm = document.getElementById("reviewForm");

            while (requestForm.firstChild) {
                reviewForm.removeChild(reviewForm.firstChild);
            }

            successText = document.createElement("p");
            successText.className = "text-success";
            successText.textContent = "Request cancelled successfully";
            reviewForm.appendChild(successText);
        },
        error: function () {
            alert("Error cancelling request.");
        }
    });
});