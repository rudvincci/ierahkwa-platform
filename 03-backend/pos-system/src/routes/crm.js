const express = require('express');
const { v4: uuidv4 } = require('uuid');
const router = express.Router();

// Helper to get database from request (set by middleware)
const getDb = (req) => req.app.locals.db || require('../db').getDb();

// ==================== CUSTOMERS ====================

router.get('/customers', (req, res) => {
  try {
    const db = getDb(req);
    const { status, search, page = 1, limit = 20 } = req.query;
    
    let customers = db.prepare('SELECT * FROM customers').all();
    
    if (status) {
      customers = customers.filter(c => c.status === status);
    }
    if (search) {
      const term = search.toLowerCase();
      customers = customers.filter(c => 
        (c.first_name || '').toLowerCase().includes(term) ||
        (c.last_name || '').toLowerCase().includes(term) ||
        (c.email || '').toLowerCase().includes(term) ||
        (c.company_name || '').toLowerCase().includes(term)
      );
    }
    
    const total = customers.length;
    const offset = (page - 1) * limit;
    customers = customers.slice(offset, offset + parseInt(limit));
    
    res.json({
      customers: customers.map(c => ({
        id: c.id,
        uuid: c.uuid,
        companyName: c.company_name,
        firstName: c.first_name,
        lastName: c.last_name,
        email: c.email,
        phone: c.phone,
        city: c.city,
        country: c.country,
        status: c.status,
        source: c.source,
        createdAt: c.created_at
      })),
      pagination: { page: parseInt(page), limit: parseInt(limit), total, pages: Math.ceil(total / limit) }
    });
  } catch (err) {
    console.error('Get customers error:', err);
    res.status(500).json({ error: 'Error fetching customers' });
  }
});

router.post('/customers', (req, res) => {
  try {
    const db = getDb(req);
    const { companyName, firstName, lastName, email, phone, address, city, country, status, source, notes } = req.body;
    
    if (!firstName || !lastName || !email) {
      return res.status(400).json({ error: 'First name, last name, and email required' });
    }
    
    const uuid = uuidv4();
    const result = db.prepare(`
      INSERT INTO customers (uuid, company_name, first_name, last_name, email, phone, address, city, country, status, source, notes, assigned_agent_id, created_at)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `).run(uuid, companyName || null, firstName, lastName, email, phone || null, address || null, city || null, country || null, status || 'lead', source || null, notes || null, req.session.user?.id || 1, new Date().toISOString());
    
    res.status(201).json({ success: true, customer: { id: result.lastInsertRowid, uuid, email, firstName, lastName } });
  } catch (err) {
    console.error('Create customer error:', err);
    res.status(500).json({ error: 'Error creating customer' });
  }
});

router.put('/customers/:id', (req, res) => {
  try {
    const db = getDb(req);
    const { id } = req.params;
    const customers = db.prepare('SELECT * FROM customers').all();
    const customer = customers.find(c => c.id === parseInt(id));
    
    if (!customer) {
      return res.status(404).json({ error: 'Customer not found' });
    }
    
    const { companyName, firstName, lastName, email, phone, address, city, country, status, source, notes } = req.body;
    
    db.prepare(`UPDATE customers SET company_name = ?, first_name = ?, last_name = ?, email = ?, phone = ?, address = ?, city = ?, country = ?, status = ?, source = ?, notes = ?, updated_at = ? WHERE id = ?`)
      .run(companyName || customer.company_name, firstName || customer.first_name, lastName || customer.last_name, email || customer.email, phone || customer.phone, address || customer.address, city || customer.city, country || customer.country, status || customer.status, source || customer.source, notes || customer.notes, new Date().toISOString(), id);
    
    res.json({ success: true, message: 'Customer updated' });
  } catch (err) {
    console.error('Update customer error:', err);
    res.status(500).json({ error: 'Error updating customer' });
  }
});

// ==================== LEADS ====================

router.get('/leads', (req, res) => {
  try {
    const db = getDb(req);
    const { status, priority } = req.query;
    
    let leads = db.prepare('SELECT * FROM leads').all();
    
    if (status) leads = leads.filter(l => l.status === status);
    if (priority) leads = leads.filter(l => l.priority === priority);
    
    // Join with customers
    const customers = db.prepare('SELECT * FROM customers').all();
    leads = leads.map(l => {
      const customer = customers.find(c => c.id === l.customer_id);
      return {
        ...l,
        customerName: customer ? `${customer.first_name} ${customer.last_name}` : null,
        companyName: customer?.company_name
      };
    });
    
    res.json({
      leads: leads.map(l => ({
        id: l.id,
        uuid: l.uuid,
        title: l.title,
        description: l.description,
        value: l.value,
        currency: l.currency || 'USD',
        status: l.status,
        priority: l.priority,
        source: l.source,
        customerId: l.customer_id,
        customerName: l.customerName,
        companyName: l.companyName,
        expectedCloseDate: l.expected_close_date,
        createdAt: l.created_at
      }))
    });
  } catch (err) {
    console.error('Get leads error:', err);
    res.status(500).json({ error: 'Error fetching leads' });
  }
});

router.post('/leads', (req, res) => {
  try {
    const db = getDb(req);
    const { customerId, title, description, value, currency, status, priority, source, expectedCloseDate } = req.body;
    
    if (!title) {
      return res.status(400).json({ error: 'Title required' });
    }
    
    const uuid = uuidv4();
    const result = db.prepare(`
      INSERT INTO leads (uuid, customer_id, title, description, value, currency, status, priority, source, assigned_agent_id, expected_close_date, created_at)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `).run(uuid, customerId || null, title, description || null, value || 0, currency || 'USD', status || 'new', priority || 'medium', source || null, req.session.user?.id || 1, expectedCloseDate || null, new Date().toISOString());
    
    res.status(201).json({ success: true, lead: { id: result.lastInsertRowid, uuid, title } });
  } catch (err) {
    console.error('Create lead error:', err);
    res.status(500).json({ error: 'Error creating lead' });
  }
});

router.put('/leads/:id', (req, res) => {
  try {
    const db = getDb(req);
    const { id } = req.params;
    const { title, description, value, status, priority, expectedCloseDate } = req.body;
    
    const leads = db.prepare('SELECT * FROM leads').all();
    const lead = leads.find(l => l.id === parseInt(id));
    
    if (!lead) {
      return res.status(404).json({ error: 'Lead not found' });
    }
    
    let actualCloseDate = lead.actual_close_date;
    if (status === 'won' || status === 'lost') {
      actualCloseDate = new Date().toISOString().split('T')[0];
    }
    
    db.prepare(`UPDATE leads SET title = ?, description = ?, value = ?, status = ?, priority = ?, expected_close_date = ?, actual_close_date = ?, updated_at = ? WHERE id = ?`)
      .run(title || lead.title, description !== undefined ? description : lead.description, value !== undefined ? value : lead.value, status || lead.status, priority || lead.priority, expectedCloseDate || lead.expected_close_date, actualCloseDate, new Date().toISOString(), id);
    
    res.json({ success: true, message: 'Lead updated' });
  } catch (err) {
    console.error('Update lead error:', err);
    res.status(500).json({ error: 'Error updating lead' });
  }
});

// ==================== TICKETS ====================

router.get('/tickets', (req, res) => {
  try {
    const db = getDb(req);
    const { status, priority } = req.query;
    
    let tickets = db.prepare('SELECT * FROM tickets').all();
    
    if (status) tickets = tickets.filter(t => t.status === status);
    if (priority) tickets = tickets.filter(t => t.priority === priority);
    
    const customers = db.prepare('SELECT * FROM customers').all();
    tickets = tickets.map(t => {
      const customer = customers.find(c => c.id === t.customer_id);
      return {
        ...t,
        customerName: customer ? `${customer.first_name} ${customer.last_name}` : null
      };
    });
    
    res.json({
      tickets: tickets.map(t => ({
        id: t.id,
        uuid: t.uuid,
        ticketNumber: t.ticket_number,
        subject: t.subject,
        description: t.description,
        category: t.category,
        priority: t.priority,
        status: t.status,
        customerName: t.customerName,
        resolution: t.resolution,
        createdAt: t.created_at
      }))
    });
  } catch (err) {
    console.error('Get tickets error:', err);
    res.status(500).json({ error: 'Error fetching tickets' });
  }
});

router.post('/tickets', (req, res) => {
  try {
    const db = getDb(req);
    const { customerId, subject, description, category, priority } = req.body;
    
    if (!subject) {
      return res.status(400).json({ error: 'Subject required' });
    }
    
    const uuid = uuidv4();
    const ticketNumber = `TKT-${Date.now().toString().slice(-8)}`;
    
    const result = db.prepare(`
      INSERT INTO tickets (uuid, ticket_number, customer_id, subject, description, category, priority, status, assigned_agent_id, created_at)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `).run(uuid, ticketNumber, customerId || null, subject, description || null, category || null, priority || 'medium', 'open', req.session.user?.id || 1, new Date().toISOString());
    
    res.status(201).json({ success: true, ticket: { id: result.lastInsertRowid, uuid, ticketNumber, subject } });
  } catch (err) {
    console.error('Create ticket error:', err);
    res.status(500).json({ error: 'Error creating ticket' });
  }
});

router.put('/tickets/:id', (req, res) => {
  try {
    const db = getDb(req);
    const { id } = req.params;
    const { subject, description, category, priority, status, resolution } = req.body;
    
    const tickets = db.prepare('SELECT * FROM tickets').all();
    const ticket = tickets.find(t => t.id === parseInt(id));
    
    if (!ticket) {
      return res.status(404).json({ error: 'Ticket not found' });
    }
    
    let resolvedAt = ticket.resolved_at;
    if ((status === 'resolved' || status === 'closed') && !ticket.resolved_at) {
      resolvedAt = new Date().toISOString();
    }
    
    db.prepare(`UPDATE tickets SET subject = ?, description = ?, category = ?, priority = ?, status = ?, resolution = ?, resolved_at = ?, updated_at = ? WHERE id = ?`)
      .run(subject || ticket.subject, description !== undefined ? description : ticket.description, category || ticket.category, priority || ticket.priority, status || ticket.status, resolution !== undefined ? resolution : ticket.resolution, resolvedAt, new Date().toISOString(), id);
    
    res.json({ success: true, message: 'Ticket updated' });
  } catch (err) {
    console.error('Update ticket error:', err);
    res.status(500).json({ error: 'Error updating ticket' });
  }
});

// ==================== INVOICES ====================

router.get('/invoices', (req, res) => {
  try {
    const db = getDb(req);
    const { status, customerId } = req.query;
    
    let invoices = db.prepare('SELECT * FROM invoices').all();
    
    if (status) invoices = invoices.filter(i => i.status === status);
    if (customerId) invoices = invoices.filter(i => i.customer_id === parseInt(customerId));
    
    const customers = db.prepare('SELECT * FROM customers').all();
    invoices = invoices.map(i => {
      const customer = customers.find(c => c.id === i.customer_id);
      return {
        ...i,
        customerName: customer ? `${customer.first_name} ${customer.last_name}` : null,
        companyName: customer?.company_name,
        customerEmail: customer?.email
      };
    });
    
    res.json({
      invoices: invoices.map(i => ({
        id: i.id,
        uuid: i.uuid,
        invoiceNumber: i.invoice_number,
        customerId: i.customer_id,
        customerName: i.customerName,
        companyName: i.companyName,
        customerEmail: i.customerEmail,
        subtotal: i.subtotal,
        taxRate: i.tax_rate,
        taxAmount: i.tax_amount,
        discount: i.discount,
        total: i.total,
        currency: i.currency || 'USD',
        status: i.status,
        dueDate: i.due_date,
        paidDate: i.paid_date,
        paidAmount: i.paid_amount || 0,
        notes: i.notes,
        createdAt: i.created_at
      }))
    });
  } catch (err) {
    console.error('Get invoices error:', err);
    res.status(500).json({ error: 'Error fetching invoices' });
  }
});

router.post('/invoices', (req, res) => {
  try {
    const db = getDb(req);
    const { customerId, leadId, items, taxRate, discount, dueDate, notes } = req.body;
    
    if (!customerId || !items || items.length === 0) {
      return res.status(400).json({ error: 'Customer and items required' });
    }
    
    let subtotal = 0;
    items.forEach(item => {
      subtotal += item.quantity * item.unitPrice;
    });
    
    const tax = taxRate || 0;
    const discountAmount = discount || 0;
    const taxAmount = subtotal * (tax / 100);
    const total = subtotal + taxAmount - discountAmount;
    
    const uuid = uuidv4();
    const invoiceNumber = `INV-${Date.now().toString().slice(-8)}`;
    
    const result = db.prepare(`
      INSERT INTO invoices (uuid, invoice_number, customer_id, lead_id, subtotal, tax_rate, tax_amount, discount, total, currency, status, due_date, notes, created_by, created_at)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `).run(uuid, invoiceNumber, customerId, leadId || null, subtotal, tax, taxAmount, discountAmount, total, 'USD', 'draft', dueDate || null, notes || null, req.session.user?.id || 1, new Date().toISOString());
    
    const invoiceId = result.lastInsertRowid;
    
    items.forEach(item => {
      const itemTotal = item.quantity * item.unitPrice;
      db.prepare(`
        INSERT INTO invoice_items (invoice_id, description, quantity, unit_price, total, created_at)
        VALUES (?, ?, ?, ?, ?, ?)
      `).run(invoiceId, item.description, item.quantity, item.unitPrice, itemTotal, new Date().toISOString());
    });
    
    res.status(201).json({ success: true, invoice: { id: invoiceId, uuid, invoiceNumber, total } });
  } catch (err) {
    console.error('Create invoice error:', err);
    res.status(500).json({ error: 'Error creating invoice' });
  }
});

router.put('/invoices/:id', (req, res) => {
  try {
    const db = getDb(req);
    const { id } = req.params;
    const { status, paidAmount, notes } = req.body;
    
    const invoices = db.prepare('SELECT * FROM invoices').all();
    const invoice = invoices.find(i => i.id === parseInt(id));
    
    if (!invoice) {
      return res.status(404).json({ error: 'Invoice not found' });
    }
    
    let newStatus = status || invoice.status;
    let paidDate = invoice.paid_date;
    let newPaidAmount = paidAmount !== undefined ? paidAmount : invoice.paid_amount;
    
    if (newPaidAmount >= invoice.total) {
      newStatus = 'paid';
      paidDate = new Date().toISOString().split('T')[0];
    } else if (newPaidAmount > 0) {
      newStatus = 'partial';
    }
    
    db.prepare(`UPDATE invoices SET status = ?, paid_amount = ?, paid_date = ?, notes = ?, updated_at = ? WHERE id = ?`)
      .run(newStatus, newPaidAmount, paidDate, notes !== undefined ? notes : invoice.notes, new Date().toISOString(), id);
    
    res.json({ success: true, message: 'Invoice updated' });
  } catch (err) {
    console.error('Update invoice error:', err);
    res.status(500).json({ error: 'Error updating invoice' });
  }
});

// ==================== PAYMENTS ====================

router.get('/payments', (req, res) => {
  try {
    const db = getDb(req);
    let payments = db.prepare('SELECT * FROM crm_payments').all();
    
    const customers = db.prepare('SELECT * FROM customers').all();
    const invoices = db.prepare('SELECT * FROM invoices').all();
    
    payments = payments.map(p => {
      const customer = customers.find(c => c.id === p.customer_id);
      const invoice = invoices.find(i => i.id === p.invoice_id);
      return {
        ...p,
        customerName: customer ? `${customer.first_name} ${customer.last_name}` : null,
        companyName: customer?.company_name,
        invoiceNumber: invoice?.invoice_number
      };
    });
    
    res.json({
      payments: payments.map(p => ({
        id: p.id,
        uuid: p.uuid,
        invoiceId: p.invoice_id,
        invoiceNumber: p.invoiceNumber,
        customerId: p.customer_id,
        customerName: p.customerName,
        companyName: p.companyName,
        amount: p.amount,
        currency: p.currency || 'USD',
        method: p.method,
        reference: p.reference,
        status: p.status,
        notes: p.notes,
        createdAt: p.created_at
      }))
    });
  } catch (err) {
    console.error('Get payments error:', err);
    res.status(500).json({ error: 'Error fetching payments' });
  }
});

router.post('/payments', (req, res) => {
  try {
    const db = getDb(req);
    const { invoiceId, customerId, amount, method, reference, notes } = req.body;
    
    if (!customerId || !amount) {
      return res.status(400).json({ error: 'Customer and amount required' });
    }
    
    const uuid = uuidv4();
    const result = db.prepare(`
      INSERT INTO crm_payments (uuid, invoice_id, customer_id, amount, currency, method, reference, status, notes, processed_by, created_at)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `).run(uuid, invoiceId || null, customerId, amount, 'USD', method || 'other', reference || null, 'completed', notes || null, req.session.user?.id || 1, new Date().toISOString());
    
    // Update invoice if linked
    if (invoiceId) {
      const invoices = db.prepare('SELECT * FROM invoices').all();
      const invoice = invoices.find(i => i.id === invoiceId);
      if (invoice) {
        const newPaidAmount = (invoice.paid_amount || 0) + amount;
        let newStatus = invoice.status;
        let paidDate = invoice.paid_date;
        
        if (newPaidAmount >= invoice.total) {
          newStatus = 'paid';
          paidDate = new Date().toISOString().split('T')[0];
        } else if (newPaidAmount > 0) {
          newStatus = 'partial';
        }
        
        db.prepare(`UPDATE invoices SET paid_amount = ?, status = ?, paid_date = ? WHERE id = ?`)
          .run(newPaidAmount, newStatus, paidDate, invoiceId);
      }
    }
    
    res.status(201).json({ success: true, payment: { id: result.lastInsertRowid, uuid, amount } });
  } catch (err) {
    console.error('Create payment error:', err);
    res.status(500).json({ error: 'Error creating payment' });
  }
});

// ==================== QA REVIEWS ====================

router.get('/qa/reviews', (req, res) => {
  try {
    const db = getDb(req);
    let reviews = db.prepare('SELECT * FROM qa_reviews').all();
    
    const users = db.prepare('SELECT * FROM users').all();
    const tickets = db.prepare('SELECT * FROM tickets').all();
    
    reviews = reviews.map(r => {
      const agent = users.find(u => u.id === r.agent_id);
      const reviewer = users.find(u => u.id === r.reviewer_id);
      const ticket = tickets.find(t => t.id === r.ticket_id);
      return {
        ...r,
        agentName: agent?.full_name,
        agentEmail: agent?.username,
        reviewerName: reviewer?.full_name,
        ticketNumber: ticket?.ticket_number,
        ticketSubject: ticket?.subject
      };
    });
    
    res.json({
      reviews: reviews.map(r => ({
        id: r.id,
        uuid: r.uuid,
        ticketId: r.ticket_id,
        ticketNumber: r.ticketNumber,
        ticketSubject: r.ticketSubject,
        agentId: r.agent_id,
        agentName: r.agentName,
        agentEmail: r.agentEmail,
        reviewerId: r.reviewer_id,
        reviewerName: r.reviewerName,
        score: r.score,
        feedback: r.feedback,
        status: r.status,
        createdAt: r.created_at
      }))
    });
  } catch (err) {
    console.error('Get reviews error:', err);
    res.status(500).json({ error: 'Error fetching reviews' });
  }
});

router.post('/qa/reviews', (req, res) => {
  try {
    const db = getDb(req);
    const { ticketId, agentId, score, feedback } = req.body;
    
    if (!agentId || score === undefined) {
      return res.status(400).json({ error: 'Agent ID and score required' });
    }
    
    const uuid = uuidv4();
    const result = db.prepare(`
      INSERT INTO qa_reviews (uuid, ticket_id, agent_id, reviewer_id, score, feedback, status, reviewed_at, created_at)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
    `).run(uuid, ticketId || null, agentId, req.session.user?.id || 1, score, feedback || null, 'completed', new Date().toISOString(), new Date().toISOString());
    
    // Update ticket if provided
    if (ticketId) {
      db.prepare(`UPDATE tickets SET qa_reviewed_by = ?, qa_score = ?, qa_notes = ? WHERE id = ?`)
        .run(req.session.user?.id || 1, score, feedback, ticketId);
    }
    
    res.status(201).json({ success: true, review: { id: result.lastInsertRowid, uuid, score } });
  } catch (err) {
    console.error('Create review error:', err);
    res.status(500).json({ error: 'Error creating review' });
  }
});

// ==================== DASHBOARD / ANALYTICS ====================

router.get('/dashboard', (req, res) => {
  try {
    const db = getDb(req);
    
    const customers = db.prepare('SELECT * FROM customers').all();
    const leads = db.prepare('SELECT * FROM leads').all();
    const tickets = db.prepare('SELECT * FROM tickets').all();
    const invoices = db.prepare('SELECT * FROM invoices').all();
    const reviews = db.prepare('SELECT * FROM qa_reviews').all();
    
    const stats = {
      customers: {
        total: customers.length,
        active: customers.filter(c => c.status === 'active').length,
        leads: customers.filter(c => c.status === 'lead').length
      },
      leads: {
        total: leads.length,
        active: leads.filter(l => !['won', 'lost'].includes(l.status)).length,
        won: leads.filter(l => l.status === 'won').length,
        lost: leads.filter(l => l.status === 'lost').length,
        totalValue: leads.reduce((sum, l) => sum + (l.value || 0), 0),
        wonValue: leads.filter(l => l.status === 'won').reduce((sum, l) => sum + (l.value || 0), 0),
        byStatus: Object.entries(leads.reduce((acc, l) => { acc[l.status] = (acc[l.status] || 0) + 1; return acc; }, {})).map(([status, count]) => ({ status, count }))
      },
      tickets: {
        total: tickets.length,
        open: tickets.filter(t => t.status === 'open').length,
        inProgress: tickets.filter(t => t.status === 'in_progress').length,
        resolved: tickets.filter(t => t.status === 'resolved').length,
        byPriority: Object.entries(tickets.reduce((acc, t) => { acc[t.priority] = (acc[t.priority] || 0) + 1; return acc; }, {})).map(([priority, count]) => ({ priority, count }))
      },
      invoices: {
        total: invoices.length,
        totalAmount: invoices.reduce((sum, i) => sum + (i.total || 0), 0),
        paidAmount: invoices.reduce((sum, i) => sum + (i.paid_amount || 0), 0),
        pendingAmount: invoices.filter(i => !['paid', 'cancelled'].includes(i.status)).reduce((sum, i) => sum + (i.total - (i.paid_amount || 0)), 0),
        byStatus: Object.entries(invoices.reduce((acc, i) => { acc[i.status] = (acc[i.status] || 0) + 1; return acc; }, {})).map(([status, count]) => ({ status, count }))
      },
      qa: {
        totalReviews: reviews.length,
        avgScore: reviews.length > 0 ? reviews.reduce((sum, r) => sum + (r.score || 0), 0) / reviews.length : null
      }
    };
    
    res.json(stats);
  } catch (err) {
    console.error('Dashboard error:', err);
    res.status(500).json({ error: 'Error fetching dashboard' });
  }
});

router.get('/qa/tickets/pending', (req, res) => {
  try {
    const db = getDb(req);
    
    let tickets = db.prepare('SELECT * FROM tickets').all();
    tickets = tickets.filter(t => ['resolved', 'closed'].includes(t.status) && !t.qa_reviewed_by);
    
    const customers = db.prepare('SELECT * FROM customers').all();
    const users = db.prepare('SELECT * FROM users').all();
    
    tickets = tickets.map(t => {
      const customer = customers.find(c => c.id === t.customer_id);
      const agent = users.find(u => u.id === t.assigned_agent_id);
      return {
        ...t,
        customerName: customer ? `${customer.first_name} ${customer.last_name}` : null,
        agentName: agent?.full_name
      };
    });
    
    res.json({
      tickets: tickets.map(t => ({
        id: t.id,
        uuid: t.uuid,
        ticketNumber: t.ticket_number,
        subject: t.subject,
        description: t.description,
        category: t.category,
        priority: t.priority,
        status: t.status,
        customerName: t.customerName,
        agentName: t.agentName,
        assignedAgentId: t.assigned_agent_id,
        resolution: t.resolution,
        resolvedAt: t.resolved_at,
        createdAt: t.created_at
      }))
    });
  } catch (err) {
    console.error('Get pending tickets error:', err);
    res.status(500).json({ error: 'Error fetching pending tickets' });
  }
});

router.get('/qa/agents/performance', (req, res) => {
  try {
    const db = getDb(req);
    
    const users = db.prepare('SELECT * FROM users').all();
    const tickets = db.prepare('SELECT * FROM tickets').all();
    const reviews = db.prepare('SELECT * FROM qa_reviews').all();
    const leads = db.prepare('SELECT * FROM leads').all();
    
    // Filter to agents (non-admin users)
    const agents = users.filter(u => u.role !== 'admin' || true); // Include all for now
    
    const performance = agents.map(agent => {
      const agentTickets = tickets.filter(t => t.assigned_agent_id === agent.id);
      const agentReviews = reviews.filter(r => r.agent_id === agent.id);
      const agentLeads = leads.filter(l => l.assigned_agent_id === agent.id);
      
      return {
        id: agent.id,
        email: agent.username,
        name: agent.full_name,
        totalTickets: agentTickets.length,
        resolvedTickets: agentTickets.filter(t => t.status === 'resolved').length,
        resolutionRate: agentTickets.length > 0 ? ((agentTickets.filter(t => t.status === 'resolved').length / agentTickets.length) * 100).toFixed(1) : 0,
        avgQaScore: agentReviews.length > 0 ? (agentReviews.reduce((sum, r) => sum + (r.score || 0), 0) / agentReviews.length).toFixed(1) : null,
        totalReviews: agentReviews.length,
        wonLeads: agentLeads.filter(l => l.status === 'won').length
      };
    });
    
    res.json({ agents: performance });
  } catch (err) {
    console.error('Get agent performance error:', err);
    res.status(500).json({ error: 'Error fetching agent performance' });
  }
});

router.get('/reports/revenue', (req, res) => {
  try {
    const db = getDb(req);
    const invoices = db.prepare('SELECT * FROM invoices').all();
    
    const revenue = {
      totalInvoiced: invoices.reduce((sum, i) => sum + (i.total || 0), 0),
      totalPaid: invoices.reduce((sum, i) => sum + (i.paid_amount || 0), 0),
      totalPending: invoices.filter(i => !['paid', 'cancelled'].includes(i.status)).reduce((sum, i) => sum + (i.total - (i.paid_amount || 0)), 0),
      byStatus: Object.entries(invoices.reduce((acc, i) => {
        if (!acc[i.status]) acc[i.status] = { count: 0, total: 0 };
        acc[i.status].count++;
        acc[i.status].total += i.total || 0;
        return acc;
      }, {})).map(([status, data]) => ({ status, ...data }))
    };
    
    res.json(revenue);
  } catch (err) {
    console.error('Revenue report error:', err);
    res.status(500).json({ error: 'Error fetching revenue report' });
  }
});

router.get('/reports/outstanding', (req, res) => {
  try {
    const db = getDb(req);
    const invoices = db.prepare('SELECT * FROM invoices').all();
    const customers = db.prepare('SELECT * FROM customers').all();
    
    const outstanding = invoices
      .filter(i => ['sent', 'partial', 'overdue'].includes(i.status))
      .map(i => {
        const customer = customers.find(c => c.id === i.customer_id);
        const daysOverdue = i.due_date ? Math.max(0, Math.floor((new Date() - new Date(i.due_date)) / (1000 * 60 * 60 * 24))) : 0;
        return {
          id: i.id,
          invoiceNumber: i.invoice_number,
          customerName: customer ? `${customer.first_name} ${customer.last_name}` : null,
          companyName: customer?.company_name,
          email: customer?.email,
          total: i.total,
          paidAmount: i.paid_amount || 0,
          balance: i.total - (i.paid_amount || 0),
          dueDate: i.due_date,
          daysOverdue,
          status: i.status
        };
      });
    
    res.json({
      invoices: outstanding,
      summary: {
        count: outstanding.length,
        totalOutstanding: outstanding.reduce((sum, i) => sum + i.balance, 0)
      }
    });
  } catch (err) {
    console.error('Outstanding report error:', err);
    res.status(500).json({ error: 'Error fetching outstanding report' });
  }
});

module.exports = router;
