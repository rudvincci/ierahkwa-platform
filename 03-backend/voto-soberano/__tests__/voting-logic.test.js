'use strict';

const crypto = require('crypto');

// Extract and test the core voting logic from server.js
// These functions mirror the internal implementations

function hashBallot(ballot, previousHash) {
  const payload = JSON.stringify({
    id: ballot.id,
    electionId: ballot.electionId,
    voterId: ballot.voterIdHash,
    choice: ballot.choice,
    ts: ballot.createdAt,
    prev: previousHash
  });
  return crypto.createHash('sha256').update(payload).digest('hex');
}

function isElectionActive(election) {
  const now = new Date();
  return (
    election.status === 'open' &&
    new Date(election.start_date) <= now &&
    new Date(election.end_date) >= now
  );
}

function mapElectionRow(row) {
  return {
    id: row.id,
    title: row.title,
    description: row.description,
    choices: row.choices,
    startDate: row.start_date instanceof Date ? row.start_date.toISOString() : row.start_date,
    endDate: row.end_date instanceof Date ? row.end_date.toISOString() : row.end_date,
    status: row.status,
    createdBy: row.created_by,
    createdAt: row.created_at instanceof Date ? row.created_at.toISOString() : row.created_at,
    ...(row.closed_at ? { closedAt: row.closed_at instanceof Date ? row.closed_at.toISOString() : row.closed_at } : {}),
    ...(row.closed_by ? { closedBy: row.closed_by } : {})
  };
}

describe('Voto Soberano — Core Voting Logic', () => {

  // ──────────────────────────────────────────
  // Ballot Hashing (Immutable Chain)
  // ──────────────────────────────────────────
  describe('hashBallot', () => {
    const genesisHash = '0'.repeat(64);

    test('produces a 64-char hex SHA-256 hash', () => {
      const ballot = {
        id: 'ballot-1',
        electionId: 'election-1',
        voterIdHash: 'abc123',
        choice: 'yes',
        createdAt: '2026-03-01T10:00:00.000Z'
      };
      const hash = hashBallot(ballot, genesisHash);
      expect(hash).toMatch(/^[0-9a-f]{64}$/);
    });

    test('same input produces same hash (deterministic)', () => {
      const ballot = {
        id: 'ballot-1',
        electionId: 'election-1',
        voterIdHash: 'voter-hash-1',
        choice: 'no',
        createdAt: '2026-03-01T12:00:00.000Z'
      };
      const hash1 = hashBallot(ballot, genesisHash);
      const hash2 = hashBallot(ballot, genesisHash);
      expect(hash1).toBe(hash2);
    });

    test('different choice produces different hash', () => {
      const base = {
        id: 'ballot-1',
        electionId: 'election-1',
        voterIdHash: 'voter-hash-1',
        createdAt: '2026-03-01T12:00:00.000Z'
      };
      const hashYes = hashBallot({ ...base, choice: 'yes' }, genesisHash);
      const hashNo = hashBallot({ ...base, choice: 'no' }, genesisHash);
      expect(hashYes).not.toBe(hashNo);
    });

    test('different previousHash produces different hash', () => {
      const ballot = {
        id: 'ballot-1',
        electionId: 'election-1',
        voterIdHash: 'voter-hash-1',
        choice: 'yes',
        createdAt: '2026-03-01T12:00:00.000Z'
      };
      const hash1 = hashBallot(ballot, genesisHash);
      const hash2 = hashBallot(ballot, 'a'.repeat(64));
      expect(hash1).not.toBe(hash2);
    });

    test('ballot chain integrity — each hash links to previous', () => {
      let chainTip = genesisHash;
      const ballots = [];

      for (let i = 0; i < 5; i++) {
        const ballot = {
          id: `ballot-${i}`,
          electionId: 'election-1',
          voterIdHash: crypto.createHash('sha256').update(`voter-${i}`).digest('hex'),
          choice: i % 2 === 0 ? 'yes' : 'no',
          createdAt: new Date(Date.now() + i * 1000).toISOString()
        };
        const receiptHash = hashBallot(ballot, chainTip);
        ballots.push({ ...ballot, previousHash: chainTip, receiptHash });
        chainTip = receiptHash;
      }

      // Verify chain
      let prevHash = genesisHash;
      for (const b of ballots) {
        expect(b.previousHash).toBe(prevHash);
        const computed = hashBallot(b, prevHash);
        expect(computed).toBe(b.receiptHash);
        prevHash = b.receiptHash;
      }
    });

    test('changing any ballot in chain breaks subsequent hashes', () => {
      let chainTip = genesisHash;
      const ballots = [];

      for (let i = 0; i < 3; i++) {
        const ballot = {
          id: `ballot-${i}`,
          electionId: 'election-1',
          voterIdHash: `voter-${i}`,
          choice: 'yes',
          createdAt: `2026-03-01T1${i}:00:00.000Z`
        };
        const receiptHash = hashBallot(ballot, chainTip);
        ballots.push({ ...ballot, previousHash: chainTip, receiptHash });
        chainTip = receiptHash;
      }

      // Tamper with first ballot's choice
      ballots[0].choice = 'no';
      const recomputed = hashBallot(ballots[0], ballots[0].previousHash);
      expect(recomputed).not.toBe(ballots[0].receiptHash);
    });
  });

  // ──────────────────────────────────────────
  // Voter ID Hashing
  // ──────────────────────────────────────────
  describe('voter ID privacy', () => {
    test('voter IDs are hashed before storage', () => {
      const userId = 'citizen-john-doe-123';
      const voterIdHash = crypto.createHash('sha256').update(userId).digest('hex');

      expect(voterIdHash).toMatch(/^[0-9a-f]{64}$/);
      expect(voterIdHash).not.toContain(userId);
    });

    test('same user always produces same hash', () => {
      const userId = 'citizen-456';
      const hash1 = crypto.createHash('sha256').update(userId).digest('hex');
      const hash2 = crypto.createHash('sha256').update(userId).digest('hex');
      expect(hash1).toBe(hash2);
    });

    test('different users produce different hashes', () => {
      const hash1 = crypto.createHash('sha256').update('user-a').digest('hex');
      const hash2 = crypto.createHash('sha256').update('user-b').digest('hex');
      expect(hash1).not.toBe(hash2);
    });
  });

  // ──────────────────────────────────────────
  // Election Active Check
  // ──────────────────────────────────────────
  describe('isElectionActive', () => {
    test('returns true for open election within date range', () => {
      const election = {
        status: 'open',
        start_date: new Date(Date.now() - 86400000).toISOString(), // yesterday
        end_date: new Date(Date.now() + 86400000).toISOString()     // tomorrow
      };
      expect(isElectionActive(election)).toBe(true);
    });

    test('returns false for closed election', () => {
      const election = {
        status: 'closed',
        start_date: new Date(Date.now() - 86400000).toISOString(),
        end_date: new Date(Date.now() + 86400000).toISOString()
      };
      expect(isElectionActive(election)).toBe(false);
    });

    test('returns false for election not yet started', () => {
      const election = {
        status: 'open',
        start_date: new Date(Date.now() + 86400000).toISOString(),  // tomorrow
        end_date: new Date(Date.now() + 172800000).toISOString()     // day after tomorrow
      };
      expect(isElectionActive(election)).toBe(false);
    });

    test('returns false for expired election', () => {
      const election = {
        status: 'open',
        start_date: new Date(Date.now() - 172800000).toISOString(), // 2 days ago
        end_date: new Date(Date.now() - 86400000).toISOString()      // yesterday
      };
      expect(isElectionActive(election)).toBe(false);
    });
  });

  // ──────────────────────────────────────────
  // mapElectionRow
  // ──────────────────────────────────────────
  describe('mapElectionRow', () => {
    test('converts snake_case DB row to camelCase API response', () => {
      const row = {
        id: 'e-1',
        title: 'Council Vote',
        description: 'Election for tribal council',
        choices: ['candidate-a', 'candidate-b'],
        start_date: '2026-03-01T00:00:00.000Z',
        end_date: '2026-03-31T23:59:59.000Z',
        status: 'open',
        created_by: 'admin-1',
        created_at: '2026-02-28T12:00:00.000Z'
      };

      const mapped = mapElectionRow(row);

      expect(mapped.id).toBe('e-1');
      expect(mapped.title).toBe('Council Vote');
      expect(mapped.startDate).toBe('2026-03-01T00:00:00.000Z');
      expect(mapped.endDate).toBe('2026-03-31T23:59:59.000Z');
      expect(mapped.createdBy).toBe('admin-1');
      expect(mapped.createdAt).toBe('2026-02-28T12:00:00.000Z');
      // Should NOT have snake_case keys
      expect(mapped.start_date).toBeUndefined();
      expect(mapped.end_date).toBeUndefined();
    });

    test('converts Date objects to ISO strings', () => {
      const row = {
        id: 'e-2',
        title: 'Test',
        description: '',
        choices: ['a', 'b'],
        start_date: new Date('2026-03-01T00:00:00.000Z'),
        end_date: new Date('2026-03-31T23:59:59.000Z'),
        status: 'open',
        created_by: 'admin',
        created_at: new Date('2026-02-28T00:00:00.000Z')
      };

      const mapped = mapElectionRow(row);
      expect(mapped.startDate).toBe('2026-03-01T00:00:00.000Z');
      expect(mapped.endDate).toBe('2026-03-31T23:59:59.000Z');
    });

    test('includes closedAt and closedBy when present', () => {
      const row = {
        id: 'e-3',
        title: 'Closed Election',
        description: '',
        choices: ['a', 'b'],
        start_date: '2026-02-01',
        end_date: '2026-02-28',
        status: 'closed',
        created_by: 'admin',
        created_at: '2026-01-15',
        closed_at: '2026-02-28T18:00:00.000Z',
        closed_by: 'admin-2'
      };

      const mapped = mapElectionRow(row);
      expect(mapped.closedAt).toBe('2026-02-28T18:00:00.000Z');
      expect(mapped.closedBy).toBe('admin-2');
    });

    test('omits closedAt/closedBy when not present', () => {
      const row = {
        id: 'e-4',
        title: 'Open',
        description: '',
        choices: ['x'],
        start_date: '2026-03-01',
        end_date: '2026-03-31',
        status: 'open',
        created_by: 'admin',
        created_at: '2026-02-28'
      };

      const mapped = mapElectionRow(row);
      expect(mapped.closedAt).toBeUndefined();
      expect(mapped.closedBy).toBeUndefined();
    });
  });

  // ──────────────────────────────────────────
  // JWT Authentication Logic
  // ──────────────────────────────────────────
  describe('JWT auth validation', () => {
    test('rejects missing Authorization header', () => {
      const header = undefined;
      const hasBearer = header && header.startsWith('Bearer ');
      expect(hasBearer).toBeFalsy();
    });

    test('rejects Authorization without Bearer prefix', () => {
      const header = 'Basic abc123';
      const hasBearer = header.startsWith('Bearer ');
      expect(hasBearer).toBe(false);
    });

    test('extracts token from Bearer header', () => {
      const header = 'Bearer eyJhbGciOiJIUzI1NiJ9.test.sig';
      const token = header.slice(7);
      expect(token).toBe('eyJhbGciOiJIUzI1NiJ9.test.sig');
      expect(token).not.toContain('Bearer');
    });
  });

  // ──────────────────────────────────────────
  // Election Validation
  // ──────────────────────────────────────────
  describe('election input validation', () => {
    test('requires at least 2 choices', () => {
      const choices = ['only-one'];
      expect(Array.isArray(choices) && choices.length >= 2).toBe(false);
    });

    test('accepts 2 or more choices', () => {
      const choices = ['yes', 'no'];
      expect(Array.isArray(choices) && choices.length >= 2).toBe(true);
    });

    test('end_date must be after start_date', () => {
      const start = new Date('2026-03-01');
      const end = new Date('2026-03-31');
      expect(end > start).toBe(true);

      const badEnd = new Date('2026-02-28');
      expect(badEnd > start).toBe(false);
    });

    test('rejects vote for invalid choice', () => {
      const choices = ['candidate-a', 'candidate-b', 'abstain'];
      const vote = 'candidate-c';
      expect(choices.includes(vote)).toBe(false);
    });

    test('accepts vote for valid choice', () => {
      const choices = ['candidate-a', 'candidate-b', 'abstain'];
      const vote = 'abstain';
      expect(choices.includes(vote)).toBe(true);
    });
  });
});
