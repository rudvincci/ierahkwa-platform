'use strict';

const { Blockchain } = require('../lib/blockchain');
const { Governance, QUORUM_PCT, APPROVAL_PCT } = require('../lib/governance');

describe('Governance Engine', () => {
  let chain, gov;
  let author, voter1, voter2;

  beforeEach(() => {
    chain = new Blockchain();
    gov = new Governance(chain);

    // Create wallets and fund them
    author = chain.createWallet('Author');
    voter1 = chain.createWallet('Voter 1');
    voter2 = chain.createWallet('Voter 2');

    chain.createTransaction('sovereign-treasury', author.address, 50000);
    chain.createTransaction('sovereign-treasury', voter1.address, 30000);
    chain.createTransaction('sovereign-treasury', voter2.address, 20000);
    chain.mineBlock();

    // Register as validators for weighted voting
    chain.registerValidator(voter1.address, 10000);
    chain.registerValidator(voter2.address, 5000);
  });

  // ── Proposals ────────────────────────────────────────

  describe('proposals', () => {
    test('createProposal returns valid proposal', () => {
      const p = gov.createProposal(
        author.address,
        'Build New School',
        'Proposal to build a new school in the northern district of the community',
        'infrastructure'
      );
      expect(p.id).toMatch(/^PROP-[A-F0-9]{8}$/);
      expect(p.state).toBe('active');
      expect(p.author).toBe(author.address);
    });

    test('rejects short title', () => {
      expect(() => gov.createProposal(author.address, 'Hi', 'This is a valid description for testing', 'general')).toThrow('at least 5');
    });

    test('rejects short description', () => {
      expect(() => gov.createProposal(author.address, 'Valid Title', 'Too short', 'general')).toThrow('at least 20');
    });

    test('rejects unknown author', () => {
      expect(() => gov.createProposal('0xfake', 'Valid Title', 'This description is long enough for testing', 'general')).toThrow('not found');
    });

    test('getProposal returns null for unknown ID', () => {
      expect(gov.getProposal('PROP-NONEXISTENT')).toBeNull();
    });

    test('listProposals returns paginated results', () => {
      gov.createProposal(author.address, 'Proposal 1', 'Description long enough for testing purposes', 'general');
      gov.createProposal(author.address, 'Proposal 2', 'Another valid description for testing purposes', 'finance');

      const result = gov.listProposals(null, 1, 10);
      expect(result.total).toBe(2);
      expect(result.proposals.length).toBe(2);
    });

    test('listProposals filters by state', () => {
      const p = gov.createProposal(author.address, 'Active One', 'This is a valid active proposal description');
      const result = gov.listProposals('active');
      expect(result.proposals.every(p => p.state === 'active')).toBe(true);
    });
  });

  // ── Voting ───────────────────────────────────────────

  describe('voting', () => {
    let proposalId;

    beforeEach(() => {
      const p = gov.createProposal(
        author.address,
        'Community Fund Allocation',
        'Allocate 10000 WMP from treasury to the community development fund',
        'finance'
      );
      proposalId = p.id;
    });

    test('vote registers correctly', () => {
      const result = gov.vote(proposalId, voter1.address, 'for');
      expect(result.voter).toBe(voter1.address);
      expect(result.choice).toBe('for');
      expect(result.weight).toBeGreaterThan(0);
    });

    test('prevents double voting', () => {
      gov.vote(proposalId, voter1.address, 'for');
      expect(() => gov.vote(proposalId, voter1.address, 'against')).toThrow('Already voted');
    });

    test('rejects invalid choice', () => {
      expect(() => gov.vote(proposalId, voter1.address, 'maybe')).toThrow('Invalid choice');
    });

    test('rejects vote on non-existent proposal', () => {
      expect(() => gov.vote('PROP-FAKE1234', voter1.address, 'for')).toThrow('not found');
    });

    test('getVotes returns all votes for proposal', () => {
      gov.vote(proposalId, voter1.address, 'for');
      gov.vote(proposalId, voter2.address, 'against');
      const votes = gov.getVotes(proposalId);
      expect(votes.length).toBe(2);
    });

    test('abstain votes are counted', () => {
      gov.vote(proposalId, voter1.address, 'abstain');
      const proposal = gov.getProposal(proposalId);
      expect(proposal.votesAbstain).toBeGreaterThan(0);
    });
  });

  // ── Tally ────────────────────────────────────────────

  describe('tally', () => {
    test('proposal passes with sufficient approval and quorum', () => {
      const p = gov.createProposal(author.address, 'Pass This', 'This proposal should definitely pass with enough votes');
      gov.vote(p.id, voter1.address, 'for', 8000);
      gov.vote(p.id, voter2.address, 'for', 3000);

      const result = gov.tallyProposal(p.id);
      expect(result.state).toBe('passed');
    });

    test('proposal rejected when majority votes against', () => {
      const p = gov.createProposal(author.address, 'Reject This', 'This proposal should be rejected by the voters');
      gov.vote(p.id, voter1.address, 'against', 8000);
      gov.vote(p.id, voter2.address, 'against', 3000);

      const result = gov.tallyProposal(p.id);
      expect(result.state).toBe('rejected');
    });
  });

  // ── Execution ────────────────────────────────────────

  describe('execution', () => {
    test('can execute a passed proposal', () => {
      const p = gov.createProposal(author.address, 'Execute Me', 'This proposal will be passed and then executed');
      gov.vote(p.id, voter1.address, 'for', 10000);
      gov.vote(p.id, voter2.address, 'for', 5000);
      gov.tallyProposal(p.id);

      const result = gov.executeProposal(p.id);
      expect(result.state).toBe('executed');
      expect(result.executedAt).toBeDefined();
    });

    test('rejects execution of non-passed proposal', () => {
      const p = gov.createProposal(author.address, 'Not Passed', 'This proposal has not been tallied yet so cannot execute');
      expect(() => gov.executeProposal(p.id)).toThrow('must be passed');
    });
  });

  // ── Constants ────────────────────────────────────────

  describe('constants', () => {
    test('quorum percentage is reasonable', () => {
      expect(QUORUM_PCT).toBeGreaterThan(0);
      expect(QUORUM_PCT).toBeLessThanOrEqual(100);
    });

    test('approval percentage requires supermajority', () => {
      expect(APPROVAL_PCT).toBeGreaterThan(50);
      expect(APPROVAL_PCT).toBeLessThanOrEqual(100);
    });
  });

  // ── Reset ────────────────────────────────────────────

  describe('reset', () => {
    test('clears all proposals and votes', () => {
      gov.createProposal(author.address, 'Temporary', 'This is a temporary proposal for testing reset');
      gov.reset();
      expect(gov.listProposals().total).toBe(0);
    });
  });
});
