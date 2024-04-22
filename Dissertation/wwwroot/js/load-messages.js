var loadButton = document.getElementById("loadPrevious");
var messagesLoaded = 0;

loadMessages();
window.scrollTo(0, document.body.scrollHeight);

function loadMessages() {
    if (loadButton) {
        loadButton.disabled = true;
    }
    $.ajax({
        type: "GET",
        url: "/Member/Chat/LoadMessages",
        data: { id: chatId, messagesLoaded: messagesLoaded },
        success: function (messageData) {
            displayMessages(messageData.messages)
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

function displayMessages(messages) {
    messages.forEach(function (message) {
        var li = document.createElement("li");
        var messageList = document.getElementById("messagesList");
        li.textContent = `${user} says ${message.messageContent}`;

        messageList.insertBefore(li, messagesList.firstChild);
    });
}