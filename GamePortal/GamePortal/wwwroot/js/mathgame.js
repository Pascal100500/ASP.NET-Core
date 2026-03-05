document.addEventListener("DOMContentLoaded", () => {

    const num1El = document.getElementById("num1");
    const num2El = document.getElementById("num2");
    const answerEl = document.getElementById("answer");
    const resultEl = document.getElementById("result");

    const checkBtn = document.getElementById("checkBtn");
    const newTaskBtn = document.getElementById("newTaskBtn");

    let num1 = 0;
    let num2 = 0;

    function generateTask() {

        num1 = Math.floor(Math.random() * 10);
        num2 = Math.floor(Math.random() * 10);

        num1El.textContent = num1;
        num2El.textContent = num2;

        answerEl.value = "";
        resultEl.textContent = "";
    }

    checkBtn.addEventListener("click", () => {

        const answer = parseInt(answerEl.value);

        if (answer === num1 + num2) {
            resultEl.textContent = "Правильно! 🎉";
            resultEl.style.color = "green";
        } else {
            resultEl.textContent = "Неправильно ❌";
            resultEl.style.color = "red";
        }

    });

    newTaskBtn.addEventListener("click", generateTask);

    generateTask();

});