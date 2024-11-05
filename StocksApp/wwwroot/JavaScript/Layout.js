
let navigationItems = document.querySelectorAll(".navigation-items");
let title = document.title;
console.log(title);
switch (title) {
    case "Stocks":
        ResetUnderline(navigationItems);
        document.querySelector(".trade-navigation").classList.add("border-underline");
        break;
    case "Orders":
        ResetUnderline(navigationItems);
        document.querySelector(".orders-navigation").classList.add("border-underline");
        break;
    default:
        ResetUnderline(navigationItems);
        document.querySelector(".trade-navigation").classList.add("border-underline");
        break;

}


function ResetUnderline(navigationItems) {
    navigationItems.forEach(item => {
        item.classList.remove("border-underline");
        item.classList.add("borderless-underline");
    });
}

