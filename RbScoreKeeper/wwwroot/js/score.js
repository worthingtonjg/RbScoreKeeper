"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/scoreHub").build();

connection.on("Refresh", function (user, message) {
    window.location.reload();
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});

