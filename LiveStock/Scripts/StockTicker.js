if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}

const
    ticker = $.connection.stockTickerMini,
    $stockTable = $('#stockTable'),
    $stockTableBody = $stockTable.find('tbody'),
    rowTemplate = '<tr data-symbol="{Symbol}"><td>{Symbol}</td><td>{Price}</td><td style="{DirectionStyle}">{PercentChange}</td></tr>';

function formatStock(stock) {
    return $.extend(stock, {
        Price: stock.Price.toFixed(2),
        PercentChange: '%' + stock.PercentChange.toFixed(2) + (stock.PercentChange === 0 ? '' : (stock.PercentChange >= 0 ? ' ▲' : ' ▼')),
        DirectionStyle: stock.PercentChange === 0 ? '' : (stock.PercentChange >= 0 ? 'color: green' : 'color: red')
    });
}

function init() {
    ticker.server.getAllStocks().done(function (stocks) {
        $stockTableBody.empty();
        $.each(stocks, function () {
            console.log(this);
            const stock = formatStock(this);
            $stockTableBody.append(rowTemplate.supplant(stock));
        });
    });
}

ticker.client.updateStockPrice = function (stock) {
    console.log(stock);
    const displayStock = formatStock(stock),
    $row = $(rowTemplate.supplant(displayStock));

    $stockTableBody.find('tr[data-symbol=' + stock.Symbol + ']')
        .replaceWith($row);
}

$.connection.hub.start().done(init);