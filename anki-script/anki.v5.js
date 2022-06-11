var front = document.getElementById("front");
try {
    var deviceId = window.localStorage.getItem('device-id');
    if (!id) {
        deviceId = Math.floor(Math.random() * 1000);I
        window.localStorage.setItem('device-id', deviceId);
    }
    front.innerHTML = `deviceId='${deviceId}'`;
}
catch (error) {
    front.innerHTML = error.toString();
}
