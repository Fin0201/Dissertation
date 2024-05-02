﻿var loadButton = document.getElementById("loadPrevious");
var messagesLoaded = 0;

loadMessages();
markAsRead();
window.scrollTo(0, document.body.scrollHeight);

function isUserAtBottom() {
    var totalPageHeight = document.body.scrollHeight;
    var scrollPoint = window.scrollY + window.innerHeight;
    return scrollPoint >= totalPageHeight;
}

function markAsRead() {
    $.ajax({
        type: "POST",
        url: "/Member/Chat/MarkAsRead",
        data: { id: chatId },
        success: function () {
            console.log("Marked as read");
        },
        error: function () {
            alert("Error marking as read.");
        }
    });
}

function loadMessages() {
    if (loadButton) {
        loadButton.disabled = true;
    }
    $.ajax({
        type: "GET",
        url: "/Member/Chat/LoadMessages",
        data: { id: chatId, messagesLoaded: messagesLoaded },
        success: function (messageData) {
            displayMessages(messageData)
            if (messageData.endOfMessages && loadButton) {
                document.getElementById("loadPrevious").remove();
            } else if (loadButton) {
                document.getElementById("loadPrevious").disabled = false;
            }
            messagesLoaded += messageData.messages.length;
        },
        error: function () {
            alert("Error loading message.");
        }
    });
}

function displayMessages(messageData) {
    messageData.messages.forEach(function (message) {
        var li = document.createElement("li");
        if (messageData.currentUserId == message.senderId) {
            li.className = "primary-message";
        } else {
            li.className = "secondary-message";
        }
        var messageList = document.getElementById("messagesList");
        li.textContent = `${message.sender.userName} says ${message.messageContent}`;

        messageList.insertBefore(li, messageList.firstChild);
    });
}

window.addEventListener('scroll', () => {
    // Check if the user is at the bottom of the page
    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight) {
        markAsRead();
    }
});