var deviceId = localStorage.getItem('device-id');
if (!id) {
    deviceId = Math.floor(Math.random() * 1000);I
    localStorage.setItem('device-id', deviceId);
}
var span = document.getElementById("front");
span.innerHTML = `deviceId='${deviceId}'`;
