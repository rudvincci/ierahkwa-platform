/**
 * Governance Screen - Voting & Proposals
 */

import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  Modal,
  TextInput,
} from 'react-native';
import { useTranslation } from 'react-i18next';
import api from '../services/api';

export default function GovernanceScreen() {
  const { t } = useTranslation();
  const [proposals, setProposals] = useState([]);
  const [activeTab, setActiveTab] = useState('active');
  const [showCreateModal, setShowCreateModal] = useState(false);

  useEffect(() => {
    loadProposals();
  }, []);

  const loadProposals = async () => {
    try {
      const data = await api.getProposals();
      setProposals(data.proposals || []);
    } catch (e) {
      console.error('Error loading proposals:', e);
    }
  };

  const filteredProposals = proposals.filter(p => 
    activeTab === 'all' ? true : p.status.toLowerCase() === activeTab
  );

  return (
    <View style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <Text style={styles.headerTitle}>🗳️ {t('governance')}</Text>
        <TouchableOpacity 
          style={styles.createBtn}
          onPress={() => setShowCreateModal(true)}
        >
          <Text style={styles.createBtnText}>+ {t('create_proposal')}</Text>
        </TouchableOpacity>
      </View>

      {/* Stats */}
      <View style={styles.statsRow}>
        <StatBox value={proposals.length} label="Total" />
        <StatBox value={proposals.filter(p => p.status === 'ACTIVE').length} label={t('active')} color="#00FF41" />
        <StatBox value={proposals.reduce((sum, p) => sum + p.totalVotes, 0)} label="Votes" color="#FFD700" />
      </View>

      {/* Tabs */}
      <View style={styles.tabs}>
        {['active', 'all', 'ended'].map(tab => (
          <TouchableOpacity
            key={tab}
            style={[styles.tab, activeTab === tab && styles.tabActive]}
            onPress={() => setActiveTab(tab)}
          >
            <Text style={[styles.tabText, activeTab === tab && styles.tabTextActive]}>
              {tab.charAt(0).toUpperCase() + tab.slice(1)}
            </Text>
          </TouchableOpacity>
        ))}
      </View>

      {/* Proposals List */}
      <ScrollView style={styles.list}>
        {filteredProposals.length === 0 ? (
          <Text style={styles.emptyText}>No proposals found</Text>
        ) : (
          filteredProposals.map(proposal => (
            <ProposalCard key={proposal.id} proposal={proposal} onVote={loadProposals} />
          ))
        )}
      </ScrollView>

      {/* Create Modal */}
      <CreateProposalModal
        visible={showCreateModal}
        onClose={() => setShowCreateModal(false)}
        onCreated={loadProposals}
      />
    </View>
  );
}

function StatBox({ value, label, color = '#fff' }) {
  return (
    <View style={styles.statBox}>
      <Text style={[styles.statValue, { color }]}>{value}</Text>
      <Text style={styles.statLabel}>{label}</Text>
    </View>
  );
}

function ProposalCard({ proposal, onVote }) {
  const [showVote, setShowVote] = useState(false);
  const totalPower = BigInt(proposal.totalVotingPower || 1);
  const daysLeft = Math.max(0, Math.ceil((proposal.endsAt - Date.now()) / 86400000));

  const handleVote = async (optionId) => {
    try {
      await api.castVote({
        proposalId: proposal.id,
        optionId,
        voter: '0x' + Math.random().toString(16).slice(2, 42),
        votingPower: '1'
      });
      setShowVote(false);
      onVote();
    } catch (e) {
      console.error('Vote error:', e);
    }
  };

  return (
    <View style={styles.proposalCard}>
      <View style={styles.proposalHeader}>
        <View>
          <Text style={styles.proposalTitle}>{proposal.title}</Text>
          <Text style={styles.proposalMeta}>
            {proposal.totalVotes} votes • {daysLeft}d left
          </Text>
        </View>
        <View style={[styles.statusBadge, proposal.status === 'ACTIVE' ? styles.statusActive : styles.statusEnded]}>
          <Text style={styles.statusText}>{proposal.status}</Text>
        </View>
      </View>

      {proposal.description && (
        <Text style={styles.proposalDesc}>{proposal.description}</Text>
      )}

      {/* Options */}
      {proposal.options.map((opt, i) => {
        const percent = totalPower > 0n ? Number((BigInt(opt.votingPower || 0) * 100n) / totalPower) : 0;
        return (
          <TouchableOpacity
            key={i}
            style={styles.optionItem}
            onPress={() => proposal.status === 'ACTIVE' && handleVote(opt.id)}
            disabled={proposal.status !== 'ACTIVE'}
          >
            <View style={styles.optionHeader}>
              <Text style={styles.optionText}>{opt.text}</Text>
              <Text style={styles.optionVotes}>{opt.votes} ({percent}%)</Text>
            </View>
            <View style={styles.progressBar}>
              <View style={[styles.progressFill, { width: `${percent}%` }]} />
            </View>
          </TouchableOpacity>
        );
      })}

      {proposal.status === 'ACTIVE' && (
        <TouchableOpacity style={styles.voteBtn} onPress={() => setShowVote(!showVote)}>
          <Text style={styles.voteBtnText}>🗳️ Vote Now</Text>
        </TouchableOpacity>
      )}
    </View>
  );
}

function CreateProposalModal({ visible, onClose, onCreated }) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [options, setOptions] = useState(['', '']);

  const addOption = () => setOptions([...options, '']);
  
  const updateOption = (index, value) => {
    const newOptions = [...options];
    newOptions[index] = value;
    setOptions(newOptions);
  };

  const handleCreate = async () => {
    try {
      await api.createProposal({
        title,
        description,
        options: options.filter(o => o.trim()),
      });
      onCreated();
      onClose();
      setTitle('');
      setDescription('');
      setOptions(['', '']);
    } catch (e) {
      console.error('Create error:', e);
    }
  };

  return (
    <Modal visible={visible} animationType="slide" transparent>
      <View style={styles.modalOverlay}>
        <View style={styles.modalContent}>
          <View style={styles.modalHeader}>
            <Text style={styles.modalTitle}>📝 Create Proposal</Text>
            <TouchableOpacity onPress={onClose}>
              <Text style={styles.closeBtn}>✕</Text>
            </TouchableOpacity>
          </View>

          <ScrollView>
            <Text style={styles.inputLabel}>Title</Text>
            <TextInput
              style={styles.input}
              placeholder="What should we decide?"
              placeholderTextColor="#666"
              value={title}
              onChangeText={setTitle}
            />

            <Text style={styles.inputLabel}>Description</Text>
            <TextInput
              style={[styles.input, { height: 80 }]}
              placeholder="Explain the proposal..."
              placeholderTextColor="#666"
              multiline
              value={description}
              onChangeText={setDescription}
            />

            <Text style={styles.inputLabel}>Options</Text>
            {options.map((opt, i) => (
              <TextInput
                key={i}
                style={styles.input}
                placeholder={`Option ${i + 1}`}
                placeholderTextColor="#666"
                value={opt}
                onChangeText={(v) => updateOption(i, v)}
              />
            ))}
            <TouchableOpacity style={styles.addOptionBtn} onPress={addOption}>
              <Text style={styles.addOptionText}>+ Add Option</Text>
            </TouchableOpacity>

            <TouchableOpacity style={styles.submitBtn} onPress={handleCreate}>
              <Text style={styles.submitBtnText}>Submit Proposal</Text>
            </TouchableOpacity>
          </ScrollView>
        </View>
      </View>
    </Modal>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#0a0e17' },
  header: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', padding: 15 },
  headerTitle: { color: '#FFD700', fontSize: 24, fontWeight: 'bold' },
  createBtn: { backgroundColor: '#FFD700', paddingHorizontal: 15, paddingVertical: 8, borderRadius: 20 },
  createBtnText: { color: '#000', fontWeight: 'bold' },
  statsRow: { flexDirection: 'row', padding: 15, gap: 10 },
  statBox: { flex: 1, backgroundColor: '#1a1f2e', borderRadius: 12, padding: 15, alignItems: 'center' },
  statValue: { fontSize: 24, fontWeight: 'bold' },
  statLabel: { color: '#888', marginTop: 5 },
  tabs: { flexDirection: 'row', paddingHorizontal: 15, gap: 10 },
  tab: { paddingHorizontal: 20, paddingVertical: 10, backgroundColor: '#1a1f2e', borderRadius: 20 },
  tabActive: { backgroundColor: 'rgba(255,215,0,0.2)', borderWidth: 1, borderColor: '#FFD700' },
  tabText: { color: '#888' },
  tabTextActive: { color: '#FFD700' },
  list: { flex: 1, padding: 15 },
  emptyText: { color: '#666', textAlign: 'center', marginTop: 40 },
  proposalCard: { backgroundColor: '#1a1f2e', borderRadius: 15, padding: 20, marginBottom: 15 },
  proposalHeader: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 10 },
  proposalTitle: { color: '#fff', fontSize: 18, fontWeight: 'bold', flex: 1 },
  proposalMeta: { color: '#888', fontSize: 12, marginTop: 5 },
  statusBadge: { paddingHorizontal: 12, paddingVertical: 5, borderRadius: 12 },
  statusActive: { backgroundColor: 'rgba(0,255,65,0.2)' },
  statusEnded: { backgroundColor: 'rgba(255,255,255,0.1)' },
  statusText: { color: '#fff', fontSize: 12 },
  proposalDesc: { color: '#888', marginBottom: 15 },
  optionItem: { backgroundColor: 'rgba(0,0,0,0.3)', borderRadius: 10, padding: 12, marginBottom: 8 },
  optionHeader: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 8 },
  optionText: { color: '#fff' },
  optionVotes: { color: '#888' },
  progressBar: { height: 6, backgroundColor: 'rgba(255,255,255,0.1)', borderRadius: 3 },
  progressFill: { height: '100%', backgroundColor: '#9D4EDD', borderRadius: 3 },
  voteBtn: { backgroundColor: '#FFD700', borderRadius: 12, padding: 15, alignItems: 'center', marginTop: 10 },
  voteBtnText: { color: '#000', fontWeight: 'bold', fontSize: 16 },
  modalOverlay: { flex: 1, backgroundColor: 'rgba(0,0,0,0.8)', justifyContent: 'flex-end' },
  modalContent: { backgroundColor: '#1a1f2e', borderTopLeftRadius: 20, borderTopRightRadius: 20, padding: 20, maxHeight: '80%' },
  modalHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 },
  modalTitle: { color: '#FFD700', fontSize: 20, fontWeight: 'bold' },
  closeBtn: { color: '#fff', fontSize: 24 },
  inputLabel: { color: '#888', marginBottom: 8, marginTop: 15 },
  input: { backgroundColor: 'rgba(0,0,0,0.3)', borderRadius: 12, padding: 15, color: '#fff', marginBottom: 10 },
  addOptionBtn: { alignSelf: 'flex-start', marginBottom: 20 },
  addOptionText: { color: '#00FFFF' },
  submitBtn: { backgroundColor: '#FFD700', borderRadius: 15, padding: 18, alignItems: 'center' },
  submitBtnText: { color: '#000', fontWeight: 'bold', fontSize: 16 },
});
