document.addEventListener("DOMContentLoaded", function () {
  
  const formulario = document.querySelector("form");
  const passwordA = document.querySelector('input[name="passwordA"]');
  const passwordB = document.querySelector('input[name="passwordB"]');
  const errorPassword = document.getElementById("errorPassword");

  formulario.addEventListener("submit", function (evento) {
    if (passwordA.value !== passwordB.value) {
      evento.preventDefault();
      
      errorPassword.textContent = "Las contraseñas no coinciden.";
      errorPassword.classList.remove("hidden");
      passwordA.style.borderColor = "#dc2626";
      passwordB.style.borderColor = "#dc2626";
      passwordB.focus();

    } else {
      errorPassword.textContent = "";
      errorPassword.classList.add("hidden");
      passwordA.style.borderColor = "";
      passwordB.style.borderColor = "";

    }
  });
});
