  <header class="header">
    <div class="header-brand">
      <div class="wampum-icon">W</div>
      <div>
        <h1>VozSoberana</h1>
        <div class="subtitle">Ierahkwa Platform &middot; Red Soberana</div>
      </div>
    </div>
    <nav class="header-nav">
      <a href="#" onclick="loadFeed(); return false;">Inicio</a>
      <a href="#" onclick="loadTrending(); return false;">Tendencias</a>
      <span class="wampum-balance">W 1,247 WMP</span>
    </nav>
  </header>
  <div class="layout">
    <main class="main-feed">
      <div class="compose-box">
        <div class="compose-header">
          <div class="compose-avatar">TU</div>
          <div style="flex:1;">
            <strong style="font-size:15px;">Tu Voz Importa</strong>
            <div style="font-size:12px;color:var(--text-secondary);">Comparte con la comunidad soberana</div>
          </div>
        </div>
        <textarea id="composeText" placeholder="Que quieres compartir con la comunidad? Usa #hashtags para unirte a la conversacion..." maxlength="500" oninput="updateCharCount()"></textarea>
        <div class="compose-footer">
          <span class="char-count" id="charCount">0 / 500</span>
          <button class="btn-publicar" id="btnPublicar" onclick="publishVoz()" disabled>Publicar Voz</button>
        </div>
      </div>
      <div id="feedContainer"><div class="loading-spinner"><div class="spinner"></div></div></div>
    </main>
    <aside class="sidebar">
      <div class="sidebar-section">
        <h3 class="sidebar-title">Tendencias</h3>
        <div id="trendingContainer"><div class="loading-spinner"><div class="spinner"></div></div></div>
      </div>
      <div class="sidebar-section">
        <h3 class="sidebar-title">W Protocolo Wampum</h3>
        <div class="protocol-info">
          <div class="protocol-stat"><span class="protocol-stat-label">Nodos activos</span><span class="protocol-stat-value">847</span></div>
          <div class="protocol-stat"><span class="protocol-stat-label">Voces hoy</span><span class="protocol-stat-value" id="vocesHoyCount">--</span></div>
          <div class="protocol-stat"><span class="protocol-stat-label">Ciudadanos</span><span class="protocol-stat-value">12,483</span></div>
          <div class="protocol-stat"><span class="protocol-stat-label">Protocolo</span><span class="protocol-stat-value">v0.9.1</span></div>
        </div>
      </div>
      <div class="sidebar-section">
        <h3 class="sidebar-title">Acerca de</h3>
        <p class="protocol-info"><strong>VozSoberana</strong> es la plataforma de microblogging de la red <strong>Ierahkwa</strong>. Tu voz, tus datos, tu soberania. Sin algoritmos extractivos. Sin vigilancia corporativa.</p>
      </div>
    </aside>
  </div>
  <div class="toast" id="toast"></div>
