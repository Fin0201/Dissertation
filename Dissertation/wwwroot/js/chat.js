"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    var userAtBottom = isUserAtBottom();

    document.getElementById("messagesList").appendChild(li);
    li.textContent = `${user} says ${message}`;

    if (userAtBottom) {
        markAsRead();
        window.scrollTo(0, document.body.scrollHeight);
    }
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    $.ajax({
        type: "POST",
        url: "/Member/Chat/SendMessage",
        data: { id: chatId, messageContent: message },
        success: function () {
            connection.invoke("SendMessage", user, message).catch(function (err) {
                return console.error(err.toString());
            });
        },
        error: function () {
            alert("Error sending message.");
        }
    });
    event.preventDefault();
});
