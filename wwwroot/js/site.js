// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

if (window.lucide) {
  window.lucide.createIcons({
    attrs: {
      "stroke-width": 1.75
    }
  });
}

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
  setButtonLabel(button, "Đang giải thích...");
  panel.hidden = false;
  panel.setAttribute("aria-busy", "true");
  panel.replaceChildren(createText("p", "Đang kết nối dịch vụ giải thích..."));

  const form = new URLSearchParams();
  form.set("sourceId", button.dataset.sourceId ?? "");
  form.set("candidateId", button.dataset.candidateId ?? "");
  form.set("__RequestVerificationToken", token);

  try {
    const response = await fetch(button.dataset.endpoint ?? "/Drugs/ExplainAlternative", {
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
        "Không thể tải giải thích lúc này. Vui lòng đọc các lý do và cảnh báo theo bộ quy tắc ở trên."
      )
    );
  } finally {
    panel.removeAttribute("aria-busy");
    button.disabled = false;
    setButtonLabel(button, "Giải thích lại");
  }
});

function renderAiExplanation(panel, result) {
  const heading = document.createElement("div");
  heading.className = "ai-explanation-heading";
  heading.append(
    createText("strong", "Giải thích đề xuất"),
    createText(
      "span",
      result.isAiGenerated ? "Nội dung do AI hỗ trợ" : "Theo bộ quy tắc",
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
    `Nguồn: ${result.provider ?? "không xác định"} / ${result.model ?? "không có"}`,
    "ai-explanation-metadata"
  );
  panel.replaceChildren(
    heading,
    createText("p", result.summary ?? "Không có nội dung tóm tắt."),
    list,
    createText("p", result.limitations ?? "Chỉ dùng để tham khảo.", "ai-explanation-limit"),
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

function setButtonLabel(button, value) {
  const label = button.querySelector("span");
  if (label) {
    label.textContent = value;
    return;
  }

  button.textContent = value;
}
