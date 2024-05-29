const options = {
    enableHighAccuracy: true,
    timeout: 5000,
    maximumAge: 0
};

async function getLocation() {
    if ("geolocation" in navigator) {
        // Request the current position
        navigator.geolocation.getCurrentPosition(async function (position) {
            // Success callback
            const latitude = position.coords.latitude;
            const longitude = position.coords.longitude;

            await toPostalCode(latitude, longitude);
        }, function (error) {
            // Error callback
            console.error(`Error: ${error.message}`);
            return null;
        }, options);
    } else {
        console.error("Geolocation is not supported by this browser.");
        return null;
    }
}


// Convert latitude and longitude to postal code using Nominatim API
// Free API
async function toPostalCode(latitude, longitude) {
    const url = `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${latitude}&lon=${longitude}`;

    try {
        const response = await fetch(url);
        const data = await response.json();
        if (data.address && data.address.postcode) {
            console.log(`Postal Code: ${data.address.postcode}`);
            document.getElementById('postcode').value = data.address.postcode;
        } else {
            console.log('Postal code not found');
        }
    } catch (error) {
        console.error('Error fetching geocode data: ', error);
    }
}
