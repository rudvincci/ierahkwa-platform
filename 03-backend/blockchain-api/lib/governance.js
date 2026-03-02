'use strict';

// ============================================================
// Sovereign Governance — On-Chain Proposal & Voting Engine
// Supports: proposals, weighted voting, quorum, execution
// ============================================================

const crypto = require('crypto');

const PROPOSAL_STATES = ['draft', 'active', 'passed', 'rejected', 'executed', 'expired'];
const QUORUM_PCT = 30;       // 30% of total stake must vote
const APPROVAL_PCT = 66;     // 66% approval to pass
const VOTING_PERIOD_MS = 7 * 24 * 60 * 60 * 1000; // 7 days

class Governance {
  constructor(blockchain) {
    this.blockchain = blockchain;
    this.proposals = new Map(); // proposalId → proposal
    this.votes = new Map();     // proposalId → Map(voterAddress → vote)
  }

  createProposal(author, title, description, category, actions = []) {
    if (!this.blockchain.wallets.has(author)) {
      throw new Error('Author wallet not found');
    }
    if (!title || title.length < 5) throw new Error('Title must be at least 5 characters');
    if (!description || description.length < 20) throw new Error('Description must be at least 20 characters');

    const id = 'PROP-' + crypto.randomBytes(4).toString('hex').toUpperCase();
    const now = new Date();

    const proposal = {
      id,
      author,
      title,
      description,
      category: category || 'general',
      actions,
      state: 'active',
      createdAt: now.toISOString(),
      votingEnds: new Date(now.getTime() + VOTING_PERIOD_MS).toISOString(),
      votesFor: 0,
      votesAgainst: 0,
      votesAbstain: 0,
      voterCount: 0,
      totalStakeVoted: 0
    };

    this.proposals.set(id, proposal);
    this.votes.set(id, new Map());
    return proposal;
  }

  vote(proposalId, voterAddress, choice, weight) {
    const proposal = this.proposals.get(proposalId);
    if (!proposal) throw new Error('Proposal not found');
    if (proposal.state !== 'active') throw new Error(`Proposal is ${proposal.state}, not active`);
    if (new Date() > new Date(proposal.votingEnds)) {
      proposal.state = 'expired';
      throw new Error('Voting period has ended');
    }

    if (!this.blockchain.wallets.has(voterAddress)) {
      throw new Error('Voter wallet not found');
    }

    const proposalVotes = this.votes.get(proposalId);
    if (proposalVotes.has(voterAddress)) {
      throw new Error('Already voted on this proposal');
    }

    // Weight = validator stake or wallet balance
    const validator = this.blockchain.validators.get(voterAddress);
    const wallet = this.blockchain.wallets.get(voterAddress);
    const voteWeight = weight || (validator ? validator.stake : Math.min(wallet.balance, 1000));

    const validChoices = ['for', 'against', 'abstain'];
    if (!validChoices.includes(choice)) {
      throw new Error(`Invalid choice. Must be: ${validChoices.join(', ')}`);
    }

    proposalVotes.set(voterAddress, {
      voter: voterAddress,
      choice,
      weight: voteWeight,
      timestamp: new Date().toISOString()
    });

    if (choice === 'for') proposal.votesFor += voteWeight;
    else if (choice === 'against') proposal.votesAgainst += voteWeight;
    else proposal.votesAbstain += voteWeight;

    proposal.voterCount++;
    proposal.totalStakeVoted += voteWeight;

    return {
      proposalId,
      voter: voterAddress,
      choice,
      weight: voteWeight,
      currentResults: this._getResults(proposal)
    };
  }

  tallyProposal(proposalId) {
    const proposal = this.proposals.get(proposalId);
    if (!proposal) throw new Error('Proposal not found');
    if (proposal.state !== 'active') return this._getResults(proposal);

    // Calculate total possible stake
    let totalStake = 0;
    for (const v of this.blockchain.validators.values()) totalStake += v.stake;
    if (totalStake === 0) totalStake = 1000; // Fallback

    const quorumMet = (proposal.totalStakeVoted / totalStake) * 100 >= QUORUM_PCT;
    const totalVotes = proposal.votesFor + proposal.votesAgainst;
    const approvalPct = totalVotes > 0 ? (proposal.votesFor / totalVotes) * 100 : 0;
    const passed = quorumMet && approvalPct >= APPROVAL_PCT;

    proposal.state = passed ? 'passed' : 'rejected';

    return this._getResults(proposal);
  }

  executeProposal(proposalId) {
    const proposal = this.proposals.get(proposalId);
    if (!proposal) throw new Error('Proposal not found');
    if (proposal.state !== 'passed') throw new Error('Proposal must be passed to execute');

    proposal.state = 'executed';
    proposal.executedAt = new Date().toISOString();

    return { id: proposal.id, state: 'executed', executedAt: proposal.executedAt };
  }

  getProposal(proposalId) {
    const proposal = this.proposals.get(proposalId);
    if (!proposal) return null;
    return { ...proposal, results: this._getResults(proposal) };
  }

  listProposals(state, page = 1, limit = 20) {
    let proposals = [...this.proposals.values()];
    if (state) proposals = proposals.filter(p => p.state === state);
    proposals.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));

    const start = (page - 1) * limit;
    return {
      proposals: proposals.slice(start, start + limit),
      total: proposals.length,
      page,
      pages: Math.ceil(proposals.length / limit)
    };
  }

  getVotes(proposalId) {
    const votes = this.votes.get(proposalId);
    if (!votes) return [];
    return [...votes.values()];
  }

  _getResults(proposal) {
    const total = proposal.votesFor + proposal.votesAgainst + proposal.votesAbstain;
    return {
      for: proposal.votesFor,
      against: proposal.votesAgainst,
      abstain: proposal.votesAbstain,
      total,
      voterCount: proposal.voterCount,
      approvalPct: total > 0 ? Math.round((proposal.votesFor / (proposal.votesFor + proposal.votesAgainst || 1)) * 100) : 0,
      quorumPct: QUORUM_PCT,
      approvalRequired: APPROVAL_PCT,
      state: proposal.state
    };
  }

  reset() {
    this.proposals.clear();
    this.votes.clear();
  }
}

module.exports = { Governance, PROPOSAL_STATES, QUORUM_PCT, APPROVAL_PCT, VOTING_PERIOD_MS };
