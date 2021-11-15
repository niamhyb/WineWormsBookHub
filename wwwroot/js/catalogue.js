// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('#example').DataTable({
        "scrollX": true,
        "scrollResize": true,
        "scrollY": 400,
        "bStateSave": true,
        "pageLength": 25,
        "ordering": true
    });
    $('.dataTables_length').addClass('bs-select');
});