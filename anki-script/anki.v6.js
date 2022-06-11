var front = document.getElementById("front");
try {
    var storage = document.defaultView.localStorage;
    var deviceId = storage.getItem('device-id');
    if (!id) {
        deviceId = Math.floor(Math.random() * 1000);I
        storage.setItem('device-id', deviceId);
    }
    front.innerHTML = `deviceId='${deviceId}'`;
}
catch (error) {
    front.innerHTML = error.toString();
}
