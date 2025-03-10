// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5062/gamehub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(() => {
    console.log("Conectado al hub");
}).catch(err => console.error(err));
