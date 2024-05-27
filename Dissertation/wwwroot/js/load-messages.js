var messagesLoaded = 0;

loadMessages();
markAsRead();
window.scrollTo(0, document.body.scrollHeight);

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
    const oldMessageCount = messagesLoaded;
    $.ajax({
        type: "GET",
        url: "/Member/Chat/LoadMessages",
        data: { id: chatId, messagesLoaded: messagesLoaded },
        success: function (messageData) {
            displayMessages(messageData)
            messagesLoaded += messageData.messages.length;
            console.log("Messages loaded");

            const chatWindow = document.getElementById('chat-messages');
            // Checks if no messages were loaded and the user is at the top of the chat. This is only needed on page load since the event listener will not activate multiple times if the user is still at the top.
            if (oldMessageCount != messagesLoaded && chatWindow.scrollTop === 0) {
                loadMessages();
            }
        },
        error: function () {
            alert("Error loading message.");
        }
    });
}

function displayMessages(messageData) {
    messageData.messages.forEach(function (message) {
        var messageList = document.getElementById("messagesList");

        // Create a new row for the message
        var messageRow = document.createElement("div");
        messageRow.className = "row";
        messageList.insertBefore(messageRow, messageList.firstChild);

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

        // Create an image for the message
        if (message.thumbnailPath != null) {
            var image = document.createElement("img");
            image.src = message.thumbnailPath;
            image.className = "message-content-thumbnail";
            image.onclick = function () {
                window.open(message.imagePath, '_blank', 'noopener, noreferrer');
            }
        }

        // Create a list item for the message
        var messageContent = document.createElement("p");
        messageContent.textContent = message.messageContent;

        // Check if the message was sent by the current user
        if (messageData.currentUserId === message.senderId) {
            if (message.thumbnailPath != null) {
                divPrimary.appendChild(image);
            }
            messageContent.className = "primary-message-content";
            divPrimary.appendChild(messageContent);
        } else {
            if (message.thumbnailPath != null) {
                divSecondary.appendChild(image);
            }
            messageContent.className = "secondary-message-content";
            divSecondary.appendChild(messageContent);
        }
    });
}

document.getElementById('chat-messages').addEventListener('scroll', function () {
    const scrollableElement = this;
    const isAtBottom = scrollableElement.scrollHeight - scrollableElement.scrollTop === scrollableElement.clientHeight;
    const isAtTop = scrollableElement.scrollTop === 0;

    if (isAtBottom) {
        markAsRead();
    }

    if (isAtTop) {
        loadMessages();
    }
});


function setMaxHeight() {
    const header = document.getElementById('header');
    const footer = document.getElementById('footer');
    const chat = document.getElementById('chat-messages');

    var headerHeight = header.clientHeight;
    var footerHeight = footer.clientHeight;
    var screenHeight = window.innerHeight;

    var chatHeight = screenHeight - headerHeight - footerHeight;
    chat.style.maxHeight = `${chatHeight}px`;
    chat.style.bottom = `${footerHeight}px`;
    console.log(footerHeight);
    console.log("resizing");
}

setMaxHeight();
window.addEventListener('resize', setMaxHeight);
