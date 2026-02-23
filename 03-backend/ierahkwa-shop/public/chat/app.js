/**
 * Ierahkwa Chat - Frontend Application
 * MVVM Pattern - Socket.IO (SignalR-style)
 * Sovereign Government of Ierahkwa Ne Kanienke
 */

// ============================================
// MODEL - Data Layer
// ============================================
class ChatModel {
  constructor() {
    this.socket = null;
    this.connected = false;
    this.currentUser = null;
    this.currentRoom = 'general';
    this.rooms = [];
    this.messages = new Map();
    this.onlineUsers = [];
    this.typingUsers = new Map();
  }

  connect(serverUrl) {
    return new Promise((resolve, reject) => {
      this.socket = io(serverUrl, {
        transports: ['websocket', 'polling'],
        reconnection: true,
        reconnectionAttempts: 10,
        reconnectionDelay: 1000
      });
      this.socket.on('connect', () => { this.connected = true; resolve(this.socket); });
      this.socket.on('connect_error', (err) => reject(err));
      this.socket.on('disconnect', () => { this.connected = false; });
    });
  }

  invoke(method, data = {}) {
    return new Promise((resolve, reject) => {
      if (!this.socket || !this.connected) { reject(new Error('Not connected')); return; }
      this.socket.emit(method, data, (response) => {
        if (response && response.success) resolve(response);
        else reject(new Error(response?.error || 'Unknown error'));
      });
    });
  }

  on(event, callback) { if (this.socket) this.socket.on(event, callback); }
  emit(event, data) { if (this.socket && this.connected) this.socket.emit(event, data); }
}

// ============================================
// VIEWMODEL - State & Logic
// ============================================
class ChatViewModel {
  constructor() {
    this.model = new ChatModel();
    this.view = null;
    this.state = {
      isLoggedIn: false,
      isConnecting: false,
      username: '',
      currentRoom: 'general',
      rooms: [],
      messages: [],
      onlineUsers: [],
      typingUsers: []
    };
    this.typingTimeout = null;
    this.isTyping = false;
    this.loadSavedUser();
  }

  setView(view) { this.view = view; }

  loadSavedUser() {
    const saved = localStorage.getItem('ierahkwa_chat_user');
    if (saved) { try { this.state.username = JSON.parse(saved).username || ''; } catch {} }
  }

  saveUser(user) { localStorage.setItem('ierahkwa_chat_user', JSON.stringify(user)); }

  async login(username) {
    if (this.state.isConnecting) return;
    this.state.isConnecting = true;
    this.state.username = username;
    this.view?.updateLoginState(true);

    try {
      await this.model.connect(window.location.origin);
      this.setupEventHandlers();
      const result = await this.model.invoke('Connect', {
        username: username,
        userId: localStorage.getItem('ierahkwa_user_id') || null
      });

      this.model.currentUser = result.user;
      this.state.isLoggedIn = true;
      this.state.rooms = result.rooms || [];
      this.state.onlineUsers = result.onlineUsers || [];

      localStorage.setItem('ierahkwa_user_id', result.user.id);
      this.saveUser(result.user);

      this.view?.showChatScreen(result.user);
      this.view?.updateRoomsList(this.state.rooms);
      this.view?.updateUsersList(this.state.onlineUsers);
      await this.loadMessages('general');
      this.view?.showToast('Connected successfully', 'success');
    } catch (error) {
      console.error('Login error:', error);
      this.view?.showToast('Connection failed. Please try again.', 'error');
    } finally {
      this.state.isConnecting = false;
      this.view?.updateLoginState(false);
    }
  }

  async sendMessage(content) {
    if (!content.trim()) return;
    try {
      await this.model.invoke('SendMessage', { content: content.trim(), roomId: this.state.currentRoom });
    } catch (error) {
      console.error('Send message error:', error);
      this.view?.showToast('Failed to send message', 'error');
    }
  }

  async loadMessages(roomId) {
    try {
      const result = await this.model.invoke('GetMessageHistory', { roomId, limit: 50 });
      this.state.messages = result.messages || [];
      this.view?.renderMessages(this.state.messages);
      this.view?.scrollToBottom();
    } catch (error) { console.error('Load messages error:', error); }
  }

  async joinRoom(roomId) {
    if (roomId === this.state.currentRoom) return;
    try {
      const result = await this.model.invoke('JoinRoom', { roomId });
      this.state.currentRoom = roomId;
      this.state.messages = result.messages || [];
      this.view?.updateCurrentRoom(result.room);
      this.view?.renderMessages(this.state.messages);
      this.view?.scrollToBottom();
      this.view?.highlightActiveRoom(roomId);
    } catch (error) {
      console.error('Join room error:', error);
      this.view?.showToast('Failed to join room', 'error');
    }
  }

  async createRoom(name, description) {
    try {
      const result = await this.model.invoke('CreateRoom', { name, description });
      this.view?.showToast(`Channel #${result.room.name} created`, 'success');
      this.view?.closeModal('createRoomModal');
    } catch (error) {
      console.error('Create room error:', error);
      this.view?.showToast(error.message || 'Failed to create room', 'error');
    }
  }

  updateTyping(isTyping) {
    if (isTyping && !this.isTyping) {
      this.isTyping = true;
      this.model.emit('StartTyping', { roomId: this.state.currentRoom });
    }
    if (this.typingTimeout) clearTimeout(this.typingTimeout);
    this.typingTimeout = setTimeout(() => {
      if (this.isTyping) {
        this.isTyping = false;
        this.model.emit('StopTyping', { roomId: this.state.currentRoom });
      }
    }, 2000);
  }

  updateStatus(status) { this.model.emit('UpdateStatus', { status }); }

  setupEventHandlers() {
    this.model.on('ReceiveMessage', (message) => {
      if (message.roomId === this.state.currentRoom) {
        this.state.messages.push(message);
        this.view?.appendMessage(message);
        if (message.userId !== this.model.currentUser?.id) this.playNotificationSound();
      }
    });

    this.model.on('UserTyping', (data) => {
      if (data.roomId !== this.state.currentRoom || data.userId === this.model.currentUser?.id) return;
      if (data.isTyping) {
        if (!this.state.typingUsers.includes(data.username)) this.state.typingUsers.push(data.username);
      } else {
        this.state.typingUsers = this.state.typingUsers.filter(u => u !== data.username);
      }
      this.view?.updateTypingIndicator(this.state.typingUsers);
    });

    this.model.on('UserConnected', (user) => { this.view?.showToast(`${user.username} joined`, 'info'); });
    this.model.on('UserDisconnected', (user) => {
      this.state.typingUsers = this.state.typingUsers.filter(u => u !== user.username);
      this.view?.updateTypingIndicator(this.state.typingUsers);
    });
    this.model.on('OnlineUsersUpdated', (users) => { this.state.onlineUsers = users; this.view?.updateUsersList(users); });
    this.model.on('UserStatusChanged', (data) => {
      const user = this.state.onlineUsers.find(u => u.id === data.userId);
      if (user) { user.status = data.status; this.view?.updateUsersList(this.state.onlineUsers); }
    });
    this.model.on('RoomCreated', (room) => { this.state.rooms.push(room); this.view?.updateRoomsList(this.state.rooms); });
    this.model.on('UserJoinedRoom', (data) => {
      if (data.roomId === this.state.currentRoom) this.view?.appendSystemMessage(`${data.user.username} joined the channel`);
    });
    this.model.socket.on('disconnect', () => { this.view?.showToast('Disconnected. Reconnecting...', 'warning'); });
    this.model.socket.on('reconnect', () => {
      this.view?.showToast('Reconnected!', 'success');
      this.model.invoke('Connect', { username: this.state.username, userId: localStorage.getItem('ierahkwa_user_id') });
    });
  }

  playNotificationSound() {
    try {
      const ctx = new (window.AudioContext || window.webkitAudioContext)();
      const osc = ctx.createOscillator();
      const gain = ctx.createGain();
      osc.connect(gain);
      gain.connect(ctx.destination);
      osc.frequency.value = 800;
      osc.type = 'sine';
      gain.gain.value = 0.1;
      osc.start();
      gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.2);
      osc.stop(ctx.currentTime + 0.2);
    } catch {}
  }
}

// ============================================
// VIEW - UI Layer
// ============================================
class ChatView {
  constructor(viewModel) {
    this.vm = viewModel;
    this.vm.setView(this);
    this.elements = {
      loginScreen: document.getElementById('loginScreen'),
      chatScreen: document.getElementById('chatScreen'),
      loginForm: document.getElementById('loginForm'),
      usernameInput: document.getElementById('usernameInput'),
      sidebar: document.getElementById('sidebar'),
      userAvatar: document.getElementById('userAvatar'),
      userDisplayName: document.getElementById('userDisplayName'),
      roomsList: document.getElementById('roomsList'),
      usersList: document.getElementById('usersList'),
      onlineCount: document.getElementById('onlineCount'),
      statusSelector: document.getElementById('statusSelector'),
      currentRoomName: document.getElementById('currentRoomName'),
      typingIndicator: document.getElementById('typingIndicator'),
      messagesContainer: document.getElementById('messagesContainer'),
      messagesList: document.getElementById('messagesList'),
      scrollToBottom: document.getElementById('scrollToBottom'),
      messageForm: document.getElementById('messageForm'),
      messageInput: document.getElementById('messageInput'),
      createRoomModal: document.getElementById('createRoomModal'),
      createRoomForm: document.getElementById('createRoomForm'),
      roomNameInput: document.getElementById('roomNameInput'),
      roomDescInput: document.getElementById('roomDescInput'),
      mobileMenuBtn: document.getElementById('mobileMenuBtn'),
      toggleSidebarBtn: document.getElementById('toggleSidebarBtn'),
      createRoomBtn: document.getElementById('createRoomBtn'),
      connectionToast: document.getElementById('connectionToast')
    };
    this.bindEvents();
    this.init();
  }

  init() {
    if (this.vm.state.username) this.elements.usernameInput.value = this.vm.state.username;
    if ('serviceWorker' in navigator) navigator.serviceWorker.register('/chat/sw.js').catch(() => {});
  }

  bindEvents() {
    this.elements.loginForm.addEventListener('submit', (e) => {
      e.preventDefault();
      const username = this.elements.usernameInput.value.trim();
      if (username) this.vm.login(username);
    });

    this.elements.messageForm.addEventListener('submit', (e) => {
      e.preventDefault();
      const content = this.elements.messageInput.value;
      if (content.trim()) { this.vm.sendMessage(content); this.elements.messageInput.value = ''; this.autoResizeInput(); }
    });

    this.elements.messageInput.addEventListener('input', () => { this.vm.updateTyping(true); this.autoResizeInput(); });
    this.elements.messageInput.addEventListener('keydown', (e) => {
      if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); this.elements.messageForm.dispatchEvent(new Event('submit')); }
    });

    this.elements.statusSelector.addEventListener('change', (e) => { this.vm.updateStatus(e.target.value); });
    this.elements.mobileMenuBtn.addEventListener('click', () => { this.elements.sidebar.classList.add('open'); });
    this.elements.toggleSidebarBtn.addEventListener('click', () => { this.elements.sidebar.classList.remove('open'); });
    this.elements.createRoomBtn.addEventListener('click', () => { this.elements.createRoomModal.classList.remove('hidden'); });

    this.elements.createRoomForm.addEventListener('submit', (e) => {
      e.preventDefault();
      const name = this.elements.roomNameInput.value.trim();
      const desc = this.elements.roomDescInput.value.trim();
      if (name) { this.vm.createRoom(name, desc); this.elements.roomNameInput.value = ''; this.elements.roomDescInput.value = ''; }
    });

    document.querySelectorAll('.modal-close').forEach(btn => {
      btn.addEventListener('click', () => this.closeModal(btn.dataset.modal));
    });
    document.querySelectorAll('.modal-backdrop').forEach(backdrop => {
      backdrop.addEventListener('click', () => backdrop.closest('.modal').classList.add('hidden'));
    });

    this.elements.scrollToBottom.addEventListener('click', () => this.scrollToBottom(true));
    this.elements.messagesContainer.addEventListener('scroll', () => {
      const { scrollTop, scrollHeight, clientHeight } = this.elements.messagesContainer;
      const isNearBottom = scrollHeight - scrollTop - clientHeight < 100;
      this.elements.scrollToBottom.classList.toggle('visible', !isNearBottom);
    });

    document.addEventListener('click', (e) => {
      if (window.innerWidth <= 768 && !this.elements.sidebar.contains(e.target) && !this.elements.mobileMenuBtn.contains(e.target)) {
        this.elements.sidebar.classList.remove('open');
      }
    });
  }

  updateLoginState(isLoading) {
    const btn = this.elements.loginForm.querySelector('button');
    if (isLoading) { btn.disabled = true; btn.innerHTML = '<span>Connecting...</span>'; }
    else { btn.disabled = false; btn.innerHTML = '<span>Join Chat</span><svg viewBox="0 0 24 24" width="20" height="20"><path fill="currentColor" d="M12 4l-1.41 1.41L16.17 11H4v2h12.17l-5.58 5.59L12 20l8-8z"/></svg>'; }
  }

  showChatScreen(user) {
    this.elements.loginScreen.classList.add('hidden');
    this.elements.chatScreen.classList.remove('hidden');
    this.elements.userAvatar.style.backgroundColor = user.avatar.color;
    this.elements.userAvatar.textContent = user.avatar.initials;
    this.elements.userDisplayName.textContent = user.username;
    setTimeout(() => this.elements.messageInput.focus(), 100);
  }

  updateRoomsList(rooms) {
    this.elements.roomsList.innerHTML = rooms.map(room => `
      <li data-room-id="${room.id}" class="${room.id === this.vm.state.currentRoom ? 'active' : ''}">
        <span class="room-icon">#</span>
        <span class="room-name">${this.escapeHtml(room.name)}</span>
      </li>
    `).join('');
    this.elements.roomsList.querySelectorAll('li').forEach(li => {
      li.addEventListener('click', () => {
        this.vm.joinRoom(li.dataset.roomId);
        if (window.innerWidth <= 768) this.elements.sidebar.classList.remove('open');
      });
    });
  }

  highlightActiveRoom(roomId) {
    this.elements.roomsList.querySelectorAll('li').forEach(li => li.classList.toggle('active', li.dataset.roomId === roomId));
  }

  updateCurrentRoom(room) { this.elements.currentRoomName.textContent = `# ${room.name}`; }

  updateUsersList(users) {
    this.elements.onlineCount.textContent = users.length;
    this.elements.usersList.innerHTML = users.map(user => `
      <li>
        <div class="avatar" style="background-color: ${user.avatar?.color || '#6c5ce7'}">
          ${user.avatar?.initials || user.username.slice(0, 2).toUpperCase()}
          <span class="status-dot ${user.status}"></span>
        </div>
        <span class="user-name">${this.escapeHtml(user.username)}</span>
      </li>
    `).join('');
  }

  updateTypingIndicator(typingUsers) {
    if (typingUsers.length === 0) this.elements.typingIndicator.textContent = '';
    else if (typingUsers.length === 1) this.elements.typingIndicator.textContent = `${typingUsers[0]} is typing...`;
    else if (typingUsers.length === 2) this.elements.typingIndicator.textContent = `${typingUsers[0]} and ${typingUsers[1]} are typing...`;
    else this.elements.typingIndicator.textContent = `${typingUsers.length} people are typing...`;
  }

  renderMessages(messages) {
    this.elements.messagesList.innerHTML = '';
    messages.forEach(msg => this.appendMessage(msg, false));
  }

  appendMessage(message, scroll = true) {
    const isOwn = message.userId === this.vm.model.currentUser?.id;
    const time = new Date(message.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    const html = `
      <div class="message ${isOwn ? 'own' : ''}">
        <div class="avatar" style="background-color: ${message.avatar?.color || '#6c5ce7'}">
          ${message.avatar?.initials || message.username.slice(0, 2).toUpperCase()}
        </div>
        <div class="message-content">
          <div class="message-header">
            <span class="message-author">${this.escapeHtml(message.username)}</span>
            <span class="message-time">${time}</span>
          </div>
          <div class="message-bubble">${this.formatMessageContent(message.content)}</div>
        </div>
      </div>
    `;
    this.elements.messagesList.insertAdjacentHTML('beforeend', html);
    if (scroll) this.scrollToBottom();
  }

  appendSystemMessage(text) {
    this.elements.messagesList.insertAdjacentHTML('beforeend', `<div class="system-message">${this.escapeHtml(text)}</div>`);
    this.scrollToBottom();
  }

  scrollToBottom(smooth = false) {
    this.elements.messagesContainer.scrollTo({ top: this.elements.messagesContainer.scrollHeight, behavior: smooth ? 'smooth' : 'auto' });
  }

  autoResizeInput() {
    const input = this.elements.messageInput;
    input.style.height = 'auto';
    input.style.height = Math.min(input.scrollHeight, 120) + 'px';
  }

  closeModal(modalId) { document.getElementById(modalId)?.classList.add('hidden'); }

  showToast(message, type = 'info') {
    const toast = this.elements.connectionToast;
    toast.className = `toast visible ${type}`;
    toast.querySelector('.toast-message').textContent = message;
    setTimeout(() => toast.classList.remove('visible'), 3000);
  }

  escapeHtml(text) { const div = document.createElement('div'); div.textContent = text; return div.innerHTML; }

  formatMessageContent(content) {
    let formatted = this.escapeHtml(content);
    formatted = formatted.replace(/(https?:\/\/[^\s<]+)/g, '<a href="$1" target="_blank" rel="noopener noreferrer">$1</a>');
    formatted = formatted.replace(/\n/g, '<br>');
    return formatted;
  }
}

// ============================================
// INITIALIZE APP
// ============================================
document.addEventListener('DOMContentLoaded', () => {
  const viewModel = new ChatViewModel();
  const view = new ChatView(viewModel);
  window.chatApp = { viewModel, view };
  console.log('🚀 Ierahkwa Chat initialized');
});
