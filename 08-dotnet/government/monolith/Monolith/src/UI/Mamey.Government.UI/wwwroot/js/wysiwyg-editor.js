// Minimal WYSIWYG editor helper for Blazor Server (contenteditable + execCommand).
// NOTE: document.execCommand is deprecated but still widely supported and fine for MVP.

window.mameyWysiwyg = (function () {
  function init(editorEl) {
    if (!editorEl) return;
    editorEl.setAttribute("contenteditable", "true");
    editorEl.classList.add("mamey-wysiwyg-ready");
  }

  function setHtml(editorEl, html) {
    if (!editorEl) return;
    editorEl.innerHTML = html || "";
  }

  function getHtml(editorEl) {
    if (!editorEl) return "";
    return editorEl.innerHTML || "";
  }

  function isFocused(editorEl) {
    if (!editorEl) return false;
    return document.activeElement === editorEl;
  }

  function exec(editorEl, command) {
    if (!editorEl) return;
    editorEl.focus();
    try {
      document.execCommand(command, false, null);
    } catch (_) {
      // ignore
    }
  }

  function insertLink(editorEl) {
    if (!editorEl) return;
    editorEl.focus();
    var url = window.prompt("Link URL:", "https://");
    if (!url) return;
    try {
      document.execCommand("createLink", false, url);
    } catch (_) {
      // ignore
    }
  }

  return {
    init,
    setHtml,
    getHtml,
    isFocused,
    exec,
    insertLink,
  };
})();


