// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

function fetchSearchResults(query, page, perPage) {
    $.ajax(`https://localhost:44384/api/search?query=${query}&page=${page}&perPage=${perPage}`)
        .done(res => {
            showResults(res);
        })
        .fail(err => {
            console.log(err);
        });
}

function showResults(res) {
    const list = $('#result-list');
    list.empty();
    for (const [i, result] of res.results.entries()) {
        console.log(result);
        const document = result.item1;
        const similarity = result.item2;
        const title = `${i + 1}. ${document.rawContents.split('\n')[0]} sim: ${similarity.toFixed(2)}`;
        const body = document.rawContents.split('\n').slice(1).join();
        const radios = $('<div>')
            .append($('<label>').append($('<input>', { type: 'radio', name: `feedback${document.hash}`, value: document.hash, class: 'relevant' })).append('Relevant').addClass('radio-inline'))
            .append($('<label>').append($('<input>', { type: 'radio', name: `feedback${document.hash}`, value: document.hash, class: 'irrelevant' })).append('Irrelevant').addClass('radio-inline'));

        const item = $('<li>').addClass('list-group-item')
            .append($('<h3>').addClass('list-group-item-heading').text(title))
            .append($('<p>').addClass('list-group-item-text').text(body))
            .append(radios);
        list.append(item);
    }
}

function search(query) {
    fetchSearchResults(query, 1, 10);
}

$(() => {
    $('#search-button').click(() => {
        const query = $('#query-input').val();
        search(query);
    });
});