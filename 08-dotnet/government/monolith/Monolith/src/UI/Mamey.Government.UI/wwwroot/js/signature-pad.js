// Minimal signature pad (no external deps)
// Exposes window.mameySignaturePad with init/clear/toDataUrl/hasInk.
(function () {
  const api = {};

  function getCtx(canvas) {
    return canvas.getContext("2d");
  }

  function resizeCanvas(canvas) {
    const dpr = window.devicePixelRatio || 1;
    const rect = canvas.getBoundingClientRect();
    const width = Math.max(1, Math.floor(rect.width));
    const height = Math.max(1, Math.floor(rect.height));

    // Preserve existing image by copying first.
    const old = document.createElement("canvas");
    old.width = canvas.width;
    old.height = canvas.height;
    old.getContext("2d").drawImage(canvas, 0, 0);

    canvas.width = Math.floor(width * dpr);
    canvas.height = Math.floor(height * dpr);

    const ctx = getCtx(canvas);
    ctx.scale(dpr, dpr);
    ctx.drawImage(old, 0, 0, width, height);
  }

  function init(canvas) {
    if (!canvas || canvas.__mameySigInit) return;
    canvas.__mameySigInit = true;
    canvas.__hasInk = false;

    const ctx = getCtx(canvas);
    ctx.lineWidth = 2;
    ctx.lineCap = "round";
    ctx.lineJoin = "round";
    ctx.strokeStyle = "#111";

    let drawing = false;
    let lastX = 0;
    let lastY = 0;

    function pos(evt) {
      const rect = canvas.getBoundingClientRect();
      const x = (evt.clientX - rect.left);
      const y = (evt.clientY - rect.top);
      return { x, y };
    }

    function pointerDown(evt) {
      evt.preventDefault();
      drawing = true;
      const p = pos(evt);
      lastX = p.x;
      lastY = p.y;
      ctx.beginPath();
      ctx.moveTo(lastX, lastY);
    }

    function pointerMove(evt) {
      if (!drawing) return;
      evt.preventDefault();
      const p = pos(evt);
      ctx.lineTo(p.x, p.y);
      ctx.stroke();
      lastX = p.x;
      lastY = p.y;
      canvas.__hasInk = true;
    }

    function pointerUp(evt) {
      if (!drawing) return;
      evt.preventDefault();
      drawing = false;
      ctx.closePath();
    }

    // Keep crisp on resize
    const ro = new ResizeObserver(() => resizeCanvas(canvas));
    ro.observe(canvas);
    canvas.__mameySigResizeObserver = ro;

    canvas.addEventListener("pointerdown", pointerDown, { passive: false });
    canvas.addEventListener("pointermove", pointerMove, { passive: false });
    canvas.addEventListener("pointerup", pointerUp, { passive: false });
    canvas.addEventListener("pointercancel", pointerUp, { passive: false });
    canvas.addEventListener("pointerleave", pointerUp, { passive: false });
  }

  api.init = init;
  api.clear = function (canvas) {
    if (!canvas) return;
    const ctx = getCtx(canvas);
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    canvas.__hasInk = false;
  };
  api.hasInk = function (canvas) {
    return !!(canvas && canvas.__hasInk);
  };
  api.toDataUrl = function (canvas) {
    if (!canvas || !canvas.__hasInk) return null;
    // Export PNG
    return canvas.toDataURL("image/png");
  };

  window.mameySignaturePad = api;
})();




