"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;


connection.on("ReceiveMessage", function (user, message, imagePath, thumbnailPath) {
    console.log("Received message: " + message);
    console.log(thumbnailPath);
    const chatWindow = document.getElementById('chat-messages');
    const userAtBottom = chatWindow.scrollHeight - chatWindow.scrollTop === chatWindow.clientHeight;

    var messageList = document.getElementById("messagesList");

    // Create a new row for the message
    var messageRow = document.createElement("div");
    messageRow.className = "row";
    messageList.appendChild(messageRow);

    var colSecondary = document.createElement("div");
    colSecondary.className = "col";
    messageRow.appendChild(colSecondary);
    var colPrimary = document.createElement("div");
    colPrimary.className = "col";
    messageRow.appendChild(colPrimary);


    // Create two columns for the message
    var divPrimary = document.createElement("div");
    divPrimary.className = "primary-message";
    colPrimary.appendChild(divPrimary);
    var divSecondary = document.createElement("div");
    divSecondary.className = "secondary-message";
    colSecondary.appendChild(divSecondary);

    if (thumbnailPath != null) {
        var image = document.createElement("img");
        image.src = thumbnailPath;
        image.className = "message-content-thumbnail";
        image.onclick = function () {
            window.open(imagePath, '_blank', 'noopener, noreferrer');
        }
    }

    if (message != null) {
        var messageContent = document.createElement("p");
        messageContent.textContent = String(message);
    }
    console.log("good")

    // Check if the message was sent by the current user
    if (user === currentUserName) {
        if (thumbnailPath != null) {
            divPrimary.appendChild(image);
        }
        if (message != null) {
            messageContent.className = "primary-message-content";
            divPrimary.appendChild(messageContent);
        }
    } else {
        if (thumbnailPath != null) {
            divSecondary.appendChild(image);
        }
        if (message != null) {
            messageContent.className = "secondary-message-content";
            divSecondary.appendChild(messageContent);
        }
    }

    // Scroll to the bottom if the user is already at the bottom
    if (userAtBottom) {
        // Marking as read is done in load-messages.js
        chatWindow.scrollTop = chatWindow.scrollHeight;
    }
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    var image = document.getElementById("imageInput").files[0];
    var formData = new FormData();

    formData.append('id', chatId);
    formData.append('messageContent', message);
    formData.append('imageFile', image);

    console.log(formData.message);


    $.ajax({
        type: "POST",
        url: "/Member/Chat/SendMessage",
        data: formData,
        processData: false, // Prevent jQuery from automatically transforming the data into a query string
        contentType: false, // Tell jQuery not to set contentType
        success: function (imageData) {
            connection.invoke("SendMessage", currentUserName, message, imageData.imagePath, imageData.thumbnailPath).catch(function (err) {
                return console.error(err.toString());
            });
        },
        error: function () {
            alert("Error sending message.");
        }
    });
    event.preventDefault();
});


function inputImageWindow() {
    document.getElementById("imageInput").click();
}