// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("click", async (event) => {
  const button = event.target.closest(".js-ai-explanation");
  if (!button) {
    return;
  }

  const panelId = button.getAttribute("aria-controls");
  const panel = panelId ? document.getElementById(panelId) : null;
  const token = document.querySelector(
    "#ai-explanation-token input[name='__RequestVerificationToken']"
  )?.value;
  if (!panel || !token) {
    return;
  }

  button.disabled = true;
  button.textContent = "Dang giai thich...";
  panel.hidden = false;
  panel.setAttribute("aria-busy", "true");
  panel.replaceChildren(createText("p", "Dang ket noi dich vu giai thich..."));

  const form = new URLSearchParams();
  form.set("sourceId", button.dataset.sourceId ?? "");
  form.set("candidateId", button.dataset.candidateId ?? "");
  form.set("__RequestVerificationToken", token);

  try {
    const response = await fetch("/Drugs/ExplainAlternative", {
      method: "POST",
      headers: { "Content-Type": "application/x-www-form-urlencoded" },
      body: form,
      credentials: "same-origin"
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }

    const result = await response.json();
    renderAiExplanation(panel, result);
    button.setAttribute("aria-expanded", "true");
  } catch {
    panel.replaceChildren(
      createText(
        "p",
        "Khong the tai giai thich luc nay. Vui long doc cac ly do va canh bao rule-based o tren."
      )
    );
  } finally {
    panel.removeAttribute("aria-busy");
    button.disabled = false;
    button.textContent = "Giai thich lai";
  }
});

function renderAiExplanation(panel, result) {
  const heading = document.createElement("div");
  heading.className = "ai-explanation-heading";
  heading.append(
    createText("strong", "Giai thich de xuat"),
    createText(
      "span",
      result.isAiGenerated ? "AI generated" : "Rule-based fallback",
      result.isAiGenerated ? "status-badge is-info" : "status-badge is-warning"
    )
  );

  const list = document.createElement("ul");
  list.className = "reason-list";
  for (const checkpoint of result.checkpoints ?? []) {
    list.append(createText("li", checkpoint));
  }

  const metadata = createText(
    "small",
    `Nguon: ${result.provider ?? "khong xac dinh"} / ${result.model ?? "none"}`,
    "ai-explanation-metadata"
  );
  panel.replaceChildren(
    heading,
    createText("p", result.summary ?? "Khong co tom tat."),
    list,
    createText("p", result.limitations ?? "Chi dung de tham khao.", "ai-explanation-limit"),
    metadata
  );
}

function createText(tagName, value, className) {
  const element = document.createElement(tagName);
  element.textContent = value;
  if (className) {
    element.className = className;
  }

  return element;
}
