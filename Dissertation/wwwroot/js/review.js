document.addEventListener('DOMContentLoaded', () => {
    const stars = document.querySelectorAll('.star');
    const ratingValue = document.getElementById('ratingValue');

    stars.forEach(star => {
        star.addEventListener('click', handleStarClick);
        star.addEventListener('mouseover', handleStarMouseOver);
        star.addEventListener('mouseout', handleStarMouseOut);
    });

    function handleStarClick(event) {
        const selectedValue = event.target.getAttribute('data-value');
        ratingValue.value = selectedValue;
        stars.forEach(star => {
            if (star.getAttribute('data-value') <= selectedValue) {
                star.classList.add('selected');
            } else {
                star.classList.remove('selected');
            }
        });
    }

    function handleStarMouseOver(event) {
        const hoverValue = event.target.getAttribute('data-value');
        stars.forEach(star => {
            if (star.getAttribute('data-value') <= hoverValue) {
                star.classList.add('hover');
            } else {
                star.classList.remove('hover');
            }
        });
    }

    function handleStarMouseOut() {
        stars.forEach(star => {
            star.classList.remove('hover');
        });
    }
});


document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("reviewText").value;
    var starRating = document.getElementById("ratingValue").value;
    var formData = new FormData();

    formData.append('id', itemId);
    formData.append('message', message);
    formData.append('rating', starRating);

    $.ajax({
        type: "POST",
        url: "/Member/Review/SendReview",
        data: formData,
        contentType: false,
        processData: false,
        success: function () {
            reviewForm = document.getElementById("reviewForm");

            while (reviewForm.firstChild) {
                reviewForm.removeChild(reviewForm.firstChild);
            }

            successText = document.createElement("p");
            successText.className = "text-success";
            successText.textContent = "Review sent successfully";
            reviewForm.appendChild(successText);
        },
        error: function () {
            alert("Error sending message.");
        }
    });
});