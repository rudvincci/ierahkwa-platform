// Red Soberana SDK v4.2 — Complete client for 20 microservices
class SoberanoSDK {
  constructor({ baseUrl = 'https://api.soberano.bo', token = null } = {}) {
    this.baseUrl = baseUrl; this.token = token;
  }
  async req(path, opts = {}) {
    const h = { 'Content-Type': 'application/json', ...(this.token && { Authorization: 'Bearer ' + this.token }) };
    const r = await fetch(this.baseUrl + path, { ...opts, headers: { ...h, ...opts.headers } });
    return r.json();
  }
  get(p) { return this.req(p); }
  post(p, b) { return this.req(p, { method: 'POST', body: JSON.stringify(b) }); }
  put(p, b) { return this.req(p, { method: 'PUT', body: JSON.stringify(b) }); }
  del(p) { return this.req(p, { method: 'DELETE' }); }

  // AUTH
  async register(email, pw, nation, lang) { const r = await this.post('/v1/auth/register', { email, password: pw, nation, language: lang }); this.token = r.token; return r; }
  async login(email, pw) { const r = await this.post('/v1/auth/login', { email, password: pw }); this.token = r.token; return r; }
  me() { return this.get('/v1/auth/me'); }

  // BDET BANK (11 engines)
  balance() { return this.get('/v1/wallet/balance'); }
  send(to, amount, currency='WMP') { return this.post('/v1/wallet/send', { to, amount, currency }); }
  history() { return this.get('/v1/wallet/history'); }
  processPayment(data) { return this.post('/v1/payments/process', data); }
  rates() { return this.get('/v1/exchange/rates'); }
  convert(from, to, amount) { return this.post('/v1/exchange/convert', { from, to, amount }); }
  placeOrder(pair, side, type, price, amount) { return this.post('/v1/trading/order', { pair, side, type, price, amount }); }
  cancelOrder(id) { return this.del('/v1/trading/order/' + id); }
  orderBook(pair) { return this.get('/v1/trading/orderbook/' + pair); }
  candles(pair, interval) { return this.get('/v1/trading/candles/' + pair + '?interval=' + (interval||'1h')); }
  sendRemittance(d) { return this.post('/v1/remittance/send', d); }
  trackRemittance(id) { return this.get('/v1/remittance/track/' + id); }
  createEscrow(d) { return this.post('/v1/escrow/create', d); }
  releaseEscrow(id) { return this.post('/v1/escrow/release/' + id); }
  applyLoan(d) { return this.post('/v1/loans/apply', d); }
  loanProducts() { return this.get('/v1/loans/products'); }
  insureProducts() { return this.get('/v1/insurance/products'); }
  stake(amount, nation) { return this.post('/v1/staking/stake', { amount, nation }); }
  unstake(id) { return this.post('/v1/staking/unstake', { stakeId: id }); }
  claimRewards() { return this.post('/v1/staking/claim-rewards'); }
  treasury() { return this.get('/v1/treasury/status'); }
  fiscal() { return this.get('/v1/fiscal/allocation'); }

  // SOCIAL MEDIA (14 routes)
  createPost(d) { return this.post('/v1/posts', d); }
  feed(type='following') { return this.get('/v1/feed?type=' + type); }
  trending() { return this.get('/v1/feed/trending'); }
  createStory(d) { return this.post('/v1/stories', d); }
  comment(postId, text) { return this.post('/v1/comments/' + postId, { text }); }
  like(type, id) { return this.post('/v1/likes/' + type + '/' + id); }
  follow(userId) { return this.post('/v1/follow/' + userId); }
  profile(userId) { return this.get('/v1/profiles/' + userId); }
  createGroup(d) { return this.post('/v1/groups', d); }
  sendMessage(convId, text) { return this.post('/v1/chat/' + convId + '/message', { text }); }
  startLive(d) { return this.post('/v1/live/start', d); }

  // SERVICES (:4010)
  serviceCategories() { return this.get('/v1/services/categories'); }
  searchProviders(q) { return this.get('/v1/services/providers/search?' + new URLSearchParams(q)); }
  bookService(d) { return this.post('/v1/bookings', d); }
  myBookings(role) { return this.get('/v1/bookings/mine?role=' + (role||'customer')); }

  // DOCTOR (:4002)
  bookAppointment(d) { return this.post('/v1/doctor/appointments', d); }
  findDoctors(q) { return this.get('/v1/doctor/doctors?' + new URLSearchParams(q||{})); }
  emergency() { return this.post('/v1/doctor/emergency'); }

  // EDUCATION (:4003)
  courses(q) { return this.get('/v1/education/courses?' + new URLSearchParams(q||{})); }
  enroll(courseId) { return this.post('/v1/education/enroll/' + courseId); }

  // TRANSPORT
  requestRide(d) { return this.post('/v1/rides/request', d); }
  orderFood(d) { return this.post('/v1/food/orders', d); }

  // GOVERNANCE
  createElection(d) { return this.post('/v1/vote/elections', d); }
  vote(electionId, candidateId) { return this.post('/v1/vote/vote/' + electionId, { candidateId }); }

  // IDENTITY
  createIdentity(d) { return this.post('/v1/identity/create', d); }
  verifyIdentity(level) { return this.post('/v1/identity/verify', { level }); }

  // MAIL
  sendEmail(d) { return this.post('/v1/mail/send', d); }
  inbox() { return this.get('/v1/mail/inbox'); }

  // TRANSLATE
  translate(text, from, to) { return this.post('/v1/atabey/translate', { text, from, to }); }
  languages() { return this.get('/v1/atabey/languages'); }

  // SEARCH
  search(q, type) { return this.get('/v1/search?q=' + encodeURIComponent(q) + '&type=' + (type||'all')); }

  // BLOCKCHAIN
  chainStatus() { return this.get('/v1/chain/status'); }
  block(n) { return this.get('/v1/chain/block/' + n); }
  tx(hash) { return this.get('/v1/chain/tx/' + hash); }

  // CLOUD
  uploadFile(d) { return this.post('/v1/cloud/upload', d); }
  myFiles() { return this.get('/v1/cloud/files'); }

  // FARM
  farmRecommend(q) { return this.get('/v1/farm/ai/recommend?' + new URLSearchParams(q||{})); }

  // FREELANCE
  postGig(d) { return this.post('/v1/freelance/gig', d); }
  gigs(q) { return this.get('/v1/freelance/gigs?' + new URLSearchParams(q||{})); }

  // POS
  processSale(d) { return this.post('/v1/pos/sale', d); }

  // TOURISM
  listTours(q) { return this.get('/v1/tourism/tours?' + new URLSearchParams(q||{})); }
  bookTour(tourId, d) { return this.post('/v1/tourism/book/' + tourId, d); }
}

if (typeof module !== 'undefined') module.exports = SoberanoSDK;
if (typeof window !== 'undefined') window.SoberanoSDK = SoberanoSDK;
