const socket = new WebSocket('wss://ws.finnhub.io?token=cqvo101r01qh7uf1fbegcqvo101r01qh7uf1fbf0');
const StockPrice = document.querySelector("#StockPrice");
const stockSymbol = document.querySelector("#StockSymbol").textContent;
// Connection opened -> Subscribe
socket.addEventListener('open', function (event) {
    socket.send(JSON.stringify({ 'type': 'subscribe', 'symbol': stockSymbol }))
});

// Listen for messages
socket.addEventListener('message', function (event) {
    console.log('Message from server ', event.data);
    if (event.data.type == "ping") {
        console.log("ping");
        return;
    }
    let tradeArray = JSON.parse(event.data);
    StockPrice.textContent = tradeArray.data[tradeArray.data.length - 1].p;
});

// Unsubscribe
var unsubscribe = function (symbol) {
    socket.send(JSON.stringify({ 'type': 'unsubscribe', 'symbol': symbol }))
}

window.addEventListener("unload", () => {
    unsubscribe();
});

