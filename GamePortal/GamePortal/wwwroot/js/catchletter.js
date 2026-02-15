document.addEventListener("DOMContentLoaded", () => {

    const target = document.getElementById("target");
    const resetBtn = document.getElementById("resetBtn");

    const baseTop = target.getBoundingClientRect().top;

    target.addEventListener("click", () => {

        const currentTop = target.getBoundingClientRect().top;

        if (currentTop < baseTop - 20) {
            alert("Успех! Вы поймали букву М в прыжке!");
        } else {
            alert("Промах! Буква М была не вверху.");
        }
    });

    resetBtn.addEventListener("click", () => {
        location.reload();
    });

});